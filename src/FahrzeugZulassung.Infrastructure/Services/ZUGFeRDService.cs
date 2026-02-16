using System.Text;
using FahrzeugZulassung.Domain.Entities;
using FahrzeugZulassung.Domain.Enums;
using s2industries.ZUGFeRD;

namespace FahrzeugZulassung.Infrastructure.Services;

public class ZUGFeRDService : IZUGFeRDService
{
    public async Task<ZUGFeRDResult> ErstelleERechnungAsync(Rechnung rechnung)
    {
        var result = new ZUGFeRDResult
        {
            DateiName = $"{rechnung.RechnungsNummer}.pdf"
        };

        try
        {
            // Generiere XML
            result.Xml = GeneriereZUGFeRDXml(rechnung);
            
            // Validiere XML
            result.IstGueltig = ValidiereZUGFeRDXml(result.Xml);
            
            // Generiere PDF mit eingebettetem XML
            result.PdfBytes = await ErstelleERechnungAlsPdfAsync(rechnung);
            
            return await Task.FromResult(result);
        }
        catch (Exception ex)
        {
            result.IstGueltig = false;
            result.Validierungsfehler.Add($"Fehler bei der Generierung: {ex.Message}");
            return result;
        }
    }

    public async Task<byte[]> ErstelleERechnungAlsPdfAsync(Rechnung rechnung)
    {
        // Für diese Demo erstellen wir ein einfaches PDF
        // In Produktion würde hier ein vollständiges PDF/A-3 mit ZUGFeRD XML embedded erstellt
        var xml = GeneriereZUGFeRDXml(rechnung);
        
        // Erstelle ein minimales PDF (für Demo-Zwecke)
        var pdfContent = ErstellePdfMitZUGFeRD(rechnung, xml);
        
        return await Task.FromResult(pdfContent);
    }

    public string GeneriereZUGFeRDXml(Rechnung rechnung)
    {
        // Erstelle ZUGFeRD InvoiceDescriptor
        var profile = MapProfile(rechnung.Format);
        
        var descriptor = InvoiceDescriptor.CreateInvoice(
            invoiceNo: rechnung.RechnungsNummer,
            invoiceDate: rechnung.ErstelltAm,
            currency: CurrencyCodes.EUR
        );

        // Setze Käufer-Daten
        var buyerName = string.IsNullOrWhiteSpace(rechnung.Kunde.Firmenname)
            ? $"{rechnung.Kunde.Vorname} {rechnung.Kunde.Nachname}"
            : rechnung.Kunde.Firmenname;

        descriptor.SetBuyer(
            name: buyerName,
            postcode: rechnung.Kunde.PLZ,
            city: rechnung.Kunde.Ort,
            street: rechnung.Kunde.Strasse,
            country: CountryCodes.DE
        );

        // Setze USt-ID für Geschäftskunden
        if (!string.IsNullOrWhiteSpace(rechnung.Kunde.UStID))
        {
            descriptor.AddBuyerTaxRegistration(rechnung.Kunde.UStID, TaxRegistrationSchemeID.VA);
        }

        // Setze Verkäufer-Daten
        descriptor.SetSeller(
            name: rechnung.Standort.Firmenname,
            postcode: rechnung.Standort.PLZ,
            city: rechnung.Standort.Ort,
            street: rechnung.Standort.Strasse,
            country: CountryCodes.DE
        );

        // Steuernummer / USt-ID
        if (!string.IsNullOrWhiteSpace(rechnung.Standort.UStID))
        {
            descriptor.AddSellerTaxRegistration(rechnung.Standort.UStID, TaxRegistrationSchemeID.VA);
        }

        // Rechnungspositionen
        foreach (var pos in rechnung.Positionen.OrderBy(p => p.Position))
        {
            descriptor.AddTradeLineItem(
                name: pos.Beschreibung,
                unitCode: QuantityCodes.C62, // Stück
                grossUnitPrice: pos.Einzelpreis,
                netUnitPrice: pos.Einzelpreis,
                billedQuantity: pos.Menge,
                taxType: TaxTypes.VAT,
                categoryCode: TaxCategoryCodes.S, // Standard rate
                taxPercent: pos.Steuersatz
            );
        }

        // Zahlungsbedingungen
        if (!string.IsNullOrWhiteSpace(rechnung.Standort.IBAN))
        {
            descriptor.SetPaymentMeans(
                paymentCode: PaymentMeansTypeCodes.SEPACreditTransfer,
                information: $"IBAN: {rechnung.Standort.IBAN}"
            );

            if (!string.IsNullOrWhiteSpace(rechnung.Standort.BIC))
            {
                descriptor.AddCreditorFinancialAccount(
                    iban: rechnung.Standort.IBAN,
                    bic: rechnung.Standort.BIC
                );
            }
        }

        descriptor.AddTradePaymentTerms(
            description: "Zahlbar innerhalb von 14 Tagen ohne Abzug",
            dueDate: rechnung.Faelligkeitsdatum
        );

        // Setze Beträge
        descriptor.SetTotals(
            lineTotalAmount: rechnung.Nettobetrag,
            chargeTotalAmount: 0m,
            allowanceTotalAmount: 0m,
            taxBasisAmount: rechnung.Nettobetrag,
            taxTotalAmount: rechnung.Steuerbetrag,
            grandTotalAmount: rechnung.Bruttobetrag,
            duePayableAmount: rechnung.Bruttobetrag
        );

        // Generiere XML
        using var ms = new MemoryStream();
        descriptor.Save(ms, ZUGFeRDVersion.Version20, profile);
        ms.Position = 0;
        
        return Encoding.UTF8.GetString(ms.ToArray());
    }

    public bool ValidiereZUGFeRDXml(string xml)
    {
        if (string.IsNullOrWhiteSpace(xml))
            return false;

        try
        {
            // Basis-Validierung: Prüfe ob XML gültig ist
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var descriptor = InvoiceDescriptor.Load(ms);
            return descriptor != null;
        }
        catch
        {
            return false;
        }
    }

    private Profile MapProfile(RechnungFormat format)
    {
        return format switch
        {
            RechnungFormat.ZUGFeRD_Minimum => Profile.Minimum,
            RechnungFormat.ZUGFeRD_BasicWL => Profile.BasicWL,
            RechnungFormat.ZUGFeRD_Basic => Profile.Basic,
            RechnungFormat.ZUGFeRD_Comfort => Profile.Comfort,
            RechnungFormat.ZUGFeRD_Extended => Profile.Extended,
            _ => Profile.Comfort
        };
    }

    private byte[] ErstellePdfMitZUGFeRD(Rechnung rechnung, string zugferdXml)
    {
        // Für diese Demo erstellen wir ein einfaches Text-basiertes PDF
        // In Produktion würde hier PdfSharpCore oder eine andere Library verwendet werden
        // um ein vollständiges PDF/A-3 mit eingebettetem ZUGFeRD XML zu erstellen
        
        var sb = new StringBuilder();
        sb.AppendLine("%PDF-1.4");
        sb.AppendLine("1 0 obj");
        sb.AppendLine("<<");
        sb.AppendLine("/Type /Catalog");
        sb.AppendLine("/Pages 2 0 R");
        sb.AppendLine(">>");
        sb.AppendLine("endobj");
        sb.AppendLine("2 0 obj");
        sb.AppendLine("<<");
        sb.AppendLine("/Type /Pages");
        sb.AppendLine("/Kids [3 0 R]");
        sb.AppendLine("/Count 1");
        sb.AppendLine(">>");
        sb.AppendLine("endobj");
        sb.AppendLine("3 0 obj");
        sb.AppendLine("<<");
        sb.AppendLine("/Type /Page");
        sb.AppendLine("/Parent 2 0 R");
        sb.AppendLine("/MediaBox [0 0 595 842]");
        sb.AppendLine("/Contents 4 0 R");
        sb.AppendLine(">>");
        sb.AppendLine("endobj");
        sb.AppendLine("4 0 obj");
        sb.AppendLine("<<");
        sb.AppendLine($"/Length {GetPdfContentLength(rechnung)}");
        sb.AppendLine(">>");
        sb.AppendLine("stream");
        sb.AppendLine("BT");
        sb.AppendLine("/F1 12 Tf");
        sb.AppendLine("50 800 Td");
        sb.AppendLine($"(Rechnung {rechnung.RechnungsNummer}) Tj");
        sb.AppendLine("0 -20 Td");
        sb.AppendLine($"(Datum: {rechnung.ErstelltAm:dd.MM.yyyy}) Tj");
        sb.AppendLine("0 -20 Td");
        sb.AppendLine($"(Bruttobetrag: {rechnung.Bruttobetrag:F2} EUR) Tj");
        sb.AppendLine("ET");
        sb.AppendLine("endstream");
        sb.AppendLine("endobj");
        sb.AppendLine("xref");
        sb.AppendLine("0 5");
        sb.AppendLine("0000000000 65535 f");
        sb.AppendLine("0000000009 00000 n");
        sb.AppendLine("0000000058 00000 n");
        sb.AppendLine("0000000115 00000 n");
        sb.AppendLine("0000000214 00000 n");
        sb.AppendLine("trailer");
        sb.AppendLine("<<");
        sb.AppendLine("/Size 5");
        sb.AppendLine("/Root 1 0 R");
        sb.AppendLine(">>");
        sb.AppendLine("startxref");
        sb.AppendLine($"{sb.Length}");
        sb.AppendLine("%%EOF");

        return Encoding.ASCII.GetBytes(sb.ToString());
    }

    private int GetPdfContentLength(Rechnung rechnung)
    {
        // Ungefähre Länge des PDF-Inhalts
        return 200;
    }
}

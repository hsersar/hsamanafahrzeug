using FahrzeugZulassung.Domain.Entities;
using FahrzeugZulassung.Domain.Enums;
using FahrzeugZulassung.Infrastructure.Services;
using Xunit;

namespace FahrzeugZulassung.Tests;

public class ZUGFeRDServiceTests
{
    [Fact]
    public void GeneriereZUGFeRDXml_SollteGueltigesXmlErstellen()
    {
        // Arrange
        var service = new ZUGFeRDService();
        var rechnung = ErstelleTestRechnung();

        // Act
        var xml = service.GeneriereZUGFeRDXml(rechnung);

        // Assert
        Assert.NotNull(xml);
        Assert.NotEmpty(xml);
        Assert.Contains("<?xml", xml);
        Assert.Contains("rsm:CrossIndustryInvoice", xml);
    }

    [Fact]
    public void ValidiereZUGFeRDXml_MitGueltigemXml_SollteWahrSein()
    {
        // Arrange
        var service = new ZUGFeRDService();
        var rechnung = ErstelleTestRechnung();
        var xml = service.GeneriereZUGFeRDXml(rechnung);

        // Act
        var istGueltig = service.ValidiereZUGFeRDXml(xml);

        // Assert
        Assert.True(istGueltig);
    }

    [Fact]
    public void ValidiereZUGFeRDXml_MitUngueltigemXml_SollteFalschSein()
    {
        // Arrange
        var service = new ZUGFeRDService();

        // Act
        var istGueltig = service.ValidiereZUGFeRDXml("ungültiges xml");

        // Assert
        Assert.False(istGueltig);
    }

    [Fact]
    public async Task ErstelleERechnungAsync_SollteVollstaendigesErgebnisBringen()
    {
        // Arrange
        var service = new ZUGFeRDService();
        var rechnung = ErstelleTestRechnung();

        // Act
        var result = await service.ErstelleERechnungAsync(rechnung);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Xml);
        Assert.NotEmpty(result.Xml);
        Assert.True(result.PdfBytes.Length > 0);
        Assert.Equal("RE-2026-00001.pdf", result.DateiName);
    }

    [Fact]
    public void GeneriereZUGFeRDXml_SollteKundenDatenKorrektSetzen()
    {
        // Arrange
        var service = new ZUGFeRDService();
        var rechnung = ErstelleTestRechnung();

        // Act
        var xml = service.GeneriereZUGFeRDXml(rechnung);

        // Assert
        Assert.Contains("Max Mustermann", xml);
        Assert.Contains("Musterstrasse 123", xml);
        Assert.Contains("12345", xml);
        Assert.Contains("Musterstadt", xml);
    }

    [Fact]
    public void GeneriereZUGFeRDXml_SollteVerkaueferDatenKorrektSetzen()
    {
        // Arrange
        var service = new ZUGFeRDService();
        var rechnung = ErstelleTestRechnung();

        // Act
        var xml = service.GeneriereZUGFeRDXml(rechnung);

        // Assert
        Assert.Contains("Musterfirma GmbH", xml);
        Assert.Contains("Firmenstrasse 1", xml);
        Assert.Contains("54321", xml);
        Assert.Contains("Firmenstadt", xml);
    }

    [Fact]
    public void GeneriereZUGFeRDXml_SolltePositionenKorrektEinfuegen()
    {
        // Arrange
        var service = new ZUGFeRDService();
        var rechnung = ErstelleTestRechnung();

        // Act
        var xml = service.GeneriereZUGFeRDXml(rechnung);

        // Assert
        Assert.Contains("Fahrzeuganmeldung", xml);
        Assert.Contains("Wunschkennzeichen-Reservierung", xml);
    }

    private Rechnung ErstelleTestRechnung()
    {
        var standort = new Standort
        {
            Id = Guid.NewGuid(),
            Name = "Hauptstelle",
            Firmenname = "Musterfirma GmbH",
            Strasse = "Firmenstrasse 1",
            PLZ = "54321",
            Ort = "Firmenstadt",
            UStID = "DE123456789",
            IBAN = "DE89370400440532013000",
            BIC = "COBADEFFXXX",
            Bankname = "Commerzbank"
        };

        var kunde = new Kunde
        {
            Id = Guid.NewGuid(),
            Vorname = "Max",
            Nachname = "Mustermann",
            Email = "max@mustermann.de",
            Strasse = "Musterstrasse 123",
            PLZ = "12345",
            Ort = "Musterstadt"
        };

        var rechnung = new Rechnung
        {
            Id = Guid.NewGuid(),
            RechnungsNummer = "RE-2026-00001",
            ErstelltAm = new DateTime(2026, 2, 16),
            Faelligkeitsdatum = new DateTime(2026, 3, 2),
            Format = RechnungFormat.ZUGFeRD_Comfort,
            KundeId = kunde.Id,
            StandortId = standort.Id,
            Kunde = kunde,
            Standort = standort,
            Nettobetrag = 150.00m,
            Steuerbetrag = 28.50m,
            Bruttobetrag = 178.50m
        };

        rechnung.Positionen.Add(new RechnungsPosition
        {
            Id = Guid.NewGuid(),
            RechnungId = rechnung.Id,
            Position = 1,
            Beschreibung = "Fahrzeuganmeldung",
            Menge = 1,
            Einzelpreis = 100.00m,
            Nettobetrag = 100.00m,
            Steuersatz = 19m,
            Steuerbetrag = 19.00m,
            Bruttobetrag = 119.00m
        });

        rechnung.Positionen.Add(new RechnungsPosition
        {
            Id = Guid.NewGuid(),
            RechnungId = rechnung.Id,
            Position = 2,
            Beschreibung = "Wunschkennzeichen-Reservierung",
            Menge = 1,
            Einzelpreis = 50.00m,
            Nettobetrag = 50.00m,
            Steuersatz = 19m,
            Steuerbetrag = 9.50m,
            Bruttobetrag = 59.50m
        });

        return rechnung;
    }
}

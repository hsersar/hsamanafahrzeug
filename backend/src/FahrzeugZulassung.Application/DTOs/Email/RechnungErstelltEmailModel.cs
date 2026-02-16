namespace FahrzeugZulassung.Application.DTOs.Email;

public class RechnungErstelltEmailModel
{
    public string KundenName { get; set; } = string.Empty;
    public string RechnungsNummer { get; set; } = string.Empty;
    public decimal NettoSumme { get; set; }
    public decimal MwSt { get; set; }
    public decimal BruttoSumme { get; set; }
    public DateTime Zahlungsfrist { get; set; }
    public string ZahlungsUrl { get; set; } = string.Empty;
    public string GiroCodeBase64 { get; set; } = string.Empty;
}

namespace FahrzeugZulassung.Application.DTOs.Email;

public class ZahlungsBestaetigungEmailModel
{
    public string KundenName { get; set; } = string.Empty;
    public string RechnungsNummer { get; set; } = string.Empty;
    public decimal Betrag { get; set; }
    public string ZahlungsMethode { get; set; } = string.Empty;
    public DateTime ZahlungsDatum { get; set; }
    public string RechnungsDownloadUrl { get; set; } = string.Empty;
}

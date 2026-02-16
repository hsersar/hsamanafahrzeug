namespace FahrzeugZulassung.Application.DTOs.Email;

public class AuftragErstelltEmailModel
{
    public string KundenName { get; set; } = string.Empty;
    public string AuftragTyp { get; set; } = string.Empty;
    public string Kennzeichen { get; set; } = string.Empty;
    public string TrackingCode { get; set; } = string.Empty;
    public string TrackingUrl { get; set; } = string.Empty;
    public string QRCodeBase64 { get; set; } = string.Empty;
    public DateTime ErstelltAm { get; set; }
    public string StandortName { get; set; } = string.Empty;
}

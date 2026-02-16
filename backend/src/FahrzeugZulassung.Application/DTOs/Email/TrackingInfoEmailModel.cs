namespace FahrzeugZulassung.Application.DTOs.Email;

public class TrackingInfoEmailModel
{
    public string KundenName { get; set; } = string.Empty;
    public string TrackingCode { get; set; } = string.Empty;
    public string TrackingUrl { get; set; } = string.Empty;
    public string QRCodeBase64 { get; set; } = string.Empty;
}

namespace FahrzeugZulassung.Application.DTOs.Email;

public class StatusUpdateEmailModel
{
    public string KundenName { get; set; } = string.Empty;
    public string AlterStatus { get; set; } = string.Empty;
    public string NeuerStatus { get; set; } = string.Empty;
    public string TrackingUrl { get; set; } = string.Empty;
    public string? Handlungsaufforderung { get; set; }
    public int Fortschritt { get; set; } // Prozent 0-100
}

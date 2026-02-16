namespace FahrzeugZulassung.Application.DTOs.Email;

public class AuftragAbgeschlossenEmailModel
{
    public string KundenName { get; set; } = string.Empty;
    public string AuftragTyp { get; set; } = string.Empty;
    public string? Kennzeichen { get; set; }
    public DateTime AbschlussDatum { get; set; }
    public string BewertungsUrl { get; set; } = string.Empty;
}

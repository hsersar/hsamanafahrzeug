namespace FahrzeugZulassung.Application.DTOs.Email;

public class PasswortZuruecksetzenEmailModel
{
    public string KundenName { get; set; } = string.Empty;
    public string ResetLink { get; set; } = string.Empty;
    public DateTime GueltigBis { get; set; }
}

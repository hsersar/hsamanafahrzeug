namespace FahrzeugZulassung.Application.DTOs.Email;

public class MitarbeiterAktiviertEmailModel
{
    public string MitarbeiterName { get; set; } = string.Empty;
    public string LoginUrl { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

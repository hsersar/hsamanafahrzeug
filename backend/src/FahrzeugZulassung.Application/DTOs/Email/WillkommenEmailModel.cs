namespace FahrzeugZulassung.Application.DTOs.Email;

public class WillkommenEmailModel
{
    public string KundenName { get; set; } = string.Empty;
    public string AktivierungsLink { get; set; } = string.Empty;
}

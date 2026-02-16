namespace FahrzeugZulassung.Application.DTOs.Email;

public class DokumenteAngefordertEmailModel
{
    public string KundenName { get; set; } = string.Empty;
    public List<string> FehlendeDokumente { get; set; } = new();
    public string UploadUrl { get; set; } = string.Empty;
    public DateTime Frist { get; set; }
}

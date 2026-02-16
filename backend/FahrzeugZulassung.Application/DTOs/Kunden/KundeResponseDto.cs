namespace FahrzeugZulassung.Application.DTOs.Kunden;

public class KundeResponseDto
{
    public Guid Id { get; set; }
    public string Vorname { get; set; } = string.Empty;
    public string Nachname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefon { get; set; } = string.Empty;
    public string Strasse { get; set; } = string.Empty;
    public string PLZ { get; set; } = string.Empty;
    public string Ort { get; set; } = string.Empty;
    public string? Bemerkungen { get; set; }
    public DateTime ErstelltAm { get; set; }
    public DateTime? AktualisiertAm { get; set; }
}

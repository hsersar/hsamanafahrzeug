namespace FahrzeugZulassung.Application.DTOs.Standorte;

public class StandortResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Strasse { get; set; } = string.Empty;
    public string PLZ { get; set; } = string.Empty;
    public string Ort { get; set; } = string.Empty;
    public string? Telefon { get; set; }
    public string? Email { get; set; }
    public bool Aktiv { get; set; }
    public DateTime ErstelltAm { get; set; }
    public DateTime? AktualisiertAm { get; set; }
}

namespace FahrzeugZulassung.Application.DTOs.Mitarbeiter;

public class MitarbeiterResponseDto
{
    public Guid Id { get; set; }
    public string Vorname { get; set; } = string.Empty;
    public string Nachname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefon { get; set; }
    public string? Position { get; set; }
    public bool Aktiv { get; set; }
    public DateTime ErstelltAm { get; set; }
    public DateTime? AktualisiertAm { get; set; }
    public StandortInfoDto Standort { get; set; } = new();
}

public class StandortInfoDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Ort { get; set; } = string.Empty;
}

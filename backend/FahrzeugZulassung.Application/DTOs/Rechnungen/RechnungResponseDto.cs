namespace FahrzeugZulassung.Application.DTOs.Rechnungen;

public class RechnungResponseDto
{
    public Guid Id { get; set; }
    public string Rechnungsnummer { get; set; } = string.Empty;
    public DateTime Rechnungsdatum { get; set; }
    public DateTime? Faelligkeitsdatum { get; set; }
    public decimal Betrag { get; set; }
    public string? Beschreibung { get; set; }
    public bool IstBezahlt { get; set; }
    public DateTime? BezahltAm { get; set; }
    public DateTime ErstelltAm { get; set; }
    public AuftragInfoDto Auftrag { get; set; } = new();
    public List<PositionDto> Positionen { get; set; } = new();
}

public class AuftragInfoDto
{
    public Guid Id { get; set; }
    public string Typ { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string KundeName { get; set; } = string.Empty;
}

public class PositionDto
{
    public Guid Id { get; set; }
    public string Beschreibung { get; set; } = string.Empty;
    public int Menge { get; set; }
    public decimal Einzelpreis { get; set; }
    public decimal Gesamtpreis { get; set; }
}

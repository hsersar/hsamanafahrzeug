namespace FahrzeugZulassung.Domain.Entities;

public class Fahrzeug
{
    public Guid Id { get; set; }
    public string FIN { get; set; } = string.Empty; // Fahrzeug-Identifizierungsnummer
    public string? Kennzeichen { get; set; }
    public string Marke { get; set; } = string.Empty;
    public string Modell { get; set; } = string.Empty;
    public DateTime? Erstzulassung { get; set; }
    public string? Farbe { get; set; }
    public int? Hubraum { get; set; }
    public int? Leistung { get; set; }
    public string? Kraftstoffart { get; set; }
    
    public DateTime ErstelltAm { get; set; } = DateTime.UtcNow;
    public DateTime? GeaendertAm { get; set; }
    
    // Navigation properties
    public ICollection<Auftrag> Auftraege { get; set; } = new List<Auftrag>();
}

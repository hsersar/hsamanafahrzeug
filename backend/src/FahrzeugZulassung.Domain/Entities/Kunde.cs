namespace FahrzeugZulassung.Domain.Entities;

public class Kunde
{
    public Guid Id { get; set; }
    public Guid BenutzerId { get; set; }
    public Benutzer Benutzer { get; set; } = null!;
    
    // Personal data
    public string Vorname { get; set; } = string.Empty;
    public string Nachname { get; set; } = string.Empty;
    public string Strasse { get; set; } = string.Empty;
    public string Hausnummer { get; set; } = string.Empty;
    public string PLZ { get; set; } = string.Empty;
    public string Ort { get; set; } = string.Empty;
    public DateTime? Geburtsdatum { get; set; }
    public string Telefon { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    // GDPR
    public bool DatenschutzAkzeptiert { get; set; } = false;
    public DateTime? DatenschutzAkzeptiertAm { get; set; }
    
    public DateTime ErstelltAm { get; set; } = DateTime.UtcNow;
    public DateTime? GeaendertAm { get; set; }
    
    // Navigation properties
    public ICollection<Auftrag> Auftraege { get; set; } = new List<Auftrag>();
}

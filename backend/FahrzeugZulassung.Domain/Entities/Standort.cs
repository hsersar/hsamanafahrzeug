namespace FahrzeugZulassung.Domain.Entities;

public class Standort
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Strasse { get; set; } = string.Empty;
    public string Hausnummer { get; set; } = string.Empty;
    public string PLZ { get; set; } = string.Empty;
    public string Ort { get; set; } = string.Empty;
    public string Telefon { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IstAktiv { get; set; } = true;
    public DateTime ErstelltAm { get; set; } = DateTime.UtcNow;
    public DateTime? GeaendertAm { get; set; }
    
    // Navigation properties
    public ICollection<Benutzer> Mitarbeiter { get; set; } = new List<Benutzer>();
    public ICollection<Auftrag> Auftraege { get; set; } = new List<Auftrag>();
}

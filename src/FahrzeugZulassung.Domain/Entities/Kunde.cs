namespace FahrzeugZulassung.Domain.Entities;

public class Kunde
{
    public Guid Id { get; set; }
    public string Vorname { get; set; } = string.Empty;
    public string Nachname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefon { get; set; }
    
    // Adressdaten für ZUGFeRD
    public string Strasse { get; set; } = string.Empty;
    public string PLZ { get; set; } = string.Empty;
    public string Ort { get; set; } = string.Empty;
    public string? Firmenname { get; set; }       // Falls Geschäftskunde
    public string? UStID { get; set; }            // Falls Geschäftskunde
    
    public DateTime ErstelltAm { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public ICollection<Rechnung> Rechnungen { get; set; } = new List<Rechnung>();
}

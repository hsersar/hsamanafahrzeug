namespace FahrzeugZulassung.Domain.Entities;

public class Standort
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    // Rechnungsdaten für ZUGFeRD
    public string Firmenname { get; set; } = string.Empty;
    public string Strasse { get; set; } = string.Empty;
    public string PLZ { get; set; } = string.Empty;
    public string Ort { get; set; } = string.Empty;
    public string? UStID { get; set; }           // USt-Identifikationsnummer
    public string? Steuernummer { get; set; }
    public string? IBAN { get; set; }
    public string? BIC { get; set; }
    public string? Bankname { get; set; }
    public string? HandelsregisterNr { get; set; }
    public string? Geschaeftsfuehrer { get; set; }
    
    public DateTime ErstelltAm { get; set; } = DateTime.UtcNow;
}

using FahrzeugZulassung.Domain.Enums;

namespace FahrzeugZulassung.Domain.Entities;

public class Rechnung
{
    public Guid Id { get; set; }
    public string RechnungsNummer { get; set; } = string.Empty;  // Fortlaufend, z.B. "RE-2026-00001"
    public DateTime ErstelltAm { get; set; } = DateTime.UtcNow;
    public DateTime Faelligkeitsdatum { get; set; }
    
    // Beträge
    public decimal Nettobetrag { get; set; }
    public decimal Steuerbetrag { get; set; }
    public decimal Bruttobetrag { get; set; }
    
    // ZUGFeRD spezifisch
    public RechnungFormat Format { get; set; } = RechnungFormat.ZUGFeRD_Comfort;
    public string? ZUGFeRDXml { get; set; }
    public string? PdfDateiPfad { get; set; }
    public string? Leitweg_ID { get; set; }  // Leitweg-ID für behördliche Rechnungen
    
    // Fremdschlüssel
    public Guid KundeId { get; set; }
    public Guid StandortId { get; set; }
    
    // Navigation
    public Kunde Kunde { get; set; } = null!;
    public Standort Standort { get; set; } = null!;
    public ICollection<RechnungsPosition> Positionen { get; set; } = new List<RechnungsPosition>();
}

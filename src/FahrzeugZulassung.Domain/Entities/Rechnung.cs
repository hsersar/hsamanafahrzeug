using FahrzeugZulassung.Domain.Enums;

namespace FahrzeugZulassung.Domain.Entities;

public class Rechnung
{
    public Guid Id { get; set; }
    public string Rechnungsnummer { get; set; } = string.Empty;
    public Guid AuftragId { get; set; }
    public decimal Betrag { get; set; }
    public string Waehrung { get; set; } = "EUR";
    public DateTime ErstelltAm { get; set; } = DateTime.UtcNow;
    public DateTime? BezahltAm { get; set; }
    public bool IstBezahlt { get; set; }
    
    // Neue Properties für Multi-Payment
    public ZahlungsMethode? GewaehlteZahlungsMethode { get; set; }
    public ICollection<Zahlung> Zahlungen { get; set; } = new List<Zahlung>();
    
    // Navigation
    public Auftrag Auftrag { get; set; } = null!;
}

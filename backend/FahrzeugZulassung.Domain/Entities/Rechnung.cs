using FahrzeugZulassung.Domain.Enums;

namespace FahrzeugZulassung.Domain.Entities;

public class Rechnung
{
    public Guid Id { get; set; }
    public string RechnungNummer { get; set; } = string.Empty;
    
    public Guid AuftragId { get; set; }
    public Auftrag Auftrag { get; set; } = null!;
    
    public decimal Betrag { get; set; }
    public RechnungStatus Status { get; set; } = RechnungStatus.Offen;
    
    public DateTime Faellig { get; set; }
    public DateTime? BezahltAm { get; set; }
    
    public string? Zahlungsmethode { get; set; }
    public string? ZahlungsReferenz { get; set; }
    
    public DateTime ErstelltAm { get; set; } = DateTime.UtcNow;
    public DateTime? GeaendertAm { get; set; }
}

using FahrzeugZulassung.Domain.Enums;

namespace FahrzeugZulassung.Domain.Entities;

public class AuftragStatusHistorie
{
    public Guid Id { get; set; }
    public Guid AuftragId { get; set; }
    public AuftragStatus AlterStatus { get; set; }
    public AuftragStatus NeuerStatus { get; set; }
    public string? Kommentar { get; set; }
    public string? BearbeitetVon { get; set; }  // Name des Mitarbeiters
    public DateTime ZeitpunktAm { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public Auftrag Auftrag { get; set; } = null!;
}

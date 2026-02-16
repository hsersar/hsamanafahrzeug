using FahrzeugZulassung.Domain.Enums;

namespace FahrzeugZulassung.Domain.Entities;

public class Auftrag
{
    public Guid Id { get; set; }
    public Guid KundeId { get; set; }
    public AuftragTyp Typ { get; set; }
    public AuftragStatus Status { get; set; } = AuftragStatus.Eingegangen;
    public string? Kennzeichen { get; set; }
    public string TrackingCode { get; set; } = string.Empty;
    public DateTime ErstelltAm { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public Benutzer Kunde { get; set; } = null!;
    public EVBNummer? EVBNummer { get; set; }
}

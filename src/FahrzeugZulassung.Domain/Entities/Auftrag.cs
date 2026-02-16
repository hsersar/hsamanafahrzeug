using FahrzeugZulassung.Domain.Enums;

namespace FahrzeugZulassung.Domain.Entities;

public class Auftrag
{
    public Guid Id { get; set; }
    public string Antragsnummer { get; set; } = string.Empty;
    public string AuftragTyp { get; set; } = string.Empty; // Anmeldung, Abmeldung, Ummeldung
    public AuftragStatus Status { get; set; } = AuftragStatus.Eingereicht;
    public string? Kennzeichen { get; set; }
    public string KundenEmail { get; set; } = string.Empty;
    public string? KundenTelefon { get; set; }
    public DateTime ErstelltAm { get; set; } = DateTime.UtcNow;
    public DateTime? LetzteAktualisierung { get; set; }
    
    // Navigation
    public Rechnung? Rechnung { get; set; }
    public AuftragTracking? Tracking { get; set; }
    public ICollection<AuftragStatusHistorie> StatusHistorie { get; set; } = new List<AuftragStatusHistorie>();
}

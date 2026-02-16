using Microsoft.AspNetCore.Identity;
using FahrzeugZulassung.Domain.Enums;

namespace FahrzeugZulassung.Domain.Entities;

public class Benutzer : IdentityUser<Guid>
{
    public string Vorname { get; set; } = string.Empty;
    public string Nachname { get; set; } = string.Empty;
    public BenutzerRolle Rolle { get; set; }
    public Guid? StandortId { get; set; }
    public Standort? Standort { get; set; }
    public bool IstAktiv { get; set; } = true;
    public int FehlgeschlageneLoginVersuche { get; set; } = 0;
    public DateTime? GesperrtBis { get; set; }
    public DateTime ErstelltAm { get; set; } = DateTime.UtcNow;
    public DateTime? GeaendertAm { get; set; }
    
    // Navigation properties
    public ICollection<Auftrag> ErstellteAuftraege { get; set; } = new List<Auftrag>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}

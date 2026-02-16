namespace FahrzeugZulassung.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; }
    
    public Guid? BenutzerId { get; set; }
    public Benutzer? Benutzer { get; set; }
    
    public string Aktion { get; set; } = string.Empty;
    public string Entitaet { get; set; } = string.Empty;
    public string? EntitaetId { get; set; }
    
    public string? AlteWerte { get; set; }
    public string? NeueWerte { get; set; }
    
    public string IPAdresse { get; set; } = string.Empty;
    public string? UserAgent { get; set; }
    
    public DateTime Zeitstempel { get; set; } = DateTime.UtcNow;
}

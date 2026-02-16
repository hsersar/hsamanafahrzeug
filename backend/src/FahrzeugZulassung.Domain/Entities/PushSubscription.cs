namespace FahrzeugZulassung.Domain.Entities;

public class PushSubscription
{
    public Guid Id { get; set; }
    public Guid BenutzerId { get; set; }
    public string Endpoint { get; set; } = string.Empty;
    public string P256dhKey { get; set; } = string.Empty;
    public string AuthKey { get; set; } = string.Empty;
    public string? UserAgent { get; set; }
    public DateTime ErstelltAm { get; set; } = DateTime.UtcNow;
    public DateTime? LetzteNutzung { get; set; }
    public bool IstAktiv { get; set; } = true;
    
    // Navigation
    public Benutzer Benutzer { get; set; } = null!;
}

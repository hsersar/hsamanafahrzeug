namespace FahrzeugZulassung.Domain.Entities;

public class Benutzer
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Telefon { get; set; }
    public DateTime ErstelltAm { get; set; } = DateTime.UtcNow;
    public bool IstAktiv { get; set; } = true;
    
    // Navigation
    public ICollection<PushSubscription> PushSubscriptions { get; set; } = new List<PushSubscription>();
}

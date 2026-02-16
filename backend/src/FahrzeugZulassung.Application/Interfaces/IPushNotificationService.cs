using FahrzeugZulassung.Application.DTOs.Push;

namespace FahrzeugZulassung.Application.Interfaces;

public interface IPushNotificationService
{
    Task SubscribeAsync(Guid benutzerId, PushSubscriptionDto subscription);
    Task UnsubscribeAsync(Guid benutzerId, string endpoint);
    Task SendeNotificationAsync(Guid benutzerId, PushNachricht nachricht);
    Task SendeAnAlleMitarbeiterImStandortAsync(Guid standortId, PushNachricht nachricht);
    Task SendeAnKundeAsync(Guid kundeId, PushNachricht nachricht);
}

public class PushNachricht
{
    public string Titel { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string? Icon { get; set; }           // URL zum Icon
    public string? Badge { get; set; }          // URL zum Badge
    public string? Url { get; set; }            // Click-Action URL
    public string? Tag { get; set; }            // Notification Tag für Gruppierung
    public Dictionary<string, string>? Data { get; set; }  // Custom Data
}

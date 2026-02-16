using FahrzeugZulassung.Application.Interfaces;
using FahrzeugZulassung.Application.DTOs.Push;
using WebPush;
using System.Text.Json;

namespace FahrzeugZulassung.Infrastructure.Services;

public class WebPushNotificationService : IPushNotificationService
{
    private readonly string _vapidPublicKey;
    private readonly string _vapidPrivateKey;
    private readonly string _vapidSubject;

    public WebPushNotificationService(string vapidPublicKey, string vapidPrivateKey, string vapidSubject)
    {
        _vapidPublicKey = vapidPublicKey;
        _vapidPrivateKey = vapidPrivateKey;
        _vapidSubject = vapidSubject;
    }

    public async Task SubscribeAsync(Guid benutzerId, PushSubscriptionDto subscription)
    {
        // In einer echten Implementierung würde hier die Subscription in der Datenbank gespeichert
        await Task.CompletedTask;
    }

    public async Task UnsubscribeAsync(Guid benutzerId, string endpoint)
    {
        // In einer echten Implementierung würde hier die Subscription aus der Datenbank entfernt
        await Task.CompletedTask;
    }

    public async Task SendeNotificationAsync(Guid benutzerId, PushNachricht nachricht)
    {
        // In einer echten Implementierung würde hier die Subscription aus der Datenbank geladen
        // und die Benachrichtigung gesendet
        await Task.CompletedTask;
    }

    public async Task SendeAnAlleMitarbeiterImStandortAsync(Guid standortId, PushNachricht nachricht)
    {
        // In einer echten Implementierung würden hier alle Subscriptions der Mitarbeiter
        // am Standort geladen und Benachrichtigungen gesendet
        await Task.CompletedTask;
    }

    public async Task SendeAnKundeAsync(Guid kundeId, PushNachricht nachricht)
    {
        // In einer echten Implementierung würde hier die Subscription des Kunden geladen
        // und die Benachrichtigung gesendet
        await Task.CompletedTask;
    }

    private async Task SendePushAsync(string endpoint, string p256dh, string auth, PushNachricht nachricht)
    {
        try
        {
            var pushSubscription = new PushSubscription(endpoint, p256dh, auth);
            var vapidDetails = new VapidDetails(_vapidSubject, _vapidPublicKey, _vapidPrivateKey);

            var payload = JsonSerializer.Serialize(new
            {
                title = nachricht.Titel,
                body = nachricht.Text,
                icon = nachricht.Icon,
                badge = nachricht.Badge,
                data = nachricht.Data,
                tag = nachricht.Tag
            });

            var webPushClient = new WebPushClient();
            await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
        }
        catch (WebPushException)
        {
            // Ungültige Subscription - sollte aus der Datenbank entfernt werden
        }
    }
}

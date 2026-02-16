using Microsoft.AspNetCore.SignalR;

namespace FahrzeugZulassung.API.Hubs;

public class TrackingHub : Hub
{
    public async Task SubscribeToTracking(string trackingCode, string token)
    {
        // Token validation would happen here in a real implementation
        await Groups.AddToGroupAsync(Context.ConnectionId, $"tracking-{trackingCode}");
        await Clients.Caller.SendAsync("Subscribed", trackingCode);
    }

    public async Task UnsubscribeFromTracking(string trackingCode)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"tracking-{trackingCode}");
        await Clients.Caller.SendAsync("Unsubscribed", trackingCode);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}

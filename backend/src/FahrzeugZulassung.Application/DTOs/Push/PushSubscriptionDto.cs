namespace FahrzeugZulassung.Application.DTOs.Push;

public class PushSubscriptionDto
{
    public string Endpoint { get; set; } = string.Empty;
    public string P256dhKey { get; set; } = string.Empty;
    public string AuthKey { get; set; } = string.Empty;
    public string? UserAgent { get; set; }
}

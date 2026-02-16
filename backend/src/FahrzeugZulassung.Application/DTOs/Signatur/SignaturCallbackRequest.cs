namespace FahrzeugZulassung.Application.DTOs.Signatur;

public class SignaturCallbackRequest
{
    public Guid SignaturId { get; set; }
    public string? ProviderSessionId { get; set; }
    public string? Status { get; set; }
    public Dictionary<string, object>? ProviderData { get; set; }
}

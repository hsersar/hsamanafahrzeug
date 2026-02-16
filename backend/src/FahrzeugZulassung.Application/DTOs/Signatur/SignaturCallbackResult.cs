namespace FahrzeugZulassung.Application.DTOs.Signatur;

public class SignaturCallbackResult
{
    public bool Erfolg { get; set; }
    public Guid? SignaturId { get; set; }
    public string? Status { get; set; }
    public string? FehlerNachricht { get; set; }
}

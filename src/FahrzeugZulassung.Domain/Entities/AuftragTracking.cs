namespace FahrzeugZulassung.Domain.Entities;

public class AuftragTracking
{
    public Guid Id { get; set; }
    public Guid AuftragId { get; set; }
    public string TrackingCode { get; set; } = string.Empty;  // z.B. "TRK-A7X9-K2M4-P8Q1"
    public string TrackingToken { get; set; } = string.Empty;  // Kryptografischer Token (SHA256)
    public string? QRCodeUrl { get; set; }                     // URL zum QR-Code Bild
    
    // Benachrichtigungen
    public bool EmailGesendet { get; set; }
    public bool SMSGesendet { get; set; }
    public DateTime? EmailGesendetAm { get; set; }
    public DateTime? SMSGesendetAm { get; set; }
    
    // Timestamps
    public DateTime ErstelltAm { get; set; } = DateTime.UtcNow;
    public DateTime? LetzterZugriffAm { get; set; }
    public int ZugriffAnzahl { get; set; }
    
    // Navigation
    public Auftrag Auftrag { get; set; } = null!;
}

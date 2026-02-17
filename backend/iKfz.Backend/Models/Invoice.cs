namespace iKfz.Backend.Models;

public class Invoice
{
    public int Id { get; set; }
    public required string UserId { get; set; }
    public required string Rechnungsnummer { get; set; }  // e.g. "RE-2026-00001"
    public int? RegistrationRequestId { get; set; }
    public required string Beschreibung { get; set; }
    public decimal Nettobetrag { get; set; }
    public decimal MwstSatz { get; set; } = 19m;
    public decimal MwstBetrag { get; set; }
    public decimal Bruttobetrag { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Offen;
    public DateTime Rechnungsdatum { get; set; } = DateTime.UtcNow;
    public DateTime Faelligkeitsdatum { get; set; }
    public DateTime? BezahltAm { get; set; }
    public string? Zahlungsmethode { get; set; }  // "SEPA", "Kreditkarte", etc.
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public enum InvoiceStatus
{
    Offen = 0,
    Bezahlt = 1,
    Ueberfaellig = 2,
    Storniert = 3
}

public class Announcement
{
    public int Id { get; set; }
    public required string Titel { get; set; }
    public required string Nachricht { get; set; }
    public string Typ { get; set; } = "info";  // info, warning, success, error
    public bool Aktiv { get; set; } = true;
    public DateTime? GueltigVon { get; set; }
    public DateTime? GueltigBis { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

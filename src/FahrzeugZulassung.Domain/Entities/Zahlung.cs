using FahrzeugZulassung.Domain.Enums;

namespace FahrzeugZulassung.Domain.Entities;

public class Zahlung
{
    public Guid Id { get; set; }
    public Guid RechnungId { get; set; }
    public ZahlungsMethode Methode { get; set; }
    public ZahlungsStatus Status { get; set; } = ZahlungsStatus.Ausstehend;
    public decimal Betrag { get; set; }
    public string Waehrung { get; set; } = "EUR";
    
    // Payment Provider Referenzen
    public string? StripePaymentIntentId { get; set; }
    public string? StripeClientSecret { get; set; }
    public string? PayPalOrderId { get; set; }
    public string? PayPalApprovalUrl { get; set; }
    public string? UeberweisungsReferenz { get; set; }  // z.B. "RE-2026-00042"
    
    // Banküberweisung Details
    public string? EmpfaengerIBAN { get; set; }
    public string? EmpfaengerBIC { get; set; }
    public string? EmpfaengerBank { get; set; }
    public string? Verwendungszweck { get; set; }
    
    // Timestamps
    public DateTime ErstelltAm { get; set; } = DateTime.UtcNow;
    public DateTime? BezahltAm { get; set; }
    public DateTime? StorniertAm { get; set; }
    
    // Audit
    public string? ProviderAntwort { get; set; }  // JSON Response vom Provider
    public string? FehlerNachricht { get; set; }
    
    // Navigation
    public Rechnung Rechnung { get; set; } = null!;
}

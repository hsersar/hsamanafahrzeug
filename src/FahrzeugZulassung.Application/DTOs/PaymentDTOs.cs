using FahrzeugZulassung.Domain.Enums;

namespace FahrzeugZulassung.Application.DTOs;

public class PaymentInitResult
{
    public Guid ZahlungId { get; set; }
    public ZahlungsMethode Methode { get; set; }
    public string? RedirectUrl { get; set; }      // PayPal Approval URL
    public string? ClientSecret { get; set; }     // Stripe Client Secret
    public UeberweisungsDetails? Ueberweisung { get; set; }  // Bankdaten
    public bool Erfolg { get; set; }
    public string? FehlerNachricht { get; set; }
}

public class UeberweisungsDetails
{
    public string Empfaenger { get; set; } = string.Empty;
    public string IBAN { get; set; } = string.Empty;
    public string BIC { get; set; } = string.Empty;
    public string Bank { get; set; } = string.Empty;
    public string Verwendungszweck { get; set; } = string.Empty;
    public decimal Betrag { get; set; }
}

public class PaymentResult
{
    public Guid ZahlungId { get; set; }
    public ZahlungsStatus Status { get; set; }
    public bool Erfolg { get; set; }
    public string? FehlerNachricht { get; set; }
}

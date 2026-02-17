namespace iKfz.Backend.Models;

/// <summary>
/// A payment method (bank account, credit card, etc.) belonging to a user.
/// Each user can have multiple payment methods, one marked as default.
/// </summary>
public class PaymentMethod
{
    public int Id { get; set; }

    /// <summary>User ID from authentication (sub claim).</summary>
    public required string UserId { get; set; }

    /// <summary>Type of payment: "SEPA", "Kreditkarte", "PayPal", "Giropay".</summary>
    public string Typ { get; set; } = "SEPA";

    /// <summary>Display label, e.g. "Mein Geschäftskonto".</summary>
    public string Bezeichnung { get; set; } = string.Empty;

    // SEPA / Bank details
    public string? Kontoinhaber { get; set; }
    public string? IBAN { get; set; }
    public string? BIC { get; set; }
    public string? Bankname { get; set; }

    // Credit card
    public string? KartenNummer { get; set; }   // last 4 digits only
    public string? KartenInhaber { get; set; }
    public string? GueltigBis { get; set; }      // "MM/YY"

    // PayPal / Giropay
    public string? PaypalEmail { get; set; }

    /// <summary>Whether this is the default payment method.</summary>
    public bool IstStandard { get; set; }

    /// <summary>Whether a SEPA-Lastschrift mandate is active.</summary>
    public bool SepaMandatErteilt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

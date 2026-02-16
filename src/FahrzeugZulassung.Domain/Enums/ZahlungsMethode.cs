namespace FahrzeugZulassung.Domain.Enums;

public enum ZahlungsMethode
{
    Ueberweisung = 1,       // Klassische Banküberweisung (SEPA)
    Kreditkarte = 2,        // Stripe Card Payment
    PayPal = 3,             // PayPal Checkout
    SEPALastschrift = 4,    // Stripe SEPA Direct Debit
    Sofortueberweisung = 5  // Klarna/Sofort (optional)
}

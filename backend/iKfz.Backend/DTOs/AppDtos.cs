using System.ComponentModel.DataAnnotations;

namespace iKfz.Backend.DTOs;

public class UserProfileDto
{
    public string UserId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime LastLoginUtc { get; set; }
}

// ── Personal Profile DTOs ──

public class PersonalProfileDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Anrede { get; set; } = "Herr";
    public string? Titel { get; set; }
    public string Vorname { get; set; } = string.Empty;
    public string Nachname { get; set; } = string.Empty;
    public DateTime? Geburtsdatum { get; set; }
    public string Strasse { get; set; } = string.Empty;
    public string Hausnummer { get; set; } = string.Empty;
    public string Plz { get; set; } = string.Empty;
    public string Ort { get; set; } = string.Empty;
    public string Telefon { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ProfilbildUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class SavePersonalProfileDto
{
    [Required, MaxLength(20)]
    public string Anrede { get; set; } = "Herr";

    [MaxLength(50)]
    public string? Titel { get; set; }

    [Required, MaxLength(100)]
    public string Vorname { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Nachname { get; set; } = string.Empty;

    public DateTime? Geburtsdatum { get; set; }

    [Required, MaxLength(200)]
    public string Strasse { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string Hausnummer { get; set; } = string.Empty;

    [Required, MaxLength(10)]
    public string Plz { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Ort { get; set; } = string.Empty;

    [MaxLength(30)]
    public string Telefon { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; set; } = string.Empty;
}

// ── Company Profile DTOs ──

public class CompanyProfileDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;

    public string Firmenname { get; set; } = string.Empty;
    public string Rechtsform { get; set; } = string.Empty;
    public string Handelsregisternummer { get; set; } = string.Empty;
    public string UstIdNr { get; set; } = string.Empty;

    public string Strasse { get; set; } = string.Empty;
    public string Hausnummer { get; set; } = string.Empty;
    public string Plz { get; set; } = string.Empty;
    public string Ort { get; set; } = string.Empty;
    public string Telefon { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;

    public string AnsprechpartnerAnrede { get; set; } = "Herr";
    public string AnsprechpartnerVorname { get; set; } = string.Empty;
    public string AnsprechpartnerNachname { get; set; } = string.Empty;
    public string AnsprechpartnerTelefon { get; set; } = string.Empty;
    public string AnsprechpartnerEmail { get; set; } = string.Empty;
    public string? AnsprechpartnerPosition { get; set; }

    public string? LogoUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class SaveCompanyProfileDto
{
    [Required, MaxLength(200)]
    public string Firmenname { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Rechtsform { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Handelsregisternummer { get; set; } = string.Empty;

    [MaxLength(30)]
    public string UstIdNr { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Strasse { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string Hausnummer { get; set; } = string.Empty;

    [Required, MaxLength(10)]
    public string Plz { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Ort { get; set; } = string.Empty;

    [MaxLength(30)]
    public string Telefon { get; set; } = string.Empty;

    [EmailAddress, MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(256)]
    public string Website { get; set; } = string.Empty;

    [MaxLength(20)]
    public string AnsprechpartnerAnrede { get; set; } = "Herr";

    [Required, MaxLength(100)]
    public string AnsprechpartnerVorname { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string AnsprechpartnerNachname { get; set; } = string.Empty;

    [MaxLength(30)]
    public string AnsprechpartnerTelefon { get; set; } = string.Empty;

    [EmailAddress, MaxLength(256)]
    public string AnsprechpartnerEmail { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? AnsprechpartnerPosition { get; set; }
}

public class DashboardOverviewDto
{
    public int OpenRequests { get; set; }
    public int TotalRequests { get; set; }
    public int RegisteredVehicles { get; set; }
    public int PendingReviews { get; set; }
    public string? LatestRequestStatus { get; set; }
    public DateTime? LatestRequestCreatedAt { get; set; }
}

public class HelpArticleDto
{
    public int Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
}

public class SearchRequestsRequestDto
{
    public string? Query { get; set; }
    public string? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

// ── Payment Method DTOs ──

public class PaymentMethodDto
{
    public int Id { get; set; }
    public string Typ { get; set; } = "SEPA";
    public string Bezeichnung { get; set; } = string.Empty;
    public string? Kontoinhaber { get; set; }
    public string? IBAN { get; set; }
    public string? BIC { get; set; }
    public string? Bankname { get; set; }
    public string? KartenNummer { get; set; }
    public string? KartenInhaber { get; set; }
    public string? GueltigBis { get; set; }
    public string? PaypalEmail { get; set; }
    public bool IstStandard { get; set; }
    public bool SepaMandatErteilt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class SavePaymentMethodDto
{
    [Required, MaxLength(30)]
    public string Typ { get; set; } = "SEPA";

    [Required, MaxLength(100)]
    public string Bezeichnung { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Kontoinhaber { get; set; }

    [MaxLength(34)]
    public string? IBAN { get; set; }

    [MaxLength(11)]
    public string? BIC { get; set; }

    [MaxLength(200)]
    public string? Bankname { get; set; }

    [MaxLength(4)]
    public string? KartenNummer { get; set; }

    [MaxLength(200)]
    public string? KartenInhaber { get; set; }

    [MaxLength(5)]
    public string? GueltigBis { get; set; }

    [MaxLength(256), EmailAddress]
    public string? PaypalEmail { get; set; }

    public bool IstStandard { get; set; }
    public bool SepaMandatErteilt { get; set; }
}

// ── Invoice DTOs ──
public class InvoiceDto
{
    public int Id { get; set; }
    public string Rechnungsnummer { get; set; } = string.Empty;
    public int? RegistrationRequestId { get; set; }
    public string Beschreibung { get; set; } = string.Empty;
    public decimal Nettobetrag { get; set; }
    public decimal MwstSatz { get; set; }
    public decimal MwstBetrag { get; set; }
    public decimal Bruttobetrag { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime Rechnungsdatum { get; set; }
    public DateTime Faelligkeitsdatum { get; set; }
    public DateTime? BezahltAm { get; set; }
    public string? Zahlungsmethode { get; set; }
    public DateTime CreatedAt { get; set; }
}

// ── Announcement DTOs ──
public class AnnouncementDto
{
    public int Id { get; set; }
    public string Titel { get; set; } = string.Empty;
    public string Nachricht { get; set; } = string.Empty;
    public string Typ { get; set; } = "info";
}

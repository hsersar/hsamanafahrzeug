namespace iKfz.Backend.Models;

/// <summary>
/// Company / Standort profile for organisational users (StandortAdmin).
/// </summary>
public class CompanyProfile
{
    public int Id { get; set; }

    /// <summary>User ID of the Standort admin who owns this profile.</summary>
    public required string UserId { get; set; }

    // Company data
    public string Firmenname { get; set; } = string.Empty;
    public string Rechtsform { get; set; } = string.Empty;        // z. B. GmbH, AG, e. K.
    public string Handelsregisternummer { get; set; } = string.Empty;
    public string UstIdNr { get; set; } = string.Empty;           // USt-ID

    // Company address
    public string Strasse { get; set; } = string.Empty;
    public string Hausnummer { get; set; } = string.Empty;
    public string Plz { get; set; } = string.Empty;
    public string Ort { get; set; } = string.Empty;

    // Company contact
    public string Telefon { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;

    // Contact person (Ansprechpartner)
    public string AnsprechpartnerAnrede { get; set; } = "Herr";
    public string AnsprechpartnerVorname { get; set; } = string.Empty;
    public string AnsprechpartnerNachname { get; set; } = string.Empty;
    public string AnsprechpartnerTelefon { get; set; } = string.Empty;
    public string AnsprechpartnerEmail { get; set; } = string.Empty;
    public string? AnsprechpartnerPosition { get; set; }

    // Company logo (stored as relative path)
    public string? LogoPfad { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

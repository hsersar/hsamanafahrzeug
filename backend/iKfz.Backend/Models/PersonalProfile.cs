namespace iKfz.Backend.Models;

/// <summary>
/// Personal profile for individual users (Bürger/Kunde).
/// </summary>
public class PersonalProfile
{
    public int Id { get; set; }

    /// <summary>User ID from authentication (sub claim).</summary>
    public required string UserId { get; set; }

    // Name
    public string Anrede { get; set; } = "Herr";
    public string? Titel { get; set; }
    public string Vorname { get; set; } = string.Empty;
    public string Nachname { get; set; } = string.Empty;
    public DateTime? Geburtsdatum { get; set; }

    // Address
    public string Strasse { get; set; } = string.Empty;
    public string Hausnummer { get; set; } = string.Empty;
    public string Plz { get; set; } = string.Empty;
    public string Ort { get; set; } = string.Empty;

    // Contact
    public string Telefon { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Profile picture (stored as relative path, e.g. "uploads/profiles/abc.jpg")
    public string? ProfilbildPfad { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

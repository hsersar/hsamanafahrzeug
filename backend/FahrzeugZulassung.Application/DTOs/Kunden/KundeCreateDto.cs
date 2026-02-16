using System.ComponentModel.DataAnnotations;

namespace FahrzeugZulassung.Application.DTOs.Kunden;

public class KundeCreateDto
{
    [Required(ErrorMessage = "Vorname ist erforderlich")]
    [StringLength(100, ErrorMessage = "Vorname darf maximal 100 Zeichen lang sein")]
    public string Vorname { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nachname ist erforderlich")]
    [StringLength(100, ErrorMessage = "Nachname darf maximal 100 Zeichen lang sein")]
    public string Nachname { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email ist erforderlich")]
    [EmailAddress(ErrorMessage = "Ungültige Email-Adresse")]
    [StringLength(255, ErrorMessage = "Email darf maximal 255 Zeichen lang sein")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefon ist erforderlich")]
    [Phone(ErrorMessage = "Ungültige Telefonnummer")]
    [StringLength(20, ErrorMessage = "Telefonnummer darf maximal 20 Zeichen lang sein")]
    public string Telefon { get; set; } = string.Empty;

    [Required(ErrorMessage = "Strasse ist erforderlich")]
    [StringLength(200, ErrorMessage = "Strasse darf maximal 200 Zeichen lang sein")]
    public string Strasse { get; set; } = string.Empty;

    [Required(ErrorMessage = "PLZ ist erforderlich")]
    [StringLength(10, ErrorMessage = "PLZ darf maximal 10 Zeichen lang sein")]
    public string PLZ { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ort ist erforderlich")]
    [StringLength(100, ErrorMessage = "Ort darf maximal 100 Zeichen lang sein")]
    public string Ort { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Bemerkungen dürfen maximal 500 Zeichen lang sein")]
    public string? Bemerkungen { get; set; }
}

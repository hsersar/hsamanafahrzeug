using System.ComponentModel.DataAnnotations;

namespace FahrzeugZulassung.Application.DTOs.Mitarbeiter;

public class MitarbeiterCreateDto
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

    [Phone(ErrorMessage = "Ungültige Telefonnummer")]
    [StringLength(20, ErrorMessage = "Telefonnummer darf maximal 20 Zeichen lang sein")]
    public string? Telefon { get; set; }

    [StringLength(100, ErrorMessage = "Position darf maximal 100 Zeichen lang sein")]
    public string? Position { get; set; }

    [Required(ErrorMessage = "Standort ist erforderlich")]
    public Guid StandortId { get; set; }

    public bool Aktiv { get; set; } = true;
}

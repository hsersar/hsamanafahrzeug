using System.ComponentModel.DataAnnotations;

namespace FahrzeugZulassung.Application.DTOs.Standorte;

public class StandortCreateDto
{
    [Required(ErrorMessage = "Name ist erforderlich")]
    [StringLength(200, ErrorMessage = "Name darf maximal 200 Zeichen lang sein")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Strasse ist erforderlich")]
    [StringLength(200, ErrorMessage = "Strasse darf maximal 200 Zeichen lang sein")]
    public string Strasse { get; set; } = string.Empty;

    [Required(ErrorMessage = "PLZ ist erforderlich")]
    [StringLength(10, ErrorMessage = "PLZ darf maximal 10 Zeichen lang sein")]
    public string PLZ { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ort ist erforderlich")]
    [StringLength(100, ErrorMessage = "Ort darf maximal 100 Zeichen lang sein")]
    public string Ort { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Ungültige Telefonnummer")]
    [StringLength(20, ErrorMessage = "Telefonnummer darf maximal 20 Zeichen lang sein")]
    public string? Telefon { get; set; }

    [EmailAddress(ErrorMessage = "Ungültige Email-Adresse")]
    [StringLength(255, ErrorMessage = "Email darf maximal 255 Zeichen lang sein")]
    public string? Email { get; set; }

    public bool Aktiv { get; set; } = true;
}

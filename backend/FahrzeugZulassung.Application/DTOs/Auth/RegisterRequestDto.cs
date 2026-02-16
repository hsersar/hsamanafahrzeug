using System.ComponentModel.DataAnnotations;

namespace FahrzeugZulassung.Application.DTOs.Auth;

public class RegisterRequestDto
{
    [Required(ErrorMessage = "Email ist erforderlich")]
    [EmailAddress(ErrorMessage = "Ungültige Email-Adresse")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Passwort ist erforderlich")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Passwort muss mindestens 6 Zeichen lang sein")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Passwortbestätigung ist erforderlich")]
    [Compare("Password", ErrorMessage = "Passwörter stimmen nicht überein")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vorname ist erforderlich")]
    [StringLength(100, ErrorMessage = "Vorname darf maximal 100 Zeichen lang sein")]
    public string Vorname { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nachname ist erforderlich")]
    [StringLength(100, ErrorMessage = "Nachname darf maximal 100 Zeichen lang sein")]
    public string Nachname { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Ungültige Telefonnummer")]
    [StringLength(20, ErrorMessage = "Telefonnummer darf maximal 20 Zeichen lang sein")]
    public string? Telefon { get; set; }
}

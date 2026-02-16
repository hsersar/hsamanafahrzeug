using System.ComponentModel.DataAnnotations;

namespace FahrzeugZulassung.Application.DTOs.Auth;

public class LoginRequestDto
{
    [Required(ErrorMessage = "Email ist erforderlich")]
    [EmailAddress(ErrorMessage = "Ungültige Email-Adresse")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Passwort ist erforderlich")]
    public string Password { get; set; } = string.Empty;
}

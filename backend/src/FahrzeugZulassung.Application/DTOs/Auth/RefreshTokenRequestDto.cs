using System.ComponentModel.DataAnnotations;

namespace FahrzeugZulassung.Application.DTOs.Auth;

public class RefreshTokenRequestDto
{
    [Required(ErrorMessage = "RefreshToken ist erforderlich")]
    public string RefreshToken { get; set; } = string.Empty;
}

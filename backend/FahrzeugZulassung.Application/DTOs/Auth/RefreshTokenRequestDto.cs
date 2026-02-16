using System.ComponentModel.DataAnnotations;

namespace FahrzeugZulassung.Application.DTOs.Auth;

public class RefreshTokenRequestDto
{
    [Required(ErrorMessage = "AccessToken ist erforderlich")]
    public string AccessToken { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "RefreshToken ist erforderlich")]
    public string RefreshToken { get; set; } = string.Empty;
}

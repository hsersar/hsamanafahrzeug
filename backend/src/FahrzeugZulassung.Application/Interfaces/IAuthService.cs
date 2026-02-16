using FahrzeugZulassung.Application.DTOs.Auth;

namespace FahrzeugZulassung.Application.Interfaces;

public interface IAuthService
{
    Task<TokenResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
    Task<TokenResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);
    Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default);
    Task LogoutAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> ValidatePasswordPolicy(string password);
    Task<bool> CheckAccountLockout(string email);
}

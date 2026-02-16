using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using FahrzeugZulassung.Application.DTOs.Auth;
using FahrzeugZulassung.Application.Interfaces;
using FahrzeugZulassung.Domain.Entities;
using FahrzeugZulassung.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FahrzeugZulassung.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<Benutzer> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IAuditLogService _auditLogService;

    private const int MaxFailedLoginAttempts = 5;
    private const int LockoutMinutes = 15;

    public AuthService(
        UserManager<Benutzer> userManager,
        IConfiguration configuration,
        IAuditLogService auditLogService)
    {
        _userManager = userManager;
        _configuration = configuration;
        _auditLogService = auditLogService;
    }

    public async Task<TokenResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        
        if (user == null)
        {
            await _auditLogService.LogAsync(new AuditLogDto
            {
                Aktion = "LoginFehlgeschlagen",
                Entitaet = "Benutzer",
                AlteWerte = $"Email: {request.Email}",
                NeueWerte = "Benutzer nicht gefunden",
                Zeitstempel = DateTime.UtcNow
            }, cancellationToken);
            
            throw new UnauthorizedAccessException("Ungültige Anmeldedaten");
        }

        // Check account lockout
        if (user.GesperrtBis.HasValue && user.GesperrtBis.Value > DateTime.UtcNow)
        {
            var remainingTime = user.GesperrtBis.Value - DateTime.UtcNow;
            await _auditLogService.LogAsync(new AuditLogDto
            {
                Aktion = "LoginGesperrt",
                Entitaet = "Benutzer",
                EntitaetId = user.Id.ToString(),
                BenutzerId = user.Id.ToString(),
                BenutzerName = $"{user.Vorname} {user.Nachname}",
                AlteWerte = $"Gesperrt bis: {user.GesperrtBis}",
                Zeitstempel = DateTime.UtcNow
            }, cancellationToken);
            
            throw new UnauthorizedAccessException($"Konto ist gesperrt für {remainingTime.Minutes} Minuten");
        }

        // Check if user is active
        if (!user.IstAktiv)
        {
            throw new UnauthorizedAccessException("Konto ist deaktiviert");
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        
        if (!passwordValid)
        {
            user.FehlgeschlageneLoginVersuche++;
            
            if (user.FehlgeschlageneLoginVersuche >= MaxFailedLoginAttempts)
            {
                user.GesperrtBis = DateTime.UtcNow.AddMinutes(LockoutMinutes);
                user.FehlgeschlageneLoginVersuche = 0;
                
                await _auditLogService.LogAsync(new AuditLogDto
                {
                    Aktion = "KontoGesperrt",
                    Entitaet = "Benutzer",
                    EntitaetId = user.Id.ToString(),
                    BenutzerId = user.Id.ToString(),
                    BenutzerName = $"{user.Vorname} {user.Nachname}",
                    NeueWerte = $"Gesperrt bis: {user.GesperrtBis}",
                    Zeitstempel = DateTime.UtcNow
                }, cancellationToken);
            }
            
            await _userManager.UpdateAsync(user);
            
            await _auditLogService.LogAsync(new AuditLogDto
            {
                Aktion = "LoginFehlgeschlagen",
                Entitaet = "Benutzer",
                EntitaetId = user.Id.ToString(),
                BenutzerId = user.Id.ToString(),
                BenutzerName = $"{user.Vorname} {user.Nachname}",
                AlteWerte = $"Fehlversuche: {user.FehlgeschlageneLoginVersuche - 1}",
                NeueWerte = $"Fehlversuche: {user.FehlgeschlageneLoginVersuche}",
                Zeitstempel = DateTime.UtcNow
            }, cancellationToken);
            
            throw new UnauthorizedAccessException("Ungültige Anmeldedaten");
        }

        // Reset failed login attempts on successful login
        user.FehlgeschlageneLoginVersuche = 0;
        user.GesperrtBis = null;
        await _userManager.UpdateAsync(user);

        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();

        // Store refresh token (in a real implementation, you would store this in a database)
        user.SecurityStamp = refreshToken;
        await _userManager.UpdateAsync(user);

        await _auditLogService.LogAsync(new AuditLogDto
        {
            Aktion = "LoginErfolgreich",
            Entitaet = "Benutzer",
            EntitaetId = user.Id.ToString(),
            BenutzerId = user.Id.ToString(),
            BenutzerName = $"{user.Vorname} {user.Nachname}",
            Zeitstempel = DateTime.UtcNow
        }, cancellationToken);

        return new TokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = 900, // 15 minutes
            UserId = user.Id,
            Email = user.Email!,
            Rolle = user.Rolle.ToString(),
            Vorname = user.Vorname,
            Nachname = user.Nachname
        };
    }

    public async Task<TokenResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
    {
        // Validate password policy
        var passwordValid = await ValidatePasswordPolicy(request.Password);
        if (!passwordValid)
        {
            throw new ArgumentException("Passwort erfüllt nicht die Sicherheitsrichtlinien (BSI)");
        }

        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Ein Benutzer mit dieser E-Mail-Adresse existiert bereits");
        }

        var user = new Benutzer
        {
            UserName = request.Email,
            Email = request.Email,
            Vorname = request.Vorname,
            Nachname = request.Nachname,
            Rolle = BenutzerRolle.Kunde,
            IstAktiv = true,
            ErstelltAm = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Fehler bei der Registrierung: {errors}");
        }

        await _auditLogService.LogAsync(new AuditLogDto
        {
            Aktion = "BenutzerRegistriert",
            Entitaet = "Benutzer",
            EntitaetId = user.Id.ToString(),
            BenutzerId = user.Id.ToString(),
            BenutzerName = $"{user.Vorname} {user.Nachname}",
            NeueWerte = $"Email: {user.Email}, Rolle: {user.Rolle}",
            Zeitstempel = DateTime.UtcNow
        }, cancellationToken);

        // Auto-login after registration
        return await LoginAsync(new LoginRequestDto 
        { 
            Email = request.Email, 
            Password = request.Password 
        }, cancellationToken);
    }

    public async Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default)
    {
        var principal = GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
        {
            throw new UnauthorizedAccessException("Ungültiger Token");
        }

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("Ungültiger Token");
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null || user.SecurityStamp != request.RefreshToken)
        {
            throw new UnauthorizedAccessException("Ungültiger Refresh-Token");
        }

        var accessToken = GenerateAccessToken(user);
        var newRefreshToken = GenerateRefreshToken();

        user.SecurityStamp = newRefreshToken;
        await _userManager.UpdateAsync(user);

        return new TokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            ExpiresIn = 900, // 15 minutes
            UserId = user.Id,
            Email = user.Email!,
            Rolle = user.Rolle.ToString(),
            Vorname = user.Vorname,
            Nachname = user.Nachname
        };
    }

    public async Task LogoutAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(userId, out var userGuid))
        {
            throw new ArgumentException("Ungültige Benutzer-ID");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            // Invalidate refresh token
            user.SecurityStamp = Guid.NewGuid().ToString();
            await _userManager.UpdateAsync(user);

            await _auditLogService.LogAsync(new AuditLogDto
            {
                Aktion = "Logout",
                Entitaet = "Benutzer",
                EntitaetId = user.Id.ToString(),
                BenutzerId = user.Id.ToString(),
                BenutzerName = $"{user.Vorname} {user.Nachname}",
                Zeitstempel = DateTime.UtcNow
            }, cancellationToken);
        }
    }

    public Task<bool> ValidatePasswordPolicy(string password)
    {
        // BSI password policy:
        // - Minimum 12 characters
        // - At least one uppercase letter
        // - At least one lowercase letter
        // - At least one digit
        // - At least one special character
        
        if (string.IsNullOrWhiteSpace(password) || password.Length < 12)
        {
            return Task.FromResult(false);
        }

        var hasUpperCase = Regex.IsMatch(password, @"[A-Z]");
        var hasLowerCase = Regex.IsMatch(password, @"[a-z]");
        var hasDigit = Regex.IsMatch(password, @"[0-9]");
        var hasSpecialChar = Regex.IsMatch(password, @"[^a-zA-Z0-9]");

        var isValid = hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar;
        
        return Task.FromResult(isValid);
    }

    public async Task<bool> CheckAccountLockout(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        
        if (user == null)
        {
            return false;
        }

        if (user.GesperrtBis.HasValue && user.GesperrtBis.Value > DateTime.UtcNow)
        {
            return true;
        }

        return false;
    }

    private string GenerateAccessToken(Benutzer user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey nicht konfiguriert");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, $"{user.Vorname} {user.Nachname}"),
            new(ClaimTypes.Role, user.Rolle.ToString()),
            new("vorname", user.Vorname),
            new("nachname", user.Nachname)
        };

        if (user.StandortId.HasValue)
        {
            claims.Add(new Claim("standortId", user.StandortId.Value.ToString()));
        }

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15), // 15 minutes
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey nicht konfiguriert");
        
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false, // We don't care about expiration here
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            
            if (securityToken is not JwtSecurityToken jwtSecurityToken || 
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }
}

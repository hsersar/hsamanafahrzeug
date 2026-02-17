using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace iKfz.Backend.Auth;

/// <summary>
/// Development-only authentication handler.
/// MUST NEVER be used in production – guarded by ASPNETCORE_ENVIRONMENT check
/// and a compile-time DEBUG flag.
/// </summary>
public class DevAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "Dev";

    // Allowed mock roles – prevents arbitrary role injection
    private static readonly HashSet<string> AllowedRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "Kunde", "Mitarbeiter", "StandortAdmin", "SuperAdmin"
    };

    public DevAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder, TimeProvider.System)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
#if !DEBUG
        // Hard safety net: never authenticate in release builds
        return Task.FromResult(AuthenticateResult.Fail("DevAuthHandler is disabled in release builds."));
#else
        if (!Context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
        {
            return Task.FromResult(AuthenticateResult.Fail("DevAuthHandler is disabled outside Development."));
        }

        // Read mock role from request header (whitelist-validated)
        var requestedRole = Request.Headers["X-Mock-Role"].FirstOrDefault() ?? "Kunde";
        var role = AllowedRoles.Contains(requestedRole) ? requestedRole : "Kunde";

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "demo-user"),
            new("sub", "demo-user"),
            new(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        Logger.LogInformation("DevAuth: authenticated demo-user with role {Role}", role);

        return Task.FromResult(AuthenticateResult.Success(ticket));
#endif
    }
}

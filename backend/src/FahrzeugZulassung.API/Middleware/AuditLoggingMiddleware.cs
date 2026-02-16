using FahrzeugZulassung.Application.Interfaces;
using System.Security.Claims;

namespace FahrzeugZulassung.API.Middleware;

public class AuditLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditLoggingMiddleware> _logger;

    public AuditLoggingMiddleware(RequestDelegate next, ILogger<AuditLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IAuditLogService auditLogService)
    {
        var method = context.Request.Method;
        
        if (method == "POST" || method == "PUT" || method == "DELETE")
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = context.User.FindFirstValue(ClaimTypes.Name) ?? "Anonymous";
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var userAgent = context.Request.Headers.UserAgent.ToString();
            var path = context.Request.Path.ToString();

            try
            {
                var auditLog = new AuditLogDto
                {
                    Aktion = $"{method} {path}",
                    Entitaet = ExtractEntityFromPath(path),
                    BenutzerId = userId,
                    BenutzerName = userName,
                    IpAdresse = ipAddress,
                    Zeitstempel = DateTime.UtcNow,
                    NeueWerte = $"User-Agent: {userAgent}"
                };

                await auditLogService.LogAsync(auditLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log audit entry for {Method} {Path}", method, path);
            }
        }

        await _next(context);
    }

    private static string ExtractEntityFromPath(string path)
    {
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return segments.Length > 1 ? segments[1] : "Unknown";
    }
}

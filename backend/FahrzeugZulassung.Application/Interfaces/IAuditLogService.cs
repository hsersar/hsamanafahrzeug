namespace FahrzeugZulassung.Application.Interfaces;

public interface IAuditLogService
{
    Task LogAsync(AuditLogDto dto, CancellationToken cancellationToken = default);
    Task<List<AuditLogDto>> GetLogsAsync(AuditLogFilterDto filter, CancellationToken cancellationToken = default);
}

public class AuditLogDto
{
    public Guid Id { get; set; }
    public string Aktion { get; set; } = string.Empty;
    public string Entitaet { get; set; } = string.Empty;
    public string? EntitaetId { get; set; }
    public string? BenutzerId { get; set; }
    public string? BenutzerName { get; set; }
    public DateTime Zeitstempel { get; set; }
    public string? AlteWerte { get; set; }
    public string? NeueWerte { get; set; }
    public string? IpAdresse { get; set; }
}

public class AuditLogFilterDto
{
    public DateTime? Von { get; set; }
    public DateTime? Bis { get; set; }
    public string? Entitaet { get; set; }
    public string? BenutzerId { get; set; }
    public string? Aktion { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

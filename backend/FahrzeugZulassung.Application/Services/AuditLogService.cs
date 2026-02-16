using FahrzeugZulassung.Application.Interfaces;
using FahrzeugZulassung.Domain.Entities;
using FahrzeugZulassung.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FahrzeugZulassung.Application.Services;

public class AuditLogService : IAuditLogService
{
    private readonly AppDbContext _context;

    public AuditLogService(AppDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(AuditLogDto dto, CancellationToken cancellationToken = default)
    {
        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            BenutzerId = dto.BenutzerId != null && Guid.TryParse(dto.BenutzerId, out var userId) 
                ? userId 
                : (Guid?)null,
            Aktion = dto.Aktion,
            Entitaet = dto.Entitaet,
            EntitaetId = dto.EntitaetId,
            AlteWerte = dto.AlteWerte,
            NeueWerte = dto.NeueWerte,
            IPAdresse = dto.IpAdresse ?? string.Empty,
            UserAgent = null,
            Zeitstempel = dto.Zeitstempel
        };

        _context.AuditLogs.Add(auditLog);
        
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception)
        {
            // Silently fail - audit logging should not break the application
            // In production, you might want to log this to a separate logging system
        }
    }

    public async Task<List<AuditLogDto>> GetLogsAsync(AuditLogFilterDto filter, CancellationToken cancellationToken = default)
    {
        var query = _context.AuditLogs
            .Include(al => al.Benutzer)
            .AsQueryable();

        // Apply filters
        if (filter.Von.HasValue)
        {
            query = query.Where(al => al.Zeitstempel >= filter.Von.Value);
        }

        if (filter.Bis.HasValue)
        {
            query = query.Where(al => al.Zeitstempel <= filter.Bis.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Entitaet))
        {
            query = query.Where(al => al.Entitaet == filter.Entitaet);
        }

        if (!string.IsNullOrWhiteSpace(filter.BenutzerId))
        {
            if (Guid.TryParse(filter.BenutzerId, out var benutzerId))
            {
                query = query.Where(al => al.BenutzerId == benutzerId);
            }
        }

        if (!string.IsNullOrWhiteSpace(filter.Aktion))
        {
            query = query.Where(al => al.Aktion == filter.Aktion);
        }

        // Apply pagination
        var totalCount = await query.CountAsync(cancellationToken);
        var skip = (filter.PageNumber - 1) * filter.PageSize;

        var logs = await query
            .OrderByDescending(al => al.Zeitstempel)
            .Skip(skip)
            .Take(filter.PageSize)
            .Select(al => new AuditLogDto
            {
                Id = al.Id,
                Aktion = al.Aktion,
                Entitaet = al.Entitaet,
                EntitaetId = al.EntitaetId,
                BenutzerId = al.BenutzerId.HasValue ? al.BenutzerId.Value.ToString() : null,
                BenutzerName = al.Benutzer != null ? $"{al.Benutzer.Vorname} {al.Benutzer.Nachname}" : null,
                Zeitstempel = al.Zeitstempel,
                AlteWerte = al.AlteWerte,
                NeueWerte = al.NeueWerte,
                IpAdresse = al.IPAdresse
            })
            .ToListAsync(cancellationToken);

        return logs;
    }
}

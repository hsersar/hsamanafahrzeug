using AutoMapper;
using FahrzeugZulassung.Application.DTOs.Standorte;
using FahrzeugZulassung.Application.Interfaces;
using FahrzeugZulassung.Domain.Entities;
using FahrzeugZulassung.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FahrzeugZulassung.Application.Services;

public class StandortService : IStandortService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IAuditLogService _auditLogService;

    public StandortService(
        AppDbContext context,
        IMapper mapper,
        IAuditLogService auditLogService)
    {
        _context = context;
        _mapper = mapper;
        _auditLogService = auditLogService;
    }

    public async Task<StandortResponseDto> CreateAsync(StandortCreateDto dto, CancellationToken cancellationToken = default)
    {
        var standort = _mapper.Map<Standort>(dto);
        standort.Id = Guid.NewGuid();
        standort.ErstelltAm = DateTime.UtcNow;

        _context.Standorte.Add(standort);
        await _context.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogAsync(new AuditLogDto
        {
            Aktion = "StandortErstellt",
            Entitaet = "Standort",
            EntitaetId = standort.Id.ToString(),
            NeueWerte = $"Name: {standort.Name}, Ort: {standort.Ort}",
            Zeitstempel = DateTime.UtcNow
        }, cancellationToken);

        return _mapper.Map<StandortResponseDto>(standort);
    }

    public async Task<StandortResponseDto> UpdateAsync(Guid id, StandortUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var standort = await _context.Standorte.FindAsync(new object[] { id }, cancellationToken);
        
        if (standort == null)
        {
            throw new KeyNotFoundException($"Standort mit ID {id} nicht gefunden");
        }

        var oldValues = $"Name: {standort.Name}, Strasse: {standort.Strasse}, PLZ: {standort.PLZ}, Ort: {standort.Ort}";

        standort.Name = dto.Name;
        standort.Strasse = dto.Strasse;
        standort.PLZ = dto.PLZ;
        standort.Ort = dto.Ort;
        standort.Telefon = dto.Telefon ?? string.Empty;
        standort.Email = dto.Email ?? string.Empty;
        standort.IstAktiv = dto.Aktiv;
        standort.GeaendertAm = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogAsync(new AuditLogDto
        {
            Aktion = "StandortAktualisiert",
            Entitaet = "Standort",
            EntitaetId = standort.Id.ToString(),
            AlteWerte = oldValues,
            NeueWerte = $"Name: {standort.Name}, Strasse: {standort.Strasse}, PLZ: {standort.PLZ}, Ort: {standort.Ort}",
            Zeitstempel = DateTime.UtcNow
        }, cancellationToken);

        return _mapper.Map<StandortResponseDto>(standort);
    }

    public async Task<StandortResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var standort = await _context.Standorte.FindAsync(new object[] { id }, cancellationToken);
        return standort == null ? null : _mapper.Map<StandortResponseDto>(standort);
    }

    public async Task<List<StandortResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var standorte = await _context.Standorte
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<StandortResponseDto>>(standorte);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var standort = await _context.Standorte.FindAsync(new object[] { id }, cancellationToken);
        
        if (standort == null)
        {
            throw new KeyNotFoundException($"Standort mit ID {id} nicht gefunden");
        }

        // Check if standort has associated auftraege or mitarbeiter
        var hasAuftraege = await _context.Auftraege.AnyAsync(a => a.StandortId == id, cancellationToken);
        var hasMitarbeiter = await _context.Users.AnyAsync(u => u.StandortId == id, cancellationToken);

        if (hasAuftraege || hasMitarbeiter)
        {
            // Soft delete - deactivate instead
            standort.IstAktiv = false;
            standort.GeaendertAm = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            await _auditLogService.LogAsync(new AuditLogDto
            {
                Aktion = "StandortDeaktiviert",
                Entitaet = "Standort",
                EntitaetId = standort.Id.ToString(),
                AlteWerte = $"IstAktiv: true",
                NeueWerte = $"IstAktiv: false",
                Zeitstempel = DateTime.UtcNow
            }, cancellationToken);
        }
        else
        {
            // Hard delete if no dependencies
            _context.Standorte.Remove(standort);
            await _context.SaveChangesAsync(cancellationToken);

            await _auditLogService.LogAsync(new AuditLogDto
            {
                Aktion = "StandortGeloescht",
                Entitaet = "Standort",
                EntitaetId = standort.Id.ToString(),
                AlteWerte = $"Name: {standort.Name}, Ort: {standort.Ort}",
                Zeitstempel = DateTime.UtcNow
            }, cancellationToken);
        }
    }
}

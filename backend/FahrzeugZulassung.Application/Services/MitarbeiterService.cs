using AutoMapper;
using FahrzeugZulassung.Application.DTOs.Mitarbeiter;
using FahrzeugZulassung.Application.Interfaces;
using FahrzeugZulassung.Domain.Entities;
using FahrzeugZulassung.Domain.Enums;
using FahrzeugZulassung.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FahrzeugZulassung.Application.Services;

public class MitarbeiterService : IMitarbeiterService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IAuditLogService _auditLogService;
    private readonly UserManager<Benutzer> _userManager;

    public MitarbeiterService(
        AppDbContext context,
        IMapper mapper,
        IAuditLogService auditLogService,
        UserManager<Benutzer> userManager)
    {
        _context = context;
        _mapper = mapper;
        _auditLogService = auditLogService;
        _userManager = userManager;
    }

    public async Task<MitarbeiterResponseDto> CreateAsync(MitarbeiterCreateDto dto, CancellationToken cancellationToken = default)
    {
        // Check if email already exists
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Ein Benutzer mit dieser E-Mail-Adresse existiert bereits");
        }

        // Validate Standort exists
        var standort = await _context.Standorte.FindAsync(new object[] { dto.StandortId }, cancellationToken);
        if (standort == null)
        {
            throw new ArgumentException($"Standort mit ID {dto.StandortId} nicht gefunden");
        }

        // Create Benutzer with default role as Mitarbeiter
        var mitarbeiter = new Benutzer
        {
            UserName = dto.Email,
            Email = dto.Email,
            Vorname = dto.Vorname,
            Nachname = dto.Nachname,
            PhoneNumber = dto.Telefon,
            Rolle = BenutzerRolle.Mitarbeiter,
            StandortId = dto.StandortId,
            IstAktiv = dto.Aktiv,
            ErstelltAm = DateTime.UtcNow
        };

        // Generate temporary password (should be changed on first login)
        var tempPassword = $"Temp{Guid.NewGuid().ToString("N")[..8]}!";
        var result = await _userManager.CreateAsync(mitarbeiter, tempPassword);
        
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Fehler beim Erstellen des Mitarbeiters: {errors}");
        }

        await _auditLogService.LogAsync(new AuditLogDto
        {
            Aktion = "MitarbeiterErstellt",
            Entitaet = "Benutzer",
            EntitaetId = mitarbeiter.Id.ToString(),
            NeueWerte = $"Email: {mitarbeiter.Email}, Rolle: {mitarbeiter.Rolle}, Standort: {standort.Name}",
            Zeitstempel = DateTime.UtcNow
        }, cancellationToken);

        var mitarbeiterWithStandort = await _context.Users
            .Include(u => u.Standort)
            .FirstAsync(u => u.Id == mitarbeiter.Id, cancellationToken);

        return _mapper.Map<MitarbeiterResponseDto>(mitarbeiterWithStandort);
    }

    public async Task<MitarbeiterResponseDto> UpdateAsync(Guid id, MitarbeiterUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var mitarbeiter = await _context.Users
            .Include(u => u.Standort)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        
        if (mitarbeiter == null)
        {
            throw new KeyNotFoundException($"Mitarbeiter mit ID {id} nicht gefunden");
        }

        var oldValues = $"Vorname: {mitarbeiter.Vorname}, Nachname: {mitarbeiter.Nachname}, Email: {mitarbeiter.Email}";

        mitarbeiter.Vorname = dto.Vorname;
        mitarbeiter.Nachname = dto.Nachname;
        mitarbeiter.PhoneNumber = dto.Telefon;
        mitarbeiter.GeaendertAm = DateTime.UtcNow;

        if (dto.StandortId != mitarbeiter.StandortId)
        {
            var standort = await _context.Standorte.FindAsync(new object[] { dto.StandortId }, cancellationToken);
            if (standort == null)
            {
                throw new ArgumentException($"Standort mit ID {dto.StandortId} nicht gefunden");
            }
            mitarbeiter.StandortId = dto.StandortId;
        }

        var result = await _userManager.UpdateAsync(mitarbeiter);
        
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Fehler beim Aktualisieren des Mitarbeiters: {errors}");
        }

        await _auditLogService.LogAsync(new AuditLogDto
        {
            Aktion = "MitarbeiterAktualisiert",
            Entitaet = "Benutzer",
            EntitaetId = mitarbeiter.Id.ToString(),
            AlteWerte = oldValues,
            NeueWerte = $"Vorname: {mitarbeiter.Vorname}, Nachname: {mitarbeiter.Nachname}, Email: {mitarbeiter.Email}",
            Zeitstempel = DateTime.UtcNow
        }, cancellationToken);

        return _mapper.Map<MitarbeiterResponseDto>(mitarbeiter);
    }

    public async Task<MitarbeiterResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var mitarbeiter = await _context.Users
            .Include(u => u.Standort)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        return mitarbeiter == null ? null : _mapper.Map<MitarbeiterResponseDto>(mitarbeiter);
    }

    public async Task<List<MitarbeiterResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var mitarbeiter = await _context.Users
            .Include(u => u.Standort)
            .Where(u => u.Rolle != BenutzerRolle.Kunde)
            .OrderBy(u => u.Nachname)
            .ThenBy(u => u.Vorname)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<MitarbeiterResponseDto>>(mitarbeiter);
    }

    public async Task ActivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var mitarbeiter = await _context.Users.FindAsync(new object[] { id }, cancellationToken);
        
        if (mitarbeiter == null)
        {
            throw new KeyNotFoundException($"Mitarbeiter mit ID {id} nicht gefunden");
        }

        if (mitarbeiter.IstAktiv)
        {
            return; // Already active
        }

        mitarbeiter.IstAktiv = true;
        mitarbeiter.GeaendertAm = DateTime.UtcNow;
        await _userManager.UpdateAsync(mitarbeiter);

        await _auditLogService.LogAsync(new AuditLogDto
        {
            Aktion = "MitarbeiterAktiviert",
            Entitaet = "Benutzer",
            EntitaetId = mitarbeiter.Id.ToString(),
            AlteWerte = "IstAktiv: false",
            NeueWerte = "IstAktiv: true",
            Zeitstempel = DateTime.UtcNow
        }, cancellationToken);
    }

    public async Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var mitarbeiter = await _context.Users.FindAsync(new object[] { id }, cancellationToken);
        
        if (mitarbeiter == null)
        {
            throw new KeyNotFoundException($"Mitarbeiter mit ID {id} nicht gefunden");
        }

        if (!mitarbeiter.IstAktiv)
        {
            return; // Already inactive
        }

        mitarbeiter.IstAktiv = false;
        mitarbeiter.GeaendertAm = DateTime.UtcNow;
        await _userManager.UpdateAsync(mitarbeiter);

        await _auditLogService.LogAsync(new AuditLogDto
        {
            Aktion = "MitarbeiterDeaktiviert",
            Entitaet = "Benutzer",
            EntitaetId = mitarbeiter.Id.ToString(),
            AlteWerte = "IstAktiv: true",
            NeueWerte = "IstAktiv: false",
            Zeitstempel = DateTime.UtcNow
        }, cancellationToken);
    }
}

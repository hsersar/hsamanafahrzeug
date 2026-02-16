using AutoMapper;
using FahrzeugZulassung.Application.DTOs.Kunden;
using FahrzeugZulassung.Application.Interfaces;
using FahrzeugZulassung.Domain.Entities;
using FahrzeugZulassung.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FahrzeugZulassung.Application.Services;

public class KundenService : IKundenService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IAuditLogService _auditLogService;

    public KundenService(
        AppDbContext context,
        IMapper mapper,
        IAuditLogService auditLogService)
    {
        _context = context;
        _mapper = mapper;
        _auditLogService = auditLogService;
    }

    public async Task<KundeResponseDto> CreateAsync(KundeCreateDto dto, CancellationToken cancellationToken = default)
    {
        // Check if kunde with email already exists
        var existingKunde = await _context.Kunden
            .FirstOrDefaultAsync(k => k.Email == dto.Email, cancellationToken);

        if (existingKunde != null)
        {
            throw new InvalidOperationException("Ein Kunde mit dieser E-Mail-Adresse existiert bereits");
        }

        var kunde = _mapper.Map<Kunde>(dto);
        kunde.Id = Guid.NewGuid();
        kunde.ErstelltAm = DateTime.UtcNow;

        _context.Kunden.Add(kunde);
        await _context.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogAsync(new AuditLogDto
        {
            Aktion = "KundeErstellt",
            Entitaet = "Kunde",
            EntitaetId = kunde.Id.ToString(),
            NeueWerte = $"Name: {kunde.Vorname} {kunde.Nachname}, Email: {kunde.Email}",
            Zeitstempel = DateTime.UtcNow
        }, cancellationToken);

        return _mapper.Map<KundeResponseDto>(kunde);
    }

    public async Task<KundeResponseDto> UpdateAsync(Guid id, KundeUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var kunde = await _context.Kunden.FindAsync(new object[] { id }, cancellationToken);
        
        if (kunde == null)
        {
            throw new KeyNotFoundException($"Kunde mit ID {id} nicht gefunden");
        }

        var oldValues = $"Name: {kunde.Vorname} {kunde.Nachname}, Email: {kunde.Email}, Telefon: {kunde.Telefon}";

        kunde.Vorname = dto.Vorname;
        kunde.Nachname = dto.Nachname;
        kunde.Email = dto.Email;
        kunde.Telefon = dto.Telefon;
        kunde.Strasse = dto.Strasse;
        kunde.PLZ = dto.PLZ;
        kunde.Ort = dto.Ort;
        kunde.GeaendertAm = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogAsync(new AuditLogDto
        {
            Aktion = "KundeAktualisiert",
            Entitaet = "Kunde",
            EntitaetId = kunde.Id.ToString(),
            AlteWerte = oldValues,
            NeueWerte = $"Name: {kunde.Vorname} {kunde.Nachname}, Email: {kunde.Email}, Telefon: {kunde.Telefon}",
            Zeitstempel = DateTime.UtcNow
        }, cancellationToken);

        return _mapper.Map<KundeResponseDto>(kunde);
    }

    public async Task<KundeResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var kunde = await _context.Kunden.FindAsync(new object[] { id }, cancellationToken);
        return kunde == null ? null : _mapper.Map<KundeResponseDto>(kunde);
    }

    public async Task<List<KundeResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var kunden = await _context.Kunden
            .OrderBy(k => k.Nachname)
            .ThenBy(k => k.Vorname)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<KundeResponseDto>>(kunden);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var kunde = await _context.Kunden.FindAsync(new object[] { id }, cancellationToken);
        
        if (kunde == null)
        {
            throw new KeyNotFoundException($"Kunde mit ID {id} nicht gefunden");
        }

        // Check if kunde has associated auftraege
        var hasAuftraege = await _context.Auftraege.AnyAsync(a => a.KundeId == id, cancellationToken);

        if (hasAuftraege)
        {
            throw new InvalidOperationException("Kunde kann nicht gelöscht werden, da zugehörige Aufträge existieren");
        }

        _context.Kunden.Remove(kunde);
        await _context.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogAsync(new AuditLogDto
        {
            Aktion = "KundeGeloescht",
            Entitaet = "Kunde",
            EntitaetId = kunde.Id.ToString(),
            AlteWerte = $"Name: {kunde.Vorname} {kunde.Nachname}, Email: {kunde.Email}",
            Zeitstempel = DateTime.UtcNow
        }, cancellationToken);
    }
}

using AutoMapper;
using FahrzeugZulassung.Application.DTOs.Rechnungen;
using FahrzeugZulassung.Application.Interfaces;
using FahrzeugZulassung.Domain.Entities;
using FahrzeugZulassung.Domain.Enums;
using FahrzeugZulassung.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FahrzeugZulassung.Application.Services;

public class RechnungService : IRechnungService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IAuditLogService _auditLogService;

    public RechnungService(
        AppDbContext context,
        IMapper mapper,
        IAuditLogService auditLogService)
    {
        _context = context;
        _mapper = mapper;
        _auditLogService = auditLogService;
    }

    public async Task<RechnungResponseDto> CreateAsync(RechnungCreateDto dto, CancellationToken cancellationToken = default)
    {
        // Validate Auftrag exists
        var auftrag = await _context.Auftraege
            .Include(a => a.Kunde)
            .FirstOrDefaultAsync(a => a.Id == dto.AuftragId, cancellationToken);

        if (auftrag == null)
        {
            throw new ArgumentException($"Auftrag mit ID {dto.AuftragId} nicht gefunden");
        }

        // Check if invoice already exists for this Auftrag
        var existingRechnung = await _context.Rechnungen
            .FirstOrDefaultAsync(r => r.AuftragId == dto.AuftragId, cancellationToken);

        if (existingRechnung != null)
        {
            throw new InvalidOperationException($"Für diesen Auftrag existiert bereits eine Rechnung");
        }

        // Calculate total amount from positions
        decimal calculatedBetrag = 0;
        if (dto.Positionen != null && dto.Positionen.Any())
        {
            calculatedBetrag = dto.Positionen.Sum(p => p.Menge * p.Einzelpreis);
        }
        else
        {
            calculatedBetrag = dto.Betrag;
        }

        var rechnung = new Rechnung
        {
            Id = Guid.NewGuid(),
            RechnungNummer = await GenerateRechnungNummerAsync(cancellationToken),
            AuftragId = dto.AuftragId,
            Betrag = calculatedBetrag,
            Status = RechnungStatus.Offen,
            Faellig = dto.Faelligkeitsdatum ?? DateTime.UtcNow.AddDays(14), // Default: 14 days
            ErstelltAm = DateTime.UtcNow
        };

        _context.Rechnungen.Add(rechnung);
        await _context.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogAsync(new AuditLogDto
        {
            Aktion = "RechnungErstellt",
            Entitaet = "Rechnung",
            EntitaetId = rechnung.Id.ToString(),
            NeueWerte = $"RechnungNummer: {rechnung.RechnungNummer}, Betrag: {rechnung.Betrag:C}, AuftragId: {rechnung.AuftragId}",
            Zeitstempel = DateTime.UtcNow
        }, cancellationToken);

        return await GetByIdAsync(rechnung.Id, cancellationToken) 
            ?? throw new InvalidOperationException("Rechnung konnte nicht geladen werden");
    }

    public async Task<RechnungResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var rechnung = await _context.Rechnungen
            .Include(r => r.Auftrag)
                .ThenInclude(a => a.Kunde)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (rechnung == null)
        {
            return null;
        }

        return _mapper.Map<RechnungResponseDto>(rechnung);
    }

    public async Task<List<RechnungResponseDto>> GetByAuftragIdAsync(Guid auftragId, CancellationToken cancellationToken = default)
    {
        var rechnungen = await _context.Rechnungen
            .Include(r => r.Auftrag)
                .ThenInclude(a => a.Kunde)
            .Where(r => r.AuftragId == auftragId)
            .OrderByDescending(r => r.ErstelltAm)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<RechnungResponseDto>>(rechnungen);
    }

    public async Task<RechnungResponseDto> BezahlenAsync(Guid id, RechnungBezahlenDto dto, CancellationToken cancellationToken = default)
    {
        var rechnung = await _context.Rechnungen
            .Include(r => r.Auftrag)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (rechnung == null)
        {
            throw new KeyNotFoundException($"Rechnung mit ID {id} nicht gefunden");
        }

        if (rechnung.Status == RechnungStatus.Bezahlt)
        {
            throw new InvalidOperationException("Rechnung wurde bereits bezahlt");
        }

        if (rechnung.Status == RechnungStatus.Storniert)
        {
            throw new InvalidOperationException("Stornierte Rechnung kann nicht bezahlt werden");
        }

        var oldStatus = rechnung.Status;
        rechnung.Status = RechnungStatus.Bezahlt;
        rechnung.BezahltAm = dto.BezahltAm;
        rechnung.Zahlungsmethode = "Manuell"; // Default for now
        rechnung.ZahlungsReferenz = dto.Bemerkungen;
        rechnung.GeaendertAm = DateTime.UtcNow;

        // Update Auftrag status if needed
        if (rechnung.Auftrag.Status == AuftragStatus.WartAufZahlung)
        {
            rechnung.Auftrag.Status = AuftragStatus.InBearbeitung;
            rechnung.Auftrag.GeaendertAm = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogAsync(new AuditLogDto
        {
            Aktion = "RechnungBezahlt",
            Entitaet = "Rechnung",
            EntitaetId = rechnung.Id.ToString(),
            AlteWerte = $"Status: {oldStatus}",
            NeueWerte = $"Status: {rechnung.Status}, Zahlungsmethode: {rechnung.Zahlungsmethode}, BezahltAm: {rechnung.BezahltAm}",
            Zeitstempel = DateTime.UtcNow
        }, cancellationToken);

        return await GetByIdAsync(id, cancellationToken) 
            ?? throw new InvalidOperationException("Rechnung konnte nicht geladen werden");
    }

    private async Task<string> GenerateRechnungNummerAsync(CancellationToken cancellationToken)
    {
        var year = DateTime.UtcNow.Year;
        var prefix = $"RE-{year}-";

        var lastRechnung = await _context.Rechnungen
            .Where(r => r.RechnungNummer.StartsWith(prefix))
            .OrderByDescending(r => r.RechnungNummer)
            .FirstOrDefaultAsync(cancellationToken);

        int nextNumber = 1;
        if (lastRechnung != null)
        {
            var lastNumberStr = lastRechnung.RechnungNummer.Replace(prefix, "");
            if (int.TryParse(lastNumberStr, out var lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
        }

        return $"{prefix}{nextNumber:D6}";
    }
}

using AutoMapper;
using FahrzeugZulassung.Application.DTOs.Auftraege;
using FahrzeugZulassung.Application.Interfaces;
using FahrzeugZulassung.Domain.Entities;
using FahrzeugZulassung.Domain.Enums;
using FahrzeugZulassung.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FahrzeugZulassung.Application.Services;

public class AuftragService : IAuftragService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IAuditLogService _auditLogService;
    private readonly IIKfzService _ikfzService;

    public AuftragService(
        AppDbContext context,
        IMapper mapper,
        IAuditLogService auditLogService,
        IIKfzService ikfzService)
    {
        _context = context;
        _mapper = mapper;
        _auditLogService = auditLogService;
        _ikfzService = ikfzService;
    }

    public async Task<AuftragResponseDto> CreateAsync(AuftragCreateDto dto, CancellationToken cancellationToken = default)
    {
        // Parse AuftragTyp from string
        if (!Enum.TryParse<AuftragTyp>(dto.Typ, true, out var auftragTyp))
        {
            throw new ArgumentException($"Ungültiger Auftragstyp: {dto.Typ}");
        }

        // Check if kunde exists by email, otherwise create new
        var kunde = await _context.Kunden
            .FirstOrDefaultAsync(k => k.Email == dto.KundeEmail, cancellationToken);

        if (kunde == null)
        {
            kunde = _mapper.Map<Kunde>(dto);
            kunde.Id = Guid.NewGuid();
            
            // Create associated Benutzer account
            var benutzer = new Benutzer
            {
                UserName = dto.KundeEmail,
                Email = dto.KundeEmail,
                Vorname = dto.KundeVorname,
                Nachname = dto.KundeNachname,
                PhoneNumber = dto.KundeTelefon,
                Rolle = BenutzerRolle.Kunde,
                IstAktiv = true,
                ErstelltAm = DateTime.UtcNow
            };
            _context.Users.Add(benutzer);
            await _context.SaveChangesAsync(cancellationToken);
            
            kunde.BenutzerId = benutzer.Id;
            _context.Kunden.Add(kunde);
        }

        // Check if fahrzeug exists by FIN, otherwise create new
        var fahrzeug = await _context.Fahrzeuge
            .FirstOrDefaultAsync(f => f.FIN == dto.FahrzeugFIN, cancellationToken);

        if (fahrzeug == null)
        {
            fahrzeug = _mapper.Map<Fahrzeug>(dto);
            fahrzeug.Id = Guid.NewGuid();
            _context.Fahrzeuge.Add(fahrzeug);
        }

        // Determine Standort
        Guid standortId;
        if (dto.StandortId.HasValue)
        {
            var standort = await _context.Standorte.FindAsync(new object[] { dto.StandortId.Value }, cancellationToken);
            if (standort == null || !standort.IstAktiv)
            {
                throw new ArgumentException("Ungültiger oder inaktiver Standort");
            }
            standortId = dto.StandortId.Value;
        }
        else
        {
            // Use first active Standort as default
            var defaultStandort = await _context.Standorte
                .Where(s => s.IstAktiv)
                .OrderBy(s => s.Name)
                .FirstOrDefaultAsync(cancellationToken);

            if (defaultStandort == null)
            {
                throw new InvalidOperationException("Kein aktiver Standort verfügbar");
            }
            standortId = defaultStandort.Id;
        }

        // Create Auftrag
        var auftrag = new Auftrag
        {
            Id = Guid.NewGuid(),
            AuftragNummer = await GenerateAuftragNummerAsync(cancellationToken),
            Typ = auftragTyp,
            Status = AuftragStatus.Entwurf,
            KundeId = kunde.Id,
            FahrzeugId = fahrzeug.Id,
            StandortId = standortId,
            ErstelltVonId = kunde.BenutzerId,
            AGBAkzeptiert = dto.AGBAkzeptiert,
            AGBAkzeptiertAm = dto.AGBAkzeptiert ? DateTime.UtcNow : null,
            Step1Abgeschlossen = true,
            Step2Abgeschlossen = false,
            Step3Abgeschlossen = false,
            ErstelltAm = DateTime.UtcNow
        };

        _context.Auftraege.Add(auftrag);
        await _context.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogAsync(new AuditLogDto
        {
            Aktion = "AuftragErstellt",
            Entitaet = "Auftrag",
            EntitaetId = auftrag.Id,
            BenutzerId = kunde.BenutzerId.ToString(),
            BenutzerName = $"{kunde.Vorname} {kunde.Nachname}",
            NeueWerte = $"Typ: {auftrag.Typ}, Status: {auftrag.Status}, AuftragNummer: {auftrag.AuftragNummer}",
            Zeitstempel = DateTime.UtcNow
        }, cancellationToken);

        return await GetByIdAsync(auftrag.Id, cancellationToken) 
            ?? throw new InvalidOperationException("Auftrag konnte nicht geladen werden");
    }

    public async Task<AuftragResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var auftrag = await _context.Auftraege
            .Include(a => a.Kunde)
            .Include(a => a.Fahrzeug)
            .Include(a => a.Standort)
            .Include(a => a.ErstelltVon)
            .Include(a => a.Rechnungen)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        return auftrag == null ? null : _mapper.Map<AuftragResponseDto>(auftrag);
    }

    public async Task<List<AuftragResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var auftraege = await _context.Auftraege
            .Include(a => a.Kunde)
            .Include(a => a.Fahrzeug)
            .Include(a => a.Standort)
            .Include(a => a.ErstelltVon)
            .Include(a => a.Rechnungen)
            .OrderByDescending(a => a.ErstelltAm)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<AuftragResponseDto>>(auftraege);
    }

    public async Task<AuftragResponseDto> UpdateStatusAsync(Guid id, AuftragUpdateStatusDto dto, CancellationToken cancellationToken = default)
    {
        var auftrag = await _context.Auftraege.FindAsync(new object[] { id }, cancellationToken);
        
        if (auftrag == null)
        {
            throw new KeyNotFoundException($"Auftrag mit ID {id} nicht gefunden");
        }

        if (!Enum.TryParse<AuftragStatus>(dto.Status, true, out var newStatus))
        {
            throw new ArgumentException($"Ungültiger Status: {dto.Status}");
        }

        var oldStatus = auftrag.Status;
        auftrag.Status = newStatus;
        auftrag.GeaendertAm = DateTime.UtcNow;

        if (newStatus == AuftragStatus.Abgeschlossen)
        {
            auftrag.AbgeschlossenAm = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogAsync(new AuditLogDto
        {
            Aktion = "AuftragStatusGeaendert",
            Entitaet = "Auftrag",
            EntitaetId = auftrag.Id,
            AlteWerte = $"Status: {oldStatus}",
            NeueWerte = $"Status: {newStatus}",
            Zeitstempel = DateTime.UtcNow
        }, cancellationToken);

        return await GetByIdAsync(id, cancellationToken) 
            ?? throw new InvalidOperationException("Auftrag konnte nicht geladen werden");
    }

    public async Task<bool> SubmitToIKfzAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var auftrag = await _context.Auftraege
            .Include(a => a.Kunde)
            .Include(a => a.Fahrzeug)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (auftrag == null)
        {
            throw new KeyNotFoundException($"Auftrag mit ID {id} nicht gefunden");
        }

        // Validate that all steps are completed
        if (!auftrag.Step1Abgeschlossen || !auftrag.Step2Abgeschlossen || !auftrag.Step3Abgeschlossen)
        {
            throw new InvalidOperationException("Nicht alle Wizard-Schritte sind abgeschlossen");
        }

        if (!auftrag.AGBAkzeptiert)
        {
            throw new InvalidOperationException("AGB müssen akzeptiert werden");
        }

        // Submit to iKfz based on type
        string? transactionId = null;
        try
        {
            switch (auftrag.Typ)
            {
                case AuftragTyp.Anmeldung:
                    transactionId = await _ikfzService.SubmitAnmeldungAsync(id, cancellationToken);
                    break;
                case AuftragTyp.Abmeldung:
                    transactionId = await _ikfzService.SubmitAbmeldungAsync(id, cancellationToken);
                    break;
                case AuftragTyp.Ummeldung:
                    transactionId = await _ikfzService.SubmitUmmeldungAsync(id, cancellationToken);
                    break;
            }

            if (!string.IsNullOrEmpty(transactionId))
            {
                auftrag.IKfzReferenz = transactionId;
                auftrag.Status = AuftragStatus.AnIKfzGesendet;
                auftrag.AnIKfzGesendetAm = DateTime.UtcNow;
                auftrag.GeaendertAm = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);

                await _auditLogService.LogAsync(new AuditLogDto
                {
                    Aktion = "AuftragAnIKfzGesendet",
                    Entitaet = "Auftrag",
                    EntitaetId = auftrag.Id,
                    NeueWerte = $"Typ: {auftrag.Typ}, Status: {auftrag.Status}, IKfzReferenz: {transactionId}",
                    Zeitstempel = DateTime.UtcNow
                }, cancellationToken);

                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            await _auditLogService.LogAsync(new AuditLogDto
            {
                Aktion = "IKfzSendungFehlgeschlagen",
                Entitaet = "Auftrag",
                EntitaetId = auftrag.Id,
                NeueWerte = $"Fehler: {ex.Message}",
                Zeitstempel = DateTime.UtcNow
            }, cancellationToken);
            
            throw;
        }
    }

    public async Task<List<AuftragResponseDto>> GetByKundeIdAsync(Guid kundeId, CancellationToken cancellationToken = default)
    {
        var auftraege = await _context.Auftraege
            .Include(a => a.Kunde)
            .Include(a => a.Fahrzeug)
            .Include(a => a.Standort)
            .Include(a => a.ErstelltVon)
            .Include(a => a.Rechnungen)
            .Where(a => a.KundeId == kundeId)
            .OrderByDescending(a => a.ErstelltAm)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<AuftragResponseDto>>(auftraege);
    }

    public async Task<List<AuftragResponseDto>> GetByStandortIdAsync(Guid standortId, CancellationToken cancellationToken = default)
    {
        var auftraege = await _context.Auftraege
            .Include(a => a.Kunde)
            .Include(a => a.Fahrzeug)
            .Include(a => a.Standort)
            .Include(a => a.ErstelltVon)
            .Include(a => a.Rechnungen)
            .Where(a => a.StandortId == standortId)
            .OrderByDescending(a => a.ErstelltAm)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<AuftragResponseDto>>(auftraege);
    }

    private async Task<string> GenerateAuftragNummerAsync(CancellationToken cancellationToken)
    {
        var year = DateTime.UtcNow.Year;
        var prefix = $"AFZ-{year}-";

        var lastAuftrag = await _context.Auftraege
            .Where(a => a.AuftragNummer.StartsWith(prefix))
            .OrderByDescending(a => a.AuftragNummer)
            .FirstOrDefaultAsync(cancellationToken);

        int nextNumber = 1;
        if (lastAuftrag != null)
        {
            var lastNumberStr = lastAuftrag.AuftragNummer.Replace(prefix, "");
            if (int.TryParse(lastNumberStr, out var lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
        }

        return $"{prefix}{nextNumber:D6}";
    }
}

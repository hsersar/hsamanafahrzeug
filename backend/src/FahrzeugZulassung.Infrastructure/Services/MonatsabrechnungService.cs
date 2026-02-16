using System.Globalization;
using System.Text;
using FahrzeugZulassung.Application.DTOs.Provision;
using FahrzeugZulassung.Application.Interfaces;
using FahrzeugZulassung.Domain.Entities;
using FahrzeugZulassung.Domain.Enums;
using FahrzeugZulassung.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FahrzeugZulassung.Infrastructure.Services;

public class MonatsabrechnungService : IMonatsabrechnungService
{
    private readonly ApplicationDbContext _context;
    private readonly IProvisionsService _provisionsService;

    public MonatsabrechnungService(ApplicationDbContext context, IProvisionsService provisionsService)
    {
        _context = context;
        _provisionsService = provisionsService;
    }

    public async Task<MonatsabrechnungDto> ErstelleAbrechnungAsync(Guid standortId, int jahr, int monat)
    {
        // Prüfe, ob bereits eine Abrechnung existiert
        var existierend = await _context.Monatsabrechnungen
            .FirstOrDefaultAsync(m => m.StandortId == standortId && m.Jahr == jahr && m.Monat == monat);

        if (existierend != null)
        {
            throw new InvalidOperationException("Für diesen Zeitraum existiert bereits eine Abrechnung.");
        }

        var standort = await _context.Standorte.FindAsync(standortId);
        if (standort == null)
        {
            throw new InvalidOperationException("Standort nicht gefunden.");
        }

        // Hole Provisionsmodell
        var modell = await _provisionsService.GetModellFuerStandortAsync(standortId);

        // Berechne Provision (hier Demo-Werte)
        var berechnung = await _provisionsService.BerechneProvisionAsync(standortId, jahr, monat);

        // Erstelle Zeitraum
        var periodeVon = new DateTime(jahr, monat, 1);
        var periodeBis = periodeVon.AddMonths(1).AddDays(-1);

        // Generiere Abrechnungsnummer
        var nummer = await GeneriereAbrechnungsNummerAsync(jahr, monat);

        // Berechne Beträge
        var nettobetrag = berechnung.Grundgebuehr + berechnung.ProvisionsBetrag;
        var steuerbetrag = nettobetrag * 0.19m;
        var bruttobetrag = nettobetrag + steuerbetrag;

        // Erstelle Abrechnung
        var abrechnung = new Monatsabrechnung
        {
            Id = Guid.NewGuid(),
            StandortId = standortId,
            Jahr = jahr,
            Monat = monat,
            PeriodeVon = periodeVon,
            PeriodeBis = periodeBis,
            AbrechnungsNummer = nummer,
            AnzahlRechnungen = berechnung.AnzahlRechnungen,
            AnzahlAuftraege = berechnung.AnzahlAuftraege,
            GesamtUmsatz = berechnung.GesamtUmsatz,
            Grundgebuehr = berechnung.Grundgebuehr,
            ProvisionsProzentsatz = berechnung.ProvisionsProzentsatz,
            ProvisionsBetrag = berechnung.ProvisionsBetrag,
            Nettobetrag = nettobetrag,
            Steuersatz = 19m,
            Steuerbetrag = steuerbetrag,
            Bruttobetrag = bruttobetrag,
            Status = MonatsabrechnungStatus.Erstellt,
            Faelligkeitsdatum = DateTime.UtcNow.AddDays(14),
            ErstelltAm = DateTime.UtcNow
        };

        // Erstelle Positionen
        var positionen = new List<MonatsabrechnungPosition>
        {
            new MonatsabrechnungPosition
            {
                Id = Guid.NewGuid(),
                MonatsabrechnungId = abrechnung.Id,
                Position = 1,
                Typ = MonatsabrechnungPositionTyp.Grundgebuehr,
                Beschreibung = "Monatliche Grundgebühr",
                Menge = 1,
                Einheit = "Monat",
                Einzelpreis = berechnung.Grundgebuehr,
                Nettobetrag = berechnung.Grundgebuehr,
                Steuersatz = 19m,
                Steuerbetrag = berechnung.Grundgebuehr * 0.19m,
                Bruttobetrag = berechnung.Grundgebuehr * 1.19m
            }
        };

        if (berechnung.ProvisionsBetrag > 0)
        {
            positionen.Add(new MonatsabrechnungPosition
            {
                Id = Guid.NewGuid(),
                MonatsabrechnungId = abrechnung.Id,
                Position = 2,
                Typ = MonatsabrechnungPositionTyp.Provision,
                Beschreibung = $"Provision ({berechnung.ProvisionsProzentsatz}% auf {berechnung.GesamtUmsatz:C})",
                Menge = berechnung.AnzahlRechnungen,
                Einheit = "Rechnungen",
                Einzelpreis = berechnung.AnzahlRechnungen > 0 ? berechnung.ProvisionsBetrag / berechnung.AnzahlRechnungen : 0,
                Nettobetrag = berechnung.ProvisionsBetrag,
                Steuersatz = 19m,
                Steuerbetrag = berechnung.ProvisionsBetrag * 0.19m,
                Bruttobetrag = berechnung.ProvisionsBetrag * 1.19m
            });
        }

        abrechnung.Positionen = positionen;

        _context.Monatsabrechnungen.Add(abrechnung);
        await _context.SaveChangesAsync();

        return await GetAbrechnungAsync(abrechnung.Id) ?? throw new InvalidOperationException("Abrechnung konnte nicht erstellt werden.");
    }

    public async Task<List<MonatsabrechnungDto>> ErstelleAlleAbrechnungenAsync(int jahr, int monat)
    {
        var standorte = await _context.Standorte
            .Where(s => s.IstAktiv)
            .ToListAsync();

        var abrechnungen = new List<MonatsabrechnungDto>();

        foreach (var standort in standorte)
        {
            try
            {
                var abrechnung = await ErstelleAbrechnungAsync(standort.Id, jahr, monat);
                abrechnungen.Add(abrechnung);
            }
            catch (InvalidOperationException)
            {
                // Abrechnung existiert bereits, überspringen
                continue;
            }
        }

        return abrechnungen;
    }

    public async Task<MonatsabrechnungDto?> GetAbrechnungAsync(Guid id)
    {
        var abrechnung = await _context.Monatsabrechnungen
            .Include(m => m.Standort)
            .Include(m => m.Positionen)
            .FirstOrDefaultAsync(m => m.Id == id);

        return abrechnung != null ? MapToDto(abrechnung) : null;
    }

    public async Task<List<MonatsabrechnungDto>> GetAbrechnungenFuerStandortAsync(Guid standortId, int? jahr = null)
    {
        var query = _context.Monatsabrechnungen
            .Include(m => m.Standort)
            .Include(m => m.Positionen)
            .Where(m => m.StandortId == standortId);

        if (jahr.HasValue)
        {
            query = query.Where(m => m.Jahr == jahr.Value);
        }

        var abrechnungen = await query
            .OrderByDescending(m => m.Jahr)
            .ThenByDescending(m => m.Monat)
            .ToListAsync();

        return abrechnungen.Select(MapToDto).ToList();
    }

    public async Task<List<MonatsabrechnungDto>> GetAlleAbrechnungenAsync(MonatsabrechnungFilter filter)
    {
        var query = _context.Monatsabrechnungen
            .Include(m => m.Standort)
            .Include(m => m.Positionen)
            .AsQueryable();

        if (filter.StandortId.HasValue)
        {
            query = query.Where(m => m.StandortId == filter.StandortId.Value);
        }

        if (filter.Jahr.HasValue)
        {
            query = query.Where(m => m.Jahr == filter.Jahr.Value);
        }

        if (filter.Monat.HasValue)
        {
            query = query.Where(m => m.Monat == filter.Monat.Value);
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(m => m.Status == filter.Status.Value);
        }

        var abrechnungen = await query
            .OrderByDescending(m => m.Jahr)
            .ThenByDescending(m => m.Monat)
            .ToListAsync();

        return abrechnungen.Select(MapToDto).ToList();
    }

    public async Task<bool> AbrechnungVersendenAsync(Guid id)
    {
        var abrechnung = await _context.Monatsabrechnungen.FindAsync(id);
        if (abrechnung == null) return false;

        abrechnung.Status = MonatsabrechnungStatus.Versandt;
        abrechnung.VersandtAm = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AbrechnungAlsBezahltMarkierenAsync(Guid id, string zahlungsReferenz)
    {
        var abrechnung = await _context.Monatsabrechnungen.FindAsync(id);
        if (abrechnung == null) return false;

        abrechnung.Status = MonatsabrechnungStatus.Bezahlt;
        abrechnung.BezahltAm = DateTime.UtcNow;
        abrechnung.ZahlungsReferenz = zahlungsReferenz;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AbrechnungStornierenAsync(Guid id, string grund)
    {
        var abrechnung = await _context.Monatsabrechnungen.FindAsync(id);
        if (abrechnung == null) return false;

        abrechnung.Status = MonatsabrechnungStatus.Storniert;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<byte[]> AbrechnungAlsPdfAsync(Guid id)
    {
        // Placeholder - würde echtes PDF generieren
        var abrechnung = await GetAbrechnungAsync(id);
        if (abrechnung == null) throw new InvalidOperationException("Abrechnung nicht gefunden.");

        var content = $"PDF Placeholder für Abrechnung {abrechnung.AbrechnungsNummer}";
        return Encoding.UTF8.GetBytes(content);
    }

    public async Task<byte[]> AbrechnungAlsCsvAsync(MonatsabrechnungFilter filter)
    {
        var abrechnungen = await GetAlleAbrechnungenAsync(filter);
        
        var csv = new StringBuilder();
        csv.AppendLine("Abrechnungsnummer;Standort;Jahr;Monat;Netto;Brutto;Status");
        
        foreach (var abr in abrechnungen)
        {
            csv.AppendLine($"{abr.AbrechnungsNummer};{abr.StandortName};{abr.Jahr};{abr.Monat};{abr.Nettobetrag};{abr.Bruttobetrag};{abr.Status}");
        }
        
        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    public async Task<PlattformUmsatzDto> GetPlattformUmsatzAsync(int jahr, int? monat = null)
    {
        var query = _context.Monatsabrechnungen
            .Include(m => m.Standort)
            .Where(m => m.Jahr == jahr);

        if (monat.HasValue)
        {
            query = query.Where(m => m.Monat == monat.Value);
        }

        var abrechnungen = await query.ToListAsync();
        var alleStandorte = await _context.Standorte.ToListAsync();

        var result = new PlattformUmsatzDto
        {
            Jahr = jahr,
            Monat = monat,
            AnzahlStandorte = alleStandorte.Count,
            AnzahlAktiveStandorte = alleStandorte.Count(s => s.IstAktiv),
            GesamtUmsatzAllerStandorte = abrechnungen.Sum(a => a.GesamtUmsatz),
            GesamtGrundgebuehren = abrechnungen.Sum(a => a.Grundgebuehr),
            GesamtProvisionen = abrechnungen.Sum(a => a.ProvisionsBetrag),
            GesamtPlattformEinnahmen = abrechnungen.Sum(a => a.Nettobetrag),
            OffeneAbrechnungen = abrechnungen.Where(a => a.Status == MonatsabrechnungStatus.Versandt || a.Status == MonatsabrechnungStatus.Erstellt).Sum(a => a.Bruttobetrag),
            BezahlteAbrechnungen = abrechnungen.Where(a => a.Status == MonatsabrechnungStatus.Bezahlt).Sum(a => a.Bruttobetrag),
            UeberfaelligeAbrechnungen = abrechnungen.Where(a => a.Status == MonatsabrechnungStatus.Ueberfaellig).Sum(a => a.Bruttobetrag),
            StandortDetails = abrechnungen.Select(a => new StandortUmsatzDetail
            {
                StandortId = a.StandortId,
                StandortName = a.Standort.Name,
                Umsatz = a.GesamtUmsatz,
                Provision = a.ProvisionsBetrag,
                Grundgebuehr = a.Grundgebuehr,
                GesamtAbrechnung = a.Bruttobetrag,
                AbrechnungStatus = a.Status,
                AnzahlAuftraege = a.AnzahlAuftraege
            }).ToList()
        };

        return result;
    }

    private async Task<string> GeneriereAbrechnungsNummerAsync(int jahr, int monat)
    {
        var count = await _context.Monatsabrechnungen
            .Where(m => m.Jahr == jahr && m.Monat == monat)
            .CountAsync();

        return $"ABR-{jahr}-{monat:D2}-{(count + 1):D3}";
    }

    private MonatsabrechnungDto MapToDto(Monatsabrechnung abrechnung)
    {
        var culture = new CultureInfo("de-DE");
        var zeitraum = new DateTime(abrechnung.Jahr, abrechnung.Monat, 1).ToString("MMMM yyyy", culture);

        return new MonatsabrechnungDto
        {
            Id = abrechnung.Id,
            StandortId = abrechnung.StandortId,
            StandortName = abrechnung.Standort?.Name ?? string.Empty,
            AbrechnungsNummer = abrechnung.AbrechnungsNummer,
            Jahr = abrechnung.Jahr,
            Monat = abrechnung.Monat,
            Zeitraum = zeitraum,
            AnzahlRechnungen = abrechnung.AnzahlRechnungen,
            AnzahlAuftraege = abrechnung.AnzahlAuftraege,
            GesamtUmsatz = abrechnung.GesamtUmsatz,
            Grundgebuehr = abrechnung.Grundgebuehr,
            ProvisionsProzentsatz = abrechnung.ProvisionsProzentsatz,
            ProvisionsBetrag = abrechnung.ProvisionsBetrag,
            Nettobetrag = abrechnung.Nettobetrag,
            Steuerbetrag = abrechnung.Steuerbetrag,
            Bruttobetrag = abrechnung.Bruttobetrag,
            Status = abrechnung.Status,
            Faelligkeitsdatum = abrechnung.Faelligkeitsdatum,
            BezahltAm = abrechnung.BezahltAm,
            Positionen = abrechnung.Positionen.Select(p => new MonatsabrechnungPositionDto
            {
                Id = p.Id,
                Position = p.Position,
                Typ = p.Typ,
                Beschreibung = p.Beschreibung,
                Menge = p.Menge,
                Einheit = p.Einheit,
                Einzelpreis = p.Einzelpreis,
                Nettobetrag = p.Nettobetrag,
                Steuersatz = p.Steuersatz,
                Steuerbetrag = p.Steuerbetrag,
                Bruttobetrag = p.Bruttobetrag
            }).OrderBy(p => p.Position).ToList()
        };
    }
}

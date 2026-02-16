using FahrzeugZulassung.Application.DTOs.Provision;
using FahrzeugZulassung.Application.Interfaces;
using FahrzeugZulassung.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FahrzeugZulassung.Infrastructure.Services;

public class ProvisionsService : IProvisionsService
{
    private readonly ApplicationDbContext _context;

    public ProvisionsService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProvisionsModellDto> GetStandardModellAsync()
    {
        var standard = await _context.ProvisionsModelle
            .Where(p => p.IstStandard && p.IstAktiv)
            .OrderByDescending(p => p.GueltigAb)
            .FirstOrDefaultAsync();

        if (standard == null)
        {
            throw new InvalidOperationException("Kein Standard-Provisionsmodell gefunden.");
        }

        return MapToDto(standard);
    }

    public async Task<ProvisionsModellDto> SetStandardModellAsync(ProvisionsModellUpdateDto dto)
    {
        // Deaktiviere bisheriges Standard-Modell
        var alteStandards = await _context.ProvisionsModelle
            .Where(p => p.IstStandard && p.IstAktiv)
            .ToListAsync();
        
        foreach (var alt in alteStandards)
        {
            alt.IstAktiv = false;
            alt.GueltigBis = DateTime.UtcNow;
        }

        // Erstelle neues Standard-Modell
        var neuesModell = new Domain.Entities.ProvisionsModell
        {
            Id = Guid.NewGuid(),
            IstStandard = true,
            StandortId = null,
            MonatlicheGrundgebuehr = dto.MonatlicheGrundgebuehr,
            ProvisionsProzentsatz = dto.ProvisionsProzentsatz,
            MinProvisionProRechnung = dto.MinProvisionProRechnung,
            MaxProvisionProRechnung = dto.MaxProvisionProRechnung,
            Aenderungsgrund = dto.Aenderungsgrund,
            GueltigAb = DateTime.UtcNow,
            IstAktiv = true,
            ErstelltAm = DateTime.UtcNow
        };

        _context.ProvisionsModelle.Add(neuesModell);
        await _context.SaveChangesAsync();

        return MapToDto(neuesModell);
    }

    public async Task<ProvisionsModellDto> GetModellFuerStandortAsync(Guid standortId)
    {
        // Suche standort-spezifisches Modell
        var standortModell = await _context.ProvisionsModelle
            .Include(p => p.Standort)
            .Where(p => p.StandortId == standortId && p.IstAktiv)
            .OrderByDescending(p => p.GueltigAb)
            .FirstOrDefaultAsync();

        if (standortModell != null)
        {
            return MapToDto(standortModell);
        }

        // Fallback auf Standard-Modell
        return await GetStandardModellAsync();
    }

    public async Task<ProvisionsModellDto> SetModellFuerStandortAsync(Guid standortId, ProvisionsModellUpdateDto dto)
    {
        // Deaktiviere bisherige standort-spezifische Modelle
        var alteModelle = await _context.ProvisionsModelle
            .Where(p => p.StandortId == standortId && p.IstAktiv)
            .ToListAsync();
        
        foreach (var alt in alteModelle)
        {
            alt.IstAktiv = false;
            alt.GueltigBis = DateTime.UtcNow;
        }

        // Erstelle neues standort-spezifisches Modell
        var neuesModell = new Domain.Entities.ProvisionsModell
        {
            Id = Guid.NewGuid(),
            IstStandard = false,
            StandortId = standortId,
            MonatlicheGrundgebuehr = dto.MonatlicheGrundgebuehr,
            ProvisionsProzentsatz = dto.ProvisionsProzentsatz,
            MinProvisionProRechnung = dto.MinProvisionProRechnung,
            MaxProvisionProRechnung = dto.MaxProvisionProRechnung,
            Aenderungsgrund = dto.Aenderungsgrund,
            GueltigAb = DateTime.UtcNow,
            IstAktiv = true,
            ErstelltAm = DateTime.UtcNow
        };

        _context.ProvisionsModelle.Add(neuesModell);
        await _context.SaveChangesAsync();

        return await GetModellFuerStandortAsync(standortId);
    }

    public async Task ResetAufStandardAsync(Guid standortId)
    {
        var standortModelle = await _context.ProvisionsModelle
            .Where(p => p.StandortId == standortId && p.IstAktiv)
            .ToListAsync();
        
        foreach (var modell in standortModelle)
        {
            modell.IstAktiv = false;
            modell.GueltigBis = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<ProvisionsBerechnung> BerechneProvisionAsync(Guid standortId, int jahr, int monat)
    {
        var standort = await _context.Standorte.FindAsync(standortId);
        if (standort == null)
        {
            throw new InvalidOperationException("Standort nicht gefunden.");
        }

        var modell = await GetModellFuerStandortAsync(standortId);

        // Für diese Demo geben wir eine Beispiel-Berechnung zurück
        // In einer echten Implementierung würden hier die tatsächlichen Rechnungen abgefragt
        var berechnung = new ProvisionsBerechnung
        {
            StandortId = standortId,
            StandortName = standort.Name,
            Jahr = jahr,
            Monat = monat,
            AnzahlRechnungen = 0,
            AnzahlAuftraege = 0,
            GesamtUmsatz = 0m,
            Grundgebuehr = modell.MonatlicheGrundgebuehr,
            ProvisionsProzentsatz = modell.ProvisionsProzentsatz,
            ProvisionsBetrag = 0m,
            Nettobetrag = modell.MonatlicheGrundgebuehr,
            Steuerbetrag = modell.MonatlicheGrundgebuehr * 0.19m,
            Bruttobetrag = modell.MonatlicheGrundgebuehr * 1.19m
        };

        return berechnung;
    }

    public async Task<List<ProvisionsModellDto>> GetHistorieAsync(Guid standortId)
    {
        var historie = await _context.ProvisionsModelle
            .Include(p => p.Standort)
            .Where(p => p.StandortId == standortId)
            .OrderByDescending(p => p.GueltigAb)
            .ToListAsync();

        return historie.Select(MapToDto).ToList();
    }

    private ProvisionsModellDto MapToDto(Domain.Entities.ProvisionsModell modell)
    {
        return new ProvisionsModellDto
        {
            Id = modell.Id,
            StandortId = modell.StandortId,
            StandortName = modell.Standort?.Name,
            IstStandard = modell.IstStandard,
            MonatlicheGrundgebuehr = modell.MonatlicheGrundgebuehr,
            ProvisionsProzentsatz = modell.ProvisionsProzentsatz,
            MinProvisionProRechnung = modell.MinProvisionProRechnung,
            MaxProvisionProRechnung = modell.MaxProvisionProRechnung,
            GueltigAb = modell.GueltigAb,
            GueltigBis = modell.GueltigBis,
            IstAktiv = modell.IstAktiv
        };
    }
}

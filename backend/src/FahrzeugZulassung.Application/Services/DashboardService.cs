using FahrzeugZulassung.Application.DTOs.Dashboard;
using FahrzeugZulassung.Application.Interfaces;
using FahrzeugZulassung.Domain.Enums;
using FahrzeugZulassung.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FahrzeugZulassung.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "DashboardData";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public DashboardService(
        AppDbContext context,
        IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<DashboardResponseDto> GetDashboardDataAsync(CancellationToken cancellationToken = default)
    {
        // Try to get cached data
        if (_cache.TryGetValue(CacheKey, out DashboardResponseDto? cachedData) && cachedData != null)
        {
            return cachedData;
        }

        var now = DateTime.UtcNow;
        var currentMonth = new DateTime(now.Year, now.Month, 1);
        var lastMonth = currentMonth.AddMonths(-1);

        // Get KPIs
        var offeneAuftraege = await _context.Auftraege
            .CountAsync(a => a.Status == AuftragStatus.Entwurf || 
                           a.Status == AuftragStatus.Eingereicht, 
                       cancellationToken);

        var inBearbeitung = await _context.Auftraege
            .CountAsync(a => a.Status == AuftragStatus.InBearbeitung || 
                           a.Status == AuftragStatus.WartAufDokumente ||
                           a.Status == AuftragStatus.WartAufZahlung ||
                           a.Status == AuftragStatus.AnIKfzGesendet, 
                       cancellationToken);

        var abgeschlossen = await _context.Auftraege
            .CountAsync(a => a.Status == AuftragStatus.Abgeschlossen, 
                       cancellationToken);

        // Get current month revenue
        var monatsUmsatz = await _context.Rechnungen
            .Where(r => r.Status == RechnungStatus.Bezahlt && 
                       r.BezahltAm.HasValue &&
                       r.BezahltAm.Value >= currentMonth)
            .SumAsync(r => r.Betrag, cancellationToken);

        // Get recent orders (last 10)
        var recentAuftraege = await _context.Auftraege
            .Include(a => a.Kunde)
            .Include(a => a.Rechnungen)
            .OrderByDescending(a => a.ErstelltAm)
            .Take(10)
            .Select(a => new RecentAuftragDto
            {
                Id = a.Id,
                Typ = a.Typ.ToString(),
                Status = a.Status.ToString(),
                KundeName = $"{a.Kunde.Vorname} {a.Kunde.Nachname}",
                ErstelltAm = a.ErstelltAm,
                Preis = a.Rechnungen.FirstOrDefault() != null ? a.Rechnungen.FirstOrDefault()!.Betrag : null
            })
            .ToListAsync(cancellationToken);

        // Get monthly revenue for the last 12 months
        var startDate = currentMonth.AddMonths(-11);
        var monthlyRevenue = await _context.Rechnungen
            .Where(r => r.Status == RechnungStatus.Bezahlt && 
                       r.BezahltAm.HasValue &&
                       r.BezahltAm.Value >= startDate)
            .GroupBy(r => new { 
                Year = r.BezahltAm!.Value.Year, 
                Month = r.BezahltAm!.Value.Month 
            })
            .Select(g => new UmsatzMonatDto
            {
                Jahr = g.Key.Year,
                Monat = g.Key.Month,
                Umsatz = g.Sum(r => r.Betrag)
            })
            .ToListAsync(cancellationToken);

        // Fill in missing months with zero revenue
        var umsatzProMonat = new List<UmsatzMonatDto>();
        for (int i = 0; i < 12; i++)
        {
            var date = startDate.AddMonths(i);
            var existing = monthlyRevenue.FirstOrDefault(m => m.Jahr == date.Year && m.Monat == date.Month);
            
            umsatzProMonat.Add(existing ?? new UmsatzMonatDto
            {
                Jahr = date.Year,
                Monat = date.Month,
                Umsatz = 0
            });
        }

        var dashboardData = new DashboardResponseDto
        {
            OffeneAuftraege = offeneAuftraege,
            InBearbeitung = inBearbeitung,
            Abgeschlossen = abgeschlossen,
            MonatsUmsatz = monatsUmsatz,
            RecentAuftraege = recentAuftraege,
            UmsatzProMonat = umsatzProMonat.OrderBy(u => u.Jahr).ThenBy(u => u.Monat).ToList()
        };

        // Cache the result
        _cache.Set(CacheKey, dashboardData, CacheDuration);

        return dashboardData;
    }
}

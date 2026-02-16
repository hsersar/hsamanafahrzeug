namespace FahrzeugZulassung.Application.DTOs.Dashboard;

public class DashboardResponseDto
{
    public int OffeneAuftraege { get; set; }
    public int InBearbeitung { get; set; }
    public int Abgeschlossen { get; set; }
    public decimal MonatsUmsatz { get; set; }
    public List<RecentAuftragDto> RecentAuftraege { get; set; } = new();
    public List<UmsatzMonatDto> UmsatzProMonat { get; set; } = new();
}

public class RecentAuftragDto
{
    public Guid Id { get; set; }
    public string Typ { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string KundeName { get; set; } = string.Empty;
    public DateTime ErstelltAm { get; set; }
    public decimal? Preis { get; set; }
}

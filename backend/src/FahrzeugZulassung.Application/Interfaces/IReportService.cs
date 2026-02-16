using FahrzeugZulassung.Domain.Enums;

namespace FahrzeugZulassung.Application.Interfaces;

public interface IReportService
{
    // Report-Daten abrufen
    Task<AuftragsReportData> GetAuftragsReportAsync(ReportFilter filter);
    Task<UmsatzReportData> GetUmsatzReportAsync(ReportFilter filter);
    Task<MitarbeiterReportData> GetMitarbeiterReportAsync(ReportFilter filter);
    Task<KundenReportData> GetKundenReportAsync(ReportFilter filter);
    Task<ZahlungsReportData> GetZahlungsReportAsync(ReportFilter filter);
    Task<StandortReportData> GetStandortReportAsync(ReportFilter filter);
    
    // Export-Funktionen
    Task<byte[]> ExportAlsCsvAsync<T>(IEnumerable<T> daten, string reportName);
    Task<byte[]> ExportAlsPdfAsync(ReportPdfModel model);
}

public class ReportFilter
{
    public Guid? StandortId { get; set; }
    public DateTime? VonDatum { get; set; }
    public DateTime? BisDatum { get; set; }
    public AuftragStatus? Status { get; set; }
    public AuftragTyp? Typ { get; set; }
    public Guid? MitarbeiterId { get; set; }
    public string? SortierungFeld { get; set; }
    public bool Absteigend { get; set; } = true;
}

public class AuftragsReportData
{
    public List<AuftragReportItem> Auftraege { get; set; } = new();
    public int GesamtAnzahl { get; set; }
    public decimal GesamtBetrag { get; set; }
}

public class AuftragReportItem
{
    public Guid Id { get; set; }
    public DateTime Datum { get; set; }
    public string Kunde { get; set; } = string.Empty;
    public string Typ { get; set; } = string.Empty;
    public string? Kennzeichen { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class UmsatzReportData
{
    public decimal NettoSumme { get; set; }
    public decimal MwStSumme { get; set; }
    public decimal BruttoSumme { get; set; }
}

public class MitarbeiterReportData
{
    public List<MitarbeiterReportItem> Mitarbeiter { get; set; } = new();
}

public class MitarbeiterReportItem
{
    public string Name { get; set; } = string.Empty;
    public int AnzahlAuftraege { get; set; }
}

public class KundenReportData
{
    public List<KundeReportItem> Kunden { get; set; } = new();
}

public class KundeReportItem
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int AnzahlAuftraege { get; set; }
}

public class ZahlungsReportData
{
    public decimal GesamtBetrag { get; set; }
}

public class StandortReportData
{
    public List<StandortReportItem> Standorte { get; set; } = new();
}

public class StandortReportItem
{
    public string Name { get; set; } = string.Empty;
    public int AnzahlAuftraege { get; set; }
    public decimal Umsatz { get; set; }
}

public class ReportPdfModel
{
    public string Titel { get; set; } = string.Empty;
    public string Zeitraum { get; set; } = string.Empty;
    public object Daten { get; set; } = new();
}

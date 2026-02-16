using FahrzeugZulassung.Application.Interfaces;

namespace FahrzeugZulassung.Infrastructure.Services;

public class ReportService : IReportService
{
    private readonly CsvExportService _csvExportService;
    private readonly PdfReportService _pdfReportService;

    public ReportService(CsvExportService csvExportService, PdfReportService pdfReportService)
    {
        _csvExportService = csvExportService;
        _pdfReportService = pdfReportService;
    }

    public async Task<AuftragsReportData> GetAuftragsReportAsync(ReportFilter filter)
    {
        // In einer echten Implementierung würden hier Daten aus der Datenbank geladen
        await Task.CompletedTask;
        
        return new AuftragsReportData
        {
            Auftraege = new List<AuftragReportItem>(),
            GesamtAnzahl = 0,
            GesamtBetrag = 0
        };
    }

    public async Task<UmsatzReportData> GetUmsatzReportAsync(ReportFilter filter)
    {
        await Task.CompletedTask;
        return new UmsatzReportData();
    }

    public async Task<MitarbeiterReportData> GetMitarbeiterReportAsync(ReportFilter filter)
    {
        await Task.CompletedTask;
        return new MitarbeiterReportData();
    }

    public async Task<KundenReportData> GetKundenReportAsync(ReportFilter filter)
    {
        await Task.CompletedTask;
        return new KundenReportData();
    }

    public async Task<ZahlungsReportData> GetZahlungsReportAsync(ReportFilter filter)
    {
        await Task.CompletedTask;
        return new ZahlungsReportData();
    }

    public async Task<StandortReportData> GetStandortReportAsync(ReportFilter filter)
    {
        await Task.CompletedTask;
        return new StandortReportData();
    }

    public async Task<byte[]> ExportAlsCsvAsync<T>(IEnumerable<T> daten, string reportName)
    {
        return await _csvExportService.ExportAlsCsvAsync(daten, reportName);
    }

    public async Task<byte[]> ExportAlsPdfAsync(ReportPdfModel model)
    {
        return await _pdfReportService.ExportAlsPdfAsync(model);
    }
}

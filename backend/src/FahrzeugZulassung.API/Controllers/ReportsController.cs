using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FahrzeugZulassung.Application.Interfaces;

namespace FahrzeugZulassung.API.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Get orders report
    /// </summary>
    [HttpGet("auftraege")]
    public async Task<ActionResult<AuftragsReportData>> GetAuftragsReport([FromQuery] ReportFilter filter)
    {
        var data = await _reportService.GetAuftragsReportAsync(filter);
        return Ok(data);
    }

    /// <summary>
    /// Export orders report as CSV
    /// </summary>
    [HttpGet("auftraege/csv")]
    public async Task<IActionResult> ExportAuftraegeAlsCsv([FromQuery] ReportFilter filter)
    {
        var data = await _reportService.GetAuftragsReportAsync(filter);
        var csv = await _reportService.ExportAlsCsvAsync(data.Auftraege, "Auftraege");
        
        return File(csv, "text/csv", $"Auftraege_{DateTime.Now:yyyyMMdd}.csv");
    }

    /// <summary>
    /// Export orders report as PDF
    /// </summary>
    [HttpGet("auftraege/pdf")]
    public async Task<IActionResult> ExportAuftraegeAlsPdf([FromQuery] ReportFilter filter)
    {
        var data = await _reportService.GetAuftragsReportAsync(filter);
        var model = new ReportPdfModel
        {
            Titel = "Auftragsübersicht",
            Zeitraum = $"{filter.VonDatum:dd.MM.yyyy} - {filter.BisDatum:dd.MM.yyyy}",
            Daten = data
        };
        
        var pdf = await _reportService.ExportAlsPdfAsync(model);
        return File(pdf, "application/pdf", $"Auftraege_{DateTime.Now:yyyyMMdd}.pdf");
    }

    /// <summary>
    /// Get revenue report
    /// </summary>
    [HttpGet("umsatz")]
    public async Task<ActionResult<UmsatzReportData>> GetUmsatzReport([FromQuery] ReportFilter filter)
    {
        var data = await _reportService.GetUmsatzReportAsync(filter);
        return Ok(data);
    }

    /// <summary>
    /// Get employee report
    /// </summary>
    [HttpGet("mitarbeiter")]
    public async Task<ActionResult<MitarbeiterReportData>> GetMitarbeiterReport([FromQuery] ReportFilter filter)
    {
        var data = await _reportService.GetMitarbeiterReportAsync(filter);
        return Ok(data);
    }

    /// <summary>
    /// Get customer report
    /// </summary>
    [HttpGet("kunden")]
    public async Task<ActionResult<KundenReportData>> GetKundenReport([FromQuery] ReportFilter filter)
    {
        var data = await _reportService.GetKundenReportAsync(filter);
        return Ok(data);
    }

    /// <summary>
    /// Get payment report
    /// </summary>
    [HttpGet("zahlungen")]
    public async Task<ActionResult<ZahlungsReportData>> GetZahlungsReport([FromQuery] ReportFilter filter)
    {
        var data = await _reportService.GetZahlungsReportAsync(filter);
        return Ok(data);
    }

    /// <summary>
    /// Get location report (SuperAdmin only)
    /// </summary>
    [HttpGet("standorte")]
    [Authorize(Policy = "SuperAdmin")]
    public async Task<ActionResult<StandortReportData>> GetStandortReport([FromQuery] ReportFilter filter)
    {
        var data = await _reportService.GetStandortReportAsync(filter);
        return Ok(data);
    }
}

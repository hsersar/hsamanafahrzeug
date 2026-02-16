using Microsoft.AspNetCore.Mvc;
using FahrzeugZulassung.Application.Interfaces;
using FahrzeugZulassung.Application.DTOs.Provision;

namespace FahrzeugZulassung.API.Controllers;

[ApiController]
[Route("api/monatsabrechnungen")]
public class MonatsabrechnungenController : ControllerBase
{
    private readonly IMonatsabrechnungService _abrechnungService;

    public MonatsabrechnungenController(IMonatsabrechnungService abrechnungService)
    {
        _abrechnungService = abrechnungService;
    }

    // ===== SuperAdmin: Abrechnungen erstellen & verwalten =====
    
    /// <summary>
    /// Erstellt eine neue Monatsabrechnung für einen Standort
    /// </summary>
    [HttpPost("erstellen")]
    //[Authorize(Policy = "SuperAdmin")]
    public async Task<ActionResult<MonatsabrechnungDto>> Erstellen([FromBody] AbrechnungErstellenRequest request)
    {
        try
        {
            var abrechnung = await _abrechnungService.ErstelleAbrechnungAsync(
                request.StandortId, request.Jahr, request.Monat);
            return CreatedAtAction(nameof(GetById), new { id = abrechnung.Id }, abrechnung);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    /// <summary>
    /// Erstellt Abrechnungen für ALLE aktiven Standorte
    /// </summary>
    [HttpPost("alle-erstellen")]
    //[Authorize(Policy = "SuperAdmin")]
    public async Task<ActionResult<List<MonatsabrechnungDto>>> AlleErstellen(
        [FromQuery] int jahr, [FromQuery] int monat)
    {
        try
        {
            var abrechnungen = await _abrechnungService.ErstelleAlleAbrechnungenAsync(jahr, monat);
            return Ok(abrechnungen);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    /// <summary>
    /// Holt alle Abrechnungen (mit optionalen Filtern)
    /// </summary>
    [HttpGet]
    //[Authorize(Policy = "SuperAdmin")]
    public async Task<ActionResult<List<MonatsabrechnungDto>>> GetAlle([FromQuery] MonatsabrechnungFilter filter)
    {
        var abrechnungen = await _abrechnungService.GetAlleAbrechnungenAsync(filter);
        return Ok(abrechnungen);
    }
    
    /// <summary>
    /// Holt eine bestimmte Abrechnung nach ID
    /// </summary>
    [HttpGet("{id}")]
    //[Authorize(Policy = "StandortAdmin")]
    public async Task<ActionResult<MonatsabrechnungDto>> GetById(Guid id)
    {
        var abrechnung = await _abrechnungService.GetAbrechnungAsync(id);
        if (abrechnung == null)
            return NotFound();
        
        return Ok(abrechnung);
    }
    
    /// <summary>
    /// Holt alle Abrechnungen für einen bestimmten Standort
    /// </summary>
    [HttpGet("standort/{standortId}")]
    //[Authorize(Policy = "StandortAdmin")]
    public async Task<ActionResult<List<MonatsabrechnungDto>>> GetFuerStandort(
        Guid standortId, [FromQuery] int? jahr)
    {
        var abrechnungen = await _abrechnungService.GetAbrechnungenFuerStandortAsync(standortId, jahr);
        return Ok(abrechnungen);
    }
    
    /// <summary>
    /// Versendet eine Abrechnung per E-Mail
    /// </summary>
    [HttpPost("{id}/versenden")]
    //[Authorize(Policy = "SuperAdmin")]
    public async Task<IActionResult> Versenden(Guid id)
    {
        var erfolg = await _abrechnungService.AbrechnungVersendenAsync(id);
        if (!erfolg)
            return NotFound();
        
        return NoContent();
    }
    
    /// <summary>
    /// Markiert eine Abrechnung als bezahlt
    /// </summary>
    [HttpPost("{id}/bezahlt")]
    //[Authorize(Policy = "SuperAdmin")]
    public async Task<IActionResult> AlsBezahltMarkieren(Guid id, [FromBody] ZahlungsBestaetigung dto)
    {
        var erfolg = await _abrechnungService.AbrechnungAlsBezahltMarkierenAsync(id, dto.ZahlungsReferenz);
        if (!erfolg)
            return NotFound();
        
        return NoContent();
    }
    
    /// <summary>
    /// Storniert eine Abrechnung
    /// </summary>
    [HttpPost("{id}/stornieren")]
    //[Authorize(Policy = "SuperAdmin")]
    public async Task<IActionResult> Stornieren(Guid id, [FromBody] StornierungsRequest request)
    {
        var erfolg = await _abrechnungService.AbrechnungStornierenAsync(id, request.Grund);
        if (!erfolg)
            return NotFound();
        
        return NoContent();
    }
    
    /// <summary>
    /// Lädt eine Abrechnung als PDF herunter
    /// </summary>
    [HttpGet("{id}/pdf")]
    //[Authorize(Policy = "StandortAdmin")]
    public async Task<IActionResult> AlsPdf(Guid id)
    {
        try
        {
            var pdfBytes = await _abrechnungService.AbrechnungAlsPdfAsync(id);
            return File(pdfBytes, "application/pdf", $"Abrechnung-{id}.pdf");
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
    
    /// <summary>
    /// Exportiert Abrechnungen als CSV
    /// </summary>
    [HttpGet("export/csv")]
    //[Authorize(Policy = "SuperAdmin")]
    public async Task<IActionResult> ExportAlsCsv([FromQuery] MonatsabrechnungFilter filter)
    {
        var csvBytes = await _abrechnungService.AbrechnungAlsCsvAsync(filter);
        return File(csvBytes, "text/csv", "Abrechnungen.csv");
    }
    
    // ===== Plattform-Dashboard für SuperAdmin =====
    
    /// <summary>
    /// Holt aggregierte Umsatzdaten der gesamten Plattform
    /// </summary>
    [HttpGet("plattform-umsatz")]
    //[Authorize(Policy = "SuperAdmin")]
    public async Task<ActionResult<PlattformUmsatzDto>> GetPlattformUmsatz(
        [FromQuery] int jahr, [FromQuery] int? monat = null)
    {
        var umsatz = await _abrechnungService.GetPlattformUmsatzAsync(jahr, monat);
        return Ok(umsatz);
    }
}

// Request DTOs
public class AbrechnungErstellenRequest
{
    public Guid StandortId { get; set; }
    public int Jahr { get; set; }
    public int Monat { get; set; }
}

public class ZahlungsBestaetigung
{
    public string ZahlungsReferenz { get; set; } = string.Empty;
}

public class StornierungsRequest
{
    public string Grund { get; set; } = string.Empty;
}

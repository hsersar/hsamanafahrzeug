using Microsoft.AspNetCore.Mvc;
using FahrzeugZulassung.Application.Interfaces;
using FahrzeugZulassung.Application.DTOs.Provision;

namespace FahrzeugZulassung.API.Controllers;

[ApiController]
[Route("api/provisionen")]
public class ProvisionsController : ControllerBase
{
    private readonly IProvisionsService _provisionsService;

    public ProvisionsController(IProvisionsService provisionsService)
    {
        _provisionsService = provisionsService;
    }

    // ===== Standard-Provisionsmodell (SuperAdmin) =====
    
    /// <summary>
    /// Holt das aktuelle Standard-Provisionsmodell
    /// </summary>
    [HttpGet("standard")]
    //[Authorize(Policy = "SuperAdmin")]
    public async Task<ActionResult<ProvisionsModellDto>> GetStandard()
    {
        try
        {
            var modell = await _provisionsService.GetStandardModellAsync();
            return Ok(modell);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
    
    /// <summary>
    /// Setzt ein neues Standard-Provisionsmodell (nur SuperAdmin)
    /// </summary>
    [HttpPut("standard")]
    //[Authorize(Policy = "SuperAdmin")]
    public async Task<ActionResult<ProvisionsModellDto>> SetStandard([FromBody] ProvisionsModellUpdateDto dto)
    {
        try
        {
            var modell = await _provisionsService.SetStandardModellAsync(dto);
            return Ok(modell);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    // ===== Standort-spezifische Provisionen =====
    
    /// <summary>
    /// Holt das Provisionsmodell für einen bestimmten Standort
    /// </summary>
    [HttpGet("standort/{standortId}")]
    //[Authorize(Policy = "StandortAdmin")]
    public async Task<ActionResult<ProvisionsModellDto>> GetFuerStandort(Guid standortId)
    {
        try
        {
            var modell = await _provisionsService.GetModellFuerStandortAsync(standortId);
            return Ok(modell);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
    
    /// <summary>
    /// Setzt ein individuelles Provisionsmodell für einen Standort
    /// SuperAdmin ODER StandortAdmin des eigenen Standorts
    /// </summary>
    [HttpPut("standort/{standortId}")]
    //[Authorize(Policy = "StandortAdmin")]
    public async Task<ActionResult<ProvisionsModellDto>> SetFuerStandort(
        Guid standortId, [FromBody] ProvisionsModellUpdateDto dto)
    {
        try
        {
            var modell = await _provisionsService.SetModellFuerStandortAsync(standortId, dto);
            return Ok(modell);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    /// <summary>
    /// Setzt das Provisionsmodell eines Standorts auf den Standard zurück
    /// </summary>
    [HttpDelete("standort/{standortId}")]
    //[Authorize(Policy = "StandortAdmin")]
    public async Task<IActionResult> ResetAufStandard(Guid standortId)
    {
        try
        {
            await _provisionsService.ResetAufStandardAsync(standortId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    /// <summary>
    /// Holt die Änderungshistorie des Provisionsmodells eines Standorts
    /// </summary>
    [HttpGet("standort/{standortId}/historie")]
    //[Authorize(Policy = "StandortAdmin")]
    public async Task<ActionResult<List<ProvisionsModellDto>>> GetHistorie(Guid standortId)
    {
        var historie = await _provisionsService.GetHistorieAsync(standortId);
        return Ok(historie);
    }
    
    /// <summary>
    /// Berechnet eine Provisions-Vorschau für einen bestimmten Zeitraum
    /// </summary>
    [HttpGet("standort/{standortId}/vorschau")]
    //[Authorize(Policy = "StandortAdmin")]
    public async Task<ActionResult<ProvisionsBerechnung>> GetVorschau(
        Guid standortId, [FromQuery] int jahr, [FromQuery] int monat)
    {
        try
        {
            var berechnung = await _provisionsService.BerechneProvisionAsync(standortId, jahr, monat);
            return Ok(berechnung);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

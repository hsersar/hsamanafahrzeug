using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FahrzeugZulassung.Application.Interfaces;

namespace FahrzeugZulassung.API.Controllers;

[ApiController]
[Route("api/evb")]
[Authorize]
public class EVBController : ControllerBase
{
    private readonly IEVBService _evbService;

    public EVBController(IEVBService evbService)
    {
        _evbService = evbService;
    }

    /// <summary>
    /// Validate eVB number format and check with GDV
    /// </summary>
    [HttpPost("validieren")]
    public async Task<ActionResult<EVBValidierungResult>> Validieren([FromBody] EVBValidierungRequest request)
    {
        try
        {
            var result = await _evbService.ValidiereEVBNummerAsync(request.EVBNummer);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get list of German insurance companies
    /// </summary>
    [HttpGet("versicherer")]
    public ActionResult<List<VersichererInfo>> GetVersicherer()
    {
        var versicherer = new List<VersichererInfo>
        {
            new() { Id = 1, Name = "Allianz" },
            new() { Id = 2, Name = "HUK-COBURG" },
            new() { Id = 3, Name = "ADAC" },
            new() { Id = 4, Name = "AXA" },
            new() { Id = 5, Name = "Generali" },
            new() { Id = 6, Name = "HDI" },
            new() { Id = 7, Name = "R+V Versicherung" },
            new() { Id = 8, Name = "DEVK" },
            new() { Id = 9, Name = "VHV" },
            new() { Id = 10, Name = "Württembergische" },
            new() { Id = 11, Name = "LVM Versicherung" },
            new() { Id = 12, Name = "Provinzial" },
            new() { Id = 13, Name = "Signal Iduna" },
            new() { Id = 14, Name = "Zurich" },
            new() { Id = 15, Name = "Gothaer" },
            new() { Id = 16, Name = "ERGO" }
        };

        return Ok(versicherer);
    }
}

public class EVBValidierungRequest
{
    public string EVBNummer { get; set; } = string.Empty;
}

public class VersichererInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

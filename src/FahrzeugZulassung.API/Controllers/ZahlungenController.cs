using FahrzeugZulassung.Application.Interfaces;
using FahrzeugZulassung.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FahrzeugZulassung.API.Controllers;

[ApiController]
[Route("api/zahlungen")]
public class ZahlungenController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<ZahlungenController> _logger;

    public ZahlungenController(IPaymentService paymentService, ILogger<ZahlungenController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    [HttpPost("initiieren")]
    public async Task<IActionResult> InitiiereZahlung([FromBody] ZahlungInitRequest request)
    {
        try
        {
            var result = await _paymentService.InitiiereZahlungAsync(request.RechnungId, request.Methode);
            
            if (!result.Erfolg)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Initiieren der Zahlung");
            return StatusCode(500, new { message = "Fehler beim Initiieren der Zahlung" });
        }
    }

    [HttpPost("{id}/bestaetigen")]
    public async Task<IActionResult> BestaetigeZahlung(Guid id)
    {
        try
        {
            var result = await _paymentService.VerarbeiteZahlungAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Bestätigen der Zahlung");
            return StatusCode(500, new { message = "Fehler beim Bestätigen der Zahlung" });
        }
    }

    [HttpGet("methoden")]
    public IActionResult GetVerfuegbareMethoden()
    {
        var methoden = new[]
        {
            new { Id = (int)ZahlungsMethode.Kreditkarte, Name = "Kreditkarte", Icon = "💳" },
            new { Id = (int)ZahlungsMethode.SEPALastschrift, Name = "SEPA Lastschrift", Icon = "🏦" },
            new { Id = (int)ZahlungsMethode.PayPal, Name = "PayPal", Icon = "🅿️" },
            new { Id = (int)ZahlungsMethode.Ueberweisung, Name = "Überweisung", Icon = "🏛️" }
        };

        return Ok(methoden);
    }

    [HttpPost("{id}/stornieren")]
    [Authorize(Policy = "Mitarbeiter")]
    public async Task<IActionResult> Stornieren(Guid id, [FromBody] StornierenRequest request)
    {
        try
        {
            var success = await _paymentService.StornierenAsync(id, request.Grund);
            
            if (!success)
                return NotFound(new { message = "Zahlung nicht gefunden" });

            return Ok(new { message = "Zahlung erfolgreich storniert" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Stornieren der Zahlung");
            return StatusCode(500, new { message = "Fehler beim Stornieren" });
        }
    }
}

public class ZahlungInitRequest
{
    public Guid RechnungId { get; set; }
    public ZahlungsMethode Methode { get; set; }
}

public class StornierenRequest
{
    public string Grund { get; set; } = string.Empty;
}

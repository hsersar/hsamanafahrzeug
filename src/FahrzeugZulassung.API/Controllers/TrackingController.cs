using FahrzeugZulassung.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FahrzeugZulassung.API.Controllers;

[ApiController]
[Route("api/tracking")]
public class TrackingController : ControllerBase
{
    private readonly ITrackingService _trackingService;
    private readonly ILogger<TrackingController> _logger;

    public TrackingController(ITrackingService trackingService, ILogger<TrackingController> logger)
    {
        _trackingService = trackingService;
        _logger = logger;
    }

    /// <summary>
    /// Get tracking status - PUBLIC endpoint, no authentication required
    /// </summary>
    [HttpGet("{trackingCode}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetStatus(string trackingCode, [FromQuery] string token)
    {
        try
        {
            var status = await _trackingService.GetStatusByTokenAsync(trackingCode, token);
            
            if (status == null)
                return NotFound(new { message = "Tracking-Code oder Token ungültig" });

            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Abrufen des Tracking-Status");
            return StatusCode(500, new { message = "Interner Serverfehler" });
        }
    }

    /// <summary>
    /// Get QR code image - PUBLIC endpoint
    /// </summary>
    [HttpGet("{trackingCode}/qrcode")]
    [AllowAnonymous]
    public async Task<IActionResult> GetQRCode(string trackingCode)
    {
        try
        {
            // This is simplified - in real implementation we'd validate the tracking code exists
            var trackingUrl = $"{Request.Scheme}://{Request.Host}/tracking/{trackingCode}";
            var qrCodeBytes = await _trackingService.GeneriereQRCodeAsync(trackingUrl);
            
            return File(qrCodeBytes, "image/png");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Generieren des QR-Codes");
            return StatusCode(500, new { message = "Fehler beim Generieren des QR-Codes" });
        }
    }
}

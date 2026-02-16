using System.Text.Json;
using FahrzeugZulassung.Application.DTOs.Signatur;
using FahrzeugZulassung.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FahrzeugZulassung.API.Controllers;

[ApiController]
[Route("api/signatur")]
public class SignaturController : ControllerBase
{
    private readonly ISignaturService _signaturService;
    private readonly ILogger<SignaturController> _logger;

    public SignaturController(
        ISignaturService signaturService,
        ILogger<SignaturController> logger)
    {
        _signaturService = signaturService;
        _logger = logger;
    }

    /// <summary>
    /// Fordert eine neue qualifizierte elektronische Signatur an
    /// </summary>
    [HttpPost("anfordern")]
    [Authorize(Policy = "Kunde")]
    public async Task<ActionResult<SignaturAnforderungResult>> SignaturAnfordern(
        [FromBody] SignaturAnforderungRequest request)
    {
        try
        {
            _logger.LogInformation("Signatur-Anforderung für Auftrag {AuftragId}", request.AuftragId);
            var result = await _signaturService.SignaturAnfordernAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler bei Signatur-Anforderung für Auftrag {AuftragId}", request.AuftragId);
            return StatusCode(500, new { error = "Fehler bei der Signatur-Anforderung", message = ex.Message });
        }
    }

    /// <summary>
    /// Ruft den Status einer Signatur ab
    /// </summary>
    [HttpGet("{id}/status")]
    [Authorize]
    public async Task<ActionResult<SignaturStatusResult>> GetStatus(Guid id)
    {
        try
        {
            _logger.LogInformation("Status-Abfrage für Signatur {SignaturId}", id);
            var result = await _signaturService.StatusPruefenAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler bei Status-Abfrage für Signatur {SignaturId}", id);
            return StatusCode(500, new { error = "Fehler bei der Status-Abfrage", message = ex.Message });
        }
    }

    /// <summary>
    /// Callback-Endpoint für QES-Provider (ÖFFENTLICH - keine Authentifizierung)
    /// </summary>
    [HttpPost("callback/{provider}")]
    [AllowAnonymous]
    public async Task<IActionResult> ProviderCallback(
        string provider,
        [FromBody] JsonElement payload)
    {
        try
        {
            _logger.LogInformation("Callback von Provider {Provider} empfangen", provider);
            
            // Extrahiere SignaturId aus dem Payload
            // Dies ist Provider-spezifisch und muss für jeden Provider angepasst werden
            var signaturId = ExtractSignaturIdFromPayload(payload);
            
            var request = new SignaturCallbackRequest
            {
                SignaturId = signaturId,
                ProviderData = payload.Deserialize<Dictionary<string, object>>()
            };
            
            var result = await _signaturService.CallbackVerarbeitenAsync(request);
            
            if (result.Erfolg)
            {
                return Ok(new { status = "success", signaturId = result.SignaturId });
            }
            
            return BadRequest(new { status = "error", message = result.FehlerNachricht });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Verarbeiten des Callbacks von Provider {Provider}", provider);
            return StatusCode(500, new { error = "Fehler beim Callback", message = ex.Message });
        }
    }

    /// <summary>
    /// Lädt das signierte Dokument herunter
    /// </summary>
    [HttpGet("{id}/dokument")]
    [Authorize]
    public async Task<IActionResult> SigniertesDokumentHerunterladen(Guid id)
    {
        try
        {
            _logger.LogInformation("Download signiertes Dokument für Signatur {SignaturId}", id);
            
            var dokumentBytes = await _signaturService.SigniertesDokumentAbrufenAsync(id);
            
            return File(dokumentBytes, "application/pdf", $"signiertes_dokument_{id}.pdf");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Dokument nicht verfügbar für Signatur {SignaturId}", id);
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Download für Signatur {SignaturId}", id);
            return StatusCode(500, new { error = "Fehler beim Dokument-Download", message = ex.Message });
        }
    }

    /// <summary>
    /// Validiert eine bestehende Signatur
    /// </summary>
    [HttpGet("{id}/validieren")]
    [Authorize]
    public async Task<ActionResult<SignaturValidierungResult>> Validieren(Guid id)
    {
        try
        {
            _logger.LogInformation("Validiere Signatur {SignaturId}", id);
            var result = await _signaturService.SignaturValidierenAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler bei Validierung für Signatur {SignaturId}", id);
            return StatusCode(500, new { error = "Fehler bei der Validierung", message = ex.Message });
        }
    }

    /// <summary>
    /// Storniert eine laufende Signatur-Anforderung
    /// </summary>
    [HttpPost("{id}/stornieren")]
    [Authorize(Policy = "Mitarbeiter")]
    public async Task<IActionResult> Stornieren(Guid id)
    {
        try
        {
            _logger.LogInformation("Storniere Signatur {SignaturId}", id);
            var erfolg = await _signaturService.StornierenAsync(id);
            
            if (erfolg)
            {
                return Ok(new { success = true, message = "Signatur wurde storniert" });
            }
            
            return NotFound(new { success = false, message = "Signatur nicht gefunden" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Stornieren für Signatur {SignaturId}", id);
            return StatusCode(500, new { error = "Fehler beim Stornieren", message = ex.Message });
        }
    }

    /// <summary>
    /// Redirect-Seite die den Kunden zum QES-Provider weiterleitet
    /// </summary>
    [HttpGet("{id}/redirect")]
    [AllowAnonymous]
    public async Task<IActionResult> RedirectZumProvider(Guid id, [FromQuery] string token)
    {
        try
        {
            _logger.LogInformation("Redirect zum Provider für Signatur {SignaturId}", id);
            
            // Validiere Token und hole Redirect-URL
            var status = await _signaturService.StatusPruefenAsync(id);
            
            // TODO: Token-Validierung implementieren
            // TODO: Redirect-URL aus Session holen
            
            // Für Mock: Zeige eine einfache HTML-Seite
            var html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Weiterleitung zur Signatur</title>
    <style>
        body {{ font-family: Arial, sans-serif; text-align: center; padding: 50px; }}
        .container {{ max-width: 600px; margin: 0 auto; }}
        .button {{ 
            background-color: #0066cc; 
            color: white; 
            padding: 15px 30px; 
            text-decoration: none; 
            border-radius: 5px; 
            display: inline-block; 
            margin-top: 20px; 
        }}
    </style>
</head>
<body>
    <div class='container'>
        <h1>Qualifizierte Elektronische Signatur</h1>
        <p>Sie werden zur Authentifizierung an den Vertrauensdiensteanbieter weitergeleitet.</p>
        <p>Signatur-ID: {id}</p>
        <p>Status: {status.Status}</p>
        <a href='#' class='button' onclick='simulateSignature(); return false;'>
            Signatur simulieren (Mock)
        </a>
    </div>
    <script>
        function simulateSignature() {{
            alert('Mock: Signatur wird simuliert...');
            // TODO: Callback an Backend senden
            window.location.href = '/';
        }}
    </script>
</body>
</html>";
            
            return Content(html, "text/html");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Redirect für Signatur {SignaturId}", id);
            return StatusCode(500, $"Fehler: {ex.Message}");
        }
    }

    private static Guid ExtractSignaturIdFromPayload(JsonElement payload)
    {
        // Provider-spezifische Implementierung
        // Für Mock: Nehme 'signaturId' aus dem JSON
        if (payload.TryGetProperty("signaturId", out var idElement))
        {
            if (Guid.TryParse(idElement.GetString(), out var id))
            {
                return id;
            }
        }
        
        throw new ArgumentException("SignaturId konnte nicht aus Payload extrahiert werden");
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FahrzeugZulassung.Application.Interfaces;
using FahrzeugZulassung.Application.DTOs.Push;

namespace FahrzeugZulassung.API.Controllers;

[ApiController]
[Route("api/push")]
[Authorize]
public class PushController : ControllerBase
{
    private readonly IPushNotificationService _pushService;
    private readonly string _vapidPublicKey;

    public PushController(IPushNotificationService pushService, IConfiguration configuration)
    {
        _pushService = pushService;
        _vapidPublicKey = configuration["PushNotifications:VapidPublicKey"] ?? string.Empty;
    }

    /// <summary>
    /// Subscribe to push notifications
    /// </summary>
    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe([FromBody] PushSubscriptionDto dto)
    {
        try
        {
            var benutzerId = Guid.Parse(User.FindFirst("sub")?.Value ?? Guid.Empty.ToString());
            await _pushService.SubscribeAsync(benutzerId, dto);
            return Ok(new { message = "Subscription erfolgreich" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Unsubscribe from push notifications
    /// </summary>
    [HttpPost("unsubscribe")]
    public async Task<IActionResult> Unsubscribe([FromBody] UnsubscribeRequest request)
    {
        try
        {
            var benutzerId = Guid.Parse(User.FindFirst("sub")?.Value ?? Guid.Empty.ToString());
            await _pushService.UnsubscribeAsync(benutzerId, request.Endpoint);
            return Ok(new { message = "Unsubscribe erfolgreich" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get VAPID public key for push notifications
    /// </summary>
    [HttpGet("vapid-public-key")]
    [AllowAnonymous]
    public IActionResult GetVapidPublicKey()
    {
        return Ok(new { publicKey = _vapidPublicKey });
    }
}

public class UnsubscribeRequest
{
    public string Endpoint { get; set; } = string.Empty;
}

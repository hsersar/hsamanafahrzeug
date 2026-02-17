using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using iKfz.Backend.DTOs;
using iKfz.Backend.Repositories;

namespace iKfz.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IPersonalProfileRepository _profileRepo;

    public UsersController(IPersonalProfileRepository profileRepo)
    {
        _profileRepo = profileRepo;
    }

    /// <summary>
    /// Get profile for current user.
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserProfileDto>> GetMe()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? User.FindFirstValue("sub")
                     ?? "demo-user";

        var personalProfile = await _profileRepo.GetByUserIdAsync(userId);

        var profile = new UserProfileDto
        {
            UserId = userId,
            DisplayName = personalProfile != null
                ? $"{personalProfile.Vorname} {personalProfile.Nachname}".Trim()
                : "Demo Nutzer",
            Email = personalProfile?.Email ?? "demo-user@example.local",
            Role = User.FindFirstValue(ClaimTypes.Role) ?? "Kunde",
            CreatedAtUtc = personalProfile?.CreatedAt ?? DateTime.UtcNow.AddMonths(-2),
            LastLoginUtc = DateTime.UtcNow
        };

        return Ok(profile);
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using iKfz.Backend.DTOs;
using iKfz.Backend.Models;
using iKfz.Backend.Repositories;

namespace iKfz.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IRegistrationRequestRepository _registrationRepository;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IVehicleRepository vehicleRepository,
        IRegistrationRequestRepository registrationRepository,
        ILogger<DashboardController> logger)
    {
        _vehicleRepository = vehicleRepository;
        _registrationRepository = registrationRepository;
        _logger = logger;
    }

    /// <summary>
    /// Get overview metrics for the current user.
    /// </summary>
    [HttpGet("overview")]
    [ProducesResponseType(typeof(DashboardOverviewDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardOverviewDto>> GetOverview()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? User.FindFirstValue("sub")
                     ?? string.Empty;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var vehicles = await _vehicleRepository.GetByOwnerIdAsync(userId);
        var requests = (await _registrationRepository.GetByUserIdAsync(userId)).ToList();

        var openRequests = requests.Count(r =>
            r.Status == RegistrationStatus.Pending || r.Status == RegistrationStatus.UnderReview);

        var latestRequest = requests.FirstOrDefault();

        var overview = new DashboardOverviewDto
        {
            OpenRequests = openRequests,
            TotalRequests = requests.Count,
            RegisteredVehicles = vehicles.Count(),
            PendingReviews = openRequests,
            LatestRequestStatus = latestRequest?.Status.ToString(),
            LatestRequestCreatedAt = latestRequest?.CreatedAt
        };

        return Ok(overview);
    }

    /// <summary>
    /// Get all pending requests (Mitarbeiter+ only).
    /// </summary>
    [HttpGet("pending-requests")]
    [Authorize(Policy = "RequireMitarbeiter")]
    [ProducesResponseType(typeof(IEnumerable<RegistrationRequestDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RegistrationRequestDto>>> GetPendingRequests()
    {
        _logger.LogInformation("Pending requests viewed by {UserId}",
            User.FindFirstValue(ClaimTypes.NameIdentifier));

        var pending = await _registrationRepository.GetPendingRequestsAsync();

        var result = pending.Select(r => new RegistrationRequestDto
        {
            Id = r.Id,
            VIN = r.VIN,
            RequestedLicensePlate = r.RequestedLicensePlate,
            Brand = r.Brand,
            Model = r.Model,
            Year = r.Year,
            Color = r.Color,
            FirstRegistrationDate = r.FirstRegistrationDate,
            Status = r.Status.ToString(),
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt,
            RejectionReason = r.RejectionReason
        });

        return Ok(result);
    }
}
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
public class SearchController : ControllerBase
{
    private readonly IRegistrationRequestRepository _registrationRepository;
    private readonly ILogger<SearchController> _logger;

    public SearchController(
        IRegistrationRequestRepository registrationRepository,
        ILogger<SearchController> logger)
    {
        _registrationRepository = registrationRepository;
        _logger = logger;
    }

    /// <summary>
    /// Search registration requests for current user.
    /// </summary>
    [HttpPost("requests")]
    [ProducesResponseType(typeof(IEnumerable<RegistrationRequestDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RegistrationRequestDto>>> SearchRequests(
        [FromBody] SearchRequestsRequestDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? User.FindFirstValue("sub")
                     ?? string.Empty;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        _logger.LogInformation("Search by user {UserId}", userId);

        var requests = await _registrationRepository.GetByUserIdAsync(userId);
        var query = dto.Query?.Trim().ToLowerInvariant();
        var status = dto.Status?.Trim().ToLowerInvariant();

        var filtered = requests.Where(r =>
        {
            var matchesQuery = string.IsNullOrWhiteSpace(query)
                               || r.VIN.ToLowerInvariant().Contains(query)
                               || r.RequestedLicensePlate.ToLowerInvariant().Contains(query)
                               || r.Brand.ToLowerInvariant().Contains(query)
                               || r.Model.ToLowerInvariant().Contains(query);

            var matchesStatus = string.IsNullOrWhiteSpace(status)
                                || r.Status.ToString().ToLowerInvariant() == status;

            var matchesFrom = !dto.FromDate.HasValue || r.CreatedAt >= dto.FromDate.Value;
            var matchesTo = !dto.ToDate.HasValue || r.CreatedAt <= dto.ToDate.Value;

            return matchesQuery && matchesStatus && matchesFrom && matchesTo;
        });

        var result = filtered.Select(MapToDto);
        return Ok(result);
    }

    private static RegistrationRequestDto MapToDto(RegistrationRequest request)
    {
        return new RegistrationRequestDto
        {
            Id = request.Id,
            VIN = request.VIN,
            RequestedLicensePlate = request.RequestedLicensePlate,
            Brand = request.Brand,
            Model = request.Model,
            Year = request.Year,
            Color = request.Color,
            FirstRegistrationDate = request.FirstRegistrationDate,
            Status = request.Status.ToString(),
            CreatedAt = request.CreatedAt,
            UpdatedAt = request.UpdatedAt,
            RejectionReason = request.RejectionReason
        };
    }
}
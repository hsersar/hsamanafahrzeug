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
public class RegistrationController : ControllerBase
{
    private readonly IRegistrationRequestRepository _registrationRepository;
    private readonly ILogger<RegistrationController> _logger;

    public RegistrationController(
        IRegistrationRequestRepository registrationRepository,
        ILogger<RegistrationController> logger)
    {
        _registrationRepository = registrationRepository;
        _logger = logger;
    }

    /// <summary>
    /// Submit a new vehicle registration request
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(RegistrationRequestDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RegistrationRequestDto>> CreateRegistrationRequest(
        [FromBody] CreateRegistrationRequestDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                     ?? User.FindFirstValue("sub") 
                     ?? throw new UnauthorizedAccessException("User ID not found in claims");

        var request = new RegistrationRequest
        {
            UserId = userId,
            VIN = dto.VIN,
            RequestedLicensePlate = dto.RequestedLicensePlate,
            Brand = dto.Brand,
            Model = dto.Model,
            Year = dto.Year,
            Color = dto.Color,
            FirstRegistrationDate = dto.FirstRegistrationDate,
            Status = RegistrationStatus.Pending
        };

        var created = await _registrationRepository.CreateAsync(request);
        
        // Log without personal data
        _logger.LogInformation("Registration request created with ID: {RequestId}", created.Id);

        var result = MapToDto(created);
        return CreatedAtAction(nameof(GetRegistrationRequest), new { id = created.Id }, result);
    }

    /// <summary>
    /// Get a specific registration request by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RegistrationRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RegistrationRequestDto>> GetRegistrationRequest(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var request = await _registrationRepository.GetByIdAsync(id);

        if (request == null)
        {
            return NotFound();
        }

        // Users can only access their own requests
        if (request.UserId != userId)
        {
            return Forbid();
        }

        return Ok(MapToDto(request));
    }

    /// <summary>
    /// Get all registration requests for the current user
    /// </summary>
    [HttpGet("my-requests")]
    [ProducesResponseType(typeof(IEnumerable<RegistrationRequestDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RegistrationRequestDto>>> GetMyRequests()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                     ?? User.FindFirstValue("sub") 
                     ?? throw new UnauthorizedAccessException("User ID not found");

        var requests = await _registrationRepository.GetByUserIdAsync(userId);
        return Ok(requests.Select(MapToDto));
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

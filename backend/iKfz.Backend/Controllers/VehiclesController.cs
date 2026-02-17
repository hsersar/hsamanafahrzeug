using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using iKfz.Backend.DTOs;
using iKfz.Backend.Services;

namespace iKfz.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VehiclesController : ControllerBase
{
    private readonly IVehicleService _vehicleService;
    private readonly ILogger<VehiclesController> _logger;

    public VehiclesController(
        IVehicleService vehicleService,
        ILogger<VehiclesController> logger)
    {
        _vehicleService = vehicleService;
        _logger = logger;
    }

    /// <summary>
    /// Get all vehicles owned by the current user
    /// </summary>
    [HttpGet("my-vehicles")]
    [ProducesResponseType(typeof(IEnumerable<VehicleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VehicleDto>>> GetMyVehicles()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                     ?? User.FindFirstValue("sub")
                     ?? string.Empty;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var vehicles = await _vehicleService.GetUserVehiclesAsync(userId);
        
        var result = vehicles.Select(v => new VehicleDto
        {
            Id = v.Id,
            VIN = v.VIN,
            LicensePlate = v.LicensePlate,
            Brand = v.Brand,
            Model = v.Model,
            Year = v.Year,
            Color = v.Color,
            FirstRegistrationDate = v.FirstRegistrationDate,
            CreatedAt = v.CreatedAt
        });

        return Ok(result);
    }

    /// <summary>
    /// Get a specific vehicle by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VehicleDto>> GetVehicle(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var vehicle = await _vehicleService.GetVehicleByIdAsync(id);

        if (vehicle == null)
        {
            return NotFound();
        }

        // Users can only access their own vehicles
        if (vehicle.OwnerId != userId)
        {
            return Forbid();
        }

        var result = new VehicleDto
        {
            Id = vehicle.Id,
            VIN = vehicle.VIN,
            LicensePlate = vehicle.LicensePlate,
            Brand = vehicle.Brand,
            Model = vehicle.Model,
            Year = vehicle.Year,
            Color = vehicle.Color,
            FirstRegistrationDate = vehicle.FirstRegistrationDate,
            CreatedAt = vehicle.CreatedAt
        };

        return Ok(result);
    }

    /// <summary>
    /// Get catalog of vehicle brands and models for suggestions
    /// </summary>
    [HttpGet("catalog")]
    [ProducesResponseType(typeof(IEnumerable<VehicleCatalogItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VehicleCatalogItemDto>>> GetCatalog()
    {
        var catalog = await _vehicleService.GetVehicleCatalogAsync();
        return Ok(catalog);
    }
}

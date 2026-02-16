using Microsoft.Extensions.Caching.Memory;
using iKfz.Backend.Models;
using iKfz.Backend.Repositories;

namespace iKfz.Backend.Services;

public interface IVehicleService
{
    Task<Vehicle?> GetVehicleByIdAsync(int id);
    Task<IEnumerable<Vehicle>> GetUserVehiclesAsync(string userId);
    Task<Vehicle> CreateVehicleFromApprovedRequestAsync(RegistrationRequest request);
}

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMemoryCache _cache;
    private readonly ILogger<VehicleService> _logger;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(10);

    public VehicleService(
        IVehicleRepository vehicleRepository,
        IMemoryCache cache,
        ILogger<VehicleService> logger)
    {
        _vehicleRepository = vehicleRepository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Vehicle?> GetVehicleByIdAsync(int id)
    {
        var cacheKey = $"vehicle_{id}";
        
        if (_cache.TryGetValue(cacheKey, out Vehicle? cachedVehicle))
        {
            _logger.LogDebug("Cache hit for vehicle ID: {VehicleId}", id);
            return cachedVehicle;
        }

        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        
        if (vehicle != null)
        {
            _cache.Set(cacheKey, vehicle, _cacheExpiration);
            _logger.LogDebug("Vehicle cached with ID: {VehicleId}", id);
        }

        return vehicle;
    }

    public async Task<IEnumerable<Vehicle>> GetUserVehiclesAsync(string userId)
    {
        var cacheKey = $"user_vehicles_{userId}";
        
        if (_cache.TryGetValue(cacheKey, out IEnumerable<Vehicle>? cachedVehicles))
        {
            _logger.LogDebug("Cache hit for user vehicles");
            return cachedVehicles!;
        }

        var vehicles = await _vehicleRepository.GetByOwnerIdAsync(userId);
        _cache.Set(cacheKey, vehicles, _cacheExpiration);
        
        return vehicles;
    }

    public async Task<Vehicle> CreateVehicleFromApprovedRequestAsync(RegistrationRequest request)
    {
        var vehicle = new Vehicle
        {
            VIN = request.VIN,
            LicensePlate = request.RequestedLicensePlate,
            Brand = request.Brand,
            Model = request.Model,
            Year = request.Year,
            Color = request.Color,
            FirstRegistrationDate = request.FirstRegistrationDate,
            OwnerId = request.UserId
        };

        var createdVehicle = await _vehicleRepository.CreateAsync(vehicle);
        
        // Invalidate cache for this user
        _cache.Remove($"user_vehicles_{request.UserId}");
        
        _logger.LogInformation("Vehicle created from approved registration request");
        
        return createdVehicle;
    }
}

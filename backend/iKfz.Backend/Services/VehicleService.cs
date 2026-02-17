using Microsoft.Extensions.Caching.Memory;
using iKfz.Backend.DTOs;
using iKfz.Backend.Models;
using iKfz.Backend.Repositories;

namespace iKfz.Backend.Services;

public interface IVehicleService
{
    Task<Vehicle?> GetVehicleByIdAsync(int id);
    Task<IEnumerable<Vehicle>> GetUserVehiclesAsync(string userId);
    Task<Vehicle> CreateVehicleFromApprovedRequestAsync(RegistrationRequest request);
    Task<IReadOnlyList<VehicleCatalogItemDto>> GetVehicleCatalogAsync();
}

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMemoryCache _cache;
    private readonly ILogger<VehicleService> _logger;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(10);
    private static readonly IReadOnlyList<VehicleCatalogItemDto> _catalog =
        new List<VehicleCatalogItemDto>
        {
            new() { Brand = "Audi", Models = new List<string> { "A1", "A3", "A4", "A6", "Q3", "Q5" } },
            new() { Brand = "BMW", Models = new List<string> { "1er", "3er", "5er", "X1", "X3", "X5" } },
            new() { Brand = "Mercedes-Benz", Models = new List<string> { "A-Klasse", "C-Klasse", "E-Klasse", "GLA", "GLC" } },
            new() { Brand = "Volkswagen", Models = new List<string> { "Golf", "Passat", "Polo", "Tiguan", "T-Roc" } },
            new() { Brand = "Skoda", Models = new List<string> { "Fabia", "Octavia", "Karoq", "Kodiaq" } },
            new() { Brand = "Opel", Models = new List<string> { "Astra", "Corsa", "Insignia", "Mokka" } },
            new() { Brand = "Ford", Models = new List<string> { "Fiesta", "Focus", "Kuga", "Puma", "Mondeo" } },
            new() { Brand = "Toyota", Models = new List<string> { "Yaris", "Corolla", "RAV4", "C-HR", "Camry" } },
            new() { Brand = "Hyundai", Models = new List<string> { "i10", "i20", "i30", "Tucson", "Kona" } },
            new() { Brand = "Kia", Models = new List<string> { "Picanto", "Ceed", "Sportage", "Niro", "Sorento" } },
            new() { Brand = "Renault", Models = new List<string> { "Clio", "Megane", "Captur", "Kadjar", "Arkana" } },
            new() { Brand = "Peugeot", Models = new List<string> { "208", "308", "3008", "5008", "2008" } },
            new() { Brand = "Citroen", Models = new List<string> { "C3", "C4", "C5 Aircross", "Berlingo" } },
            new() { Brand = "Seat", Models = new List<string> { "Ibiza", "Leon", "Arona", "Ateca" } },
            new() { Brand = "Mazda", Models = new List<string> { "Mazda2", "Mazda3", "CX-3", "CX-5" } },
            new() { Brand = "Nissan", Models = new List<string> { "Micra", "Qashqai", "Juke", "X-Trail" } },
            new() { Brand = "Honda", Models = new List<string> { "Jazz", "Civic", "CR-V", "HR-V" } },
            new() { Brand = "Volvo", Models = new List<string> { "XC40", "XC60", "XC90", "V60" } },
            new() { Brand = "Fiat", Models = new List<string> { "500", "Panda", "Tipo", "500X" } },
            new() { Brand = "Dacia", Models = new List<string> { "Sandero", "Duster", "Jogger", "Logan" } },
            new() { Brand = "Mini", Models = new List<string> { "Cooper", "Countryman", "Clubman" } },
            new() { Brand = "Suzuki", Models = new List<string> { "Swift", "Vitara", "S-Cross", "Ignis" } },
            new() { Brand = "Mitsubishi", Models = new List<string> { "ASX", "Outlander", "Eclipse Cross" } },
            new() { Brand = "Jeep", Models = new List<string> { "Renegade", "Compass", "Wrangler" } },
            new() { Brand = "Land Rover", Models = new List<string> { "Defender", "Discovery", "Range Rover Evoque" } },
            new() { Brand = "Porsche", Models = new List<string> { "911", "Cayenne", "Macan", "Taycan" } },
            new() { Brand = "Tesla", Models = new List<string> { "Model 3", "Model S", "Model X", "Model Y" } },
            new() { Brand = "Jaguar", Models = new List<string> { "XE", "XF", "F-Pace", "I-Pace" } },
            new() { Brand = "Alfa Romeo", Models = new List<string> { "Giulia", "Stelvio", "Tonale" } },
            new() { Brand = "Cupra", Models = new List<string> { "Formentor", "Born", "Leon" } },
            new() { Brand = "Lexus", Models = new List<string> { "UX", "NX", "RX", "IS" } },
            new() { Brand = "Subaru", Models = new List<string> { "Impreza", "Outback", "Forester" } },
            new() { Brand = "Chevrolet", Models = new List<string> { "Spark", "Cruze", "Captiva" } },
            new() { Brand = "Smart", Models = new List<string> { "Fortwo", "Forfour" } },
            new() { Brand = "Saab", Models = new List<string> { "9-3", "9-5" } },
            new() { Brand = "Lancia", Models = new List<string> { "Ypsilon" } },
            new() { Brand = "DS", Models = new List<string> { "DS 3", "DS 4", "DS 7" } },
            new() { Brand = "MG", Models = new List<string> { "ZS", "HS", "MG4" } },
            new() { Brand = "Polestar", Models = new List<string> { "2", "3" } },
            new() { Brand = "Genesis", Models = new List<string> { "G70", "G80", "GV70" } },
            new() { Brand = "Lada", Models = new List<string> { "Niva", "Vesta" } },
            new() { Brand = "Infiniti", Models = new List<string> { "Q30", "Q50", "QX50" } },
            new() { Brand = "Acura", Models = new List<string> { "ILX", "TLX", "RDX" } },
            new() { Brand = "Cadillac", Models = new List<string> { "ATS", "CT5", "XT4" } },
            new() { Brand = "GMC", Models = new List<string> { "Terrain", "Acadia" } },
            new() { Brand = "Chrysler", Models = new List<string> { "300", "Pacifica" } },
            new() { Brand = "Dodge", Models = new List<string> { "Charger", "Challenger", "Durango" } },
            new() { Brand = "RAM", Models = new List<string> { "1500", "2500" } },
            new() { Brand = "Volvo Trucks", Models = new List<string> { "FH", "FM" } },
            new() { Brand = "MAN", Models = new List<string> { "TGX", "TGS" } },
            new() { Brand = "Scania", Models = new List<string> { "R", "S" } }
        };

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

    public Task<IReadOnlyList<VehicleCatalogItemDto>> GetVehicleCatalogAsync()
    {
        return Task.FromResult(_catalog);
    }
}

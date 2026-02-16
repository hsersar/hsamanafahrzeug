using Microsoft.EntityFrameworkCore;
using iKfz.Backend.Data;
using iKfz.Backend.Models;

namespace iKfz.Backend.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly ApplicationDbContext _context;

    public VehicleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Vehicle?> GetByIdAsync(int id)
    {
        return await _context.Vehicles
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<Vehicle?> GetByVINAsync(string vin)
    {
        return await _context.Vehicles
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.VIN == vin);
    }

    public async Task<IEnumerable<Vehicle>> GetByOwnerIdAsync(string ownerId)
    {
        return await _context.Vehicles
            .AsNoTracking()
            .Where(v => v.OwnerId == ownerId)
            .ToListAsync();
    }

    public async Task<Vehicle> CreateAsync(Vehicle vehicle)
    {
        vehicle.CreatedAt = DateTime.UtcNow;
        vehicle.UpdatedAt = DateTime.UtcNow;
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();
        return vehicle;
    }

    public async Task<Vehicle?> UpdateAsync(Vehicle vehicle)
    {
        var existing = await _context.Vehicles.FindAsync(vehicle.Id);
        if (existing == null) return null;

        existing.LicensePlate = vehicle.LicensePlate;
        existing.Brand = vehicle.Brand;
        existing.Model = vehicle.Model;
        existing.Year = vehicle.Year;
        existing.Color = vehicle.Color;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle == null) return false;

        _context.Vehicles.Remove(vehicle);
        await _context.SaveChangesAsync();
        return true;
    }
}

public class RegistrationRequestRepository : IRegistrationRequestRepository
{
    private readonly ApplicationDbContext _context;

    public RegistrationRequestRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RegistrationRequest?> GetByIdAsync(int id)
    {
        return await _context.RegistrationRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<RegistrationRequest>> GetByUserIdAsync(string userId)
    {
        return await _context.RegistrationRequests
            .AsNoTracking()
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<RegistrationRequest>> GetPendingRequestsAsync()
    {
        return await _context.RegistrationRequests
            .AsNoTracking()
            .Where(r => r.Status == RegistrationStatus.Pending || r.Status == RegistrationStatus.UnderReview)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<RegistrationRequest> CreateAsync(RegistrationRequest request)
    {
        request.CreatedAt = DateTime.UtcNow;
        request.UpdatedAt = DateTime.UtcNow;
        request.Status = RegistrationStatus.Pending;
        _context.RegistrationRequests.Add(request);
        await _context.SaveChangesAsync();
        return request;
    }

    public async Task<RegistrationRequest?> UpdateAsync(RegistrationRequest request)
    {
        var existing = await _context.RegistrationRequests.FindAsync(request.Id);
        if (existing == null) return null;

        existing.Status = request.Status;
        existing.ProcessedBy = request.ProcessedBy;
        existing.ProcessedAt = request.ProcessedAt;
        existing.RejectionReason = request.RejectionReason;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }
}

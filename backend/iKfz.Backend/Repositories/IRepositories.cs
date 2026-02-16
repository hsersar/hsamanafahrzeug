using iKfz.Backend.Models;

namespace iKfz.Backend.Repositories;

public interface IVehicleRepository
{
    Task<Vehicle?> GetByIdAsync(int id);
    Task<Vehicle?> GetByVINAsync(string vin);
    Task<IEnumerable<Vehicle>> GetByOwnerIdAsync(string ownerId);
    Task<Vehicle> CreateAsync(Vehicle vehicle);
    Task<Vehicle?> UpdateAsync(Vehicle vehicle);
    Task<bool> DeleteAsync(int id);
}

public interface IRegistrationRequestRepository
{
    Task<RegistrationRequest?> GetByIdAsync(int id);
    Task<IEnumerable<RegistrationRequest>> GetByUserIdAsync(string userId);
    Task<IEnumerable<RegistrationRequest>> GetPendingRequestsAsync();
    Task<RegistrationRequest> CreateAsync(RegistrationRequest request);
    Task<RegistrationRequest?> UpdateAsync(RegistrationRequest request);
}

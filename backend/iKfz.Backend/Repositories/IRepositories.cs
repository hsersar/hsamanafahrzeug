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

public interface IPersonalProfileRepository
{
    Task<PersonalProfile?> GetByUserIdAsync(string userId);
    Task<PersonalProfile> CreateOrUpdateAsync(PersonalProfile profile);
}

public interface ICompanyProfileRepository
{
    Task<CompanyProfile?> GetByUserIdAsync(string userId);
    Task<CompanyProfile> CreateOrUpdateAsync(CompanyProfile profile);
}

public interface IPaymentMethodRepository
{
    Task<IEnumerable<PaymentMethod>> GetByUserIdAsync(string userId);
    Task<PaymentMethod?> GetByIdAsync(int id);
    Task<PaymentMethod> CreateAsync(PaymentMethod method);
    Task<PaymentMethod?> UpdateAsync(PaymentMethod method);
    Task<bool> DeleteAsync(int id, string userId);
    Task SetDefaultAsync(int id, string userId);
}

public interface IInvoiceRepository
{
    Task<IEnumerable<Invoice>> GetByUserIdAsync(string userId);
    Task<Invoice?> GetByIdAsync(int id);
    Task<Invoice> CreateAsync(Invoice invoice);
    Task<Invoice?> UpdateStatusAsync(int id, InvoiceStatus status);
}

public interface IAnnouncementRepository
{
    Task<IEnumerable<Announcement>> GetActiveAsync();
    Task<Announcement> CreateAsync(Announcement announcement);
}

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

public class PersonalProfileRepository : IPersonalProfileRepository
{
    private readonly ApplicationDbContext _context;

    public PersonalProfileRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PersonalProfile?> GetByUserIdAsync(string userId)
    {
        return await _context.PersonalProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<PersonalProfile> CreateOrUpdateAsync(PersonalProfile profile)
    {
        var existing = await _context.PersonalProfiles
            .FirstOrDefaultAsync(p => p.UserId == profile.UserId);

        if (existing == null)
        {
            profile.CreatedAt = DateTime.UtcNow;
            profile.UpdatedAt = DateTime.UtcNow;
            _context.PersonalProfiles.Add(profile);
        }
        else
        {
            existing.Anrede = profile.Anrede;
            existing.Titel = profile.Titel;
            existing.Vorname = profile.Vorname;
            existing.Nachname = profile.Nachname;
            existing.Geburtsdatum = profile.Geburtsdatum;
            existing.Strasse = profile.Strasse;
            existing.Hausnummer = profile.Hausnummer;
            existing.Plz = profile.Plz;
            existing.Ort = profile.Ort;
            existing.Telefon = profile.Telefon;
            existing.Email = profile.Email;
            existing.ProfilbildPfad = profile.ProfilbildPfad ?? existing.ProfilbildPfad;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return existing ?? profile;
    }
}

public class CompanyProfileRepository : ICompanyProfileRepository
{
    private readonly ApplicationDbContext _context;

    public CompanyProfileRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CompanyProfile?> GetByUserIdAsync(string userId)
    {
        return await _context.CompanyProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<CompanyProfile> CreateOrUpdateAsync(CompanyProfile profile)
    {
        var existing = await _context.CompanyProfiles
            .FirstOrDefaultAsync(c => c.UserId == profile.UserId);

        if (existing == null)
        {
            profile.CreatedAt = DateTime.UtcNow;
            profile.UpdatedAt = DateTime.UtcNow;
            _context.CompanyProfiles.Add(profile);
        }
        else
        {
            existing.Firmenname = profile.Firmenname;
            existing.Rechtsform = profile.Rechtsform;
            existing.Handelsregisternummer = profile.Handelsregisternummer;
            existing.UstIdNr = profile.UstIdNr;
            existing.Strasse = profile.Strasse;
            existing.Hausnummer = profile.Hausnummer;
            existing.Plz = profile.Plz;
            existing.Ort = profile.Ort;
            existing.Telefon = profile.Telefon;
            existing.Email = profile.Email;
            existing.Website = profile.Website;
            existing.AnsprechpartnerAnrede = profile.AnsprechpartnerAnrede;
            existing.AnsprechpartnerVorname = profile.AnsprechpartnerVorname;
            existing.AnsprechpartnerNachname = profile.AnsprechpartnerNachname;
            existing.AnsprechpartnerTelefon = profile.AnsprechpartnerTelefon;
            existing.AnsprechpartnerEmail = profile.AnsprechpartnerEmail;
            existing.AnsprechpartnerPosition = profile.AnsprechpartnerPosition;
            existing.LogoPfad = profile.LogoPfad ?? existing.LogoPfad;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return existing ?? profile;
    }
}

public class PaymentMethodRepository : IPaymentMethodRepository
{
    private readonly ApplicationDbContext _context;

    public PaymentMethodRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PaymentMethod>> GetByUserIdAsync(string userId)
    {
        return await _context.PaymentMethods
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.IstStandard)
            .ThenByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<PaymentMethod?> GetByIdAsync(int id)
    {
        return await _context.PaymentMethods
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<PaymentMethod> CreateAsync(PaymentMethod method)
    {
        method.CreatedAt = DateTime.UtcNow;
        method.UpdatedAt = DateTime.UtcNow;

        if (method.IstStandard)
        {
            await ClearDefaultAsync(method.UserId);
        }
        else
        {
            var existing = await _context.PaymentMethods
                .AnyAsync(p => p.UserId == method.UserId);
            if (!existing) method.IstStandard = true;
        }

        _context.PaymentMethods.Add(method);
        await _context.SaveChangesAsync();
        return method;
    }

    public async Task<PaymentMethod?> UpdateAsync(PaymentMethod method)
    {
        var existing = await _context.PaymentMethods.FindAsync(method.Id);
        if (existing == null || existing.UserId != method.UserId) return null;

        existing.Typ = method.Typ;
        existing.Bezeichnung = method.Bezeichnung;
        existing.Kontoinhaber = method.Kontoinhaber;
        existing.IBAN = method.IBAN;
        existing.BIC = method.BIC;
        existing.Bankname = method.Bankname;
        existing.KartenNummer = method.KartenNummer;
        existing.KartenInhaber = method.KartenInhaber;
        existing.GueltigBis = method.GueltigBis;
        existing.PaypalEmail = method.PaypalEmail;
        existing.SepaMandatErteilt = method.SepaMandatErteilt;
        existing.UpdatedAt = DateTime.UtcNow;

        if (method.IstStandard && !existing.IstStandard)
        {
            await ClearDefaultAsync(method.UserId);
            existing.IstStandard = true;
        }

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id, string userId)
    {
        var method = await _context.PaymentMethods.FindAsync(id);
        if (method == null || method.UserId != userId) return false;

        _context.PaymentMethods.Remove(method);
        await _context.SaveChangesAsync();

        if (method.IstStandard)
        {
            var next = await _context.PaymentMethods
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefaultAsync();
            if (next != null)
            {
                next.IstStandard = true;
                await _context.SaveChangesAsync();
            }
        }
        return true;
    }

    public async Task SetDefaultAsync(int id, string userId)
    {
        await ClearDefaultAsync(userId);
        var method = await _context.PaymentMethods.FindAsync(id);
        if (method != null && method.UserId == userId)
        {
            method.IstStandard = true;
            await _context.SaveChangesAsync();
        }
    }

    private async Task ClearDefaultAsync(string userId)
    {
        var defaults = await _context.PaymentMethods
            .Where(p => p.UserId == userId && p.IstStandard)
            .ToListAsync();
        foreach (var d in defaults) d.IstStandard = false;
    }
}

public class InvoiceRepository : IInvoiceRepository
{
    private readonly ApplicationDbContext _context;

    public InvoiceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Invoice>> GetByUserIdAsync(string userId)
    {
        return await _context.Invoices
            .Where(i => i.UserId == userId)
            .OrderByDescending(i => i.Rechnungsdatum)
            .ToListAsync();
    }

    public async Task<Invoice?> GetByIdAsync(int id)
    {
        return await _context.Invoices.FindAsync(id);
    }

    public async Task<Invoice> CreateAsync(Invoice invoice)
    {
        // Generate Rechnungsnummer
        var count = await _context.Invoices.CountAsync() + 1;
        invoice.Rechnungsnummer = $"RE-{DateTime.UtcNow.Year}-{count:D5}";
        invoice.MwstBetrag = Math.Round(invoice.Nettobetrag * invoice.MwstSatz / 100m, 2);
        invoice.Bruttobetrag = invoice.Nettobetrag + invoice.MwstBetrag;
        invoice.CreatedAt = DateTime.UtcNow;
        invoice.UpdatedAt = DateTime.UtcNow;

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();
        return invoice;
    }

    public async Task<Invoice?> UpdateStatusAsync(int id, InvoiceStatus status)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice == null) return null;

        invoice.Status = status;
        invoice.UpdatedAt = DateTime.UtcNow;
        if (status == InvoiceStatus.Bezahlt)
            invoice.BezahltAm = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return invoice;
    }
}

public class AnnouncementRepository : IAnnouncementRepository
{
    private readonly ApplicationDbContext _context;

    public AnnouncementRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Announcement>> GetActiveAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Announcements
            .Where(a => a.Aktiv
                && (a.GueltigVon == null || a.GueltigVon <= now)
                && (a.GueltigBis == null || a.GueltigBis >= now))
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<Announcement> CreateAsync(Announcement announcement)
    {
        announcement.CreatedAt = DateTime.UtcNow;
        _context.Announcements.Add(announcement);
        await _context.SaveChangesAsync();
        return announcement;
    }
}

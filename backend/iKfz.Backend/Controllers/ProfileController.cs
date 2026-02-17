using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Security.Claims;
using iKfz.Backend.DTOs;
using iKfz.Backend.Models;
using iKfz.Backend.Repositories;

namespace iKfz.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IPersonalProfileRepository _personalRepo;
    private readonly ICompanyProfileRepository _companyRepo;
    private readonly IPaymentMethodRepository _paymentRepo;
    private readonly IWebHostEnvironment _env;

    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp" };
    private const long MaxFileSize = 2 * 1024 * 1024; // 2 MB

    public ProfileController(
        IPersonalProfileRepository personalRepo,
        ICompanyProfileRepository companyRepo,
        IPaymentMethodRepository paymentRepo,
        IWebHostEnvironment env)
    {
        _personalRepo = personalRepo;
        _companyRepo = companyRepo;
        _paymentRepo = paymentRepo;
        _env = env;
    }

    private string GetUserId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub")
        ?? "demo-user";

    // ── Personal Profile ──

    [HttpGet("personal")]
    [ProducesResponseType(typeof(PersonalProfileDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PersonalProfileDto>> GetPersonalProfile()
    {
        var userId = GetUserId();
        var profile = await _personalRepo.GetByUserIdAsync(userId);

        if (profile == null)
        {
            return Ok(new PersonalProfileDto { UserId = userId });
        }

        return Ok(MapPersonalToDto(profile));
    }

    [HttpPut("personal")]
    [ProducesResponseType(typeof(PersonalProfileDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PersonalProfileDto>> SavePersonalProfile(
        [FromBody] SavePersonalProfileDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();
        var profile = new PersonalProfile
        {
            UserId = userId,
            Anrede = dto.Anrede,
            Titel = dto.Titel,
            Vorname = dto.Vorname,
            Nachname = dto.Nachname,
            Geburtsdatum = dto.Geburtsdatum,
            Strasse = dto.Strasse,
            Hausnummer = dto.Hausnummer,
            Plz = dto.Plz,
            Ort = dto.Ort,
            Telefon = dto.Telefon,
            Email = dto.Email,
        };

        var saved = await _personalRepo.CreateOrUpdateAsync(profile);
        Log.Information("Personal profile saved for user {UserId}", userId);
        return Ok(MapPersonalToDto(saved));
    }

    [HttpPost("personal/photo")]
    [ProducesResponseType(typeof(PersonalProfileDto), StatusCodes.Status200OK)]
    [RequestSizeLimit(MaxFileSize)]
    public async Task<ActionResult<PersonalProfileDto>> UploadPersonalPhoto(IFormFile file)
    {
        var userId = GetUserId();
        var (error, path) = await SaveUploadedFile(file, "profiles");
        if (error != null) return BadRequest(new { error });

        // Update profile with photo path
        var existing = await _personalRepo.GetByUserIdAsync(userId);
        var profile = existing ?? new PersonalProfile { UserId = userId };
        profile.ProfilbildPfad = path;
        var saved = await _personalRepo.CreateOrUpdateAsync(profile);
        return Ok(MapPersonalToDto(saved));
    }

    // ── Company Profile ──

    [HttpGet("company")]
    [ProducesResponseType(typeof(CompanyProfileDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CompanyProfileDto>> GetCompanyProfile()
    {
        var userId = GetUserId();
        var profile = await _companyRepo.GetByUserIdAsync(userId);

        if (profile == null)
        {
            return Ok(new CompanyProfileDto { UserId = userId });
        }

        return Ok(MapCompanyToDto(profile));
    }

    [HttpPut("company")]
    [ProducesResponseType(typeof(CompanyProfileDto), StatusCodes.Status200OK)]
    [Authorize(Policy = "RequireStandortAdmin")]
    public async Task<ActionResult<CompanyProfileDto>> SaveCompanyProfile(
        [FromBody] SaveCompanyProfileDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();
        var profile = new CompanyProfile
        {
            UserId = userId,
            Firmenname = dto.Firmenname,
            Rechtsform = dto.Rechtsform,
            Handelsregisternummer = dto.Handelsregisternummer,
            UstIdNr = dto.UstIdNr,
            Strasse = dto.Strasse,
            Hausnummer = dto.Hausnummer,
            Plz = dto.Plz,
            Ort = dto.Ort,
            Telefon = dto.Telefon,
            Email = dto.Email,
            Website = dto.Website,
            AnsprechpartnerAnrede = dto.AnsprechpartnerAnrede,
            AnsprechpartnerVorname = dto.AnsprechpartnerVorname,
            AnsprechpartnerNachname = dto.AnsprechpartnerNachname,
            AnsprechpartnerTelefon = dto.AnsprechpartnerTelefon,
            AnsprechpartnerEmail = dto.AnsprechpartnerEmail,
            AnsprechpartnerPosition = dto.AnsprechpartnerPosition,
        };

        var saved = await _companyRepo.CreateOrUpdateAsync(profile);
        Log.Information("Company profile saved for user {UserId}", userId);
        return Ok(MapCompanyToDto(saved));
    }

    [HttpPost("company/logo")]
    [ProducesResponseType(typeof(CompanyProfileDto), StatusCodes.Status200OK)]
    [Authorize(Policy = "RequireStandortAdmin")]
    [RequestSizeLimit(MaxFileSize)]
    public async Task<ActionResult<CompanyProfileDto>> UploadCompanyLogo(IFormFile file)
    {
        var userId = GetUserId();
        var (error, path) = await SaveUploadedFile(file, "logos");
        if (error != null) return BadRequest(new { error });

        var existing = await _companyRepo.GetByUserIdAsync(userId);
        var profile = existing ?? new CompanyProfile { UserId = userId };
        profile.LogoPfad = path;
        var saved = await _companyRepo.CreateOrUpdateAsync(profile);
        return Ok(MapCompanyToDto(saved));
    }

    // ── Helpers ──

    private async Task<(string? error, string? path)> SaveUploadedFile(IFormFile? file, string subfolder)
    {
        if (file == null || file.Length == 0)
            return ("Keine Datei ausgewählt.", null);

        if (file.Length > MaxFileSize)
            return ("Datei ist zu groß (max. 2 MB).", null);

        var ext = Path.GetExtension(file.FileName);
        if (!AllowedExtensions.Contains(ext))
            return ("Nur JPG, PNG und WebP sind erlaubt.", null);

        var uploadsDir = Path.Combine(_env.ContentRootPath, "uploads", subfolder);
        Directory.CreateDirectory(uploadsDir);

        // Generate a unique filename to avoid collisions
        var fileName = $"{Guid.NewGuid()}{ext}";
        var fullPath = Path.Combine(uploadsDir, fileName);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        var relativePath = $"uploads/{subfolder}/{fileName}";
        return (null, relativePath);
    }

    private PersonalProfileDto MapPersonalToDto(PersonalProfile p) => new()
    {
        Id = p.Id,
        UserId = p.UserId,
        Anrede = p.Anrede,
        Titel = p.Titel,
        Vorname = p.Vorname,
        Nachname = p.Nachname,
        Geburtsdatum = p.Geburtsdatum,
        Strasse = p.Strasse,
        Hausnummer = p.Hausnummer,
        Plz = p.Plz,
        Ort = p.Ort,
        Telefon = p.Telefon,
        Email = p.Email,
        ProfilbildUrl = p.ProfilbildPfad != null ? $"/api/files/{p.ProfilbildPfad}" : null,
        CreatedAt = p.CreatedAt,
        UpdatedAt = p.UpdatedAt,
    };

    private CompanyProfileDto MapCompanyToDto(CompanyProfile c) => new()
    {
        Id = c.Id,
        UserId = c.UserId,
        Firmenname = c.Firmenname,
        Rechtsform = c.Rechtsform,
        Handelsregisternummer = c.Handelsregisternummer,
        UstIdNr = c.UstIdNr,
        Strasse = c.Strasse,
        Hausnummer = c.Hausnummer,
        Plz = c.Plz,
        Ort = c.Ort,
        Telefon = c.Telefon,
        Email = c.Email,
        Website = c.Website,
        AnsprechpartnerAnrede = c.AnsprechpartnerAnrede,
        AnsprechpartnerVorname = c.AnsprechpartnerVorname,
        AnsprechpartnerNachname = c.AnsprechpartnerNachname,
        AnsprechpartnerTelefon = c.AnsprechpartnerTelefon,
        AnsprechpartnerEmail = c.AnsprechpartnerEmail,
        AnsprechpartnerPosition = c.AnsprechpartnerPosition,
        LogoUrl = c.LogoPfad != null ? $"/api/files/{c.LogoPfad}" : null,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt,
    };

    // ── Zahlungsmethoden ──────────────────────────────────────────────

    [HttpGet("payments")]
    public async Task<ActionResult<List<PaymentMethodDto>>> GetPaymentMethods()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "demo-user";
        var methods = await _paymentRepo.GetByUserIdAsync(userId);
        return Ok(methods.Select(MapPaymentToDto).ToList());
    }

    [HttpPost("payments")]
    public async Task<ActionResult<PaymentMethodDto>> CreatePaymentMethod([FromBody] SavePaymentMethodDto dto)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "demo-user";

        var method = new PaymentMethod
        {
            UserId = userId,
            Typ = dto.Typ,
            Bezeichnung = dto.Bezeichnung,
            Kontoinhaber = dto.Kontoinhaber,
            IBAN = dto.IBAN,
            BIC = dto.BIC,
            Bankname = dto.Bankname,
            KartenNummer = dto.KartenNummer,
            KartenInhaber = dto.KartenInhaber,
            GueltigBis = dto.GueltigBis,
            PaypalEmail = dto.PaypalEmail,
            IstStandard = dto.IstStandard,
            SepaMandatErteilt = dto.SepaMandatErteilt,
        };

        var created = await _paymentRepo.CreateAsync(method);
        return CreatedAtAction(nameof(GetPaymentMethods), MapPaymentToDto(created));
    }

    [HttpPut("payments/{id}")]
    public async Task<ActionResult<PaymentMethodDto>> UpdatePaymentMethod(int id, [FromBody] SavePaymentMethodDto dto)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "demo-user";
        var existing = await _paymentRepo.GetByIdAsync(id);

        if (existing == null || existing.UserId != userId)
            return NotFound();

        existing.Typ = dto.Typ;
        existing.Bezeichnung = dto.Bezeichnung;
        existing.Kontoinhaber = dto.Kontoinhaber;
        existing.IBAN = dto.IBAN;
        existing.BIC = dto.BIC;
        existing.Bankname = dto.Bankname;
        existing.KartenNummer = dto.KartenNummer;
        existing.KartenInhaber = dto.KartenInhaber;
        existing.GueltigBis = dto.GueltigBis;
        existing.PaypalEmail = dto.PaypalEmail;
        existing.IstStandard = dto.IstStandard;
        existing.SepaMandatErteilt = dto.SepaMandatErteilt;

        var updated = await _paymentRepo.UpdateAsync(existing);
        return Ok(MapPaymentToDto(updated));
    }

    [HttpDelete("payments/{id}")]
    public async Task<IActionResult> DeletePaymentMethod(int id)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "demo-user";
        var existing = await _paymentRepo.GetByIdAsync(id);

        if (existing == null || existing.UserId != userId)
            return NotFound();

        await _paymentRepo.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("payments/{id}/default")]
    public async Task<IActionResult> SetDefaultPaymentMethod(int id)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "demo-user";
        var existing = await _paymentRepo.GetByIdAsync(id);

        if (existing == null || existing.UserId != userId)
            return NotFound();

        await _paymentRepo.SetDefaultAsync(id, userId);
        return NoContent();
    }

    private PaymentMethodDto MapPaymentToDto(PaymentMethod m) => new()
    {
        Id = m.Id,
        Typ = m.Typ,
        Bezeichnung = m.Bezeichnung,
        Kontoinhaber = m.Kontoinhaber,
        IBAN = m.IBAN,
        BIC = m.BIC,
        Bankname = m.Bankname,
        KartenNummer = m.KartenNummer,
        KartenInhaber = m.KartenInhaber,
        GueltigBis = m.GueltigBis,
        PaypalEmail = m.PaypalEmail,
        IstStandard = m.IstStandard,
        SepaMandatErteilt = m.SepaMandatErteilt,
        CreatedAt = m.CreatedAt,
        UpdatedAt = m.UpdatedAt,
    };
}

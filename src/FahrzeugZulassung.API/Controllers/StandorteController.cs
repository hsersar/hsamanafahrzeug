using FahrzeugZulassung.API.DTOs;
using FahrzeugZulassung.Domain.Entities;
using FahrzeugZulassung.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FahrzeugZulassung.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StandorteController : ControllerBase
{
    private readonly FahrzeugZulassungDbContext _context;

    public StandorteController(FahrzeugZulassungDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<StandortDto>>> ListeStandorte()
    {
        var standorte = await _context.Standorte
            .Select(s => new StandortDto
            {
                Id = s.Id,
                Name = s.Name,
                Firmenname = s.Firmenname,
                Strasse = s.Strasse,
                PLZ = s.PLZ,
                Ort = s.Ort,
                UStID = s.UStID,
                IBAN = s.IBAN,
                BIC = s.BIC
            })
            .ToListAsync();

        return Ok(standorte);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StandortDto>> HoleStandort(Guid id)
    {
        var standort = await _context.Standorte.FindAsync(id);
        if (standort == null)
            return NotFound();

        return Ok(new StandortDto
        {
            Id = standort.Id,
            Name = standort.Name,
            Firmenname = standort.Firmenname,
            Strasse = standort.Strasse,
            PLZ = standort.PLZ,
            Ort = standort.Ort,
            UStID = standort.UStID,
            IBAN = standort.IBAN,
            BIC = standort.BIC
        });
    }

    [HttpPost]
    public async Task<ActionResult<StandortDto>> ErstelleStandort([FromBody] StandortErstellenDto dto)
    {
        var standort = new Standort
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Firmenname = dto.Firmenname,
            Strasse = dto.Strasse,
            PLZ = dto.PLZ,
            Ort = dto.Ort,
            UStID = dto.UStID,
            Steuernummer = dto.Steuernummer,
            IBAN = dto.IBAN,
            BIC = dto.BIC,
            Bankname = dto.Bankname
        };

        _context.Standorte.Add(standort);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(HoleStandort), new { id = standort.Id }, new StandortDto
        {
            Id = standort.Id,
            Name = standort.Name,
            Firmenname = standort.Firmenname,
            Strasse = standort.Strasse,
            PLZ = standort.PLZ,
            Ort = standort.Ort,
            UStID = standort.UStID,
            IBAN = standort.IBAN,
            BIC = standort.BIC
        });
    }
}

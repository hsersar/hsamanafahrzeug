using FahrzeugZulassung.API.DTOs;
using FahrzeugZulassung.Domain.Entities;
using FahrzeugZulassung.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FahrzeugZulassung.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KundenController : ControllerBase
{
    private readonly FahrzeugZulassungDbContext _context;

    public KundenController(FahrzeugZulassungDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<KundeDto>>> ListeKunden()
    {
        var kunden = await _context.Kunden
            .Select(k => new KundeDto
            {
                Id = k.Id,
                Vorname = k.Vorname,
                Nachname = k.Nachname,
                Email = k.Email,
                Strasse = k.Strasse,
                PLZ = k.PLZ,
                Ort = k.Ort,
                Firmenname = k.Firmenname,
                UStID = k.UStID
            })
            .ToListAsync();

        return Ok(kunden);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<KundeDto>> HoleKunde(Guid id)
    {
        var kunde = await _context.Kunden.FindAsync(id);
        if (kunde == null)
            return NotFound();

        return Ok(new KundeDto
        {
            Id = kunde.Id,
            Vorname = kunde.Vorname,
            Nachname = kunde.Nachname,
            Email = kunde.Email,
            Strasse = kunde.Strasse,
            PLZ = kunde.PLZ,
            Ort = kunde.Ort,
            Firmenname = kunde.Firmenname,
            UStID = kunde.UStID
        });
    }

    [HttpPost]
    public async Task<ActionResult<KundeDto>> ErstelleKunde([FromBody] KundeErstellenDto dto)
    {
        var kunde = new Kunde
        {
            Id = Guid.NewGuid(),
            Vorname = dto.Vorname,
            Nachname = dto.Nachname,
            Email = dto.Email,
            Telefon = dto.Telefon,
            Strasse = dto.Strasse,
            PLZ = dto.PLZ,
            Ort = dto.Ort,
            Firmenname = dto.Firmenname,
            UStID = dto.UStID
        };

        _context.Kunden.Add(kunde);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(HoleKunde), new { id = kunde.Id }, new KundeDto
        {
            Id = kunde.Id,
            Vorname = kunde.Vorname,
            Nachname = kunde.Nachname,
            Email = kunde.Email,
            Strasse = kunde.Strasse,
            PLZ = kunde.PLZ,
            Ort = kunde.Ort,
            Firmenname = kunde.Firmenname,
            UStID = kunde.UStID
        });
    }
}

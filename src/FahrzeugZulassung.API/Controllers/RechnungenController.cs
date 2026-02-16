using FahrzeugZulassung.API.DTOs;
using FahrzeugZulassung.Domain.Entities;
using FahrzeugZulassung.Infrastructure.Data;
using FahrzeugZulassung.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FahrzeugZulassung.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RechnungenController : ControllerBase
{
    private readonly FahrzeugZulassungDbContext _context;
    private readonly IZUGFeRDService _zugferdService;
    private readonly ILogger<RechnungenController> _logger;

    public RechnungenController(
        FahrzeugZulassungDbContext context,
        IZUGFeRDService zugferdService,
        ILogger<RechnungenController> logger)
    {
        _context = context;
        _zugferdService = zugferdService;
        _logger = logger;
    }

    /// <summary>
    /// Erstellt eine neue Rechnung mit ZUGFeRD XML und PDF
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<RechnungDto>> ErstelleRechnung([FromBody] RechnungErstellenDto dto)
    {
        try
        {
            // Validierung
            var kunde = await _context.Kunden.FindAsync(dto.KundeId);
            if (kunde == null)
                return NotFound("Kunde nicht gefunden");

            var standort = await _context.Standorte.FindAsync(dto.StandortId);
            if (standort == null)
                return NotFound("Standort nicht gefunden");

            if (dto.Positionen == null || dto.Positionen.Count == 0)
                return BadRequest("Mindestens eine Position ist erforderlich");

            // Erstelle Rechnung
            var rechnung = new Rechnung
            {
                Id = Guid.NewGuid(),
                RechnungsNummer = await GeneriereRechnungsNummer(),
                ErstelltAm = DateTime.UtcNow,
                Faelligkeitsdatum = DateTime.UtcNow.AddDays(14),
                Format = dto.Format,
                KundeId = dto.KundeId,
                StandortId = dto.StandortId,
                Kunde = kunde,
                Standort = standort
            };

            // Erstelle Positionen
            int position = 1;
            foreach (var posDto in dto.Positionen)
            {
                var nettobetrag = posDto.Menge * posDto.Einzelpreis;
                var steuerbetrag = nettobetrag * (posDto.Steuersatz / 100m);
                var bruttobetrag = nettobetrag + steuerbetrag;

                var pos = new RechnungsPosition
                {
                    Id = Guid.NewGuid(),
                    RechnungId = rechnung.Id,
                    Position = position++,
                    Beschreibung = posDto.Beschreibung,
                    Artikelnummer = posDto.Artikelnummer,
                    Menge = posDto.Menge,
                    Einzelpreis = posDto.Einzelpreis,
                    Nettobetrag = nettobetrag,
                    Steuersatz = posDto.Steuersatz,
                    Steuerbetrag = steuerbetrag,
                    Bruttobetrag = bruttobetrag
                };

                rechnung.Positionen.Add(pos);
            }

            // Berechne Gesamtbeträge
            rechnung.Nettobetrag = rechnung.Positionen.Sum(p => p.Nettobetrag);
            rechnung.Steuerbetrag = rechnung.Positionen.Sum(p => p.Steuerbetrag);
            rechnung.Bruttobetrag = rechnung.Positionen.Sum(p => p.Bruttobetrag);

            // Generiere ZUGFeRD XML und PDF
            var zugferdResult = await _zugferdService.ErstelleERechnungAsync(rechnung);
            
            if (!zugferdResult.IstGueltig)
            {
                _logger.LogWarning("ZUGFeRD Validierung fehlgeschlagen: {Fehler}", 
                    string.Join(", ", zugferdResult.Validierungsfehler));
            }

            rechnung.ZUGFeRDXml = zugferdResult.Xml;

            // Speichere in Datenbank
            _context.Rechnungen.Add(rechnung);
            await _context.SaveChangesAsync();

            return Ok(new RechnungDto
            {
                Id = rechnung.Id,
                RechnungsNummer = rechnung.RechnungsNummer,
                ErstelltAm = rechnung.ErstelltAm,
                Faelligkeitsdatum = rechnung.Faelligkeitsdatum,
                Nettobetrag = rechnung.Nettobetrag,
                Steuerbetrag = rechnung.Steuerbetrag,
                Bruttobetrag = rechnung.Bruttobetrag,
                Format = rechnung.Format,
                HatZUGFeRDXml = !string.IsNullOrEmpty(rechnung.ZUGFeRDXml),
                HatPdf = zugferdResult.PdfBytes.Length > 0,
                KundeId = rechnung.KundeId,
                KundeName = $"{kunde.Vorname} {kunde.Nachname}"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Erstellen der Rechnung");
            return StatusCode(500, "Interner Serverfehler");
        }
    }

    /// <summary>
    /// Lädt das ZUGFeRD PDF einer Rechnung herunter
    /// </summary>
    [HttpGet("{id}/pdf")]
    public async Task<IActionResult> LadePdf(Guid id)
    {
        var rechnung = await _context.Rechnungen
            .Include(r => r.Kunde)
            .Include(r => r.Standort)
            .Include(r => r.Positionen)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (rechnung == null)
            return NotFound();

        var pdfBytes = await _zugferdService.ErstelleERechnungAlsPdfAsync(rechnung);
        
        return File(pdfBytes, "application/pdf", $"{rechnung.RechnungsNummer}.pdf");
    }

    /// <summary>
    /// Lädt das ZUGFeRD XML einer Rechnung herunter
    /// </summary>
    [HttpGet("{id}/xml")]
    public async Task<IActionResult> LadeXml(Guid id)
    {
        var rechnung = await _context.Rechnungen.FindAsync(id);

        if (rechnung == null)
            return NotFound();

        if (string.IsNullOrEmpty(rechnung.ZUGFeRDXml))
            return NotFound("Kein ZUGFeRD XML vorhanden");

        var xmlBytes = System.Text.Encoding.UTF8.GetBytes(rechnung.ZUGFeRDXml);
        
        return File(xmlBytes, "application/xml", $"{rechnung.RechnungsNummer}.xml");
    }

    /// <summary>
    /// Listet alle Rechnungen auf
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<RechnungDto>>> ListeRechnungen()
    {
        var rechnungen = await _context.Rechnungen
            .Include(r => r.Kunde)
            .OrderByDescending(r => r.ErstelltAm)
            .Select(r => new RechnungDto
            {
                Id = r.Id,
                RechnungsNummer = r.RechnungsNummer,
                ErstelltAm = r.ErstelltAm,
                Faelligkeitsdatum = r.Faelligkeitsdatum,
                Nettobetrag = r.Nettobetrag,
                Steuerbetrag = r.Steuerbetrag,
                Bruttobetrag = r.Bruttobetrag,
                Format = r.Format,
                HatZUGFeRDXml = !string.IsNullOrEmpty(r.ZUGFeRDXml),
                HatPdf = !string.IsNullOrEmpty(r.PdfDateiPfad),
                KundeId = r.KundeId,
                KundeName = $"{r.Kunde.Vorname} {r.Kunde.Nachname}"
            })
            .ToListAsync();

        return Ok(rechnungen);
    }

    /// <summary>
    /// Holt eine einzelne Rechnung
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<RechnungDto>> HoleRechnung(Guid id)
    {
        var rechnung = await _context.Rechnungen
            .Include(r => r.Kunde)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (rechnung == null)
            return NotFound();

        return Ok(new RechnungDto
        {
            Id = rechnung.Id,
            RechnungsNummer = rechnung.RechnungsNummer,
            ErstelltAm = rechnung.ErstelltAm,
            Faelligkeitsdatum = rechnung.Faelligkeitsdatum,
            Nettobetrag = rechnung.Nettobetrag,
            Steuerbetrag = rechnung.Steuerbetrag,
            Bruttobetrag = rechnung.Bruttobetrag,
            Format = rechnung.Format,
            HatZUGFeRDXml = !string.IsNullOrEmpty(rechnung.ZUGFeRDXml),
            HatPdf = !string.IsNullOrEmpty(rechnung.PdfDateiPfad),
            KundeId = rechnung.KundeId,
            KundeName = $"{rechnung.Kunde.Vorname} {rechnung.Kunde.Nachname}"
        });
    }

    private async Task<string> GeneriereRechnungsNummer()
    {
        var jahr = DateTime.UtcNow.Year;
        var prefix = $"RE-{jahr}-";
        
        var letzteNummer = await _context.Rechnungen
            .Where(r => r.RechnungsNummer.StartsWith(prefix))
            .OrderByDescending(r => r.RechnungsNummer)
            .Select(r => r.RechnungsNummer)
            .FirstOrDefaultAsync();

        int naechsteNummer = 1;
        if (letzteNummer != null)
        {
            var nummerTeil = letzteNummer.Replace(prefix, "");
            if (int.TryParse(nummerTeil, out int aktuelleNummer))
            {
                naechsteNummer = aktuelleNummer + 1;
            }
        }

        return $"{prefix}{naechsteNummer:D5}";
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using iKfz.Backend.DTOs;
using iKfz.Backend.Models;
using iKfz.Backend.Repositories;

namespace iKfz.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceRepository _invoiceRepo;

    public InvoicesController(IInvoiceRepository invoiceRepo)
    {
        _invoiceRepo = invoiceRepo;
    }

    [HttpGet]
    public async Task<ActionResult<List<InvoiceDto>>> GetMyInvoices()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "demo-user";
        var invoices = await _invoiceRepo.GetByUserIdAsync(userId);
        return Ok(invoices.Select(MapToDto).ToList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<InvoiceDto>> GetInvoice(int id)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "demo-user";
        var invoice = await _invoiceRepo.GetByIdAsync(id);
        if (invoice == null || invoice.UserId != userId)
            return NotFound();
        return Ok(MapToDto(invoice));
    }

    private static InvoiceDto MapToDto(Invoice i) => new()
    {
        Id = i.Id,
        Rechnungsnummer = i.Rechnungsnummer,
        RegistrationRequestId = i.RegistrationRequestId,
        Beschreibung = i.Beschreibung,
        Nettobetrag = i.Nettobetrag,
        MwstSatz = i.MwstSatz,
        MwstBetrag = i.MwstBetrag,
        Bruttobetrag = i.Bruttobetrag,
        Status = i.Status.ToString(),
        Rechnungsdatum = i.Rechnungsdatum,
        Faelligkeitsdatum = i.Faelligkeitsdatum,
        BezahltAm = i.BezahltAm,
        Zahlungsmethode = i.Zahlungsmethode,
        CreatedAt = i.CreatedAt,
    };
}

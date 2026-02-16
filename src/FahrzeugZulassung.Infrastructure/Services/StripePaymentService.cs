using FahrzeugZulassung.Application.DTOs;
using FahrzeugZulassung.Application.Interfaces;
using FahrzeugZulassung.Domain.Entities;
using FahrzeugZulassung.Domain.Enums;
using FahrzeugZulassung.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace FahrzeugZulassung.Infrastructure.Services;

public class StripePaymentService : IPaymentService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public StripePaymentService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
        StripeConfiguration.ApiKey = _configuration["Payment:Stripe:SecretKey"];
    }

    public async Task<PaymentInitResult> InitiiereZahlungAsync(Guid rechnungId, ZahlungsMethode methode)
    {
        var rechnung = await _context.Rechnungen.FindAsync(rechnungId);
        if (rechnung == null)
            throw new ArgumentException("Rechnung nicht gefunden");

        try
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(rechnung.Betrag * 100), // Convert to cents
                Currency = rechnung.Waehrung.ToLower(),
                PaymentMethodTypes = methode == ZahlungsMethode.Kreditkarte 
                    ? new List<string> { "card" }
                    : new List<string> { "sepa_debit" },
                Metadata = new Dictionary<string, string>
                {
                    { "RechnungId", rechnungId.ToString() },
                    { "Rechnungsnummer", rechnung.Rechnungsnummer }
                }
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            var zahlung = new Zahlung
            {
                Id = Guid.NewGuid(),
                RechnungId = rechnungId,
                Methode = methode,
                Betrag = rechnung.Betrag,
                Waehrung = rechnung.Waehrung,
                StripePaymentIntentId = paymentIntent.Id,
                StripeClientSecret = paymentIntent.ClientSecret
            };

            _context.Zahlungen.Add(zahlung);
            await _context.SaveChangesAsync();

            return new PaymentInitResult
            {
                ZahlungId = zahlung.Id,
                Methode = methode,
                ClientSecret = paymentIntent.ClientSecret,
                Erfolg = true
            };
        }
        catch (Exception ex)
        {
            return new PaymentInitResult
            {
                Erfolg = false,
                FehlerNachricht = ex.Message
            };
        }
    }

    public async Task<PaymentResult> VerarbeiteZahlungAsync(Guid zahlungId)
    {
        var zahlung = await _context.Zahlungen
            .Include(z => z.Rechnung)
            .FirstOrDefaultAsync(z => z.Id == zahlungId);

        if (zahlung == null)
            throw new ArgumentException("Zahlung nicht gefunden");

        try
        {
            if (!string.IsNullOrEmpty(zahlung.StripePaymentIntentId))
            {
                var service = new PaymentIntentService();
                var paymentIntent = await service.GetAsync(zahlung.StripePaymentIntentId);

                if (paymentIntent.Status == "succeeded")
                {
                    zahlung.Status = ZahlungsStatus.Erfolgreich;
                    zahlung.BezahltAm = DateTime.UtcNow;
                    zahlung.Rechnung.IstBezahlt = true;
                    zahlung.Rechnung.BezahltAm = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    return new PaymentResult
                    {
                        ZahlungId = zahlungId,
                        Status = ZahlungsStatus.Erfolgreich,
                        Erfolg = true
                    };
                }
            }

            return new PaymentResult
            {
                ZahlungId = zahlungId,
                Status = zahlung.Status,
                Erfolg = false,
                FehlerNachricht = "Zahlung noch nicht abgeschlossen"
            };
        }
        catch (Exception ex)
        {
            zahlung.Status = ZahlungsStatus.Fehlgeschlagen;
            zahlung.FehlerNachricht = ex.Message;
            await _context.SaveChangesAsync();

            return new PaymentResult
            {
                ZahlungId = zahlungId,
                Status = ZahlungsStatus.Fehlgeschlagen,
                Erfolg = false,
                FehlerNachricht = ex.Message
            };
        }
    }

    public async Task<PaymentResult> BestaetigeZahlungAsync(string providerReferenz)
    {
        var zahlung = await _context.Zahlungen
            .Include(z => z.Rechnung)
            .FirstOrDefaultAsync(z => z.StripePaymentIntentId == providerReferenz);

        if (zahlung == null)
            throw new ArgumentException("Zahlung nicht gefunden");

        return await VerarbeiteZahlungAsync(zahlung.Id);
    }

    public async Task<bool> StornierenAsync(Guid zahlungId, string grund)
    {
        var zahlung = await _context.Zahlungen.FindAsync(zahlungId);
        if (zahlung == null)
            return false;

        try
        {
            if (!string.IsNullOrEmpty(zahlung.StripePaymentIntentId))
            {
                var service = new PaymentIntentService();
                await service.CancelAsync(zahlung.StripePaymentIntentId);
            }

            zahlung.Status = ZahlungsStatus.Storniert;
            zahlung.StorniertAm = DateTime.UtcNow;
            zahlung.FehlerNachricht = grund;
            await _context.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ErstattenAsync(Guid zahlungId, decimal? betrag = null)
    {
        var zahlung = await _context.Zahlungen.FindAsync(zahlungId);
        if (zahlung == null || string.IsNullOrEmpty(zahlung.StripePaymentIntentId))
            return false;

        try
        {
            var options = new RefundCreateOptions
            {
                PaymentIntent = zahlung.StripePaymentIntentId
            };

            if (betrag.HasValue)
                options.Amount = (long)(betrag.Value * 100);

            var service = new RefundService();
            await service.CreateAsync(options);

            zahlung.Status = ZahlungsStatus.Erstattet;
            await _context.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }
}

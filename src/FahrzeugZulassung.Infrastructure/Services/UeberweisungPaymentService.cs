using FahrzeugZulassung.Application.DTOs;
using FahrzeugZulassung.Application.Interfaces;
using FahrzeugZulassung.Domain.Entities;
using FahrzeugZulassung.Domain.Enums;
using FahrzeugZulassung.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FahrzeugZulassung.Infrastructure.Services;

public class UeberweisungPaymentService : IPaymentService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public UeberweisungPaymentService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<PaymentInitResult> InitiiereZahlungAsync(Guid rechnungId, ZahlungsMethode methode)
    {
        var rechnung = await _context.Rechnungen.FindAsync(rechnungId);
        if (rechnung == null)
            throw new ArgumentException("Rechnung nicht gefunden");

        var iban = _configuration["Payment:Ueberweisung:IBAN"] ?? "DE89370400440532013000";
        var bic = _configuration["Payment:Ueberweisung:BIC"] ?? "COBADEFFXXX";
        var bank = _configuration["Payment:Ueberweisung:Bank"] ?? "Commerzbank";
        var empfaenger = _configuration["Payment:Ueberweisung:Empfaenger"] ?? "KFZ-Zulassungsstelle";

        var zahlung = new Zahlung
        {
            Id = Guid.NewGuid(),
            RechnungId = rechnungId,
            Methode = methode,
            Betrag = rechnung.Betrag,
            Waehrung = rechnung.Waehrung,
            UeberweisungsReferenz = rechnung.Rechnungsnummer,
            EmpfaengerIBAN = iban,
            EmpfaengerBIC = bic,
            EmpfaengerBank = bank,
            Verwendungszweck = $"Rechnung {rechnung.Rechnungsnummer}"
        };

        _context.Zahlungen.Add(zahlung);
        await _context.SaveChangesAsync();

        return new PaymentInitResult
        {
            ZahlungId = zahlung.Id,
            Methode = methode,
            Ueberweisung = new UeberweisungsDetails
            {
                Empfaenger = empfaenger,
                IBAN = iban,
                BIC = bic,
                Bank = bank,
                Verwendungszweck = zahlung.Verwendungszweck!,
                Betrag = rechnung.Betrag
            },
            Erfolg = true
        };
    }

    public async Task<PaymentResult> VerarbeiteZahlungAsync(Guid zahlungId)
    {
        var zahlung = await _context.Zahlungen.FindAsync(zahlungId);
        if (zahlung == null)
            throw new ArgumentException("Zahlung nicht gefunden");

        // Überweisung muss manuell bestätigt werden
        return new PaymentResult
        {
            ZahlungId = zahlungId,
            Status = zahlung.Status,
            Erfolg = zahlung.Status == ZahlungsStatus.Erfolgreich
        };
    }

    public async Task<PaymentResult> BestaetigeZahlungAsync(string providerReferenz)
    {
        var zahlung = await _context.Zahlungen
            .FirstOrDefaultAsync(z => z.UeberweisungsReferenz == providerReferenz);

        if (zahlung == null)
            throw new ArgumentException("Zahlung nicht gefunden");

        return await VerarbeiteZahlungAsync(zahlung.Id);
    }

    public async Task<bool> StornierenAsync(Guid zahlungId, string grund)
    {
        var zahlung = await _context.Zahlungen.FindAsync(zahlungId);
        if (zahlung == null)
            return false;

        zahlung.Status = ZahlungsStatus.Storniert;
        zahlung.StorniertAm = DateTime.UtcNow;
        zahlung.FehlerNachricht = grund;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ErstattenAsync(Guid zahlungId, decimal? betrag = null)
    {
        var zahlung = await _context.Zahlungen.FindAsync(zahlungId);
        if (zahlung == null)
            return false;

        zahlung.Status = ZahlungsStatus.Erstattet;
        await _context.SaveChangesAsync();

        return true;
    }
}

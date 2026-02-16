using FahrzeugZulassung.Application.DTOs;
using FahrzeugZulassung.Domain.Enums;

namespace FahrzeugZulassung.Application.Interfaces;

public interface IPaymentService
{
    Task<PaymentInitResult> InitiiereZahlungAsync(Guid rechnungId, ZahlungsMethode methode);
    Task<PaymentResult> VerarbeiteZahlungAsync(Guid zahlungId);
    Task<PaymentResult> BestaetigeZahlungAsync(string providerReferenz);
    Task<bool> StornierenAsync(Guid zahlungId, string grund);
    Task<bool> ErstattenAsync(Guid zahlungId, decimal? betrag = null);
}

using FahrzeugZulassung.Domain.Enums;

namespace FahrzeugZulassung.Application.Interfaces;

public interface IBenachrichtigungService
{
    Task<bool> SendeTrackingEmailAsync(Guid auftragId, string empfaengerEmail);
    Task<bool> SendeTrackingSMSAsync(Guid auftragId, string telefonnummer);
    Task<bool> SendeStatusUpdateEmailAsync(Guid auftragId, AuftragStatus neuerStatus);
    Task<bool> SendeZahlungsbestaetigungAsync(Guid rechnungId);
}

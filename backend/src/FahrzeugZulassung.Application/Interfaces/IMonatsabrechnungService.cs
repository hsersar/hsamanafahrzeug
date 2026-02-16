using FahrzeugZulassung.Application.DTOs.Provision;

namespace FahrzeugZulassung.Application.Interfaces;

public interface IMonatsabrechnungService
{
    // Abrechnungen erstellen (SuperAdmin)
    Task<MonatsabrechnungDto> ErstelleAbrechnungAsync(Guid standortId, int jahr, int monat);
    Task<List<MonatsabrechnungDto>> ErstelleAlleAbrechnungenAsync(int jahr, int monat);  // Für alle Standorte
    
    // Abrechnungen abrufen
    Task<MonatsabrechnungDto?> GetAbrechnungAsync(Guid id);
    Task<List<MonatsabrechnungDto>> GetAbrechnungenFuerStandortAsync(Guid standortId, int? jahr = null);
    Task<List<MonatsabrechnungDto>> GetAlleAbrechnungenAsync(MonatsabrechnungFilter filter);
    
    // Aktionen
    Task<bool> AbrechnungVersendenAsync(Guid id);           // Per E-Mail als ZUGFeRD PDF
    Task<bool> AbrechnungAlsBezahltMarkierenAsync(Guid id, string zahlungsReferenz);
    Task<bool> AbrechnungStornierenAsync(Guid id, string grund);
    
    // PDF/Export
    Task<byte[]> AbrechnungAlsPdfAsync(Guid id);            // ZUGFeRD PDF
    Task<byte[]> AbrechnungAlsCsvAsync(MonatsabrechnungFilter filter);
    
    // Dashboard-Daten für SuperAdmin
    Task<PlattformUmsatzDto> GetPlattformUmsatzAsync(int jahr, int? monat = null);
}

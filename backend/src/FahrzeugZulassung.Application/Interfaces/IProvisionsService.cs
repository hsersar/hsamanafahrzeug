using FahrzeugZulassung.Application.DTOs.Provision;

namespace FahrzeugZulassung.Application.Interfaces;

public interface IProvisionsService
{
    // Standard-Provisionsmodell
    Task<ProvisionsModellDto> GetStandardModellAsync();
    Task<ProvisionsModellDto> SetStandardModellAsync(ProvisionsModellUpdateDto dto);  // Nur SuperAdmin
    
    // Standort-spezifisches Provisionsmodell
    Task<ProvisionsModellDto> GetModellFuerStandortAsync(Guid standortId);
    Task<ProvisionsModellDto> SetModellFuerStandortAsync(Guid standortId, ProvisionsModellUpdateDto dto);
    Task ResetAufStandardAsync(Guid standortId);  // Zurück auf Standard
    
    // Berechnung
    Task<ProvisionsBerechnung> BerechneProvisionAsync(Guid standortId, int jahr, int monat);
    
    // Provisionshistorie
    Task<List<ProvisionsModellDto>> GetHistorieAsync(Guid standortId);
}

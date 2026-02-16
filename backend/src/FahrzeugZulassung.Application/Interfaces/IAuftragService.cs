using FahrzeugZulassung.Application.DTOs.Auftraege;

namespace FahrzeugZulassung.Application.Interfaces;

public interface IAuftragService
{
    Task<AuftragResponseDto> CreateAsync(AuftragCreateDto dto, CancellationToken cancellationToken = default);
    Task<AuftragResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<AuftragResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AuftragResponseDto> UpdateStatusAsync(Guid id, AuftragUpdateStatusDto dto, CancellationToken cancellationToken = default);
    Task<bool> SubmitToIKfzAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<AuftragResponseDto>> GetByKundeIdAsync(Guid kundeId, CancellationToken cancellationToken = default);
    Task<List<AuftragResponseDto>> GetByStandortIdAsync(Guid standortId, CancellationToken cancellationToken = default);
}

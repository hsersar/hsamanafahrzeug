using FahrzeugZulassung.Application.DTOs.Kunden;

namespace FahrzeugZulassung.Application.Interfaces;

public interface IKundenService
{
    Task<KundeResponseDto> CreateAsync(KundeCreateDto dto, CancellationToken cancellationToken = default);
    Task<KundeResponseDto> UpdateAsync(Guid id, KundeUpdateDto dto, CancellationToken cancellationToken = default);
    Task<KundeResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<KundeResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

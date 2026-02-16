using FahrzeugZulassung.Application.DTOs.Mitarbeiter;

namespace FahrzeugZulassung.Application.Interfaces;

public interface IMitarbeiterService
{
    Task<MitarbeiterResponseDto> CreateAsync(MitarbeiterCreateDto dto, CancellationToken cancellationToken = default);
    Task<MitarbeiterResponseDto> UpdateAsync(Guid id, MitarbeiterUpdateDto dto, CancellationToken cancellationToken = default);
    Task<MitarbeiterResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<MitarbeiterResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task ActivateAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
}

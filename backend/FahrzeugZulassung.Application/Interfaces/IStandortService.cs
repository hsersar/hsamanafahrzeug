using FahrzeugZulassung.Application.DTOs.Standorte;

namespace FahrzeugZulassung.Application.Interfaces;

public interface IStandortService
{
    Task<StandortResponseDto> CreateAsync(StandortCreateDto dto, CancellationToken cancellationToken = default);
    Task<StandortResponseDto> UpdateAsync(Guid id, StandortUpdateDto dto, CancellationToken cancellationToken = default);
    Task<StandortResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<StandortResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

using FahrzeugZulassung.Application.DTOs.Rechnungen;

namespace FahrzeugZulassung.Application.Interfaces;

public interface IRechnungService
{
    Task<RechnungResponseDto> CreateAsync(RechnungCreateDto dto, CancellationToken cancellationToken = default);
    Task<RechnungResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<RechnungResponseDto>> GetByAuftragIdAsync(Guid auftragId, CancellationToken cancellationToken = default);
    Task<RechnungResponseDto> BezahlenAsync(Guid id, RechnungBezahlenDto dto, CancellationToken cancellationToken = default);
}

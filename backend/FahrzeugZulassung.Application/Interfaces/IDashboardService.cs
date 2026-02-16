using FahrzeugZulassung.Application.DTOs.Dashboard;

namespace FahrzeugZulassung.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardResponseDto> GetDashboardDataAsync(CancellationToken cancellationToken = default);
}

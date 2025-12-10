using BE_Portfolio.DTOs.Dashboard;

namespace BE_Portfolio.Services;

public interface IDashboardService
{
    Task<DashboardResponseDto> GetDashboardDataAsync(CancellationToken ct = default);
}

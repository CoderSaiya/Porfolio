using BE_Portfolio.DTOs.Admin;

namespace BE_Portfolio.Services;

public interface IDashboardService
{
    Task<DashboardResponseDto> GetDashboardDataAsync(CancellationToken ct = default);
}

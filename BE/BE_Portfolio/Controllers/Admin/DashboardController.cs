using BE_Portfolio.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE_Portfolio.Controllers.Admin;

[ApiController]
[Route("api/admin/dashboard")]
// [Authorize(Roles = "Admin")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboardData(CancellationToken ct)
    {
        var data = await _dashboardService.GetDashboardDataAsync(ct);
        return Ok(data);
    }
}

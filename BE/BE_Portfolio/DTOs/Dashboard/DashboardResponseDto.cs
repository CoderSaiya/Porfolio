namespace BE_Portfolio.DTOs.Dashboard;

public class DashboardResponseDto
{
    public DashboardStatsResponseDto Stats { get; set; } = new();
    public List<DashboardChartResponseDto> ChartData { get; set; } = new();
    public List<DashboardActivityResponseDto> RecentActivities { get; set; } = new();
}

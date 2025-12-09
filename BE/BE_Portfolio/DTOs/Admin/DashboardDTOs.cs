namespace BE_Portfolio.DTOs.Admin;

public class DashboardStatsDto
{
    public long TotalViews { get; set; }
    public long TotalProjects { get; set; }
    public long TotalBlogs { get; set; }
    public long TotalMessages { get; set; }
}

public class DashboardChartDto
{
    public string Label { get; set; } = string.Empty;
    public int Value { get; set; }
}

public class DashboardActivityDto
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "blog", "project", "message"
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}

public class DashboardResponseDto
{
    public DashboardStatsDto Stats { get; set; } = new();
    public List<DashboardChartDto> ChartData { get; set; } = new();
    public List<DashboardActivityDto> RecentActivities { get; set; } = new();
}

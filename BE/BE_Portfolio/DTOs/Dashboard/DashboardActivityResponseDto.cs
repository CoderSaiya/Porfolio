namespace BE_Portfolio.DTOs.Dashboard;

public class DashboardActivityResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "blog", "project", "message"
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}

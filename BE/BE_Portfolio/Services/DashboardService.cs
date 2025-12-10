using BE_Portfolio.DTOs.Dashboard;
using BE_Portfolio.Helpers;
using BE_Portfolio.Models.Domain;
using BE_Portfolio.Persistence.Repositories.Interfaces;

namespace BE_Portfolio.Services;

public class DashboardService : IDashboardService
{
    private readonly IBlogPostRepository _blogRepo;
    private readonly IProjectRepository _projectRepo;
    private readonly IContactMessageRepository _messageRepo;

    public DashboardService(
        IBlogPostRepository blogRepo, 
        IProjectRepository projectRepo, 
        IContactMessageRepository messageRepo)
    {
        _blogRepo = blogRepo;
        _projectRepo = projectRepo;
        _messageRepo = messageRepo;
    }

    public async Task<DashboardResponseDto> GetDashboardDataAsync(CancellationToken ct = default)
    {
        var response = new DashboardResponseDto();

        // Parallel execution for Stats and Chart Data
        var totalBlogsTask = _blogRepo.CountAsync(new BlogFilter(), ct);
        var totalProjectsTask = _projectRepo.CountAsync(null, ct);
        var totalMessagesTask = _messageRepo.CountAsync(new MessageFilter(), ct);
        var totalViewsTask = _blogRepo.GetTotalViewsAsync(ct);

        await Task.WhenAll(totalBlogsTask, totalProjectsTask, totalMessagesTask, totalViewsTask);

        response.Stats.TotalBlogs = totalBlogsTask.Result;
        response.Stats.TotalProjects = totalProjectsTask.Result;
        response.Stats.TotalMessages = totalMessagesTask.Result;
        response.Stats.TotalViews = totalViewsTask.Result;

        // Chart Data (Still mock for now as per requirement)
        response.ChartData = new List<DashboardChartResponseDto>
        {
            new() { Label = "Mon", Value = 12 },
            new() { Label = "Tue", Value = 19 },
            new() { Label = "Wed", Value = 3 },
            new() { Label = "Thu", Value = 5 },
            new() { Label = "Fri", Value = 2 },
            new() { Label = "Sat", Value = 3 },
            new() { Label = "Sun", Value = 10 }
        };

        // Parallel execution for Recent Activities
        var recentBlogsTask = _blogRepo.GetAllAsync(new BlogFilter { Page = 1, PageSize = 3 }, ct);
        var recentProjectsTask = _projectRepo.GetAllAsync(new ProjectFilter { Limit = 3 }, ct);
        var recentMessagesTask = _messageRepo.ListAsync(new MessageFilter { PageSize = 3 }, ct);

        await Task.WhenAll(recentBlogsTask, recentProjectsTask, recentMessagesTask);
        
        var recentBlogs = recentBlogsTask.Result.Select(b => new DashboardActivityResponseDto
        {
            Id = b.Id.ToString(),
            Type = "blog",
            Title = "New Blog: " + b.Title,
            Description = "Published in " + (string.IsNullOrEmpty(b.CategoryId?.ToString()) ? "General" : "Category"),
            Time = DateHelper.GetTimeAgo(b.PublishedAt ?? b.CreatedAt),
            Icon = "FileText"
        });

        var recentProjects = recentProjectsTask.Result.Select(p => new DashboardActivityResponseDto
        {
            Id = p.Id.ToString(),
            Type = "project",
            Title = "New Project: " + p.Title,
            Description = p.Description,
            Time = DateHelper.GetTimeAgo(p.CreatedAt),
            Icon = "FolderGit2"
        });

        var recentMessages = recentMessagesTask.Result.Select(m => new DashboardActivityResponseDto
        {
            Id = m.Id.ToString(),
            Type = "message",
            Title = "New Message from " + m.Name,
            Description = m.Subject,
            Time = DateHelper.GetTimeAgo(m.CreatedAt),
            Icon = "MessageSquare"
        });

        // Merge and sort
        response.RecentActivities = recentBlogs
            .Concat(recentProjects)
            .Concat(recentMessages)
            .Take(5)
            .ToList();

        return response;
    }
}

using BE_Portfolio.DTOs.Project;
using BE_Portfolio.Models.Domain;
using BE_Portfolio.Persistence.Repositories.Interfaces;
using BE_Portfolio.Services.Common;

namespace BE_Portfolio.Services;

public class ProjectService(
    IProjectRepository projectRepo,
    IImageService imageService) : IProjectService
{
    public async Task<IEnumerable<ProjectItemDto>> GetProjectsAsync(ProjectFilterQueryDto filter, CancellationToken ct = default)
    {
        var domainFilter = new ProjectFilter
        {
            HighlightOnly = filter.HighlightOnly,
            Limit = filter.Limit
        };

        var projects = await projectRepo.GetAllAsync(domainFilter, ct);
        
        return projects.Select(p => new ProjectItemDto(
            Id: p.Id.ToString(),
            Slug: p.Slug,
            Title: p.Title,
            Description: p.Description,
            Highlight: p.Highlight,
            Duration: p.Duration,
            TeamSize: p.TeamSize,
            Technologies: p.Technologies,
            Features: p.Features,
            Thumb: $"/api/projects/{p.Id}/image/thumb", // Updated route to new Controller
            Github: p.Github,
            Demo: p.Demo
        ));
    }

    public async Task<ProjectResponseDto?> GetProjectBySlugAsync(string slug, CancellationToken ct = default)
    {
        var p = await projectRepo.GetBySlugAsync(slug, ct);
        if (p is null) return null;

        return new ProjectResponseDto(
            Id: p.Id.ToString(),
            Slug: p.Slug,
            Title: p.Title,
            Description: p.Description,
            Highlight: p.Highlight,
            Duration: p.Duration,
            TeamSize: p.TeamSize,
            Technologies: p.Technologies,
            Features: p.Features,
            Github: p.Github,
            Demo: p.Demo,
            Image: $"/api/projects/{p.Id}/image/full", // Updated route
            Thumb: $"/api/projects/{p.Id}/image/thumb"  // Updated route
        );
    }
}

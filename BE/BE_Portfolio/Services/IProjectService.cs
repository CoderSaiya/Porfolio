using BE_Portfolio.DTOs.Project;

namespace BE_Portfolio.Services;

public interface IProjectService
{
    Task<IEnumerable<ProjectItemDto>> GetProjectsAsync(ProjectFilterQueryDto filter, CancellationToken ct = default);
    Task<ProjectResponseDto?> GetProjectBySlugAsync(string slug, CancellationToken ct = default);
}

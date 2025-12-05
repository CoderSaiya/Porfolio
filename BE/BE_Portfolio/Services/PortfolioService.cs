using BE_Portfolio.DTOs;
using BE_Portfolio.Models.Documents;
using BE_Portfolio.Models.ValueObjects;
using BE_Portfolio.Persistence.Repositories.Interfaces;
using MongoDB.Bson;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Webp;
using Image = SixLabors.ImageSharp.Image;
using Size = SixLabors.ImageSharp.Size;

namespace BE_Portfolio.Services;

public class PortfolioService(
    IProfileRepository profiles,
    IProjectRepository projects,
    ISkillRepository skills,
    IImageRepository images)
{
    public Task<Profile?> GetProfileAsync(CancellationToken ct = default)
        => profiles.GetAsync(ct);

    public async Task<IEnumerable<SkillCategory>> GetSkillsAsync(CancellationToken ct = default)
        => await skills.GetAllAsync(ct);

    public async Task<IEnumerable<ProjectListItemDto>> GetProjectsAsync(bool? highlightOnly, int? limit, CancellationToken ct = default)
        => (await projects.GetAllAsync(highlightOnly, limit, ct)).Select(p => new ProjectListItemDto(
            Id:  p.Id.ToString(),
            Slug: p.Slug,
            Title: p.Title,
            Description: p.Description,
            Highlight: p.Highlight,
            Duration: p.Duration,
            TeamSize: p.TeamSize,
            Technologies: p.Technologies,
            Features: p.Features,
            Thumb: ThumbRoute(p.Id.ToString()),
            Github: p.Github,
            Demo: p.Demo
        )).ToList();

    public async Task<ProjectDetailDto?> GetProjectBySlugAsync(string slug, CancellationToken ct = default)
    {
        var p = await projects.GetBySlugAsync(slug, ct);
        if (p is null)
            return null;

        return new ProjectDetailDto(
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
            Image: FullRoute(p.Id.ToString()),
            Thumb: ThumbRoute(p.Id.ToString())
        );
    }

    // Ảnh: lấy base64 (thumb/full)
    public async Task<string?> GetProjectImageDataUrlAsync(ObjectId projectId, ImageVariant variant, CancellationToken ct = default)
    {
        var res = await images.GetBase64Async(ImageOwnerType.Project, projectId, variant, ct);
        if (res is null) return null;
        var (base64, mime) = res.Value;
        return $"data:{mime};base64,{base64}";
    }

    public async Task SetProjectImageAsync(ObjectId projectId, IFormFile file, CancellationToken ct = default)
    {
        if (file is null || file.Length == 0)
            throw new ArgumentException("Tệp ảnh trống.", nameof(file));

        // Validate MIME cơ bản
        var allowed = new[] { "image/jpeg", "image/png", "image/webp", "image/avif" };
        if (!allowed.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException($"Định dạng ảnh không hỗ trợ: {file.ContentType}");

        // Đọc & xử lý bằng ImageSharp
        await using var input = file.OpenReadStream();

        using var image = await Image.LoadAsync(input, ct);
        image.Mutate(x => x.AutoOrient()); // sửa EXIF orientation

        // FULL: max width 1920 (giữ tỉ lệ)
        using var fullImg = image.Clone(ctx => ctx.Resize(new ResizeOptions
        {
            Mode = ResizeMode.Max,
            Size = new Size(1920, 1920) // theo chiều lớn
        }));

        using var fullMs = new MemoryStream();
        await fullImg.SaveAsync(fullMs, new WebpEncoder { Quality = 80 }, ct);
        var fullBytes = fullMs.ToArray();
        await images.SetImageAsync(
            ImageOwnerType.Project, projectId, ImageVariant.Full,
            "image/webp", fullBytes, fullImg.Width, fullImg.Height, ct);

        // THUMB: max width 480
        using var thumbImg = image.Clone(x => x.Resize(new ResizeOptions
        {
            Mode = ResizeMode.Max,
            Size = new Size(720, 720),
            Sampler = KnownResamplers.Lanczos3,
            Compand = true
        }));
        using var thumbMs = new MemoryStream();
        await thumbImg.SaveAsync(thumbMs, new WebpEncoder { Quality = 82 }, ct);
        await images.SetImageAsync(
            ImageOwnerType.Project, projectId, ImageVariant.Thumb, "image/webp",
            thumbMs.ToArray(), thumbImg.Width, thumbImg.Height, ct);
    }
    
    public async Task<(byte[] bytes, string mime)?> GetProjectImageBytesAsync(ObjectId projectId, ImageVariant variant, CancellationToken ct = default)
    {
        // gọi base64 rồi decode
        var res = await images.GetBase64Async(ImageOwnerType.Project, projectId, variant, ct);
        if (res is null) return null;
        var (base64, mime) = res.Value;
        return (Convert.FromBase64String(base64), mime);
    }

    public Task DeleteProjectImageAsync(ObjectId projectId, ImageVariant variant, CancellationToken ct = default)
        => images.DeleteAsync(ImageOwnerType.Project, projectId, variant, ct);
    
    private static string ThumbRoute(string id) => $"/api/portfolio/projects/{id}/image/thumb";
    private static string FullRoute (string id) => $"/api/portfolio/projects/{id}/image/full";
    private static string ProfileImageRoute(string variant) => $"/api/portfolio/profile/image/{variant}";

    // --- Profile Image Logic ---

    public async Task SetProfileImageAsync(ObjectId profileId, IFormFile file, CancellationToken ct = default)
    {
        if (file is null || file.Length == 0)
            throw new ArgumentException("Tệp ảnh trống.", nameof(file));

        var allowed = new[] { "image/jpeg", "image/png", "image/webp", "image/avif" };
        if (!allowed.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException($"Định dạng ảnh không hỗ trợ: {file.ContentType}");

        await using var input = file.OpenReadStream();
        using var image = await Image.LoadAsync(input, ct);
        image.Mutate(x => x.AutoOrient());

        // FULL: max 1024x1024 for profile
        using var fullImg = image.Clone(ctx => ctx.Resize(new ResizeOptions
        {
            Mode = ResizeMode.Max,
            Size = new Size(1024, 1024)
        }));

        using var fullMs = new MemoryStream();
        await fullImg.SaveAsync(fullMs, new WebpEncoder { Quality = 80 }, ct);
        var fullBytes = fullMs.ToArray();
        await images.SetImageAsync(
            ImageOwnerType.Profile, profileId, ImageVariant.Full,
            "image/webp", fullBytes, fullImg.Width, fullImg.Height, ct);

        // THUMB: max 256x256
        using var thumbImg = image.Clone(x => x.Resize(new ResizeOptions
        {
            Mode = ResizeMode.Max,
            Size = new Size(256, 256),
            Sampler = KnownResamplers.Lanczos3,
            Compand = true
        }));
        using var thumbMs = new MemoryStream();
        await thumbImg.SaveAsync(thumbMs, new WebpEncoder { Quality = 82 }, ct);
        await images.SetImageAsync(
            ImageOwnerType.Profile, profileId, ImageVariant.Thumb, "image/webp",
            thumbMs.ToArray(), thumbImg.Width, thumbImg.Height, ct);
            
        // Update Profile AvatarUrl
        var profile = await profiles.GetAsync(ct);
        if (profile != null)
        {
            profile.AvatarUrl = ProfileImageRoute("full");
            await profiles.UpsertAsync(profile, ct);
        }
    }

    public async Task<(byte[] bytes, string mime)?> GetProfileImageBytesAsync(ImageVariant variant, CancellationToken ct = default)
    {
        var profile = await profiles.GetAsync(ct);
        if (profile == null) return null;

        var res = await images.GetBase64Async(ImageOwnerType.Profile, profile.Id, variant, ct);
        if (res is null) return null;
        var (base64, mime) = res.Value;
        return (Convert.FromBase64String(base64), mime);
    }
}
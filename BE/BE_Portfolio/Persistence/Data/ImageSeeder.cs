using BE_Portfolio.Models.ValueObjects;
using BE_Portfolio.Persistence.Repositories.Interfaces;
using MongoDB.Driver;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace BE_Portfolio.Persistence.Data;

public static class ImageSeeder
{
    private static readonly string[] Exts = [".jpg", ".jpeg", ".png", ".webp", ".avif"];

    public static async Task SeedProjectImagesAsync(IServiceProvider sp, CancellationToken ct = default)
    {
        var db = sp.GetRequiredService<IMongoDbContext>();
        var projects = db.Projects;
        var imagesRepo = sp.GetRequiredService<IImageRepository>();
        var env = sp.GetRequiredService<IHostEnvironment>();

        var folder = Path.Combine(env.ContentRootPath, "Assets", "Seed", "Projects");
        Console.WriteLine($"Seeding projects from {folder}");
        if (!Directory.Exists(folder)) return;

        // lấy toàn bộ project hiện có để map theo slug
        var all = await projects.Find(_ => true).ToListAsync(ct);
        Console.WriteLine($"Found {all.Count} projects");
        foreach (var p in all)
        {
            // tìm file theo slug.* trong thư mục
            var path = Exts
                .Select(ext => Path.Combine(folder, $"{p.Slug}{ext}"))
                .FirstOrDefault(File.Exists);

            if (path is null) continue; // không có ảnh thì bỏ qua

            // tạo cả full & thumb -> WebP
            await using var input = File.OpenRead(path);
            using var img = await Image.LoadAsync(input, ct);
            img.Mutate(x => x.AutoOrient());

            // Full (<=1920)
            using var fullImg = img.Clone(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max, Size = new Size(1920, 1920)
            }));
            using var fullMs = new MemoryStream();
            await fullImg.SaveAsync(fullMs, new WebpEncoder { Quality = 80 }, ct);
            await imagesRepo.SetImageAsync(
                ImageOwnerType.Project, p.Id, ImageVariant.Full, "image/webp",
                fullMs.ToArray(), fullImg.Width, fullImg.Height, ct);

            // Thumb (<=480)
            using var thumbImg = img.Clone(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(720, 720),
                Sampler = KnownResamplers.Lanczos3,
                Compand = true
            }));
            using var thumbMs = new MemoryStream();
            await thumbImg.SaveAsync(thumbMs, new WebpEncoder { Quality = 82 }, ct);
            await imagesRepo.SetImageAsync(
                ImageOwnerType.Project, p.Id, ImageVariant.Thumb, "image/webp",
                thumbMs.ToArray(), thumbImg.Width, thumbImg.Height, ct);
        }
    }
}
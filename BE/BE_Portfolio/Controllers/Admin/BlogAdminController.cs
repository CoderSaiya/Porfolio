using BE_Portfolio.DTOs.Blog;
using BE_Portfolio.Models.Documents;
using BE_Portfolio.Persistence.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace BE_Portfolio.Controllers.Admin;

[ApiController]
[Route("api/admin/blogs")]
// [Authorize(Roles = "Admin")]
public class BlogAdminController : ControllerBase
{
    private readonly IBlogPostRepository _postRepo;
    private readonly IBlogCategoryRepository _categoryRepo;

    public BlogAdminController(IBlogPostRepository postRepo, IBlogCategoryRepository categoryRepo)
    {
        _postRepo = postRepo;
        _categoryRepo = categoryRepo;
    }

    [HttpGet]
    public async Task<IActionResult> GetBlogs([FromQuery] BlogFilterDTO filter, CancellationToken ct)
    {
        var posts = await _postRepo.GetAllAsync(filter, ct);
        var total = await _postRepo.CountAsync(filter, ct);

        var dtos = posts.Select(p => new BlogResponseDto(
            p.Id.ToString(),
            p.Title,
            p.Slug,
            p.Summary,
            p.Content,
            p.FeaturedImage ?? "undefined",
            p.CategoryId.ToString() ?? "undefined",
            p.Tags,
            p.Published,
            p.PublishedAt,
            p.ViewCount,
            p.CreatedAt,
            p.UpdateDate));

        return Ok(new { data = dtos, total });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBlog(string id, CancellationToken ct)
    {
        var post = await _postRepo.GetByIdAsync(id, ct);
        if (post == null)
            return NotFound(new { message = "Blog post not found" });

        var dto = new BlogResponseDto(
            post.Id.ToString(),
            post.Title,
            post.Slug,
            post.Summary,
            post.Content,
            post.FeaturedImage ?? "undefined",
            post.CategoryId.ToString() ?? "undefined",
            post.Tags,
            post.Published,
            post.PublishedAt,
            post.ViewCount,
            post.CreatedAt,
            post.UpdateDate
        );
        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBlog([FromForm] CreateBlogPostDTO dto, IFormFile? file, CancellationToken ct)
    {
        ObjectId? categoryId = null;
        if (!string.IsNullOrEmpty(dto.CategoryId) && ObjectId.TryParse(dto.CategoryId, out var catId))
        {
            categoryId = catId;
        }

        string? featuredImageData = null;
        if (file != null && file.Length > 0)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms, ct);
            var base64 = Convert.ToBase64String(ms.ToArray());
            featuredImageData = $"data:{file.ContentType};base64,{base64}";
        }

        var post = new BlogPost
        {
            Title = dto.Title,
            Slug = dto.Slug,
            Summary = dto.Summary,
            Content = dto.Content,
            FeaturedImage = featuredImageData,
            CategoryId = categoryId,
            Tags = dto.Tags ?? new List<string>(),
            Published = dto.Published
        };

        await _postRepo.CreateAsync(post, ct);

        return CreatedAtAction(nameof(GetBlog), new { id = post.Id.ToString() }, post);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBlog(
        string id,
        [FromForm] UpdateBlogPostDTO dto,
        IFormFile? file,
        CancellationToken ct)
    {
        var existing = await _postRepo.GetByIdAsync(id, ct);
        if (existing == null)
            return NotFound(new { message = "Blog post not found" });

        if (!string.IsNullOrWhiteSpace(dto.Title)) existing.Title = dto.Title;
        if (!string.IsNullOrWhiteSpace(dto.Slug)) existing.Slug = dto.Slug;
        if (!string.IsNullOrWhiteSpace(dto.Summary)) existing.Summary = dto.Summary;
        if (!string.IsNullOrWhiteSpace(dto.Content)) existing.Content = dto.Content;
        if (dto.Tags != null) existing.Tags = dto.Tags;
        if (dto.Published.HasValue) existing.Published = dto.Published.Value;

        if (!string.IsNullOrEmpty(dto.CategoryId) &&
            ObjectId.TryParse(dto.CategoryId, out var catId))
        {
            existing.CategoryId = catId;
        }
        
        if (file != null && file.Length > 0)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms, ct);
            var base64 = Convert.ToBase64String(ms.ToArray());
            existing.FeaturedImage = $"data:{file.ContentType};base64,{base64}";
        }

        await _postRepo.UpdateAsync(existing, ct);

        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBlog(string id, CancellationToken ct)
    {
        var existing = await _postRepo.GetByIdAsync(id, ct);
        if (existing == null)
            return NotFound(new { message = "Blog post not found" });

        await _postRepo.DeleteAsync(id, ct);

        return Ok(new { message = "Blog post deleted successfully" });
    }

    // Categories management
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(CancellationToken ct)
    {
        var categories = await _categoryRepo.GetAllAsync(ct);
        return Ok(categories);
    }

    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory([FromBody] CreateBlogCategoryDTO dto, CancellationToken ct)
    {
        var category = new BlogCategory
        {
            Name = dto.Name,
            Slug = dto.Slug,
            Description = dto.Description,
            Order = dto.Order
        };

        await _categoryRepo.CreateAsync(category, ct);

        return CreatedAtAction(nameof(GetCategories), new { id = category.Id.ToString() }, category);
    }

    [HttpPut("categories/{id}")]
    public async Task<IActionResult> UpdateCategory(string id, [FromBody] CreateBlogCategoryDTO dto, CancellationToken ct)
    {
        var existing = await _categoryRepo.GetByIdAsync(id, ct);
        if (existing == null)
            return NotFound(new { message = "Category not found" });

        existing.Name = dto.Name;
        existing.Slug = dto.Slug;
        existing.Description = dto.Description;
        existing.Order = dto.Order;

        await _categoryRepo.UpdateAsync(existing, ct);

        return Ok(existing);
    }

    [HttpDelete("categories/{id}")]
    public async Task<IActionResult> DeleteCategory(string id, CancellationToken ct)
    {
        var existing = await _categoryRepo.GetByIdAsync(id, ct);
        if (existing == null)
            return NotFound(new { message = "Category not found" });

        await _categoryRepo.DeleteAsync(id, ct);

        return Ok(new { message = "Category deleted successfully" });
    }

    [HttpPost("{id}/image")]
    public async Task<IActionResult> UploadImage(string id, IFormFile file, CancellationToken ct)
    {
        var blog = await _postRepo.GetByIdAsync(id, ct);
        if (blog == null)
            return NotFound(new { message = "Blog post not found" });

        if (file == null || file.Length == 0)
            return BadRequest(new { message = "No file uploaded" });

        // Convert to base64
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms, ct);
        var base64 = Convert.ToBase64String(ms.ToArray());
        var dataUrl = $"data:{file.ContentType};base64,{base64}";

        // Update blog featured image
        blog.FeaturedImage = dataUrl;
        await _postRepo.UpdateAsync(blog, ct);

        return Ok(new { message = "Image uploaded successfully", imageUrl = dataUrl });
    }
}

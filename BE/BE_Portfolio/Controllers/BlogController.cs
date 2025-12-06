using BE_Portfolio.DTOs.Blog;
using BE_Portfolio.Persistence.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BE_Portfolio.Controllers;

[ApiController]
[Route("api/blog")]
public class BlogController : ControllerBase
{
    private readonly IBlogPostRepository _postRepo;
    private readonly IBlogCategoryRepository _categoryRepo;

    public BlogController(IBlogPostRepository postRepo, IBlogCategoryRepository categoryRepo)
    {
        _postRepo = postRepo;
        _categoryRepo = categoryRepo;
    }

    [HttpGet("posts")]
    public async Task<IActionResult> GetPosts([FromQuery] BlogFilterDTO filter, CancellationToken ct)
    {
        // Force published only for public API
        filter = filter with { Published = true };

        var posts = await _postRepo.GetAllAsync(filter, ct);
        var total = await _postRepo.CountAsync(filter, ct);

        return Ok(new { data = posts, total });
    }

    [HttpGet("posts/{slug}")]
    public async Task<IActionResult> GetPost(string slug, CancellationToken ct)
    {
        var post = await _postRepo.GetBySlugAsync(slug, ct);
        
        if (post == null || !post.Published)
            return NotFound(new { message = "Blog post not found" });

        // Increment view count
        await _postRepo.IncrementViewCountAsync(post.Id.ToString(), ct);

        return Ok(post);
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(CancellationToken ct)
    {
        var categories = await _categoryRepo.GetAllAsync(ct);
        return Ok(categories);
    }
}

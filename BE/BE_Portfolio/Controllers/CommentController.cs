using System.Security.Claims;
using BE_Portfolio.DTOs.Comment;
using BE_Portfolio.Services.Comment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE_Portfolio.Controllers;

[ApiController]
[Route("api/comments")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet("{blogId}")]
    public async Task<IActionResult> GetComments(string blogId, CancellationToken ct)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var comments = await _commentService.GetCommentsByBlogIdAsync(blogId, userId, ct);
        return Ok(comments);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddComment([FromBody] CreateCommentRequestDTO request, CancellationToken ct)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var comment = await _commentService.AddCommentAsync(userId, request, ct);
        return Ok(comment);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteComment(string id, CancellationToken ct)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        var isAdmin = role == "Admin";

        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _commentService.DeleteCommentAsync(id, userId, isAdmin, ct);
        if (!result) return BadRequest(new { message = "Could not delete comment (not found or permission denied)" });

        return Ok(new { message = "Comment deleted" });
    }
}

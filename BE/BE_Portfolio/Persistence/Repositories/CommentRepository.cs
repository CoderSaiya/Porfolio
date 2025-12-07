using BE_Portfolio.Models.Documents;
using BE_Portfolio.Persistence.Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BE_Portfolio.Persistence.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly IMongoDbContext _context;

    public CommentRepository(IMongoDbContext context)
    {
        _context = context;
    }

    public async Task<List<Comment>> GetByBlogIdAsync(string blogId, CancellationToken ct = default)
    {
        var filter = Builders<Comment>.Filter.Eq(c => c.BlogPostId, blogId) & 
                     Builders<Comment>.Filter.Eq(c => c.IsDeleted, false);
        return await _context.Comments.Find(filter).ToListAsync(ct);
    }

    public async Task<Comment?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        if (!ObjectId.TryParse(id, out var objId)) return null;
        return await _context.Comments.Find(c => c.Id == objId).FirstOrDefaultAsync(ct);
    }

    public async Task CreateAsync(Comment comment, CancellationToken ct = default)
    {
        await _context.Comments.InsertOneAsync(comment, cancellationToken: ct);
    }

    public async Task UpdateAsync(Comment comment, CancellationToken ct = default)
    {
        await _context.Comments.ReplaceOneAsync(c => c.Id == comment.Id, comment, cancellationToken: ct);
    }

    public async Task DeleteAsync(string id, CancellationToken ct = default)
    {
        if (!ObjectId.TryParse(id, out var objId)) return;
        await _context.Comments.DeleteOneAsync(c => c.Id == objId, cancellationToken: ct);
    }
}

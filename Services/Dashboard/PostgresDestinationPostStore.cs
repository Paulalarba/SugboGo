using Microsoft.EntityFrameworkCore;
using SugboGo.Data;
using SugboGo.Models;

namespace SugboGo.Services.Dashboard;

public sealed class PostgresDestinationPostStore : IDestinationPostStore
{
    private readonly SugboGoDbContext _dbContext;

    public PostgresDestinationPostStore(SugboGoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<DestinationPost>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.DestinationPosts
            .Include(p => p.CommentsList)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<DestinationPost>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DestinationPosts
            .Include(p => p.CommentsList)
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<DestinationPost> CreateAsync(DestinationPost post, CancellationToken cancellationToken = default)
    {
        _dbContext.DestinationPosts.Add(post);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return post;
    }

    public async Task<DestinationPost?> IncrementLikesAsync(string postId, CancellationToken cancellationToken = default)
    {
        var post = await _dbContext.DestinationPosts
            .Include(p => p.CommentsList)
            .FirstOrDefaultAsync(p => p.Id == postId, cancellationToken);

        if (post is null) return null;

        post.Likes++;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return post;
    }

    public async Task<DestinationPost?> AddCommentAsync(string postId, PostComment comment, CancellationToken cancellationToken = default)
    {
        var post = await _dbContext.DestinationPosts
            .Include(p => p.CommentsList)
            .FirstOrDefaultAsync(p => p.Id == postId, cancellationToken);

        if (post is null) return null;

        post.CommentsList.Add(comment);
        post.Comments = post.CommentsList.Count;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return post;
    }
}

using System.Text.Json;
using SugboGo.Models;

namespace SugboGo.Services.Dashboard;

public sealed class LocalJsonDestinationPostStore : IDestinationPostStore
{
    private static readonly SemaphoreSlim FileLock = new(1, 1);
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web) { WriteIndented = true };

    public LocalJsonDestinationPostStore(IWebHostEnvironment environment)
    {
        _filePath = Path.Combine(environment.ContentRootPath, "App_Data", "destination-posts.json");
    }

    public async Task<List<DestinationPost>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await FileLock.WaitAsync(cancellationToken);
        try
        {
            var posts = await ReadPostsAsync(cancellationToken);
            return posts.OrderByDescending(post => post.CreatedAt).ToList();
        }
        finally
        {
            FileLock.Release();
        }
    }

    public async Task<List<DestinationPost>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        await FileLock.WaitAsync(cancellationToken);
        try
        {
            var posts = await ReadPostsAsync(cancellationToken);
            return posts
                .Where(post => post.UserId == userId)
                .OrderByDescending(post => post.CreatedAt)
                .ToList();
        }
        finally
        {
            FileLock.Release();
        }
    }

    public async Task<DestinationPost> CreateAsync(DestinationPost post, CancellationToken cancellationToken = default)
    {
        post.Id = string.IsNullOrWhiteSpace(post.Id) ? Guid.NewGuid().ToString("N") : post.Id;
        post.CreatedAt = post.CreatedAt == default ? DateTimeOffset.UtcNow : post.CreatedAt;

        await FileLock.WaitAsync(cancellationToken);
        try
        {
            var posts = await ReadPostsAsync(cancellationToken);
            posts.Add(post);
            await WritePostsAsync(posts, cancellationToken);
            return post;
        }
        finally
        {
            FileLock.Release();
        }
    }

    public async Task<DestinationPost?> IncrementLikesAsync(string postId, CancellationToken cancellationToken = default)
    {
        await FileLock.WaitAsync(cancellationToken);
        try
        {
            var posts = await ReadPostsAsync(cancellationToken);
            var post = posts.FirstOrDefault(post => post.Id == postId);

            if (post is null)
            {
                return null;
            }

            post.Likes++;
            await WritePostsAsync(posts, cancellationToken);
            return post;
        }
        finally
        {
            FileLock.Release();
        }
    }

    public async Task<DestinationPost?> AddCommentAsync(string postId, PostComment comment, CancellationToken cancellationToken = default)
    {
        comment.Id = string.IsNullOrWhiteSpace(comment.Id) ? Guid.NewGuid().ToString("N") : comment.Id;
        comment.CreatedAt = comment.CreatedAt == default ? DateTimeOffset.UtcNow : comment.CreatedAt;

        await FileLock.WaitAsync(cancellationToken);
        try
        {
            var posts = await ReadPostsAsync(cancellationToken);
            var post = posts.FirstOrDefault(p => p.Id == postId);

            if (post is null)
            {
                return null;
            }

            post.CommentsList ??= [];
            post.CommentsList.Add(comment);
            post.Comments = post.CommentsList.Count;

            await WritePostsAsync(posts, cancellationToken);
            return post;
        }
        finally
        {
            FileLock.Release();
        }
    }

    private async Task<List<DestinationPost>> ReadPostsAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_filePath))
        {
            return [];
        }

        await using var stream = File.OpenRead(_filePath);
        return await JsonSerializer.DeserializeAsync<List<DestinationPost>>(stream, _jsonOptions, cancellationToken) ?? [];
    }

    private async Task WritePostsAsync(List<DestinationPost> posts, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
        await using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, posts, _jsonOptions, cancellationToken);
    }
}

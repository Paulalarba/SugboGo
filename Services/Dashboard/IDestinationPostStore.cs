using SugboGo.Models;

namespace SugboGo.Services.Dashboard;

public interface IDestinationPostStore
{
    Task<List<DestinationPost>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<DestinationPost>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<DestinationPost> CreateAsync(DestinationPost post, CancellationToken cancellationToken = default);
    Task<DestinationPost?> IncrementLikesAsync(string postId, CancellationToken cancellationToken = default);
    Task<DestinationPost?> AddCommentAsync(string postId, PostComment comment, CancellationToken cancellationToken = default);
}

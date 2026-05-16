using SugboGo.Models;

namespace SugboGo.Services.Dashboard;

public interface IUserSavedGemStore
{
    Task<List<SavedGem>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<SavedGem> SaveGemAsync(SavedGem gem, CancellationToken cancellationToken = default);
    Task<bool> RemoveGemAsync(string userId, string gemId, CancellationToken cancellationToken = default);
}

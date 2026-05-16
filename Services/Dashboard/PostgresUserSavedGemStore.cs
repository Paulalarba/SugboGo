using Microsoft.EntityFrameworkCore;
using SugboGo.Data;
using SugboGo.Models;

namespace SugboGo.Services.Dashboard;

public sealed class PostgresUserSavedGemStore : IUserSavedGemStore
{
    private readonly SugboGoDbContext _dbContext;

    public PostgresUserSavedGemStore(SugboGoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<SavedGem>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.SavedGems
            .Where(g => g.UserId == userId)
            .OrderByDescending(g => g.SavedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<SavedGem> SaveGemAsync(SavedGem gem, CancellationToken cancellationToken = default)
    {
        // Avoid duplicates by title for the same user
        var exists = await _dbContext.SavedGems
            .AnyAsync(g => g.UserId == gem.UserId && g.Title == gem.Title, cancellationToken);

        if (!exists)
        {
            _dbContext.SavedGems.Add(gem);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return gem;
    }

    public async Task<bool> RemoveGemAsync(string userId, string gemId, CancellationToken cancellationToken = default)
    {
        var gem = await _dbContext.SavedGems
            .FirstOrDefaultAsync(g => g.UserId == userId && g.Id == gemId, cancellationToken);

        if (gem is null) return false;

        _dbContext.SavedGems.Remove(gem);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}

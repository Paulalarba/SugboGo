using Microsoft.EntityFrameworkCore;
using SugboGo.Data;
using SugboGo.Models;

namespace SugboGo.Services.Travel;

public sealed class PostgresTravelPreferenceStore : ITravelPreferenceStore
{
    private readonly SugboGoDbContext _dbContext;

    public PostgresTravelPreferenceStore(SugboGoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<TravelPreferenceRecord>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.TravelPreferences
            .OrderByDescending(p => p.UpdatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<TravelPreferenceRecord?> FindLatestByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }

        return await _dbContext.TravelPreferences
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.UpdatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TravelPreferenceRecord> SaveAsync(TravelPreferenceRecord preference, CancellationToken cancellationToken = default)
    {
        preference.UpdatedAt = DateTimeOffset.UtcNow;

        var existing = await _dbContext.TravelPreferences
            .FirstOrDefaultAsync(p => p.UserId == preference.UserId, cancellationToken);

        if (existing is not null)
        {
            // Update existing record
            existing.Email = preference.Email;
            existing.PlaceInterests = preference.PlaceInterests;
            existing.ActivityInterests = preference.ActivityInterests;
            existing.AdventureLevel = preference.AdventureLevel;
            existing.TravelPace = preference.TravelPace;
            existing.BudgetRange = preference.BudgetRange;
            existing.Notes = preference.Notes;
            existing.UpdatedAt = preference.UpdatedAt;
        }
        else
        {
            // Add new record
            _dbContext.TravelPreferences.Add(preference);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return preference;
    }
}

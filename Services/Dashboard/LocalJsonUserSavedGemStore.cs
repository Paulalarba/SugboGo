using System.Text.Json;
using SugboGo.Models;

namespace SugboGo.Services.Dashboard;

public sealed class LocalJsonUserSavedGemStore : IUserSavedGemStore
{
    private static readonly SemaphoreSlim FileLock = new(1, 1);
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web) { WriteIndented = true };

    public LocalJsonUserSavedGemStore(IWebHostEnvironment environment)
    {
        _filePath = Path.Combine(environment.ContentRootPath, "App_Data", "saved-gems.json");
    }

    public async Task<List<SavedGem>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        await FileLock.WaitAsync(cancellationToken);
        try
        {
            var gems = await ReadGemsAsync(cancellationToken);
            return gems
                .Where(gem => gem.UserId == userId)
                .OrderByDescending(gem => gem.SavedAt)
                .ToList();
        }
        finally
        {
            FileLock.Release();
        }
    }

    public async Task<SavedGem> SaveGemAsync(SavedGem gem, CancellationToken cancellationToken = default)
    {
        gem.Id = string.IsNullOrWhiteSpace(gem.Id) ? Guid.NewGuid().ToString("N") : gem.Id;
        gem.SavedAt = gem.SavedAt == default ? DateTimeOffset.UtcNow : gem.SavedAt;

        await FileLock.WaitAsync(cancellationToken);
        try
        {
            var gems = await ReadGemsAsync(cancellationToken);
            
            // Avoid duplicates
            if (!gems.Any(g => g.UserId == gem.UserId && g.Title == gem.Title))
            {
                gems.Add(gem);
                await WriteGemsAsync(gems, cancellationToken);
            }
            
            return gem;
        }
        finally
        {
            FileLock.Release();
        }
    }

    public async Task<bool> RemoveGemAsync(string userId, string gemId, CancellationToken cancellationToken = default)
    {
        await FileLock.WaitAsync(cancellationToken);
        try
        {
            var gems = await ReadGemsAsync(cancellationToken);
            var index = gems.FindIndex(g => g.UserId == userId && g.Id == gemId);

            if (index < 0)
            {
                return false;
            }

            gems.RemoveAt(index);
            await WriteGemsAsync(gems, cancellationToken);
            return true;
        }
        finally
        {
            FileLock.Release();
        }
    }

    private async Task<List<SavedGem>> ReadGemsAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_filePath))
        {
            return [];
        }

        await using var stream = File.OpenRead(_filePath);
        return await JsonSerializer.DeserializeAsync<List<SavedGem>>(stream, _jsonOptions, cancellationToken) ?? [];
    }

    private async Task WriteGemsAsync(List<SavedGem> gems, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
        await using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, gems, _jsonOptions, cancellationToken);
    }
}

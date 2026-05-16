using System.Text.Json;
using SugboGo.Models;

namespace SugboGo.Services.Travel;

public sealed class LocalJsonTravelPreferenceStore : ITravelPreferenceStore
{
    private static readonly SemaphoreSlim FileLock = new(1, 1);
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web) { WriteIndented = true };

    public LocalJsonTravelPreferenceStore(IWebHostEnvironment environment)
    {
        _filePath = Path.Combine(environment.ContentRootPath, "App_Data", "travel-preferences.json");
    }

    public async Task<List<TravelPreferenceRecord>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await FileLock.WaitAsync(cancellationToken);
        try
        {
            var preferences = await ReadPreferencesAsync(cancellationToken);
            return preferences.OrderByDescending(preference => preference.UpdatedAt).ToList();
        }
        finally
        {
            FileLock.Release();
        }
    }

    public async Task<TravelPreferenceRecord?> FindLatestByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        await FileLock.WaitAsync(cancellationToken);
        try
        {
            var preferences = await ReadPreferencesAsync(cancellationToken);
            return preferences
                .Where(preference => preference.UserId == userId)
                .OrderByDescending(preference => preference.UpdatedAt)
                .FirstOrDefault();
        }
        finally
        {
            FileLock.Release();
        }
    }

    public async Task<TravelPreferenceRecord> SaveAsync(TravelPreferenceRecord preference, CancellationToken cancellationToken = default)
    {
        preference.UpdatedAt = DateTimeOffset.UtcNow;

        await FileLock.WaitAsync(cancellationToken);
        try
        {
            var preferences = await ReadPreferencesAsync(cancellationToken);
            var existing = preferences.FindIndex(item => item.UserId == preference.UserId);

            if (existing >= 0)
            {
                preference.Id = preferences[existing].Id;
                preference.CreatedAt = preferences[existing].CreatedAt;
                preferences[existing] = preference;
            }
            else
            {
                preferences.Add(preference);
            }

            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
            await using var stream = File.Create(_filePath);
            await JsonSerializer.SerializeAsync(stream, preferences, _jsonOptions, cancellationToken);

            return preference;
        }
        finally
        {
            FileLock.Release();
        }
    }

    private async Task<List<TravelPreferenceRecord>> ReadPreferencesAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_filePath))
        {
            return [];
        }

        await using var stream = File.OpenRead(_filePath);
        return await JsonSerializer.DeserializeAsync<List<TravelPreferenceRecord>>(stream, _jsonOptions, cancellationToken) ?? [];
    }
}

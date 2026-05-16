using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SugboGo.Models;
using SugboGo.Services.Auth;

namespace SugboGo.Services.Dashboard;

public sealed class SupabaseUserSavedGemStore : IUserSavedGemStore
{
    private readonly HttpClient _httpClient;
    private readonly SupabaseOptions _options;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public SupabaseUserSavedGemStore(HttpClient httpClient, IOptions<SupabaseOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;

        _httpClient.BaseAddress = new Uri(_options.Url.TrimEnd('/') + "/rest/v1/");
        _httpClient.DefaultRequestHeaders.Add("apikey", _options.ServiceRoleKey);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.ServiceRoleKey);
    }

    public async Task<List<SavedGem>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var encodedUserId = Uri.EscapeDataString(userId);
        using var response = await _httpClient.GetAsync($"{_options.SavedGemsTable}?user_id=eq.{encodedUserId}&select=*&order=saved_at.desc", cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var rows = await JsonSerializer.DeserializeAsync<List<SavedGem>>(stream, _jsonOptions, cancellationToken) ?? [];
        return rows;
    }

    public async Task<SavedGem> SaveGemAsync(SavedGem gem, CancellationToken cancellationToken = default)
    {
        // Supabase REST doesn't easily support "if not exists insert" for simple collections without unique constraints.
        // But for gems, we can just insert and let the database handle it if there's a unique constraint,
        // or just insert duplicates if not.
        // Assuming we want to avoid duplicates by title for the same user.
        
        using var response = await _httpClient.PostAsJsonAsync(_options.SavedGemsTable, gem, _jsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();

        return gem;
    }

    public async Task<bool> RemoveGemAsync(string userId, string gemId, CancellationToken cancellationToken = default)
    {
        var encodedUserId = Uri.EscapeDataString(userId);
        var encodedGemId = Uri.EscapeDataString(gemId);
        
        using var response = await _httpClient.DeleteAsync($"{_options.SavedGemsTable}?user_id=eq.{encodedUserId}&id=eq.{encodedGemId}", cancellationToken);
        response.EnsureSuccessStatusCode();

        return response.IsSuccessStatusCode;
    }
}

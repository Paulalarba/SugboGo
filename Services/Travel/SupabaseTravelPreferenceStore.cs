using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SugboGo.Models;
using SugboGo.Services.Auth;

namespace SugboGo.Services.Travel;

public sealed class SupabaseTravelPreferenceStore : ITravelPreferenceStore
{
    private readonly HttpClient _httpClient;
    private readonly SupabaseOptions _options;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public SupabaseTravelPreferenceStore(HttpClient httpClient, IOptions<SupabaseOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;

        _httpClient.BaseAddress = new Uri(_options.Url.TrimEnd('/') + "/rest/v1/");
        _httpClient.DefaultRequestHeaders.Add("apikey", _options.ServiceRoleKey);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.ServiceRoleKey);
    }

    public async Task<List<TravelPreferenceRecord>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync($"{_options.PreferencesTable}?select=*&order=updated_at.desc", cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var rows = await JsonSerializer.DeserializeAsync<List<SupabasePreferenceRow>>(stream, _jsonOptions, cancellationToken) ?? [];
        return rows.Select(row => row.ToRecord(_jsonOptions)).ToList();
    }

    public async Task<TravelPreferenceRecord?> FindLatestByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var encodedUserId = Uri.EscapeDataString(userId);
        using var response = await _httpClient.GetAsync($"{_options.PreferencesTable}?user_id=eq.{encodedUserId}&select=*&order=updated_at.desc&limit=1", cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var rows = await JsonSerializer.DeserializeAsync<List<SupabasePreferenceRow>>(stream, _jsonOptions, cancellationToken) ?? [];
        return rows.FirstOrDefault()?.ToRecord(_jsonOptions);
    }

    public async Task<TravelPreferenceRecord> SaveAsync(TravelPreferenceRecord preference, CancellationToken cancellationToken = default)
    {
        preference.UpdatedAt = DateTimeOffset.UtcNow;

        var row = SupabasePreferenceRow.FromRecord(preference, _jsonOptions);
        using var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.PreferencesTable}?on_conflict=user_id")
        {
            Content = JsonContent.Create(new[] { row }, options: _jsonOptions)
        };
        request.Headers.Add("Prefer", "resolution=merge-duplicates");

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return preference;
    }

    private sealed class SupabasePreferenceRow
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("user_id")]
        public string UserId { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("interests_json")]
        public string InterestsJson { get; set; } = "[]";

        [JsonPropertyName("place_interests_json")]
        public string PlaceInterestsJson { get; set; } = "[]";

        [JsonPropertyName("activity_interests_json")]
        public string ActivityInterestsJson { get; set; } = "[]";

        [JsonPropertyName("adventure_level")]
        public int AdventureLevel { get; set; }

        [JsonPropertyName("travel_pace")]
        public string TravelPace { get; set; } = string.Empty;

        [JsonPropertyName("budget_range")]
        public string BudgetRange { get; set; } = string.Empty;

        [JsonPropertyName("notes")]
        public string? Notes { get; set; }

        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        public static SupabasePreferenceRow FromRecord(TravelPreferenceRecord record, JsonSerializerOptions jsonOptions)
        {
            return new SupabasePreferenceRow
            {
                Id = record.Id,
                UserId = record.UserId,
                Email = record.Email,
                InterestsJson = JsonSerializer.Serialize(record.Interests, jsonOptions),
                PlaceInterestsJson = JsonSerializer.Serialize(record.PlaceInterests, jsonOptions),
                ActivityInterestsJson = JsonSerializer.Serialize(record.ActivityInterests, jsonOptions),
                AdventureLevel = record.AdventureLevel,
                TravelPace = record.TravelPace,
                BudgetRange = record.BudgetRange,
                Notes = record.Notes,
                CreatedAt = record.CreatedAt,
                UpdatedAt = record.UpdatedAt
            };
        }

        public TravelPreferenceRecord ToRecord(JsonSerializerOptions jsonOptions)
        {
            var legacyInterests = JsonSerializer.Deserialize<List<string>>(InterestsJson, jsonOptions) ?? [];
            var placeInterests = JsonSerializer.Deserialize<List<string>>(PlaceInterestsJson, jsonOptions) ?? [];
            var activityInterests = JsonSerializer.Deserialize<List<string>>(ActivityInterestsJson, jsonOptions) ?? [];

            return new TravelPreferenceRecord
            {
                Id = Id,
                UserId = UserId,
                Email = Email,
                PlaceInterests = placeInterests.Count == 0 ? legacyInterests : placeInterests,
                ActivityInterests = activityInterests,
                AdventureLevel = AdventureLevel,
                TravelPace = TravelPace,
                BudgetRange = BudgetRange,
                Notes = Notes,
                CreatedAt = CreatedAt,
                UpdatedAt = UpdatedAt
            };
        }
    }
}

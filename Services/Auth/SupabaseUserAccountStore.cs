
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SugboGo.Models;

namespace SugboGo.Services.Auth;

public sealed class SupabaseOptions
{
    public string Url { get; set; } = string.Empty;
    public string ServiceRoleKey { get; set; } = string.Empty;
    public string UsersTable { get; set; } = "sogbogo_users";
    public string PreferencesTable { get; set; } = "sogbogo_travel_preferences";
    public string DestinationPostsTable { get; set; } = "sogbogo_destination_posts";
    public string SavedGemsTable { get; set; } = "sogbogo_saved_gems";
    public string AdminGemsTable { get; set; } = "sogbogo_admin_gems";
    public string ItineraryTemplatesTable { get; set; } = "sogbogo_itinerary_templates";
    public string AdminPartnersTable { get; set; } = "sogbogo_admin_partners";
}

public sealed class SupabaseUserAccountStore : IUserAccountStore
{
    private readonly HttpClient _httpClient;
    private readonly SupabaseOptions _options;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public SupabaseUserAccountStore(HttpClient httpClient, IOptions<SupabaseOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;

        _httpClient.BaseAddress = new Uri(_options.Url.TrimEnd('/') + "/rest/v1/");
        _httpClient.DefaultRequestHeaders.Add("apikey", _options.ServiceRoleKey);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.ServiceRoleKey);
    }

    public async Task<List<UserAccount>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync($"{_options.UsersTable}?select=id,email,full_name,password_hash,role,created_at&order=created_at.desc", cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var rows = await JsonSerializer.DeserializeAsync<List<SupabaseUserRow>>(stream, _jsonOptions, cancellationToken) ?? [];

        return rows.Select(row => new UserAccount
        {
            Id = row.Id,
            Email = row.Email,
            FullName = row.FullName,
            PasswordHash = row.PasswordHash,
            Role = AccountRoles.Normalize(row.Role),
            CreatedAt = row.CreatedAt
        }).ToList();
    }

    public async Task<UserAccount?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var encodedEmail = Uri.EscapeDataString(email.Trim().ToLowerInvariant());
        using var response = await _httpClient.GetAsync($"{_options.UsersTable}?email=eq.{encodedEmail}&select=id,email,full_name,password_hash,role,created_at&limit=1", cancellationToken);

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var rows = await JsonSerializer.DeserializeAsync<List<SupabaseUserRow>>(stream, _jsonOptions, cancellationToken) ?? [];
        var row = rows.FirstOrDefault();

        return row is null
            ? null
            : new UserAccount
            {
                Id = row.Id,
                Email = row.Email,
                FullName = row.FullName,
                PasswordHash = row.PasswordHash,
                Role = AccountRoles.Normalize(row.Role),
                CreatedAt = row.CreatedAt
            };
    }

    public async Task<UserAccount> CreateAsync(UserAccount account, CancellationToken cancellationToken = default)
    {
        account.Email = account.Email.Trim().ToLowerInvariant();

        var row = new SupabaseUserRow
        {
            Id = account.Id,
            Email = account.Email,
            FullName = account.FullName,
            PasswordHash = account.PasswordHash,
            Role = AccountRoles.Normalize(account.Role),
            CreatedAt = account.CreatedAt
        };

        using var response = await _httpClient.PostAsJsonAsync(_options.UsersTable, row, _jsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();

        return account;
    }

    private sealed class SupabaseUserRow
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("full_name")]
        public string FullName { get; set; } = string.Empty;

        [JsonPropertyName("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [JsonPropertyName("role")]
        public string Role { get; set; } = AccountRoles.Client;

        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
    }
}

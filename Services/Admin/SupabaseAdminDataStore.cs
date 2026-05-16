using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SugboGo.Models;
using SugboGo.Services.Auth;

namespace SugboGo.Services.Admin;

public sealed class SupabaseAdminDataStore : IAdminDataStore
{
    private readonly HttpClient _httpClient;
    private readonly SupabaseOptions _options;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public SupabaseAdminDataStore(HttpClient httpClient, IOptions<SupabaseOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;

        _httpClient.BaseAddress = new Uri(_options.Url.TrimEnd('/') + "/rest/v1/");
        _httpClient.DefaultRequestHeaders.Add("apikey", _options.ServiceRoleKey);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.ServiceRoleKey);
    }

    public async Task<List<AdminGem>> GetGemsAsync(CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync($"{_options.AdminGemsTable}?select=*", cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await JsonSerializer.DeserializeAsync<List<AdminGem>>(stream, _jsonOptions, cancellationToken) ?? [];
    }

    public async Task<List<ItineraryTemplate>> GetTemplatesAsync(CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync($"{_options.ItineraryTemplatesTable}?select=*", cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await JsonSerializer.DeserializeAsync<List<ItineraryTemplate>>(stream, _jsonOptions, cancellationToken) ?? [];
    }

    public async Task<List<AdminPartner>> GetPartnersAsync(CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync($"{_options.AdminPartnersTable}?select=*", cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await JsonSerializer.DeserializeAsync<List<AdminPartner>>(stream, _jsonOptions, cancellationToken) ?? [];
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        // Check if gems exist to decide if seeding is needed
        var gems = await GetGemsAsync(cancellationToken);
        if (gems.Count > 0) return;

        var data = new AdminDataContainer
        {
            Gems =
            [
                new() { Name = "Hidden Heritage Cafe", Category = "Cafe", FlashpackerScore = 9, QualityCheckDate = "May 1, 2026", ContactPerson = "Ana Lim", Latitude = 10.2961m, Longitude = 123.8993m, Status = "Active", MapX = 34, MapY = 58 },
                new() { Name = "Private Mountain View", Category = "Viewpoint", FlashpackerScore = 8, QualityCheckDate = "Apr 22, 2026", ContactPerson = "Ramon Uy", Latitude = 10.3713m, Longitude = 123.8830m, Status = "Seasonal", MapX = 47, MapY = 28 },
                new() { Name = "Museo Alley Studio", Category = "Museum", FlashpackerScore = 7, QualityCheckDate = "Apr 18, 2026", ContactPerson = "Tessa Co", Latitude = 10.3002m, Longitude = 123.8967m, Status = "Under Review", MapX = 39, MapY = 54 },
                new() { Name = "Curated Rooftop Dinner", Category = "Dining", FlashpackerScore = 10, QualityCheckDate = "May 3, 2026", ContactPerson = "Marco Dizon", Latitude = 10.3190m, Longitude = 123.9057m, Status = "Active", MapX = 63, MapY = 48 }
            ],
            Templates =
            [
                new() { Name = "Urban Explorer 36h", Vibe = "Urban Explorer", Stops = "Cafe, gallery, rooftop dinner", AvgDuration = "1.5 days" },
                new() { Name = "Heritage Hunter Core", Vibe = "Heritage Hunter", Stops = "Parian walk, museum, ancestral supper", AvgDuration = "1 day" },
                new() { Name = "Soft Mountain Reset", Vibe = "Soft Adventure", Stops = "Design stay, ridge route, garden hideout", AvgDuration = "2 days" }
            ],
            Partners =
            [
                new() { Name = "The Helix House", Type = "Boutique hotel", Contact = "Nina Yu", Commission = "15%", LastAudit = "Apr 30, 2026", Status = "Excellent" },
                new() { Name = "South Ridge Guides", Type = "Local guide", Contact = "Ramon Uy", Commission = "Per route", LastAudit = "Apr 22, 2026", Status = "Watch weather" },
                new() { Name = "Red Door Supper Club", Type = "Dining", Contact = "Marco Dizon", Commission = "12%", LastAudit = "May 3, 2026", Status = "Excellent" }
            ]
        };

        await _httpClient.PostAsJsonAsync(_options.AdminGemsTable, data.Gems, _jsonOptions, cancellationToken);
        await _httpClient.PostAsJsonAsync(_options.ItineraryTemplatesTable, data.Templates, _jsonOptions, cancellationToken);
        await _httpClient.PostAsJsonAsync(_options.AdminPartnersTable, data.Partners, _jsonOptions, cancellationToken);
    }
}

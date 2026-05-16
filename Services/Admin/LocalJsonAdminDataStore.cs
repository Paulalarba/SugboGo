using System.Text.Json;
using SugboGo.Models;

namespace SugboGo.Services.Admin;

public sealed class LocalJsonAdminDataStore : IAdminDataStore
{
    private static readonly SemaphoreSlim FileLock = new(1, 1);
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web) { WriteIndented = true };

    public LocalJsonAdminDataStore(IWebHostEnvironment environment)
    {
        _filePath = Path.Combine(environment.ContentRootPath, "App_Data", "admin-data.json");
    }

    public async Task<List<AdminGem>> GetGemsAsync(CancellationToken cancellationToken = default)
    {
        var data = await ReadDataAsync(cancellationToken);
        return data.Gems;
    }

    public async Task<List<ItineraryTemplate>> GetTemplatesAsync(CancellationToken cancellationToken = default)
    {
        var data = await ReadDataAsync(cancellationToken);
        return data.Templates;
    }

    public async Task<List<AdminPartner>> GetPartnersAsync(CancellationToken cancellationToken = default)
    {
        var data = await ReadDataAsync(cancellationToken);
        return data.Partners;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await FileLock.WaitAsync(cancellationToken);
        try
        {
            if (File.Exists(_filePath))
            {
                return;
            }

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

            await WriteDataAsync(data, cancellationToken);
        }
        finally
        {
            FileLock.Release();
        }
    }

    private async Task<AdminDataContainer> ReadDataAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_filePath))
        {
            await SeedAsync(cancellationToken);
        }

        await FileLock.WaitAsync(cancellationToken);
        try
        {
            await using var stream = File.OpenRead(_filePath);
            return await JsonSerializer.DeserializeAsync<AdminDataContainer>(stream, _jsonOptions, cancellationToken) ?? new AdminDataContainer();
        }
        finally
        {
            FileLock.Release();
        }
    }

    private async Task WriteDataAsync(AdminDataContainer data, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
        await using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, data, _jsonOptions, cancellationToken);
    }
}

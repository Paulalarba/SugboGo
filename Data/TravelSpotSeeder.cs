using Microsoft.EntityFrameworkCore;

namespace SugboGo.Data;

public static class TravelSpotSeeder
{
    public static async Task SeedAsync(SugboGoDbContext dbContext, CancellationToken cancellationToken = default)
    {
        var seedSpots = TravelSpotSeedData.GetTravelSpots();
        var seedIds = seedSpots.Select(seed => seed.Id).ToList();
        var existingById = await dbContext.TravelSpots
            .Where(spot => seedIds.Contains(spot.Id))
            .ToDictionaryAsync(spot => spot.Id, cancellationToken);

        foreach (var seed in seedSpots)
        {
            if (existingById.TryGetValue(seed.Id, out var existing))
            {
                existing.Name = seed.Name;
                existing.Location = seed.Location;
                existing.Description = seed.Description;
                existing.Category = seed.Category;
                existing.Region = seed.Region;
                existing.IsPopular = seed.IsPopular;
                existing.ImageUrl = seed.ImageUrl;
                existing.AdventureLevel = seed.AdventureLevel;
                existing.BasePrice = seed.BasePrice;
                continue;
            }

            dbContext.TravelSpots.Add(seed);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

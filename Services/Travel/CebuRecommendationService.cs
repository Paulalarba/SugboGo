using Microsoft.EntityFrameworkCore;
using SugboGo.Data;
using SugboGo.Models;

namespace SugboGo.Services.Travel;

public interface ICebuRecommendationService
{
    Task<TravelRecommendationsViewModel> BuildRecommendationsAsync(TravelPreferenceRecord preferences);
}

public sealed class CebuRecommendationService : ICebuRecommendationService
{
    private readonly SugboGoDbContext _dbContext;

    public CebuRecommendationService(SugboGoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TravelRecommendationsViewModel> BuildRecommendationsAsync(TravelPreferenceRecord preferences)
    {
        var spots = await _dbContext.TravelSpots.ToListAsync();
        var selected = preferences.Interests.Select(Normalize).ToHashSet(StringComparer.OrdinalIgnoreCase);
        
        var recommendations = spots
            .Select(spot => MapToCebuDestination(spot))
            .Select(destination => Score(destination, selected, preferences))
            .OrderByDescending(item => item.MatchScore)
            .ThenBy(item => Math.Abs(item.Destination.AdventureLevel - preferences.AdventureLevel))
            .Take(4)
            .ToList();

        return new TravelRecommendationsViewModel
        {
            Preferences = preferences,
            Recommendations = recommendations,
            Explanation =
            [
                new("Preference collection", "The survey stores selected Cebu places, activity types, adventure level, budget range, travel pace, and optional notes against the signed-in user ID and email."),
                new("Destination categories", "Each Cebu destination has searchable place and activity tags, plus metadata for intensity and trip style."),
                new("AI matching", "The first version uses transparent scoring: category overlap, adventure fit, and travel pace. In production, this scoring can be sent to an LLM or embeddings model to explain and re-rank results."),
                new("Continuous learning", "Clicks, swaps, saved places, completed bookings, ratings, and skipped recommendations can adjust category weights so future suggestions become more personal.")
            ]
        };
    }

    private static CebuDestination MapToCebuDestination(TravelSpot spot)
    {
        // Map TravelSpot Category to recommended categories
        var categories = new List<string> { spot.Category.ToLowerInvariant() };
        if (spot.Category == "Waterfall") categories.Add("water-sports");
        if (spot.Category == "Island") categories.Add("islands");
        if (spot.Category == "Beach") categories.Add("beaches");
        if (spot.Category == "Mountain") categories.Add("hiking");

        return new CebuDestination(
            spot.Id.ToString(),
            spot.Name,
            spot.Location,
            spot.Description,
            categories,
            spot.AdventureLevel,
            $"Travelers interested in {spot.Category}",
            spot.ImageUrl ?? "/images/hero-bg.jpg");
    }

    private static RecommendedDestination Score(CebuDestination destination, HashSet<string> selected, TravelPreferenceRecord preferences)
    {
        var matched = destination.Categories.Where(category => selected.Contains(category)).ToList();
        var interestScore = matched.Count * 25;
        var adventureFit = Math.Max(0, 20 - (Math.Abs(destination.AdventureLevel - preferences.AdventureLevel) * 5));
        var paceBoost = preferences.TravelPace.Equals("Relaxed", StringComparison.OrdinalIgnoreCase) && destination.AdventureLevel <= 2 ? 8 : 0;
        paceBoost += preferences.TravelPace.Equals("Packed", StringComparison.OrdinalIgnoreCase) && (destination.Categories.Contains("water-sports") || destination.Categories.Contains("hiking")) ? 8 : 0;
        var score = Math.Clamp(interestScore + adventureFit + paceBoost, 0, 98);

        var reason = matched.Count > 0
            ? $"Matches your {string.Join(", ", matched.Select(ToLabel))} interests with a {preferences.TravelPace.ToLowerInvariant()} pace."
            : $"Adds variety while staying close to your adventure level of {preferences.AdventureLevel}/5.";

        return new RecommendedDestination(destination, score, matched.Select(ToLabel).ToList(), reason);
    }

    private static string Normalize(string value) => value.Trim().ToLowerInvariant();

    private static string ToLabel(string key)
    {
        return TravelInterestCatalog.Options.FirstOrDefault(option => option.Key == key)?.Label ?? key;
    }
}

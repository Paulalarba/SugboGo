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

        var selected = preferences.PlaceInterests
            .Concat(preferences.ActivityInterests)
            .Select(Normalize)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var maxAdventureLevel = Math.Clamp(preferences.AdventureLevel, 1, 5);

        var scoredDestinations = spots
            .Select(MapToCebuDestination)
            .Where(destination => destination.AdventureLevel <= maxAdventureLevel)
            .Select(destination => Score(destination, selected, preferences))
            .ToList();

        var recommendations = scoredDestinations
            .GroupBy(item => item.Destination.AdventureLevel)
            .OrderByDescending(group => group.Key)
            .Select(group => group
                .OrderByDescending(item => item.MatchScore)
                .ThenBy(item => item.Destination.Name)
                .First())
            .Take(4)
            .ToList();


        return new TravelRecommendationsViewModel
        {
            Preferences = preferences,
            Recommendations = recommendations,
            Explanation =
            [
                new("How we match you", "We compare your selected places and activities against each destination's category tags, then factor in your adventure level and travel pace to produce a ranked score."),
                new("Adventure fit", "We only recommend destinations at or below your selected adventure level, then prioritize the strongest matches from the highest suitable levels first."),
                new("Group boost", "Popular and highly-rated spots get a small boost so you always get reliable starting points alongside hidden gems."),
                new("Getting smarter", "Future versions will learn from your bookings, saved spots, and skipped suggestions to make every recommendation feel more personal.")

                ]
        };
    }

    private static CebuDestination MapToCebuDestination(TravelSpot spot)
    {
        var categories = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            spot.Category.ToLowerInvariant()
        };

        switch (spot.Category)
        {
            // ── Water / Nature ────────────────────────────────────────────
            case "Waterfall":
                categories.Add("water-sports");
                categories.Add("hiking");
                categories.Add("mountains");
                break;

            case "Island":
                categories.Add("islands");
                categories.Add("beaches");
                categories.Add("water-sports");
                break;

            case "Beach":
                categories.Add("beaches");
                categories.Add("water-sports");
                categories.Add("slow-travel");
                break;

            case "Mountain":
                categories.Add("hiking");
                categories.Add("mountains");
                break;

            case "Viewpoint":
                categories.Add("mountains");
                categories.Add("hiking");
                categories.Add("slow-travel");
                break;

            case "Nature Park":
            case "Farm":
                categories.Add("mountains");
                categories.Add("slow-travel");
                categories.Add("hiking");
                break;

            case "Garden":
                categories.Add("slow-travel");
                categories.Add("mountains");
                break;

            case "Wildlife":
                categories.Add("water-sports");
                categories.Add("hiking");
                categories.Add("mountains");
                break;

            case "Eco Tour":
                categories.Add("water-sports");
                categories.Add("cultural-tours");
                categories.Add("slow-travel");
                break;

            // ── Heritage / Culture ────────────────────────────────────────
            case "Heritage":
            case "Historical":
                categories.Add("historical-sites");
                categories.Add("cultural-tours");
                break;

            case "Religious":
                categories.Add("historical-sites");
                categories.Add("cultural-tours");
                break;

            case "Monument":
            case "Landmark":
                categories.Add("historical-sites");
                categories.Add("cultural-tours");
                categories.Add("city-districts");
                break;

            case "Museum":
                categories.Add("historical-sites");
                categories.Add("cultural-tours");
                categories.Add("city-districts");
                break;

            case "Street":
            case "Park":
                categories.Add("historical-sites");
                categories.Add("city-districts");
                categories.Add("cultural-tours");
                break;

            // ── Food / Markets ────────────────────────────────────────────
            case "Food":
            case "Market":
            case "Cafe":
                categories.Add("dining");
                categories.Add("cultural-tours");
                categories.Add("city-districts");
                break;

            // ── Urban / Shopping ──────────────────────────────────────────
            case "City":
            case "Urban":
                categories.Add("city-districts");
                categories.Add("nightlife");
                categories.Add("shopping-malls");
                break;

            case "Shopping":
                categories.Add("shopping-malls");
                categories.Add("city-districts");
                categories.Add("dining");
                break;

            // ── Entertainment / Leisure ───────────────────────────────────
            case "Theme Park":
            case "Resort":
                categories.Add("slow-travel");
                categories.Add("city-districts");
                break;
        }

        return new CebuDestination(
            spot.Id.ToString(),
            spot.Name,
            spot.Location,
            spot.Description,
            categories.ToList(),
            spot.AdventureLevel,
            $"Great for {spot.Category.ToLowerInvariant()} lovers",
            spot.ImageUrl ?? "/images/hero-bg.jpg");
    }

    private static RecommendedDestination Score(
        CebuDestination destination,
        HashSet<string> selected,
        TravelPreferenceRecord preferences)
    {
        var matched = destination.Categories
            .Where(c => selected.Contains(c))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        // ── Interest score: PRIMARY factor, up to 75pts ───────────────────
        // Each matched interest = 25pts, max 3 counted.
        // This ensures cultural/mountain/etc spots always rank above
        // unmatched beach spots regardless of adventure level.
        var interestScore = Math.Min(matched.Count, 3) * 25;

        // ── Adventure fit: SECONDARY tiebreaker, up to 20pts ─────────────
        // Perfect match = 20pts, each level off = -4pts.
        // Kept small so it only separates spots with equal interest scores.
        var adventureDiff = Math.Abs(destination.AdventureLevel - preferences.AdventureLevel);
        var adventureFit = Math.Max(0, 20 - adventureDiff * 4);

        // ── Pace boost: up to 5pts ────────────────────────────────────────
        var paceBoost = preferences.TravelPace switch
        {
            var p when p.Equals("Relaxed", StringComparison.OrdinalIgnoreCase)
                && destination.AdventureLevel <= 2 => 5,
            var p when p.Equals("Packed", StringComparison.OrdinalIgnoreCase)
                && destination.AdventureLevel >= 4 => 5,
            var p when p.Equals("Balanced", StringComparison.OrdinalIgnoreCase)
                && destination.AdventureLevel is >= 2 and <= 4 => 3,
            _ => 0
        };

        var score = Math.Clamp(interestScore + adventureFit + paceBoost, 10, 99);

        var reason = matched.Count > 0
            ? $"Matches your {string.Join(" & ", matched.Take(2).Select(ToLabel))} interests" +
              (adventureDiff == 0
                  ? " with a perfect adventure fit."
                  : $" at adventure level {destination.AdventureLevel}/5.")
            : $"A great {preferences.TravelPace.ToLowerInvariant()}-pace pick at adventure level {destination.AdventureLevel}/5.";

        return new RecommendedDestination(destination, score, matched.Select(ToLabel).ToList(), reason);
    }

    private static string Normalize(string value) => value.Trim().ToLowerInvariant();

    private static string ToLabel(string key)
        => TravelInterestCatalog.Options.FirstOrDefault(o => o.Key == key)?.Label ?? key;
}
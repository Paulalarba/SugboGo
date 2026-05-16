using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SugboGo.Models;

public sealed class TravelPreferenceSurveyViewModel
{
    public IReadOnlyList<TravelInterestOption> PlaceOptions { get; init; } = TravelPreferenceSeedData.PlaceOptions;
    public IReadOnlyList<TravelInterestOption> ActivityOptions { get; init; } = TravelPreferenceSeedData.ActivityOptions;

    [Required(ErrorMessage = "Choose at least one place in Cebu.")]
    public List<string> SelectedPlaces { get; set; } = [];

    [Required(ErrorMessage = "Choose at least one activity.")]
    public List<string> SelectedActivities { get; set; } = [];

    [Range(1, 5)]
    [Display(Name = "Adventure level")]
    public int AdventureLevel { get; set; } = 3;

    [Required]
    [Display(Name = "Travel pace")]
    public string TravelPace { get; set; } = "Balanced";

    [Required]
    [Display(Name = "Budget range")]
    public string BudgetRange { get; set; } = "Mid-range";

    public string? ReturnUrl { get; set; }

    [Display(Name = "Notes for the curator")]
    [StringLength(500)]
    public string? Notes { get; set; }
}

public sealed class TravelPreferenceRecord
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string UserId { get; set; } = string.Empty;

    [ForeignKey("UserId")]
    public UserAccount? User { get; set; }

    public string Email { get; set; } = string.Empty;
    public List<string> PlaceInterests { get; set; } = [];
    public List<string> ActivityInterests { get; set; } = [];

    [NotMapped]
    public List<string> Interests
    {
        get => PlaceInterests.Concat(ActivityInterests).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        set
        {
            PlaceInterests = value ?? [];
            ActivityInterests = [];
        }
    }

    public int AdventureLevel { get; set; }
    public string TravelPace { get; set; } = string.Empty;
    public string BudgetRange { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public sealed record TravelInterestOption(string Key, string Label, string Description);

public sealed record CebuDestination(
    string Id,
    string Name,
    string Location,
    string Summary,
    IReadOnlyList<string> Categories,
    int AdventureLevel,
    string BestFor,
    string ImageUrl);

public sealed record RecommendedDestination(
    CebuDestination Destination,
    int MatchScore,
    IReadOnlyList<string> MatchedInterests,
    string Reason);

public sealed class TravelRecommendationsViewModel
{
    public TravelPreferenceRecord Preferences { get; init; } = new();
    public IReadOnlyList<RecommendedDestination> Recommendations { get; init; } = [];
    public IReadOnlyList<AiRecommendationStep> Explanation { get; init; } = [];
}

public sealed record AiRecommendationStep(string Title, string Body);

public static class TravelInterestCatalog
{
    public static IReadOnlyList<TravelInterestOption> Options { get; } =
        TravelPreferenceSeedData.PlaceOptions.Concat(TravelPreferenceSeedData.ActivityOptions).ToList();
}

public static class TravelPreferenceSeedData
{
    public static IReadOnlyList<TravelInterestOption> PlaceOptions { get; } =
    [
        new("beaches", "Beaches", "Island hopping, reefs, coves, and slower coastal days."),
        new("mountains", "Mountains", "Peaks, highland views, waterfalls, and cool ridge towns."),
        new("historical-sites", "Historical sites", "Churches, forts, old streets, museums, and ancestral houses."),
        new("shopping-malls", "Shopping malls", "City comforts, food halls, cinema stops, and easy rainy-day plans."),
        new("islands", "Islands", "Boat days, sandbars, marine sanctuaries, and coastal villages."),
        new("city-districts", "City districts", "Markets, IT Park nights, heritage blocks, and urban Cebu energy.")
    ];

    public static IReadOnlyList<TravelInterestOption> ActivityOptions { get; } =
    [
        new("hiking", "Hiking", "Sunrise trails, peak walks, waterfall treks, and guided ridge routes."),
        new("dining", "Dining", "Lechon, seafood, markets, scenic cafes, and chef-led tables."),
        new("water-sports", "Water sports", "Snorkeling, diving, sardine runs, canyoneering, and island swims."),
        new("cultural-tours", "Cultural tours", "Community-led stories, festivals, crafts, museums, and heritage walks."),
        new("nightlife", "Nightlife", "Rooftops, live music, cocktail rooms, late bites, and safe transfers."),
        new("slow-travel", "Slow travel", "Quiet stays, spa time, scenic cafes, and restorative pacing.")
    ];
}

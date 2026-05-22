namespace SugboGo.Models;

public sealed class DashboardViewModel
{
    public string FirstName { get; set; } = "Traveler";
    public string UserInitial { get; set; } = "T";
    public string Greeting { get; set; } = string.Empty;
    public ActiveTripViewModel? ActiveTrip { get; set; }
    public List<DestinationPostViewModel> SocialFeed { get; set; } = [];
    public List<VibeTagViewModel> VibeTags { get; set; } = [];
    public List<GemRecommendationViewModel> CuratedGems { get; set; } = [];
    public List<BookingVaultItemViewModel> Bookings { get; set; } = [];
    public List<SavedGemViewModel> SavedGems { get; set; } = [];
    public List<PastAdventureViewModel> PastAdventures { get; set; } = [];
    public TravelProfileViewModel TravelProfile { get; set; } = new();
    public List<DashboardFeatureSuggestionViewModel> FeatureSuggestions { get; set; } = [];
}

public sealed class ActiveTripViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string DateRange { get; set; } = string.Empty;
    public string Hotel { get; set; } = string.Empty;
    public string SogboKeyCode { get; set; } = string.Empty;
    public string ConciergeUrl { get; set; } = "https://wa.me/639170002841";
    public List<ItineraryStopViewModel> Stops { get; set; } = [];
    public List<MapPinViewModel> MapPins { get; set; } = [];
}

public sealed class ItineraryStopViewModel
{
    public string Time { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string PartnerStatus { get; set; } = string.Empty;
}

public sealed class MapPinViewModel
{
    public string Label { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}

public sealed class DestinationPostViewModel
{
    public string Id { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorInitial { get; set; } = string.Empty;
    public string AuthorRole { get; set; } = string.Empty;
    public string Timestamp { get; set; } = string.Empty;
    public string DestinationName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = [];
    public int Likes { get; set; }
    public int Comments { get; set; }
    public List<PostCommentViewModel> CommentsList { get; set; } = [];
    public bool IsLikedByUser { get; set; }
    public string RecommendationReason { get; set; } = string.Empty;
    public int MatchScore { get; set; }
}

public sealed class PostCommentViewModel
{
    public string AuthorName { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string Timestamp { get; set; } = string.Empty;
}

public sealed class VibeTagViewModel
{
    public string Label { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public sealed class GemRecommendationViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string MatchReason { get; set; } = string.Empty;
    public string Neighborhood { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int MatchScore { get; set; }
}

public sealed class BookingVaultItemViewModel
{
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public sealed class SavedGemViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
}

public sealed class PastAdventureViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
}

public sealed class TravelProfileViewModel
{
    public string StayPreference { get; set; } = string.Empty;
    public string FoodPreference { get; set; } = string.Empty;
    public string PacePreference { get; set; } = string.Empty;
    public string Notifications { get; set; } = string.Empty;
    public string PaymentSummary { get; set; } = string.Empty;
}

public sealed class DashboardFeatureSuggestionViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

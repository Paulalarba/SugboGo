using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SugboGo.Data;
using SugboGo.Models;
using SugboGo.Services.Travel;

namespace SugboGo.Services.Dashboard;

public sealed class DashboardExperienceService : IDashboardExperienceService
{
    private readonly IDestinationPostStore _postStore;
    private readonly ITravelPreferenceStore _preferenceStore;
    private readonly IUserSavedGemStore _savedGemStore;
    private readonly SugboGoDbContext _dbContext;

    public DashboardExperienceService(
        IDestinationPostStore postStore,
        ITravelPreferenceStore preferenceStore,
        IUserSavedGemStore savedGemStore,
        SugboGoDbContext dbContext)
    {
        _postStore = postStore;
        _preferenceStore = preferenceStore;
        _savedGemStore = savedGemStore;
        _dbContext = dbContext;
    }

    public async Task<DashboardViewModel> BuildForUserAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        var fullName = user.FindFirstValue(ClaimTypes.Name) ?? "Traveler";
        var email = user.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var firstName = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "Traveler";
        var seed = Math.Abs(email.GetHashCode());
        var preferences = string.IsNullOrWhiteSpace(userId)
            ? null
            : await _preferenceStore.FindLatestByUserIdAsync(userId, cancellationToken);
        var posts = await _postStore.GetAllAsync(cancellationToken);
        var savedGems = string.IsNullOrWhiteSpace(userId)
            ? []
            : await _savedGemStore.GetByUserIdAsync(userId, cancellationToken);
        var bookings = await GetBookingsForUserAsync(userId, cancellationToken);

        return new DashboardViewModel
        {
            FirstName = firstName,
            UserInitial = firstName[..1].ToUpperInvariant(),
            Greeting = BuildGreeting(firstName),
            ActiveTrip = BuildActiveTrip(firstName, seed, bookings.FirstOrDefault()),
            SocialFeed = BuildSocialFeed(posts, preferences),
            VibeTags = BuildVibeTags(preferences),
            CuratedGems = BuildCuratedGems(seed),
            Bookings = BuildBookings(bookings),
            SavedGems = savedGems.Select(gem => new SavedGemViewModel
            {
                Id = gem.Id,
                Title = gem.Title,
                Note = gem.Note
            }).ToList(),
            PastAdventures = BuildPastAdventures(seed),
            TravelProfile = BuildTravelProfile(preferences, seed),
            FeatureSuggestions = BuildFeatureSuggestions()
        };
    }

    private static string BuildGreeting(string firstName)
    {
        var hour = DateTime.Now.Hour;
        var dayPart = hour < 12 ? "Good morning" : hour < 18 ? "Good afternoon" : "Good evening";
        return $"{dayPart}, {firstName}";
    }

    private static ActiveTripViewModel? BuildActiveTrip(string firstName, int seed, Booking? booking)
    {
        if (booking is null)
        {
            return null;
        }

        var startDate = DateTime.Today.AddDays(seed % 2 == 0 ? 3 : -1);
        startDate = booking.TravelDate.ToLocalTime().Date;
        var status = DateTime.Today >= startDate
            ? "You are currently in Cebu!"
            : $"Your Cebu Adventure begins in {(startDate - DateTime.Today).Days} days!";

        var stops = new List<ItineraryStopViewModel>
        {
            new() { Time = "10:00 AM", Title = "Hidden Heritage Cafe", Location = "Parian", Type = "Coffee ritual", PartnerStatus = "Priority table ready" },
            new() { Time = "2:00 PM", Title = "Private Mountain View", Location = "Busay", Type = "Soft adventure", PartnerStatus = "Guide confirmed" },
            new() { Time = "7:00 PM", Title = "Curated Rooftop Dinner", Location = "Cebu Business Park", Type = "Secret table", PartnerStatus = "Sogbo-Key perk active" }
        };

        return new ActiveTripViewModel
        {
            Title = $"{firstName}'s {booking.DestinationName} trip",
            Status = status,
            DateRange = $"{startDate:MMM d} to {startDate.AddDays(1):MMM d, yyyy}",
            Hotel = ExtractSelectedName(booking.SelectedAccommodationJson, "Accommodation pending"),
            SogboKeyCode = booking.QrCode,
            Stops = stops,
            MapPins =
            [
                new() { Label = "Heritage Cafe", Time = "10:00 AM", Latitude = 10.2961m, Longitude = 123.8993m, X = 34, Y = 58 },
                new() { Label = "Mountain View", Time = "2:00 PM", Latitude = 10.3713m, Longitude = 123.8830m, X = 47, Y = 28 },
                new() { Label = "Rooftop Dinner", Time = "7:00 PM", Latitude = 10.3190m, Longitude = 123.9057m, X = 63, Y = 48 }
            ]
        };
    }

    private static List<DestinationPostViewModel> BuildSocialFeed(IEnumerable<DestinationPost> posts, TravelPreferenceRecord? preferences)
    {
        var selected = preferences?.Interests ?? [];

        return posts.Select(post =>
        {
            var tagKey = NormalizeInterest(post.Tag);
            var isMatch = selected.Any(interest => NormalizeInterest(interest) == tagKey);

            return new DestinationPostViewModel
            {
                Id = post.Id,
                AuthorName = post.AuthorName,
                AuthorInitial = BuildInitial(post.AuthorName),
                AuthorRole = string.Equals(post.UserId, preferences?.UserId, StringComparison.OrdinalIgnoreCase) ? "SugboGo client" : "Cebu traveler",
                Timestamp = post.CreatedAt.ToLocalTime().ToString("MMM d, h:mm tt"),
                DestinationName = post.DestinationName,
                Location = post.Location,
                Description = post.Description,
                Caption = post.Caption,
                ImageUrl = BuildPostImageUrl(post.ImageFileName),
                Tags = string.IsNullOrWhiteSpace(post.Tag) ? ["Cebu"] : [BuildInterestLabel(post.Tag)],
                Likes = post.Likes,
                Comments = post.Comments,
                CommentsList = (post.CommentsList ?? []).Select(c => new PostCommentViewModel
                {
                    AuthorName = c.AuthorName,
                    Text = c.Text,
                    Timestamp = c.CreatedAt.ToLocalTime().ToString("MMM d, h:mm tt")
                }).ToList(),
                RecommendationReason = isMatch
                    ? "This matches your saved Cebu travel interests."
                    : "This post is part of the live Cebu community feed.",
                MatchScore = isMatch ? 92 : 74
            };
        }).ToList();
    }

    private static List<VibeTagViewModel> BuildVibeTags(TravelPreferenceRecord? preferences)
    {
        if (preferences?.Interests.Count > 0)
        {
            return TravelInterestCatalog.Options
                .Where(option => preferences.Interests.Contains(option.Key, StringComparer.OrdinalIgnoreCase))
                .Select(option => new VibeTagViewModel { Label = option.Label, IsActive = true })
                .ToList();
        }

        return TravelInterestCatalog.Options
            .Take(6)
            .Select(option => new VibeTagViewModel { Label = option.Label, IsActive = false })
            .ToList();
    }

    private static List<GemRecommendationViewModel> BuildCuratedGems(int seed)
    {
        var gems = new List<GemRecommendationViewModel>
        {
            new() { Title = "Kamagayan Vinyl Supper", Category = "Secret dining", Neighborhood = "Kamagayan", MatchReason = "Quiet room, smoky seafood, chef-hosted", ImageUrl = "https://images.unsplash.com/photo-1555396273-367ea4eb4db5?auto=format&fit=crop&w=900&q=80", MatchScore = 96 },
            new() { Title = "Mactan Ceramic Courtyard", Category = "Hidden gallery", Neighborhood = "Mactan", MatchReason = "Design-led, low crowd, local maker access", ImageUrl = "https://images.unsplash.com/photo-1493106641515-6b5631de4bb9?auto=format&fit=crop&w=900&q=80", MatchScore = 92 },
            new() { Title = "Liloan Moon Tide Table", Category = "Coastal dinner", Neighborhood = "Liloan", MatchReason = "Sea breeze, clean kitchen, golden-hour transfer", ImageUrl = "https://images.unsplash.com/photo-1507525428034-b723cf961d3e?auto=format&fit=crop&w=900&q=80", MatchScore = 89 },
            new() { Title = "Busay Garden Hideout", Category = "Mountain pause", Neighborhood = "Busay", MatchReason = "Soft adventure with boutique comfort", ImageUrl = "https://images.unsplash.com/photo-1500530855697-b586d89ba3ee?auto=format&fit=crop&w=900&q=80", MatchScore = 88 }
        };

        return gems.OrderBy(gem => (gem.MatchScore + seed) % 17).ToList();
    }

    private static List<BookingVaultItemViewModel> BuildBookings(IEnumerable<Booking> bookings)
    {
        return bookings.Select(booking => new BookingVaultItemViewModel
        {
            Type = booking.TravelerType,
            Title = booking.DestinationName,
            Date = booking.TravelDate.ToLocalTime().ToString("MMM d, yyyy"),
            Status = $"{booking.Status} · {booking.QrCode}"
        }).ToList();
    }

    private async Task<List<Booking>> GetBookingsForUserAsync(string userId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return [];
        }

        try
        {
            return await _dbContext.Bookings
                .Where(booking => booking.UserId == userId)
                .OrderByDescending(booking => booking.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception exception) when (IsMissingBookingsSchema(exception))
        {
            return [];
        }
    }

    private static bool IsMissingBookingsSchema(Exception exception)
    {
        for (var current = exception; current is not null; current = current.InnerException!)
        {
            if (current.Message.Contains("Bookings", StringComparison.OrdinalIgnoreCase) ||
                current.Message.Contains("bookings", StringComparison.OrdinalIgnoreCase) ||
                current.Message.Contains("column", StringComparison.OrdinalIgnoreCase) ||
                current.Message.Contains("relation", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static string ExtractSelectedName(string json, string fallback)
    {
        try
        {
            using var document = System.Text.Json.JsonDocument.Parse(json);
            return document.RootElement.TryGetProperty("Name", out var name) && !string.IsNullOrWhiteSpace(name.GetString())
                ? name.GetString()!
                : fallback;
        }
        catch
        {
            return fallback;
        }
    }

    private static List<PastAdventureViewModel> BuildPastAdventures(int seed)
    {
        if (seed % 3 == 0)
        {
            return [];
        }

        return
        [
            new() { Title = "Old Cebu Food Crawl", Date = "March 2025" },
            new() { Title = "South Cebu Reef Reset", Date = "August 2025" }
        ];
    }

    private static TravelProfileViewModel BuildTravelProfile(TravelPreferenceRecord? preferences, int seed)
    {
        if (preferences is not null)
        {
            var interestLabels = preferences.Interests
                .Select(BuildInterestLabel)
                .Where(label => !string.IsNullOrWhiteSpace(label))
                .ToList();

            return new TravelProfileViewModel
            {
                StayPreference = $"{preferences.BudgetRange} comfort profile",
                FoodPreference = interestLabels.Count == 0 ? "No interests selected yet" : string.Join(", ", interestLabels),
                PacePreference = $"{preferences.TravelPace} pace, adventure level {preferences.AdventureLevel}/5",
                Notifications = "Hidden Gem alerts and itinerary changes enabled",
                PaymentSummary = "Add a payment method when booking persistence is enabled"
            };
        }

        return new TravelProfileViewModel
        {
            StayPreference = seed % 2 == 0 ? "Boutique hotels over luxury chains" : "Quiet design stays near local food",
            FoodPreference = "Street food energy, high hygiene standards",
            PacePreference = "Two anchors per day, flexible in-between time",
            Notifications = "Hidden Gem alerts and itinerary changes enabled",
            PaymentSummary = "Visa ending 4242 ready for Quick-Pay"
        };
    }

    private static string BuildPostImageUrl(string imageFileName)
    {
        return string.IsNullOrWhiteSpace(imageFileName)
            ? "https://images.unsplash.com/photo-1507525428034-b723cf961d3e?auto=format&fit=crop&w=1200&q=80"
            : $"/uploads/destination-posts/{imageFileName}";
    }

    private static string BuildInitial(string name)
    {
        return name.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?[..1].ToUpperInvariant() ?? "T";
    }

    private static string BuildInterestLabel(string key)
    {
        return TravelInterestCatalog.Options.FirstOrDefault(option => option.Key == NormalizeInterest(key))?.Label
            ?? key.Trim();
    }

    private static string NormalizeInterest(string value) => value.Trim().ToLowerInvariant().Replace(" ", "-");

    private static List<DashboardFeatureSuggestionViewModel> BuildFeatureSuggestions()
    {
        return
        [
            new() { Title = "Live partner check-ins", Description = "Let hotels, guides, and restaurants update arrival readiness in real time." },
            new() { Title = "Weather-aware route swaps", Description = "Automatically suggest indoor gems or safer beach timing when conditions change." },
            new() { Title = "Group vibe matching", Description = "Merge multiple travelers' quiz results into one route everyone can tolerate, maybe even love." },
            new() { Title = "Expense split and travel wallet", Description = "Let flashpacker groups split deposits, perks, and concierge add-ons inside SogboGo." }
        ];
    }
}

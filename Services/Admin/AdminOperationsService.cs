using SugboGo.Models;
using SugboGo.Data;
using SugboGo.Services.Auth;
using SugboGo.Services.Dashboard;
using SugboGo.Services.Travel;
using Microsoft.EntityFrameworkCore;

namespace SugboGo.Services.Admin;

public sealed class AdminOperationsService : IAdminOperationsService
{
    private readonly IUserAccountStore _userStore;
    private readonly ITravelPreferenceStore _preferenceStore;
    private readonly IDestinationPostStore _postStore;
    private readonly IAdminDataStore _adminDataStore;
    private readonly SugboGoDbContext _dbContext;

    public AdminOperationsService(
        IUserAccountStore userStore,
        ITravelPreferenceStore preferenceStore,
        IDestinationPostStore postStore,
        IAdminDataStore adminDataStore,
        SugboGoDbContext dbContext)
    {
        _userStore = userStore;
        _preferenceStore = preferenceStore;
        _postStore = postStore;
        _adminDataStore = adminDataStore;
        _dbContext = dbContext;
    }

    public async Task<AdminDashboardViewModel> BuildDashboardAsync(CancellationToken cancellationToken = default)
    {
        var users = await _dbContext.Users.ToListAsync(cancellationToken);
        var preferences = await _dbContext.TravelPreferences.ToListAsync(cancellationToken);
        var posts = await _dbContext.DestinationPosts.ToListAsync(cancellationToken);
        var bookings = await GetBookingsAsync(cancellationToken);
        var gems = await _adminDataStore.GetGemsAsync(cancellationToken);
        var templates = await _adminDataStore.GetTemplatesAsync(cancellationToken);
        var partners = await _adminDataStore.GetPartnersAsync(cancellationToken);
        
        var latestUser = users.OrderByDescending(user => user.CreatedAt).FirstOrDefault();

        return new AdminDashboardViewModel
        {
            Kpis = BuildKpis(users, preferences, posts, bookings, latestUser),
            VibeTrends = BuildVibeTrends(preferences),
            UrgentAlerts = BuildUrgentAlerts(users, preferences),
            Pipeline = BuildPipeline(bookings, users, preferences),
            Flashpackers = BuildFlashpackers(users, preferences, posts),
            Gems = gems.Select(gem => new GemAdminViewModel
            {
                Name = gem.Name,
                Category = gem.Category,
                FlashpackerScore = gem.FlashpackerScore,
                QualityCheckDate = gem.QualityCheckDate,
                ContactPerson = gem.ContactPerson,
                Latitude = gem.Latitude,
                Longitude = gem.Longitude,
                Status = gem.Status,
                MapX = gem.MapX,
                MapY = gem.MapY
            }).ToList(),
            Templates = templates.Select(template => new ItineraryTemplateViewModel
            {
                Name = template.Name,
                Vibe = template.Vibe,
                Stops = template.Stops,
                AvgDuration = template.AvgDuration
            }).ToList(),
            Partners = partners.Select(partner => new PartnerAdminViewModel
            {
                Name = partner.Name,
                Type = partner.Type,
                Contact = partner.Contact,
                Commission = partner.Commission,
                LastAudit = partner.LastAudit,
                Status = partner.Status
            }).ToList(),
            Bookings = bookings.Select(b => new BookingAdminViewModel
            {
                Id = b.Id,
                UserName = users.FirstOrDefault(u => u.Id == b.UserId)?.FullName ?? "Unknown User",
                Destination = b.DestinationName,
                Date = b.TravelDate.ToString("MMM d, yyyy"),
                Status = b.Status,
                Amount = b.TotalPrice
            }).ToList(),
            CollaborationQueue = BuildCollaborationQueue()
        };
    }

    private static List<AdminKpiViewModel> BuildKpis(
        IReadOnlyCollection<UserAccount> users,
        IReadOnlyCollection<TravelPreferenceRecord> preferences,
        IReadOnlyCollection<DestinationPost> posts,
        IReadOnlyCollection<Booking> bookings,
        UserAccount? latestUser)
    {
        var bookingRevenue = bookings.Sum(booking => booking.TotalPrice);

        return
        [
            new() { Label = "Total Users", Value = users.Count.ToString(), Delta = $"{users.Count(user => AccountRoles.Normalize(user.Role) == AccountRoles.Client)} client account(s)" },
            new() { Label = "Live Bookings", Value = bookings.Count.ToString(), Delta = $"PHP {bookingRevenue:N0} confirmed revenue" },
            new() { Label = "Preferences Submitted", Value = preferences.Count.ToString(), Delta = $"{preferences.Select(preference => preference.UserId).Distinct().Count()} traveler profile(s)" },
            new() { Label = "Latest Signup", Value = latestUser?.CreatedAt.ToLocalTime().ToString("MMM d") ?? "None", Delta = latestUser?.Email ?? "No registered users yet" }
        ];
    }

    private static List<VibeTrendViewModel> BuildVibeTrends(IEnumerable<TravelPreferenceRecord> preferences)
    {
        var interestCounts = preferences
            .SelectMany(preference => preference.Interests)
            .Select(NormalizeInterest)
            .Where(interest => !string.IsNullOrWhiteSpace(interest))
            .GroupBy(interest => interest)
            .Select(group => new { Key = group.Key, Count = group.Count() })
            .OrderByDescending(item => item.Count)
            .ThenBy(item => item.Key)
            .ToList();

        var total = interestCounts.Sum(item => item.Count);

        if (total == 0)
        {
            return [new() { Vibe = "No preferences yet", Percentage = 0 }];
        }

        return interestCounts
            .Take(6)
            .Select(item => new VibeTrendViewModel
            {
                Vibe = BuildInterestLabel(item.Key),
                Percentage = (int)Math.Round(item.Count * 100m / total)
            })
            .ToList();
    }

    private static List<UrgentAlertViewModel> BuildUrgentAlerts(
        IReadOnlyCollection<UserAccount> users,
        IReadOnlyCollection<TravelPreferenceRecord> preferences)
    {
        var missingPreferences = users
            .Where(user => AccountRoles.Normalize(user.Role) == AccountRoles.Client)
            .Where(user => preferences.All(preference => preference.UserId != user.Id))
            .Take(3)
            .Select(user => new UrgentAlertViewModel
            {
                Traveler = user.FullName,
                Issue = "No travel preference survey submitted yet",
                Location = user.Email,
                ChatUrl = "https://wa.me/639170002841"
            })
            .ToList();

        return missingPreferences.Count == 0
            ? [new() { Traveler = "System", Issue = "All registered clients have preference data or no clients are registered yet.", Location = "SugboGo data flow", ChatUrl = "https://wa.me/639170002841" }]
            : missingPreferences;
    }

    private static List<FlashpackerProfileViewModel> BuildFlashpackers(
        IEnumerable<UserAccount> users,
        IEnumerable<TravelPreferenceRecord> preferences,
        IEnumerable<DestinationPost> posts)
    {
        var preferenceByUser = preferences
            .GroupBy(preference => preference.UserId)
            .ToDictionary(group => group.Key, group => group.OrderByDescending(preference => preference.UpdatedAt).First());
        var postCounts = posts.GroupBy(post => post.UserId).ToDictionary(group => group.Key, group => group.Count());

        var profiles = users.Select(user =>
        {
            preferenceByUser.TryGetValue(user.Id, out var preference);
            postCounts.TryGetValue(user.Id, out var postCount);
            var interests = preference?.Interests.Select(BuildInterestLabel).ToList() ?? [];

            return new FlashpackerProfileViewModel
            {
                Name = string.IsNullOrWhiteSpace(user.FullName) ? user.Email : user.FullName,
                Vibe = interests.FirstOrDefault() ?? "Profile pending",
                Constraints = preference is null
                    ? "No preference survey submitted yet."
                    : $"{preference.TravelPace} pace, {preference.BudgetRange}, adventure {preference.AdventureLevel}/5",
                Feedback = preference?.Notes ?? (interests.Count == 0 ? "No notes yet." : string.Join(", ", interests)),
                CebuTrips = postCount
            };
        }).ToList();

        return profiles.Count == 0
            ? [new() { Name = "No registered users yet", Vibe = "Waiting for data", Constraints = "Register an account to populate the CRM.", Feedback = "Live user data will appear here.", CebuTrips = 0 }]
            : profiles;
    }

    private static List<PipelineColumnViewModel> BuildPipeline(
        IReadOnlyCollection<Booking> bookings,
        IReadOnlyCollection<UserAccount> users,
        IReadOnlyCollection<TravelPreferenceRecord> preferences)
    {
        var userById = users.ToDictionary(user => user.Id, user => user);
        var preferenceByUser = preferences
            .GroupBy(preference => preference.UserId)
            .ToDictionary(group => group.Key, group => group.OrderByDescending(preference => preference.UpdatedAt).First());

        var columns = new[]
        {
            "New Requests",
            "In Curation",
            "Awaiting User Approval",
            "Confirmed/Paid",
            "In Progress",
            "Completed"
        }.Select(name => new PipelineColumnViewModel { Name = name }).ToList();

        foreach (var booking in bookings.OrderByDescending(booking => booking.CreatedAt))
        {
            userById.TryGetValue(booking.UserId, out var user);
            preferenceByUser.TryGetValue(booking.UserId, out var preference);

            var status = booking.Status.Trim().ToLowerInvariant();
            var column = status switch
            {
                "pending" => columns[0],
                "curating" => columns[1],
                "awaiting approval" => columns[2],
                "confirmed" or "paid" => columns[3],
                "in progress" => columns[4],
                "completed" => columns[5],
                _ => columns[0]
            };

            column.Cards.Add(new PipelineCardViewModel
            {
                Traveler = user?.FullName ?? user?.Email ?? "Traveler",
                Vibe = preference?.Interests.Select(BuildInterestLabel).FirstOrDefault() ?? booking.TravelerType,
                TravelWindow = $"{booking.TravelDate:MMM d, yyyy} - {booking.DestinationName}",
                Priority = $"{booking.PaymentMethod ?? "Payment"} · PHP {booking.TotalPrice:N0}"
            });
        }

        return columns;
    }

    private static List<PartnerAdminViewModel> BuildPartners()
    {
        return
        [
            new() { Name = "The Helix House", Type = "Boutique hotel", Contact = "Nina Yu", Commission = "15%", LastAudit = "Apr 30, 2026", Status = "Excellent" },
            new() { Name = "South Ridge Guides", Type = "Local guide", Contact = "Ramon Uy", Commission = "Per route", LastAudit = "Apr 22, 2026", Status = "Watch weather" },
            new() { Name = "Red Door Supper Club", Type = "Dining", Contact = "Marco Dizon", Commission = "12%", LastAudit = "May 3, 2026", Status = "Excellent" }
        ];
    }

    private async Task<List<Booking>> GetBookingsAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await _dbContext.Bookings
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

    private static List<CollaborationSuggestionViewModel> BuildCollaborationQueue()
    {
        return
        [
            new() { SuggestedBy = "Local Expert: Ana", Spot = "Mactan Ceramic Courtyard", Reason = "Strong design signal for Urban Explorer profiles.", ApprovalStatus = "Needs admin approval" },
            new() { SuggestedBy = "Developer: Ken", Spot = "Liloan Moon Tide Table", Reason = "Route engine says it pairs well with north transfers.", ApprovalStatus = "Map validation pending" }
        ];
    }

    private static string BuildInterestLabel(string key)
    {
        return TravelInterestCatalog.Options.FirstOrDefault(option => option.Key == NormalizeInterest(key))?.Label
            ?? key.Trim();
    }

    private static string NormalizeInterest(string value) => value.Trim().ToLowerInvariant().Replace(" ", "-");
}

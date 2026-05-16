namespace SugboGo.Models;

public sealed class UserAccount
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Client";
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Navigation properties
    public ICollection<Booking> Bookings { get; set; } = [];
    public ICollection<SavedGem> SavedGems { get; set; } = [];
    public ICollection<DestinationPost> DestinationPosts { get; set; } = [];
    public ICollection<TravelPreferenceRecord> TravelPreferences { get; set; } = [];
}

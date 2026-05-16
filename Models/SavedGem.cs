using System.ComponentModel.DataAnnotations.Schema;

namespace SugboGo.Models;

public sealed class SavedGem
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string UserId { get; set; } = string.Empty;

    [ForeignKey("UserId")]
    public UserAccount? User { get; set; }

    public int? TravelSpotId { get; set; }

    [ForeignKey("TravelSpotId")]
    public TravelSpot? TravelSpot { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Neighborhood { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public DateTimeOffset SavedAt { get; set; } = DateTimeOffset.UtcNow;
}

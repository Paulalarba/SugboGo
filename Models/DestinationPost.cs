using System.ComponentModel.DataAnnotations.Schema;

namespace SugboGo.Models;

public sealed class DestinationPost
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string UserId { get; set; } = string.Empty;

    [ForeignKey("UserId")]
    public UserAccount? User { get; set; }

    public int? TravelSpotId { get; set; }

    [ForeignKey("TravelSpotId")]
    public TravelSpot? TravelSpot { get; set; }

    public string AuthorName { get; set; } = string.Empty;
    public string AuthorEmail { get; set; } = string.Empty;
    public string DestinationName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public string ImageFileName { get; set; } = string.Empty;
    public int Likes { get; set; }
    public int Comments { get; set; }
    public List<PostComment> CommentsList { get; set; } = [];
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

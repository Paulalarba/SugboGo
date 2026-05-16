using System.ComponentModel.DataAnnotations.Schema;

namespace SugboGo.Models;

public sealed class PostComment
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string UserId { get; set; } = string.Empty;

    [ForeignKey("UserId")]
    public UserAccount? User { get; set; }

    public string AuthorName { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

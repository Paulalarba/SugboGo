using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SugboGo.Models;

public sealed class TravelSpot
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(160)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(160)]
    public string Location { get; set; } = string.Empty;

    [Required]
    [StringLength(600)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string Region { get; set; } = string.Empty;

    public bool IsPopular { get; set; }

    [StringLength(255)]
    public string? ImageUrl { get; set; }

    [Range(1, 5)]
    public int AdventureLevel { get; set; } = 3;

    [Column(TypeName = "decimal(18,2)")]
    public decimal BasePrice { get; set; } = 2500m;

    // Navigation properties
    public ICollection<Booking> Bookings { get; set; } = [];
    public ICollection<SavedGem> SavedGems { get; set; } = [];
    public ICollection<DestinationPost> DestinationPosts { get; set; } = [];
}

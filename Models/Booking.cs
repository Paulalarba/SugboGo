using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SugboGo.Models;

public sealed class Booking
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [Required]
    public string UserId { get; set; } = string.Empty;

    [ForeignKey("UserId")]
    public UserAccount? User { get; set; }

    public int? TravelSpotId { get; set; }

    [ForeignKey("TravelSpotId")]
    public TravelSpot? TravelSpot { get; set; }

    [Required]
    public string SelectionType { get; set; } = "UserSelected";

    [Required]
    public string DestinationName { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public string Location { get; set; } = "Cebu, Philippines";

    [Required]
    public DateTime TravelDate { get; set; }

    [Required]
    public string TravelerType { get; set; } = "Solo"; // Solo, Couple, Group, Family

    public int TravelerCount { get; set; } = 1;

    // JSON stored lists for flexibility
    public string SelectedActivitiesJson { get; set; } = "[]";
    
    public string SelectedAccommodationJson { get; set; } = "{}";
    
    public string SelectedTransportationJson { get; set; } = "{}";

    [Column(TypeName = "decimal(18,2)")]
    public decimal BasePrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal AddOnsPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxesAndFees { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }

    public string? TravelerNotes { get; set; }

    public string Status { get; set; } = "Pending"; // Pending, Paid, Confirmed, Cancelled

    public string? PaymentMethod { get; set; }

    public string QrCode { get; set; } = Guid.NewGuid().ToString("N").ToUpper()[..8];

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

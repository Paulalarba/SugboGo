using System.ComponentModel.DataAnnotations;

namespace SugboGo.Models;

public sealed class BookingStepViewModel
{
    public string CurrentStep { get; set; } = "details"; // details, options, review, payment, success
    public string BookingType { get; set; } = "UserSelected"; // SystemSelected, UserSelected
    public BookingDataViewModel Data { get; set; } = new();
    public List<RecommendedDestination> RecommendationOptions { get; set; } = [];
    public List<BookingActivityOption> ActivityOptions { get; set; } = [];
    public List<BookingAccommodationOption> AccommodationOptions { get; set; } = [];
    public List<BookingTransportOption> TransportOptions { get; set; } = [];
    public List<string> SmartRecommendations { get; set; } = [];
    public TravelPreferenceRecord? Preferences { get; set; }
}

public sealed class BookingDataViewModel
{
    public string DestinationId { get; set; } = string.Empty;
    public string BookingType { get; set; } = "UserSelected";
    public string DestinationName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = "Cebu, Philippines";
    public string Duration { get; set; } = string.Empty;
    public string BestTimeToVisit { get; set; } = string.Empty;
    public string RatingSummary { get; set; } = string.Empty;
    public string MapUrl { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }

    [Required]
    public DateTime? TravelDate { get; set; }

    public string TravelerType { get; set; } = "Solo";
    public int TravelerCount { get; set; } = 1;

    public List<string> SelectedActivities { get; set; } = [];
    public string SelectedAccommodation { get; set; } = string.Empty;
    public string SelectedTransportation { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;

    public string? TravelerNotes { get; set; }
    
    public decimal AddOnsPrice { get; set; }
    public decimal TaxesAndFees { get; set; }
    public decimal TotalPrice { get; set; }
}

public sealed class BookingActivityOption
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
}

public sealed class BookingAccommodationOption
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public string Amenities { get; set; } = string.Empty;
    public string Distance { get; set; } = string.Empty;
    public string Rating { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}

public sealed class BookingTransportOption
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Details { get; set; } = string.Empty;
}

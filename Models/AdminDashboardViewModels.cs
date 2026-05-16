namespace SugboGo.Models;

public sealed class AdminDashboardViewModel
{
    public List<AdminKpiViewModel> Kpis { get; set; } = [];
    public List<VibeTrendViewModel> VibeTrends { get; set; } = [];
    public List<UrgentAlertViewModel> UrgentAlerts { get; set; } = [];
    public List<GemAdminViewModel> Gems { get; set; } = [];
    public List<ItineraryTemplateViewModel> Templates { get; set; } = [];
    public List<PipelineColumnViewModel> Pipeline { get; set; } = [];
    public List<FlashpackerProfileViewModel> Flashpackers { get; set; } = [];
    public List<PartnerAdminViewModel> Partners { get; set; } = [];
    public List<BookingAdminViewModel> Bookings { get; set; } = [];
    public List<CollaborationSuggestionViewModel> CollaborationQueue { get; set; } = [];
}

public sealed class BookingAdminViewModel
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public sealed class AdminKpiViewModel
{
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Delta { get; set; } = string.Empty;
}

public sealed class VibeTrendViewModel
{
    public string Vibe { get; set; } = string.Empty;
    public int Percentage { get; set; }
}

public sealed class UrgentAlertViewModel
{
    public string Traveler { get; set; } = string.Empty;
    public string Issue { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string ChatUrl { get; set; } = string.Empty;
}

public sealed class GemAdminViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int FlashpackerScore { get; set; }
    public string QualityCheckDate { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string Status { get; set; } = string.Empty;
    public int MapX { get; set; }
    public int MapY { get; set; }
}

public sealed class ItineraryTemplateViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Vibe { get; set; } = string.Empty;
    public string Stops { get; set; } = string.Empty;
    public string AvgDuration { get; set; } = string.Empty;
}

public sealed class PipelineColumnViewModel
{
    public string Name { get; set; } = string.Empty;
    public List<PipelineCardViewModel> Cards { get; set; } = [];
}

public sealed class PipelineCardViewModel
{
    public string Traveler { get; set; } = string.Empty;
    public string Vibe { get; set; } = string.Empty;
    public string TravelWindow { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
}

public sealed class FlashpackerProfileViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Vibe { get; set; } = string.Empty;
    public string Constraints { get; set; } = string.Empty;
    public string Feedback { get; set; } = string.Empty;
    public int CebuTrips { get; set; }
}

public sealed class PartnerAdminViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Contact { get; set; } = string.Empty;
    public string Commission { get; set; } = string.Empty;
    public string LastAudit { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public sealed class CollaborationSuggestionViewModel
{
    public string SuggestedBy { get; set; } = string.Empty;
    public string Spot { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string ApprovalStatus { get; set; } = string.Empty;
}

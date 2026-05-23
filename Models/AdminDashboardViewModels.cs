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
    public List<string> SelectedActivities { get; set; } = [];
    public string SelectedTransport { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string TravelerType { get; set; } = string.Empty;
    public int TravelerCount { get; set; }
}
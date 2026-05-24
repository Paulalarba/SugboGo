using Microsoft.EntityFrameworkCore;
using SugboGo.Data;
using SugboGo.Models;

namespace SugboGo.Services.Admin;

public sealed class PartnerOperationsService : IPartnerOperationsService
{
    private readonly SugboGoDbContext _dbContext;
    private readonly IAdminDataStore _adminDataStore;

    public PartnerOperationsService(SugboGoDbContext dbContext, IAdminDataStore adminDataStore)
    {
        _dbContext = dbContext;
        _adminDataStore = adminDataStore;
    }

    public async Task<PartnerDashboardViewModel> BuildDashboardAsync(string userId, CancellationToken cancellationToken = default)
    {
        var partners = await _adminDataStore.GetPartnersAsync(cancellationToken);
        var partner = partners.FirstOrDefault(p => p.UserId == userId);
        
        if (partner == null) return new PartnerDashboardViewModel();

        var bookings = await _dbContext.Bookings
            .Where(b => b.AssignedPartnerId == partner.Id)
            .OrderByDescending(b => b.TravelDate)
            .ToListAsync(cancellationToken);

        var users = await _dbContext.Users.ToListAsync(cancellationToken);

        var mappedBookings = bookings.Select(b => MapToViewModel(b, users.FirstOrDefault(u => u.Id == b.UserId))).ToList();

        return new PartnerDashboardViewModel
        {
            Profile = new PartnerAdminViewModel
            {
                Id = partner.Id,
                UserId = partner.UserId,
                Name = partner.Name,
                Type = partner.Type,
                Contact = partner.Contact,
                Commission = partner.Commission,
                LastAudit = partner.LastAudit,
                Status = partner.Status
            },
            ActiveTrips = mappedBookings.Where(b => b.Status == "In Progress").ToList(),
            UpcomingBookings = mappedBookings.Where(b => b.Status == "Paid" || b.Status == "Confirmed").ToList(),
            CompletedTrips = mappedBookings.Where(b => b.Status == "Completed").ToList(),
            TotalRevenue = mappedBookings.Where(b => b.Status == "Completed").Sum(b => b.Amount),
            TotalTrips = mappedBookings.Count
        };
    }

    public async Task<BookingAdminViewModel?> GetBookingDetailsAsync(string userId, string bookingId, CancellationToken cancellationToken = default)
    {
        var partners = await _adminDataStore.GetPartnersAsync(cancellationToken);
        var partner = partners.FirstOrDefault(p => p.UserId == userId);
        if (partner == null) return null;

        var booking = await _dbContext.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId && b.AssignedPartnerId == partner.Id, cancellationToken);
        if (booking == null) return null;

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == booking.UserId, cancellationToken);

        return MapToViewModel(booking, user);
    }

    public async Task UpdateBookingStatusAsync(string userId, string bookingId, string status, CancellationToken cancellationToken = default)
    {
        var partners = await _adminDataStore.GetPartnersAsync(cancellationToken);
        var partner = partners.FirstOrDefault(p => p.UserId == userId);
        if (partner == null) return;

        var booking = await _dbContext.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId && b.AssignedPartnerId == partner.Id, cancellationToken);
        if (booking == null) return;

        booking.Status = status;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static BookingAdminViewModel MapToViewModel(Booking b, UserAccount? user)
    {
        return new BookingAdminViewModel
        {
            Id = b.Id,
            UserName = user?.FullName ?? "Unknown User",
            UserEmail = user?.Email ?? "N/A",
            Destination = b.DestinationName,
            Date = b.TravelDate.ToString("MMM d, yyyy"),
            Status = b.Status,
            Amount = b.TotalPrice,
            TravelerType = b.TravelerType,
            TravelerCount = b.TravelerCount,
            TravelerNotes = b.TravelerNotes ?? string.Empty,
            AdminNotes = b.AdminNotes ?? string.Empty,
            PaymentMethod = b.PaymentMethod ?? "Pending",
            QrCode = b.QrCode,
            CreatedAt = b.CreatedAt.DateTime,
            SelectedActivities = b.SelectedActivitiesJson != null ? System.Text.Json.JsonSerializer.Deserialize<List<string>>(b.SelectedActivitiesJson) ?? [] : [],
            SelectedTransport = b.SelectedTransportationJson != null ? System.Text.Json.JsonSerializer.Deserialize<string>(b.SelectedTransportationJson) ?? string.Empty : string.Empty,
            SelectedAccommodation = b.SelectedAccommodationJson != null ? System.Text.Json.JsonSerializer.Deserialize<string>(b.SelectedAccommodationJson) ?? string.Empty : string.Empty
        };
    }
}

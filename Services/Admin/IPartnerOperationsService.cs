using SugboGo.Models;

namespace SugboGo.Services.Admin;

public interface IPartnerOperationsService
{
    Task<PartnerDashboardViewModel> BuildDashboardAsync(string userId, CancellationToken cancellationToken = default);
    Task<BookingAdminViewModel?> GetBookingDetailsAsync(string userId, string bookingId, CancellationToken cancellationToken = default);
    Task UpdateBookingStatusAsync(string userId, string bookingId, string status, CancellationToken cancellationToken = default);
}

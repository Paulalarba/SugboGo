using SugboGo.Models;

namespace SugboGo.Services.Admin;

public interface IAdminOperationsService
{
    Task<AdminDashboardViewModel> BuildDashboardAsync(CancellationToken cancellationToken = default);
    
    Task<GemAdminViewModel?> GetGemByIdAsync(string id, CancellationToken cancellationToken = default);
    Task AddGemAsync(GemAdminViewModel model, CancellationToken cancellationToken = default);
    Task UpdateGemAsync(string id, GemAdminViewModel model, CancellationToken cancellationToken = default);
    Task DeleteGemAsync(string id, CancellationToken cancellationToken = default);
    
    Task<BookingAdminViewModel?> GetBookingByIdAsync(string id, CancellationToken cancellationToken = default);
    Task UpdateBookingAsync(string id, string status, string? assignedPartnerId, string? adminNotes, CancellationToken cancellationToken = default);
    Task<PartnerAdminViewModel?> GetPartnerByIdAsync(string id, CancellationToken cancellationToken = default);
    Task AddPartnerAsync(PartnerAdminViewModel model, CancellationToken cancellationToken = default);
    Task UpdatePartnerAsync(string id, PartnerAdminViewModel model, CancellationToken cancellationToken = default);
    Task DeletePartnerAsync(string id, CancellationToken cancellationToken = default);
}

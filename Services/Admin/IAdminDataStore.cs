using SugboGo.Models;

namespace SugboGo.Services.Admin;

public interface IAdminDataStore
{
    Task<List<AdminGem>> GetGemsAsync(CancellationToken cancellationToken = default);
    Task<List<ItineraryTemplate>> GetTemplatesAsync(CancellationToken cancellationToken = default);
    Task<List<AdminPartner>> GetPartnersAsync(CancellationToken cancellationToken = default);
    Task SeedAsync(CancellationToken cancellationToken = default);
}

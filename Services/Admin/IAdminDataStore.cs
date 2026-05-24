using SugboGo.Models;

namespace SugboGo.Services.Admin;

public interface IAdminDataStore
{
    Task<List<AdminGem>> GetGemsAsync(CancellationToken cancellationToken = default);
    Task<AdminGem?> GetGemByIdAsync(string id, CancellationToken cancellationToken = default);
    Task AddGemAsync(AdminGem gem, CancellationToken cancellationToken = default);
    Task UpdateGemAsync(string id, AdminGem gem, CancellationToken cancellationToken = default);
    Task DeleteGemAsync(string id, CancellationToken cancellationToken = default);
    
    Task<List<ItineraryTemplate>> GetTemplatesAsync(CancellationToken cancellationToken = default);
    Task<List<AdminPartner>> GetPartnersAsync(CancellationToken cancellationToken = default);
    Task<AdminPartner?> GetPartnerByIdAsync(string id, CancellationToken cancellationToken = default);
    Task AddPartnerAsync(AdminPartner partner, CancellationToken cancellationToken = default);
    Task UpdatePartnerAsync(string id, AdminPartner partner, CancellationToken cancellationToken = default);
    Task DeletePartnerAsync(string id, CancellationToken cancellationToken = default);
    Task SeedAsync(CancellationToken cancellationToken = default);
}

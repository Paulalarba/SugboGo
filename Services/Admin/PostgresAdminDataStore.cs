using Microsoft.EntityFrameworkCore;
using SugboGo.Data;
using SugboGo.Models;

namespace SugboGo.Services.Admin;

public sealed class PostgresAdminDataStore : IAdminDataStore
{
    private readonly SugboGoDbContext _dbContext;

    public PostgresAdminDataStore(SugboGoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<AdminGem>> GetGemsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.AdminGems.ToListAsync(cancellationToken);
    }

    public async Task<AdminGem?> GetGemByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AdminGems.FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public async Task AddGemAsync(AdminGem gem, CancellationToken cancellationToken = default)
    {
        _dbContext.AdminGems.Add(gem);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateGemAsync(string id, AdminGem gem, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.AdminGems.FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
        if (existing == null) return;

        existing.Name = gem.Name;
        existing.Category = gem.Category;
        existing.FlashpackerScore = gem.FlashpackerScore;
        existing.ContactPerson = gem.ContactPerson;
        existing.Latitude = gem.Latitude;
        existing.Longitude = gem.Longitude;
        existing.Status = gem.Status;
        existing.MapX = gem.MapX;
        existing.MapY = gem.MapY;
        existing.QualityCheckDate = DateTime.UtcNow.ToString("MMM d, yyyy");

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteGemAsync(string id, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.AdminGems.FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
        if (existing == null) return;

        _dbContext.AdminGems.Remove(existing);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<ItineraryTemplate>> GetTemplatesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.ItineraryTemplates.ToListAsync(cancellationToken);
    }

    public async Task<List<AdminPartner>> GetPartnersAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.AdminPartners.ToListAsync(cancellationToken);
    }

    public async Task<AdminPartner?> GetPartnerByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AdminPartners.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task AddPartnerAsync(AdminPartner partner, CancellationToken cancellationToken = default)
    {
        _dbContext.AdminPartners.Add(partner);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdatePartnerAsync(string id, AdminPartner partner, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.AdminPartners.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (existing == null) return;

        existing.Name = partner.Name;
        existing.UserId = partner.UserId;
        existing.Type = partner.Type;
        existing.Contact = partner.Contact;
        existing.Commission = partner.Commission;
        existing.Status = partner.Status;
        existing.LastAudit = DateTime.UtcNow.ToString("MMM d, yyyy");

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeletePartnerAsync(string id, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.AdminPartners.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (existing == null) return;

        _dbContext.AdminPartners.Remove(existing);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!await _dbContext.AdminGems.AnyAsync(cancellationToken))
        {
            List<AdminGem> gems =
            [
                new() { Name = "Hidden Heritage Cafe", Category = "Cafe", FlashpackerScore = 9, QualityCheckDate = "May 1, 2026", ContactPerson = "Ana Lim", Latitude = 10.2961m, Longitude = 123.8993m, Status = "Active", MapX = 34, MapY = 58 },
                new() { Name = "Private Mountain View", Category = "Viewpoint", FlashpackerScore = 8, QualityCheckDate = "Apr 22, 2026", ContactPerson = "Ramon Uy", Latitude = 10.3713m, Longitude = 123.8830m, Status = "Seasonal", MapX = 47, MapY = 28 },
                new() { Name = "Museo Alley Studio", Category = "Museum", FlashpackerScore = 7, QualityCheckDate = "Apr 18, 2026", ContactPerson = "Tessa Co", Latitude = 10.3002m, Longitude = 123.8967m, Status = "Under Review", MapX = 39, MapY = 54 },
                new() { Name = "Curated Rooftop Dinner", Category = "Dining", FlashpackerScore = 10, QualityCheckDate = "May 3, 2026", ContactPerson = "Marco Dizon", Latitude = 10.3190m, Longitude = 123.9057m, Status = "Active", MapX = 63, MapY = 48 },
                new() { Name = "Sardine Run Secret Entry", Category = "Wildlife", FlashpackerScore = 9, QualityCheckDate = "May 5, 2026", ContactPerson = "Benjie Moal", Latitude = 9.9400m, Longitude = 123.3667m, Status = "Active", MapX = 20, MapY = 85 },
                new() { Name = "Bantayan Sandbar Secret", Category = "Beach", FlashpackerScore = 10, QualityCheckDate = "May 10, 2026", ContactPerson = "Lani StaFe", Latitude = 11.1667m, Longitude = 123.7333m, Status = "Active", MapX = 50, MapY = 10 },
                new() { Name = "Busay Flower Ridge", Category = "Garden", FlashpackerScore = 8, QualityCheckDate = "May 12, 2026", ContactPerson = "Tita Bu", Latitude = 10.3800m, Longitude = 123.8600m, Status = "Active", MapX = 45, MapY = 40 }
            ];
            _dbContext.AdminGems.AddRange(gems);
        }

        if (!await _dbContext.ItineraryTemplates.AnyAsync(cancellationToken))
        {
            List<ItineraryTemplate> templates =
            [
                new() { Name = "Urban Explorer 36h", Vibe = "Urban Explorer", Stops = "Cafe, gallery, rooftop dinner", AvgDuration = "1.5 days" },
                new() { Name = "Heritage Hunter Core", Vibe = "Heritage Hunter", Stops = "Parian walk, museum, ancestral supper", AvgDuration = "1 day" },
                new() { Name = "Soft Mountain Reset", Vibe = "Soft Adventure", Stops = "Design stay, ridge route, garden hideout", AvgDuration = "2 days" }
            ];
            _dbContext.ItineraryTemplates.AddRange(templates);
        }

        if (!await _dbContext.AdminPartners.AnyAsync(cancellationToken))
        {
            List<AdminPartner> partners =
            [
                new() { Id = Guid.NewGuid().ToString("N"), Name = "Metro Cebu Transpo Services", Type = "Transport", Contact = "+63 917 111 2222", Commission = "10%", LastAudit = "May 20, 2026", Status = "Confirmed" },
                new() { Id = Guid.NewGuid().ToString("N"), Name = "Mactan Island Boat Charters", Type = "Maritime", Contact = "info@mactanboats.ph", Commission = "15%", LastAudit = "May 18, 2026", Status = "Excellent" },
                new() { Id = Guid.NewGuid().ToString("N"), Name = "Kawasan Canyoneering Experts", Type = "Guiding", Contact = "+63 928 333 4444", Commission = "Per head", LastAudit = "May 22, 2026", Status = "Excellent" },
                new() { Id = Guid.NewGuid().ToString("N"), Name = "Kota Beach Resort & Logistics", Type = "Lodging", Contact = "stay@kotabeach.com", Commission = "12%", LastAudit = "May 15, 2026", Status = "Confirmed" },
                new() { Id = Guid.NewGuid().ToString("N"), Name = "North Express Van Group", Type = "Transport", Contact = "+63 905 555 6666", Commission = "8%", LastAudit = "May 10, 2026", Status = "Watch weather" },
                new() { Id = Guid.NewGuid().ToString("N"), Name = "Sumilon Island Ferry Services", Type = "Maritime", Contact = "oslobferry@gmail.com", Commission = "10%", LastAudit = "May 21, 2026", Status = "Excellent" },
                new() { Id = Guid.NewGuid().ToString("N"), Name = "Old Cebu Heritage Walks", Type = "Guiding", Contact = "guides@cebuheritage.org", Commission = "Per group", LastAudit = "Apr 30, 2026", Status = "Confirmed" },
                new() { Id = Guid.NewGuid().ToString("N"), Name = "Moalboal Dive Lodge", Type = "Lodging", Contact = "dive@moalboal.ph", Commission = "15%", LastAudit = "May 05, 2026", Status = "Excellent" },
                new() { Id = Guid.NewGuid().ToString("N"), Name = "Camotes Ocean Fast Ferry", Type = "Maritime", Contact = "booking@camotesfast.ph", Commission = "5%", LastAudit = "May 12, 2026", Status = "Confirmed" },
                new() { Id = Guid.NewGuid().ToString("N"), Name = "South Road Van Rentals", Type = "Transport", Contact = "+63 919 777 8888", Commission = "10%", LastAudit = "May 19, 2026", Status = "Confirmed" },
                new() { Id = Guid.NewGuid().ToString("N"), Name = "Bantayan Island Hopping Co.", Type = "Maritime", Contact = "island@bantayan.ph", Commission = "20%", LastAudit = "May 23, 2026", Status = "Excellent" },
                new() { Id = Guid.NewGuid().ToString("N"), Name = "Thresher Shark Divers Malapascua", Type = "Guiding", Contact = "dive@malapascua.com", Commission = "15%", LastAudit = "May 08, 2026", Status = "Excellent" },
                new() { Id = Guid.NewGuid().ToString("N"), Name = "Balamban Eco Mountain Trail Guides", Type = "Guiding", Contact = "+63 947 999 0000", Commission = "Per trail", LastAudit = "May 14, 2026", Status = "Watch weather" },
                new() { Id = Guid.NewGuid().ToString("N"), Name = "Carcar Chicharon & Tour Logistics", Type = "Transport", Contact = "tours@carcar.gov.ph", Commission = "7%", LastAudit = "May 01, 2026", Status = "Confirmed" },
                new() { Id = Guid.NewGuid().ToString("N"), Name = "Danao Port Adventure Shuttles", Type = "Transport", Contact = "+63 922 123 4567", Commission = "10%", LastAudit = "May 17, 2026", Status = "Confirmed" },
                new() { Id = Guid.NewGuid().ToString("N"), Name = "Santiago Bay Garden Resort Camotes", Type = "Lodging", Contact = "info@santiagobay.com", Commission = "12%", LastAudit = "May 11, 2026", Status = "Excellent" },
                new() { Id = Guid.NewGuid().ToString("N"), Name = "Toledo Lake Bensis Guided Tours", Type = "Guiding", Contact = "+63 933 222 3333", Commission = "Per group", LastAudit = "May 20, 2026", Status = "Confirmed" },
                new() { Id = Guid.NewGuid().ToString("N"), Name = "Daanbantayan Northern Tip Marine", Type = "Maritime", Contact = "marine@daanbantayan.ph", Commission = "15%", LastAudit = "May 16, 2026", Status = "Excellent" },
                new() { Id = Guid.NewGuid().ToString("N"), Name = "San Remigio Beach Club Logistics", Type = "Lodging", Contact = "logistics@sanremigio.ph", Commission = "10%", LastAudit = "May 09, 2026", Status = "Confirmed" }
            ];
            _dbContext.AdminPartners.AddRange(partners);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

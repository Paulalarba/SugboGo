using SugboGo.Models;

namespace SugboGo.Services.Admin;

public interface IAdminOperationsService
{
    Task<AdminDashboardViewModel> BuildDashboardAsync(CancellationToken cancellationToken = default);
}

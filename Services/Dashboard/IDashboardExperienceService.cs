using System.Security.Claims;
using SugboGo.Models;

namespace SugboGo.Services.Dashboard;

public interface IDashboardExperienceService
{
    Task<DashboardViewModel> BuildForUserAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default);
    Task<UserProfilePageViewModel> BuildProfileForUserAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default);
}

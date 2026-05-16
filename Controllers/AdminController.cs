using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SugboGo.Services.Auth;
using SugboGo.Services.Admin;

namespace SugboGo.Controllers;

[Authorize(Roles = AccountRoles.Admin)]
public sealed class AdminController : Controller
{
    private readonly IAdminOperationsService _adminOperationsService;

    public AdminController(IAdminOperationsService adminOperationsService)
    {
        _adminOperationsService = adminOperationsService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Admin Dashboard";
        return View(await _adminOperationsService.BuildDashboardAsync(cancellationToken));
    }
}

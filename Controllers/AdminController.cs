using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SugboGo.Services.Auth;
using SugboGo.Services.Admin;
using SugboGo.Models;

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

    // Master Booking Ledger
    public async Task<IActionResult> Bookings(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Master Bookings";
        return View(await _adminOperationsService.BuildDashboardAsync(cancellationToken));
    }

    // Travelers CRM / Profiles
    public async Task<IActionResult> Travelers(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Traveler Profiles";
        return View(await _adminOperationsService.BuildDashboardAsync(cancellationToken));
    }

    // Hidden Gems Content Inventory
    public async Task<IActionResult> Gems(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Curated Hidden Gems";
        return View(await _adminOperationsService.BuildDashboardAsync(cancellationToken));
    }

    // --- HIDDEN GEMS CRUD OPERATIONS ---

    // GET: /Admin/CreateGem
    [HttpGet]
    public IActionResult CreateGem()
    {
        ViewData["Title"] = "Add New Hidden Gem";
        return View();
    }

    // POST: /Admin/CreateGem
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateGem(TravelSpot model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Backend integration point hook:
        // await _adminOperationsService.AddHiddenGemAsync(model, cancellationToken);

        return RedirectToAction(nameof(Gems));
    }

    // GET: /Admin/EditGem/{id}
    [HttpGet("Admin/EditGem/{id:int}")]
    public async Task<IActionResult> EditGem(int id, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Edit Hidden Gem";

        // Backend integration point hook:
        // var gem = await _adminOperationsService.GetGemByIdAsync(id, cancellationToken);
        // if (gem == null) return NotFound();
        // return View(gem);

        return View();
    }

    // POST: /Admin/EditGem/{id}
    [HttpPost("Admin/EditGem/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditGem(int id, TravelSpot model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Backend integration point hook:
        // await _adminOperationsService.UpdateHiddenGemAsync(id, model, cancellationToken);

        return RedirectToAction(nameof(Gems));
    }

    // POST: /Admin/DeleteGem/{id}
    [HttpPost("Admin/DeleteGem/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteGem(int id, CancellationToken cancellationToken)
    {
        // Backend integration point hook:
        // await _adminOperationsService.DeleteHiddenGemAsync(id, cancellationToken);

        return RedirectToAction(nameof(Gems));
    }

    // B2B Vendor Partners Network
    public async Task<IActionResult> Partners(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Vendor Partners";
        return View(await _adminOperationsService.BuildDashboardAsync(cancellationToken));
    }

    // --- B2B VENDOR PARTNERS CRUD ---

    // GET: /Admin/CreatePartner
    [HttpGet]
    public IActionResult CreatePartner()
    {
        ViewData["Title"] = "Register Vendor Partner";
        return View();
    }

    // POST: /Admin/CreatePartner
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePartner(dynamic model, CancellationToken cancellationToken)
    {
        // Note: Replace 'dynamic' with your exact Partner model type name if different from AdminPartner
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Backend integration point hook:
        // await _adminOperationsService.AddPartnerAsync(model, cancellationToken);

        return RedirectToAction(nameof(Partners));
    }

    // GET: /Admin/EditPartner/{id}
    [HttpGet("Admin/EditPartner/{id:int}")]
    public async Task<IActionResult> EditPartner(int id, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Edit Vendor Partner";

        // Backend integration point hook:
        // var partner = await _adminOperationsService.GetPartnerByIdAsync(id, cancellationToken);
        // if (partner == null) return NotFound();
        // return View(partner);

        return View();
    }

    // POST: /Admin/EditPartner/{id}
    [HttpPost("Admin/EditPartner/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPartner(int id, dynamic model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Backend integration point hook:
        // await _adminOperationsService.UpdatePartnerAsync(id, model, cancellationToken);

        return RedirectToAction(nameof(Partners));
    }

    // POST: /Admin/DeletePartner/{id}
    [HttpPost("Admin/DeletePartner/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePartner(int id, CancellationToken cancellationToken)
    {
        // Backend integration point hook:
        // await _adminOperationsService.DeletePartnerAsync(id, cancellationToken);

        return RedirectToAction(nameof(Partners));
    }
}

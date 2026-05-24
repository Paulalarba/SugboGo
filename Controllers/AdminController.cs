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

    // GET: /Admin/BookingDetails/{id}
    [HttpGet("Admin/BookingDetails/{id}")]
    public async Task<IActionResult> BookingDetails(string id, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Booking Management Detail";
        var booking = await _adminOperationsService.GetBookingByIdAsync(id, cancellationToken);
        if (booking == null) return NotFound();

        var dashboard = await _adminOperationsService.BuildDashboardAsync(cancellationToken);
        ViewBag.Partners = dashboard.Partners;
        
        return View(booking);
    }

    // POST: /Admin/UpdateBooking/{id}
    [HttpPost("Admin/UpdateBooking/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateBooking(string id, string status, string? assignedPartnerId, string? adminNotes, CancellationToken cancellationToken)
    {
        await _adminOperationsService.UpdateBookingAsync(id, status, assignedPartnerId, adminNotes, cancellationToken);
        return RedirectToAction(nameof(BookingDetails), new { id });
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
        return View(new GemAdminViewModel());
    }

    // POST: /Admin/CreateGem
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateGem(GemAdminViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _adminOperationsService.AddGemAsync(model, cancellationToken);

        return RedirectToAction(nameof(Gems));
    }

    // GET: /Admin/EditGem/{id}
    [HttpGet("Admin/EditGem/{id}")]
    public async Task<IActionResult> EditGem(string id, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Edit Hidden Gem";

        var gem = await _adminOperationsService.GetGemByIdAsync(id, cancellationToken);
        if (gem == null) return NotFound();
        
        return View(gem);
    }

    // POST: /Admin/EditGem/{id}
    [HttpPost("Admin/EditGem/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditGem(string id, GemAdminViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _adminOperationsService.UpdateGemAsync(id, model, cancellationToken);

        return RedirectToAction(nameof(Gems));
    }

    // POST: /Admin/DeleteGem/{id}
    [HttpPost("Admin/DeleteGem/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteGem(string id, CancellationToken cancellationToken)
    {
        await _adminOperationsService.DeleteGemAsync(id, cancellationToken);

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
    public async Task<IActionResult> CreatePartner(PartnerAdminViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _adminOperationsService.AddPartnerAsync(model, cancellationToken);

        return RedirectToAction(nameof(Partners));
    }

    // GET: /Admin/EditPartner/{id}
    [HttpGet("Admin/EditPartner/{id}")]
    public async Task<IActionResult> EditPartner(string id, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Edit Vendor Partner";

        var partner = await _adminOperationsService.GetPartnerByIdAsync(id, cancellationToken);
        if (partner == null) return NotFound();
        
        return View(partner);
    }

    // POST: /Admin/EditPartner/{id}
    [HttpPost("Admin/EditPartner/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPartner(string id, PartnerAdminViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _adminOperationsService.UpdatePartnerAsync(id, model, cancellationToken);

        return RedirectToAction(nameof(Partners));
    }

    // POST: /Admin/DeletePartner/{id}
    [HttpPost("Admin/DeletePartner/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePartner(string id, CancellationToken cancellationToken)
    {
        await _adminOperationsService.DeletePartnerAsync(id, cancellationToken);

        return RedirectToAction(nameof(Partners));
    }
}

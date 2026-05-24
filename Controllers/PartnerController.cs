using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SugboGo.Services.Admin;
using SugboGo.Services.Auth;

namespace SugboGo.Controllers;

[Authorize(Roles = AccountRoles.Partner)]
public sealed class PartnerController : Controller
{
    private readonly IPartnerOperationsService _partnerOperationsService;

    public PartnerController(IPartnerOperationsService partnerOperationsService)
    {
        _partnerOperationsService = partnerOperationsService;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Partner Command Center";
        var dashboard = await _partnerOperationsService.BuildDashboardAsync(GetUserId(), cancellationToken);
        return View(dashboard);
    }

    public async Task<IActionResult> Bookings(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Assigned Itineraries";
        var dashboard = await _partnerOperationsService.BuildDashboardAsync(GetUserId(), cancellationToken);
        return View(dashboard);
    }

    [HttpGet("Partner/BookingDetails/{id}")]
    public async Task<IActionResult> BookingDetails(string id, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Itinerary Details";
        var booking = await _partnerOperationsService.GetBookingDetailsAsync(GetUserId(), id, cancellationToken);
        if (booking == null) return NotFound();

        return View(booking);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(string bookingId, string status, CancellationToken cancellationToken)
    {
        await _partnerOperationsService.UpdateBookingStatusAsync(GetUserId(), bookingId, status, cancellationToken);
        return RedirectToAction(nameof(BookingDetails), new { id = bookingId });
    }
}

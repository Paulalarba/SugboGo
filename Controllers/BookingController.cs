using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SugboGo.Data;
using SugboGo.Models;
using SugboGo.Services.BookingOptions;
using SugboGo.Services.Dashboard;
using SugboGo.Services.Travel;

namespace SugboGo.Controllers;

public class BookingController : Controller
{
    private readonly ITravelPreferenceStore _preferenceStore;
    private readonly ICebuRecommendationService _recommendationService;
    private readonly IBookingOptionsService _optionsService;
    private readonly IUserSavedGemStore _savedGemStore;
    private readonly SugboGoDbContext _dbContext;
    private readonly ILogger<BookingController> _logger;

    public BookingController(
        ITravelPreferenceStore preferenceStore,
        ICebuRecommendationService recommendationService,
        IBookingOptionsService optionsService,
        IUserSavedGemStore savedGemStore,
        SugboGoDbContext dbContext,
        ILogger<BookingController> logger)
    {
        _preferenceStore = preferenceStore;
        _recommendationService = recommendationService;
        _optionsService = optionsService;
        _savedGemStore = savedGemStore;
        _dbContext = dbContext;
        _logger = logger;
    }

    // STEP 1: Entry point
    [Authorize]
    public async Task<IActionResult> Start()
    {
        var userId = GetUserId();
        var preferences = await _preferenceStore.FindLatestByUserIdAsync(userId);

        if (preferences == null)
        {
            return RedirectToAction(nameof(Survey));
        }

        return RedirectToAction(nameof(ChoosePath));
    }

    // STEP 2: The Decision Phase
    [Authorize]
    public async Task<IActionResult> ChoosePath(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Choose Your Journey";
        ViewBag.CuratedDestinations = await GetCuratedLibraryAsync(cancellationToken);
        return View();
    }

    // STEP 3: The AI Resolver — shows ranked recommendations for user to pick from
    [Authorize]
    public async Task<IActionResult> ResolveAiDestination(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var preferences = await _preferenceStore.FindLatestByUserIdAsync(userId, cancellationToken);

        if (preferences == null) return RedirectToAction(nameof(Survey));

        var results = await _recommendationService.BuildRecommendationsAsync(preferences);

        if (!results.Recommendations.Any())
        {
            return RedirectToAction(nameof(Index), new { type = "UserSelected" });
        }

        ViewData["Title"] = "AI Cebu Recommendations";
        return View("Recommendations", results);
    }

    // STEP 4: The Booking Wizard
    [Authorize]
    [HttpGet]
    [Route("Booking")]
    [Route("Booking/Index")]
    public async Task<IActionResult> Index(
        [FromQuery(Name = "id")] int? id, 
        [FromQuery(Name = "spotId")] int? spotId,
        [FromQuery(Name = "destinationId")] int? destinationId,
        string? destination, 
        decimal? price, 
        string? image, 
        string type = "UserSelected")
    {
        var userId = GetUserId();
        var preferences = await _preferenceStore.FindLatestByUserIdAsync(userId);
        if (preferences is null)
        {
            return RedirectToAction(nameof(Survey), new { returnUrl = $"{Request.Path}{Request.QueryString}" });
        }
        
        // Support multiple parameter names for maximum compatibility
        int? effectiveId = spotId ?? id ?? destinationId;
        
        TravelSpot? spot = null;
        if (effectiveId.HasValue)
        {
            spot = await _dbContext.TravelSpots.FindAsync(effectiveId.Value);
            if (spot is null)
            {
                return NotFound($"Travel spot {effectiveId.Value} was not found.");
            }
        }
        else if (!string.IsNullOrWhiteSpace(destination))
        {
            spot = await _dbContext.TravelSpots.FirstOrDefaultAsync(s => s.Name == destination);
        }

        // Build destination data with fallbacks
        var destinationData = BuildDestinationData(spot, destination ?? "Custom Adventure", price ?? 3000m, image ?? "/images/hero-bg.jpg");

        ViewBag.AllDestinations = await _dbContext.TravelSpots
            .OrderByDescending(spot => spot.IsPopular)
            .ThenBy(spot => spot.Name)
            .ToListAsync();

        var model = new BookingStepViewModel
        {
            CurrentStep = "config",
            BookingType = type,
            Preferences = preferences,
            ActivityOptions = _optionsService.GetActivities(),
            AccommodationOptions = _optionsService.GetAccommodations(),
            TransportOptions = _optionsService.GetTransportOptions(),
            SmartRecommendations = BuildSmartRecommendations(preferences, destinationData),
            Data = new BookingDataViewModel
            {
                DestinationId = destinationData.Id,
                BookingType = type,
                DestinationName = destinationData.Name,
                BasePrice = destinationData.BasePrice,
                ImageUrl = destinationData.ImageUrl,
                Description = destinationData.Description,
                Location = destinationData.Location,
                Duration = destinationData.Duration,
                BestTimeToVisit = destinationData.BestTimeToVisit,
                RatingSummary = destinationData.RatingSummary,
                MapUrl = destinationData.MapUrl,
                TotalPrice = destinationData.BasePrice,
                TravelerType = "Solo",
                TravelerCount = 1
            }
        };

        ViewData["Title"] = spot != null ? $"Book {spot.Name}" : "Choose Your Destination";
        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveFavorite(
        string destinationId,
        string title,
        string category,
        string neighborhood,
        string? returnUrl,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return BadRequest();
        }

        int? travelSpotId = int.TryParse(destinationId, out var parsedId) ? parsedId : null;
        await _savedGemStore.SaveGemAsync(new SavedGem
        {
            UserId = GetUserId(),
            TravelSpotId = travelSpotId,
            Title = title.Trim(),
            Category = string.IsNullOrWhiteSpace(category) ? "Saved destination" : category.Trim(),
            Neighborhood = neighborhood?.Trim() ?? string.Empty,
            Note = $"Saved from booking on {DateTime.Today:MMM d, yyyy}."
        }, cancellationToken);

        TempData["BookingMessage"] = $"{title.Trim()} was saved to your favorites.";

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToAction(nameof(Index), new { spotId = travelSpotId, type = "UserSelected" });
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmBooking([FromBody] BookingDataViewModel data, CancellationToken cancellationToken)
    {
        if (data == null || string.IsNullOrWhiteSpace(data.DestinationName))
        {
            return BadRequest(new { success = false, message = "Invalid booking data." });
        }

        if (!data.TravelDate.HasValue)
        {
            return BadRequest(new { success = false, message = "Please choose a travel date before completing your booking." });
        }

        var travelDate = EnsureUtc(data.TravelDate.Value);
        if (travelDate.Date <= DateTime.UtcNow.Date)
        {
            return BadRequest(new { success = false, message = "Please choose a future travel date." });
        }

        var userId = GetUserId();
        var preferences = await _preferenceStore.FindLatestByUserIdAsync(userId, cancellationToken);
        if (preferences is null)
        {
            return BadRequest(new { success = false, message = "Please complete your travel preferences before booking." });
        }

        int? travelSpotId = int.TryParse(data.DestinationId, out int id) ? id : null;
        var spot = travelSpotId.HasValue
            ? await _dbContext.TravelSpots.AsNoTracking().FirstOrDefaultAsync(s => s.Id == travelSpotId.Value, cancellationToken)
            : null;

        if (travelSpotId.HasValue && spot is null)
        {
            return BadRequest(new { success = false, message = "The selected destination is no longer available." });
        }

        var selectionType = string.Equals(data.BookingType, "SystemSelected", StringComparison.OrdinalIgnoreCase)
            ? "SystemSelected"
            : "UserSelected";
        var travelerCount = Math.Clamp(data.TravelerCount, 1, 20);
        var selectedActivities = NormalizeOptionNames(data.SelectedActivities);
        var selectedAccommodation = data.SelectedAccommodation?.Trim() ?? string.Empty;
        var selectedTransportation = data.SelectedTransportation?.Trim() ?? string.Empty;
        var priceSummary = CalculateServerPrice(
            spot?.BasePrice ?? data.BasePrice,
            selectedActivities,
            selectedAccommodation,
            selectedTransportation);

        var booking = new Booking
        {
            UserId = userId,
            TravelSpotId = travelSpotId,
            SelectionType = selectionType,
            DestinationName = spot?.Name ?? data.DestinationName.Trim(),
            ImageUrl = spot?.ImageUrl ?? data.ImageUrl,
            Location = spot?.Location ?? data.Location,
            TravelDate = travelDate,
            TravelerType = NormalizeTravelerType(data.TravelerType),
            TravelerCount = travelerCount,
            SelectedActivitiesJson = JsonSerializer.Serialize(selectedActivities),
            SelectedAccommodationJson = JsonSerializer.Serialize(new { Name = selectedAccommodation }),
            SelectedTransportationJson = JsonSerializer.Serialize(new { Name = selectedTransportation }),
            BasePrice = priceSummary.BasePrice,
            AddOnsPrice = priceSummary.AddOnsPrice,
            TaxesAndFees = priceSummary.TaxesAndFees,
            TotalPrice = priceSummary.TotalPrice,
            TravelerNotes = data.TravelerNotes,
            Status = "Confirmed",
            PaymentMethod = string.IsNullOrWhiteSpace(data.PaymentMethod) ? "Card" : data.PaymentMethod.Trim(),
            CreatedAt = DateTimeOffset.UtcNow
        };

        _dbContext.Bookings.Add(booking);
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError(exception, "Booking could not be saved for user {UserId}.", booking.UserId);
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                success = false,
                message = "Booking is temporarily unavailable while the database finishes syncing."
            });
        }

        return Json(new { success = true, bookingId = booking.Id, qrCode = booking.QrCode });
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Survey(string? returnUrl = null, CancellationToken cancellationToken = default)
    {
        ViewData["Title"] = "Cebu Preference Survey";
        var existing = await _preferenceStore.FindLatestByUserIdAsync(GetUserId(), cancellationToken);
        return View(new TravelPreferenceSurveyViewModel
        {
            SelectedPlaces = existing?.PlaceInterests ?? [],
            SelectedActivities = existing?.ActivityInterests ?? [],
            AdventureLevel = existing?.AdventureLevel ?? 3,
            TravelPace = existing?.TravelPace ?? "Balanced",
            BudgetRange = existing?.BudgetRange ?? "Mid-range",
            ReturnUrl = returnUrl,
            Notes = existing?.Notes
        });
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Survey(TravelPreferenceSurveyViewModel model, CancellationToken cancellationToken)
    {
        model.SelectedPlaces = NormalizeSelections(model.SelectedPlaces, TravelPreferenceSeedData.PlaceOptions);
        model.SelectedActivities = NormalizeSelections(model.SelectedActivities, TravelPreferenceSeedData.ActivityOptions);

        if (!ModelState.IsValid) return View(model);

        await _preferenceStore.SaveAsync(new TravelPreferenceRecord
        {
            UserId = GetUserId(),
            Email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
            PlaceInterests = model.SelectedPlaces,
            ActivityInterests = model.SelectedActivities,
            AdventureLevel = model.AdventureLevel,
            TravelPace = model.TravelPace,
            BudgetRange = model.BudgetRange,
            Notes = model.Notes
        }, cancellationToken);

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return LocalRedirect(model.ReturnUrl);
        }

        return RedirectToAction(nameof(Start));
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) 
        ?? throw new InvalidOperationException("User not authenticated");

    private static BookingDestinationSeed BuildDestinationData(TravelSpot? spot, string fallbackName, decimal fallbackPrice, string fallbackImage)
    {
        if (spot != null)
        {
            return new BookingDestinationSeed(
                spot.Id.ToString(), spot.Name, spot.ImageUrl ?? $"/images/{spot.Id}.jpg",
                spot.Description, spot.Location, "1-2 days", "4.8 (Community Rated)",
                "November to May", "https://www.openstreetmap.org", spot.BasePrice,
                [], [], []);
        }
        return new BookingDestinationSeed("custom", fallbackName, fallbackImage, "Custom Experience", "Cebu", "Variable", "New", "Year-round", "", fallbackPrice, [], [], []);
    }

    private List<string> BuildSmartRecommendations(TravelPreferenceRecord? preferences, BookingDestinationSeed destination)
    {
        var recommendations = new List<string>();
        if (preferences != null && preferences.AdventureLevel >= 4)
            recommendations.Add("Since you love adventure, we suggest a private guide for hidden trails.");
        
        recommendations.Add($"Best matched schedule: {destination.BestTimeToVisit}.");
        return recommendations.Distinct().Take(4).ToList();
    }

    private static DateTime EnsureUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value.Date, DateTimeKind.Utc)
        };
    }

    private static List<string> NormalizeSelections(IEnumerable<string> selections, IReadOnlyList<TravelInterestOption> options)
    {
        return selections.Select(s => s.Trim().ToLowerInvariant()).Where(s => options.Any(o => o.Key == s)).Distinct().ToList();
    }

    private async Task<List<TravelSpot>> GetCuratedLibraryAsync(CancellationToken cancellationToken)
    {
        var seedSpots = TravelSpotSeedData.GetTravelSpots();

        try
        {
            var seedIds = seedSpots.Select(spot => spot.Id).ToList();
            var storedSpots = await _dbContext.TravelSpots
                .AsNoTracking()
                .Where(spot => seedIds.Contains(spot.Id))
                .ToDictionaryAsync(spot => spot.Id, cancellationToken);

            return seedSpots
                .Select(seed => storedSpots.TryGetValue(seed.Id, out var stored) ? stored : seed)
                .OrderByDescending(spot => spot.IsPopular)
                .ThenBy(spot => spot.Region)
                .ThenBy(spot => spot.Name)
                .ToList();
        }
        catch (Exception exception) when (exception is InvalidOperationException or DbUpdateException)
        {
            _logger.LogWarning(exception, "Falling back to seeded travel spots for the manual explorer library.");
            return seedSpots
                .OrderByDescending(spot => spot.IsPopular)
                .ThenBy(spot => spot.Region)
                .ThenBy(spot => spot.Name)
                .ToList();
        }
    }

    private BookingPriceSummary CalculateServerPrice(
        decimal submittedBasePrice,
        IReadOnlyCollection<string> selectedActivities,
        string selectedAccommodation,
        string selectedTransportation)
    {
        var basePrice = Math.Max(0m, submittedBasePrice);
        var activityPrice = _optionsService.GetActivities()
            .Where(option => selectedActivities.Contains(option.Name, StringComparer.OrdinalIgnoreCase))
            .Sum(option => option.Price);
        var accommodationPrice = _optionsService.GetAccommodations()
            .Where(option => option.Name.Equals(selectedAccommodation, StringComparison.OrdinalIgnoreCase))
            .Sum(option => option.PricePerNight);
        var transportPrice = _optionsService.GetTransportOptions()
            .Where(option => option.Name.Equals(selectedTransportation, StringComparison.OrdinalIgnoreCase))
            .Sum(option => option.Price);
        var addOnsPrice = activityPrice + accommodationPrice + transportPrice;
        var taxesAndFees = Math.Round((basePrice + addOnsPrice) * 0.12m, 2, MidpointRounding.AwayFromZero);

        return new BookingPriceSummary(basePrice, addOnsPrice, taxesAndFees, basePrice + addOnsPrice + taxesAndFees);
    }

    private static List<string> NormalizeOptionNames(IEnumerable<string>? values)
    {
        return values?
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList() ?? [];
    }

    private static string NormalizeTravelerType(string? value)
    {
        var normalized = value?.Trim();
        return normalized is "Solo" or "Couple" or "Group" or "Family" ? normalized : "Solo";
    }

    private sealed record BookingDestinationSeed(string Id, string Name, string ImageUrl, string Description, string Location, string Duration, string RatingSummary, string BestTimeToVisit, string MapUrl, decimal BasePrice, List<BookingActivityOption> Activities, List<BookingAccommodationOption> Accommodations, List<BookingTransportOption> TransportOptions);
    private sealed record BookingPriceSummary(decimal BasePrice, decimal AddOnsPrice, decimal TaxesAndFees, decimal TotalPrice);
}

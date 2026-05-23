using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SugboGo.Data;
using SugboGo.Models;
using SugboGo.Services.Auth;
using SugboGo.Services.Dashboard;
using SugboGo.Services.Travel;

namespace SugboGo.Controllers;

[Authorize(Roles = AccountRoles.AdminOrClient)]
public sealed class DashboardController : Controller
{
    private readonly IDashboardExperienceService _dashboardExperienceService;
    private readonly IDestinationPostStore _postStore;
    private readonly IUserSavedGemStore _savedGemStore;
    private readonly ITravelPreferenceStore _preferenceStore;
    private readonly IUserAccountStore _userStore;
    private readonly IUserSignInService _signInService;
    private readonly SugboGoDbContext _dbContext;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IDashboardExperienceService dashboardExperienceService,
        IDestinationPostStore postStore,
        IUserSavedGemStore savedGemStore,
        ITravelPreferenceStore preferenceStore,
        IUserAccountStore userStore,
        IUserSignInService signInService,
        SugboGoDbContext dbContext,
        IWebHostEnvironment environment,
        ILogger<DashboardController> logger)
    {
        _dashboardExperienceService = dashboardExperienceService;
        _postStore = postStore;
        _savedGemStore = savedGemStore;
        _preferenceStore = preferenceStore;
        _userStore = userStore;
        _signInService = signInService;
        _dbContext = dbContext;
        _environment = environment;
        _logger = logger;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Dashboard";
        return View("~/Views/User/Dashboard/Index.cshtml", await _dashboardExperienceService.BuildForUserAsync(User, cancellationToken));
    }

    public async Task<IActionResult> Profile(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Profile";
        var viewModel = await _dashboardExperienceService.BuildForProfileAsync(User, cancellationToken);
        return View("~/Views/User/Dashboard/Profile.cshtml", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(string fullName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            TempData["ProfileError"] = "Full name is required.";
            return RedirectToAction(nameof(Profile));
        }

        var userId = GetUserId();
        var user = await _userStore.FindByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return NotFound();
        }

        user.FullName = fullName.Trim();
        await _userStore.UpdateAsync(user, cancellationToken);

        // We need to re-sign in to update the name claim in the cookie
        await _signInService.SignInAsync(HttpContext, user, rememberMe: true);

        TempData["ProfileMessage"] = "Profile updated successfully.";
        return RedirectToAction(nameof(Profile));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateTravelPreferences(
        List<string> selectedPlaces,
        List<string> selectedActivities,
        int adventureLevel,
        string travelPace,
        string budgetRange,
        string? notes,
        CancellationToken cancellationToken)
    {
        selectedPlaces = NormalizeSelections(selectedPlaces, TravelPreferenceSeedData.PlaceOptions);
        selectedActivities = NormalizeSelections(selectedActivities, TravelPreferenceSeedData.ActivityOptions);

        if (selectedPlaces.Count == 0 || selectedActivities.Count == 0)
        {
            TempData["ProfileError"] = "Choose at least one place and one activity for your recommendation profile.";
            return RedirectToAction(nameof(Profile));
        }

        var userId = GetUserId();
        var existing = await _preferenceStore.FindLatestByUserIdAsync(userId, cancellationToken);

        await _preferenceStore.SaveAsync(new TravelPreferenceRecord
        {
            Id = existing?.Id ?? Guid.NewGuid().ToString("N"),
            UserId = userId,
            Email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
            PlaceInterests = selectedPlaces,
            ActivityInterests = selectedActivities,
            AdventureLevel = Math.Clamp(adventureLevel, 1, 5),
            TravelPace = NormalizeChoice(travelPace, ["Relaxed", "Balanced", "Packed"], "Balanced"),
            BudgetRange = NormalizeChoice(budgetRange, ["Budget", "Mid-range", "Premium"], "Mid-range"),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
            CreatedAt = existing?.CreatedAt ?? DateTimeOffset.UtcNow
        }, cancellationToken);

        TempData["ProfileMessage"] = "Travel preference reference saved to your profile.";

        var user = await _dbContext.Users.FindAsync([userId], cancellationToken);
        if (user != null)
        {
            user.HasCompletedSurvey = true;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return RedirectToAction(nameof(Profile));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateCheckoutPreference(
        string firstName,
        string lastName,
        string addressLine1,
        string city,
        string stateProvince,
        string postalCode,
        string emailAddress,
        string cardholderName,
        string cardLast4,
        string cardExpiry,
        string paymentMethod,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var preference = await _dbContext.UserCheckoutPreferences
            .FirstOrDefaultAsync(item => item.UserId == userId, cancellationToken);

        preference ??= new UserCheckoutPreference { UserId = userId };
        preference.FirstName = Clean(firstName, 80);
        preference.LastName = Clean(lastName, 80);
        preference.AddressLine1 = Clean(addressLine1, 180);
        preference.City = Clean(city, 90);
        preference.StateProvince = Clean(stateProvince, 90);
        preference.PostalCode = Clean(postalCode, 24);
        preference.EmailAddress = Clean(emailAddress, 160);
        preference.CardholderName = Clean(cardholderName, 100);
        preference.CardLast4 = LastFourDigits(cardLast4);
        preference.CardExpiry = Clean(cardExpiry, 7);
        preference.PaymentMethod = string.IsNullOrWhiteSpace(paymentMethod) ? "Card" : Clean(paymentMethod, 40);
        preference.UpdatedAt = DateTimeOffset.UtcNow;

        if (_dbContext.Entry(preference).State == EntityState.Detached)
        {
            _dbContext.UserCheckoutPreferences.Add(preference);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        TempData["ProfileMessage"] = "Checkout reference saved successfully.";
        return RedirectToAction(nameof(Profile));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveGem(string title, string category, string neighborhood, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return BadRequest();
        }

        var gem = await _savedGemStore.SaveGemAsync(new SavedGem
        {
            UserId = GetUserId(),
            Title = title.Trim(),
            Category = category?.Trim() ?? string.Empty,
            Neighborhood = neighborhood?.Trim() ?? string.Empty,
            Note = $"Saved from AI recommendations on {DateTime.Today:MMM d, yyyy}."
        }, cancellationToken);

        return Json(new { id = gem.Id, title = gem.Title });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveGem(string gemId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(gemId))
        {
            return BadRequest();
        }

        var removed = await _savedGemStore.RemoveGemAsync(GetUserId(), gemId, cancellationToken);
        return removed ? Ok() : NotFound();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePost(
        string destination,
        string location,
        string description,
        string? caption,
        string tag,
        IFormFile? photo,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(destination) ||
            string.IsNullOrWhiteSpace(location) ||
            string.IsNullOrWhiteSpace(description))
        {
            TempData["DashboardError"] = "Destination, location, and travel experience are required.";
            return RedirectToAction(nameof(Index));
        }

        var imageFileName = await SavePostImageAsync(photo, cancellationToken);
        await _postStore.CreateAsync(new DestinationPost
        {
            UserId = GetUserId(),
            AuthorName = User.FindFirstValue(ClaimTypes.Name) ?? "Traveler",
            AuthorEmail = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
            DestinationName = destination.Trim(),
            Location = location.Trim(),
            Description = description.Trim(),
            Caption = caption?.Trim() ?? string.Empty,
            Tag = NormalizeTag(tag),
            ImageFileName = imageFileName,
            CreatedAt = DateTimeOffset.UtcNow
        }, cancellationToken);

        TempData["DashboardMessage"] = "Your Cebu destination post is live.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LikePost(string postId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(postId))
        {
            return BadRequest();
        }

        DestinationPost? post;
        try
        {
            post = await _postStore.IncrementLikesAsync(postId, cancellationToken);
        }
        catch (InvalidOperationException exception) when (exception.InnerException is not null)
        {
            _logger.LogWarning(exception, "Could not increment likes for post {PostId}.", postId);
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                message = "Likes are temporarily unavailable. Please try again."
            });
        }

        return post is null ? NotFound() : Json(new { likes = post.Likes });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(string postId, string text, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(postId) || string.IsNullOrWhiteSpace(text))
        {
            return BadRequest();
        }

        var comment = new PostComment
        {
            UserId = GetUserId(),
            AuthorName = User.FindFirstValue(ClaimTypes.Name) ?? "Traveler",
            Text = text.Trim(),
            CreatedAt = DateTimeOffset.UtcNow
        };

        var post = await _postStore.AddCommentAsync(postId, comment, cancellationToken);

        if (post is null)
        {
            return NotFound();
        }

        return Json(new
        {
            authorName = comment.AuthorName,
            text = comment.Text,
            timestamp = comment.CreatedAt.ToLocalTime().ToString("MMM d, h:mm tt"),
            commentCount = post.Comments
        });
    }

    private async Task<string> SavePostImageAsync(IFormFile? photo, CancellationToken cancellationToken)
    {
        if (photo is null || photo.Length == 0)
        {
            return string.Empty;
        }

        var extension = Path.GetExtension(photo.FileName).ToLowerInvariant();
        var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp", ".gif" };

        if (!allowedExtensions.Contains(extension))
        {
            return string.Empty;
        }

        var uploadRoot = Path.Combine(_environment.WebRootPath, "uploads", "destination-posts");
        Directory.CreateDirectory(uploadRoot);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(uploadRoot, fileName);

        await using var stream = System.IO.File.Create(filePath);
        await photo.CopyToAsync(stream, cancellationToken);

        return fileName;
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Signed-in user is missing a name identifier claim.");
    }

    private static string NormalizeTag(string tag)
    {
        return string.IsNullOrWhiteSpace(tag)
            ? "beaches"
            : tag.Trim().ToLowerInvariant().Replace(" ", "-");
    }

    private static string Clean(string? value, int maxLength)
    {
        var cleaned = value?.Trim() ?? string.Empty;
        return cleaned.Length <= maxLength ? cleaned : cleaned[..maxLength];
    }

    private static string LastFourDigits(string? value)
    {
        var digits = new string((value ?? string.Empty).Where(char.IsDigit).ToArray());
        return digits.Length <= 4 ? digits : digits[^4..];
    }

    private static List<string> NormalizeSelections(IEnumerable<string>? selections, IReadOnlyList<TravelInterestOption> options)
    {
        return selections?
            .Where(selection => !string.IsNullOrWhiteSpace(selection))
            .Select(selection => selection.Trim().ToLowerInvariant())
            .Where(selection => options.Any(option => option.Key == selection))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList() ?? [];
    }

    private static string NormalizeChoice(string? value, IReadOnlyCollection<string> allowed, string fallback)
    {
        var normalized = value?.Trim() ?? string.Empty;
        return allowed.Contains(normalized) ? normalized : fallback;
    }
}

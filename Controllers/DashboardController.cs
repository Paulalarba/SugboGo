using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SugboGo.Models;
using SugboGo.Services.Auth;
using SugboGo.Services.Dashboard;

namespace SugboGo.Controllers;

[Authorize(Roles = AccountRoles.AdminOrClient)]
public sealed class DashboardController : Controller
{
    private readonly IDashboardExperienceService _dashboardExperienceService;
    private readonly IDestinationPostStore _postStore;
    private readonly IUserSavedGemStore _savedGemStore;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IDashboardExperienceService dashboardExperienceService,
        IDestinationPostStore postStore,
        IUserSavedGemStore savedGemStore,
        IWebHostEnvironment environment,
        ILogger<DashboardController> logger)
    {
        _dashboardExperienceService = dashboardExperienceService;
        _postStore = postStore;
        _savedGemStore = savedGemStore;
        _environment = environment;
        _logger = logger;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Dashboard";
        return View("~/Views/User/Dashboard/Index.cshtml", await _dashboardExperienceService.BuildForUserAsync(User, cancellationToken));
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
}

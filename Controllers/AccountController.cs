using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SugboGo.Models;
using SugboGo.Services.Auth;
using SugboGo.Services.Travel;

namespace SugboGo.Controllers;

public sealed class AccountController : Controller
{
    private readonly IUserAccountStore _userStore;
    private readonly IPasswordService _passwordService;
    private readonly IAccountRoleService _accountRoleService;
    private readonly ITravelPreferenceStore _preferenceStore;

    public AccountController(
        IUserAccountStore userStore,
        IPasswordService passwordService,
        IAccountRoleService accountRoleService,
        ITravelPreferenceStore preferenceStore)
    {
        _userStore = userStore;
        _passwordService = passwordService;
        _accountRoleService = accountRoleService;
        _preferenceStore = preferenceStore;
    }

    [HttpGet]
    public IActionResult Index(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectAuthenticatedUser(returnUrl);
        }

        ViewData["Title"] = "Sign in or create an account";
        return View(new EmailEntryViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckEmail(EmailEntryViewModel model, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Sign in or create an account";

        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        var email = NormalizeEmail(model.Email);
        if (RequiresGmailAccount(model.ReturnUrl) && !email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(model.Email), "Use your Gmail address to continue with SugboGo booking.");
            return View("Index", model);
        }

        var existingUser = await _userStore.FindByEmailAsync(email, cancellationToken);

        return existingUser is null
            ? RedirectToAction(nameof(Register), new { email, model.ReturnUrl })
            : RedirectToAction(nameof(SignIn), new { email, model.ReturnUrl });
    }

    [HttpGet]
    public IActionResult SignIn(string email, string? returnUrl = null)
    {
        ViewData["Title"] = "Enter your password";
        return View(new SignInViewModel { Email = NormalizeEmail(email), ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SignIn(SignInViewModel model, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Enter your password";

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userStore.FindByEmailAsync(model.Email, cancellationToken);

        if (user is null || !_passwordService.VerifyPassword(model.Password, user.PasswordHash))
        {
            model.ErrorMessage = "The password does not match this SogboGo account.";
            return View(model);
        }

        await SignUserInAsync(user);
        return await RedirectAfterAuthenticationAsync(user, model.ReturnUrl, cancellationToken);
    }

    [HttpGet]
    public IActionResult Register(string email, string? returnUrl = null)
    {
        ViewData["Title"] = "Create your account";
        return View(new RegisterViewModel { Email = NormalizeEmail(email), ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Create your account";

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var email = NormalizeEmail(model.Email);
        if (RequiresGmailAccount(model.ReturnUrl) && !email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(model.Email), "Use your Gmail address to continue with SugboGo booking.");
            return View(model);
        }

        var existingUser = await _userStore.FindByEmailAsync(email, cancellationToken);

        if (existingUser is not null)
        {
            return RedirectToAction(nameof(SignIn), new { email, model.ReturnUrl });
        }

        var user = new UserAccount
        {
            Email = email,
            FullName = model.FullName.Trim(),
            PasswordHash = _passwordService.HashPassword(model.Password),
            Role = _accountRoleService.GetRegistrationRole(email)
        };

        try
        {
            await _userStore.CreateAsync(user, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            model.ErrorMessage = ex.Message;
            return View(model);
        }

        await SignUserInAsync(user);
        return await RedirectAfterAuthenticationAsync(user, model.ReturnUrl, cancellationToken);
    }

    [HttpGet]
    public IActionResult Recover()
    {
        ViewData["Title"] = "Recover your account";
        return View(new RecoverAccountViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Recover(RecoverAccountViewModel model)
    {
        ViewData["Title"] = "Recover your account";

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        model.Submitted = true;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        ViewData["Title"] = "Access denied";
        return View();
    }

    private async Task SignUserInAsync(UserAccount user)
    {
        var role = _accountRoleService.ResolveEffectiveRole(user.Email, user.Role);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, role)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties { IsPersistent = false });
    }

    private IActionResult RedirectToRoleHome(UserAccount user)
    {
        var role = _accountRoleService.ResolveEffectiveRole(user.Email, user.Role);
        return role == AccountRoles.Admin
            ? RedirectToAction("Index", "Admin")
            : RedirectToAction("Index", "Dashboard");
    }

    private async Task<IActionResult> RedirectAfterAuthenticationAsync(UserAccount user, string? returnUrl, CancellationToken cancellationToken)
    {
        var role = _accountRoleService.ResolveEffectiveRole(user.Email, user.Role);
        if (role != AccountRoles.Admin)
        {
            var preferences = await _preferenceStore.FindLatestByUserIdAsync(user.Id, cancellationToken);
            if (preferences is null || preferences.PlaceInterests.Count == 0 || preferences.ActivityInterests.Count == 0)
            {
                return RedirectToAction("Survey", "Booking", new { returnUrl });
            }
        }

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToRoleHome(user);
    }

    private IActionResult RedirectAuthenticatedUser(string? returnUrl)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return User.IsInRole(AccountRoles.Admin)
            ? RedirectToAction("Index", "Admin")
            : RedirectToAction("Index", "Dashboard");
    }

    private static bool RequiresGmailAccount(string? returnUrl)
    {
        return !string.IsNullOrWhiteSpace(returnUrl)
            && returnUrl.StartsWith("/Booking", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeEmail(string email) => (email ?? string.Empty).Trim().ToLowerInvariant();
}

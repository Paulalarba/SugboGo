using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SugboGo.Models;
using SugboGo.Services.Auth;

namespace SugboGo.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthApiController : ControllerBase
{
    private readonly IUserAccountStore _userStore;
    private readonly IPasswordService _passwordService;
    private readonly IAccountRoleService _accountRoleService;

    public AuthApiController(IUserAccountStore userStore, IPasswordService passwordService, IAccountRoleService accountRoleService)
    {
        _userStore = userStore;
        _passwordService = passwordService;
        _accountRoleService = accountRoleService;
    }

    [HttpPost("check-email")]
    public async Task<IActionResult> CheckEmail([FromBody] EmailRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(new { message = "Email address is required." });
        }

        var email = NormalizeEmail(request.Email);
        var user = await _userStore.FindByEmailAsync(email, cancellationToken);

        return Ok(new
        {
            exists = user is not null,
            next = user is null ? "register" : "sign-in",
            redirectUrl = user is null
                ? Url.Action("Register", "Account", new { email })
                : Url.Action("SignIn", "Account", new { email })
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.FullName) ||
            string.IsNullOrWhiteSpace(request.Password) ||
            request.Password.Length < 8)
        {
            return BadRequest(new { message = "Email, name, and an 8+ character password are required." });
        }

        var email = NormalizeEmail(request.Email);

        if (await _userStore.FindByEmailAsync(email, cancellationToken) is not null)
        {
            return Conflict(new { message = "An account already exists for this email address." });
        }

        var user = new UserAccount
        {
            Email = email,
            FullName = request.FullName.Trim(),
            PasswordHash = _passwordService.HashPassword(request.Password),
            Role = _accountRoleService.GetRegistrationRole(email)
        };

        await _userStore.CreateAsync(user, cancellationToken);
        await SignUserInAsync(user);

        return Created("/api/auth/session", new { user.Id, user.Email, user.FullName, Role = _accountRoleService.ResolveEffectiveRole(user.Email, user.Role) });
    }

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn([FromBody] SignInRequest request, CancellationToken cancellationToken)
    {
        var user = await _userStore.FindByEmailAsync(request.Email, cancellationToken);

        if (user is null || !_passwordService.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        await SignUserInAsync(user);
        return Ok(new { user.Id, user.Email, user.FullName, Role = _accountRoleService.ResolveEffectiveRole(user.Email, user.Role) });
    }

    [HttpGet("session")]
    public IActionResult Session()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Ok(new { authenticated = false });
        }

        return Ok(new
        {
            authenticated = true,
            user = new
            {
                id = User.FindFirstValue(ClaimTypes.NameIdentifier),
                email = User.FindFirstValue(ClaimTypes.Email),
                fullName = User.FindFirstValue(ClaimTypes.Name),
                role = User.FindFirstValue(ClaimTypes.Role)
            }
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return NoContent();
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

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = false });
    }

    private static string NormalizeEmail(string email) => (email ?? string.Empty).Trim().ToLowerInvariant();

    public sealed record EmailRequest(string Email);
    public sealed record RegisterRequest(string Email, string FullName, string Password);
    public sealed record SignInRequest(string Email, string Password);
}

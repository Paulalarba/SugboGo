using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using SugboGo.Models;

namespace SugboGo.Services.Auth;

public sealed class UserSignInService : IUserSignInService
{
    private readonly IAccountRoleService _accountRoleService;

    public UserSignInService(IAccountRoleService accountRoleService)
    {
        _accountRoleService = accountRoleService;
    }

    public async Task SignInAsync(HttpContext httpContext, UserAccount user, bool rememberMe)
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

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = rememberMe });
    }
}

using SugboGo.Models;

namespace SugboGo.Services.Auth;

public interface IUserSignInService
{
    Task SignInAsync(HttpContext httpContext, UserAccount user, bool rememberMe);
}

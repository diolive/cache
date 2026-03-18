using System.Security.Claims;

using DioRed.Cache.Infrastructure.Auth;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DioRed.Cache.WebApp.ViewComponents;

public class LoginViewComponent(
    SignInManager<IdentityUser> signInManager,
    AppUserManager userManager
) : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return User is ClaimsPrincipal user && signInManager.IsSignedIn(user)
            ? View("User", userManager.GetUserName(user))
            : View("Guest");
    }
}
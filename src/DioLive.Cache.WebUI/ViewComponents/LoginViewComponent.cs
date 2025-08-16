using System.Security.Claims;

using DioLive.Cache.Auth;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DioLive.Cache.WebUI.ViewComponents;

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
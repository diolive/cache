using System.Security.Claims;

using DioLive.Cache.Auth;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DioLive.Cache.WebUI.ViewComponents
{
	public class LoginViewComponent : ViewComponent
	{
		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly AppUserManager _userManager;

		public LoginViewComponent(SignInManager<IdentityUser> signInManager,
		                          AppUserManager userManager)
		{
			_signInManager = signInManager;
			_userManager = userManager;
		}

		public IViewComponentResult Invoke()
		{
			return User is ClaimsPrincipal user && _signInManager.IsSignedIn(user)
				? View("User", _userManager.GetUserName(user))
				: View("Guest");
		}
	}
}
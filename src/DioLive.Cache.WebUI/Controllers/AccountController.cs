using DioLive.Cache.Auth;
using DioLive.Cache.Common;
using DioLive.Cache.CoreLogic.Contacts;
using DioLive.Cache.WebUI.Models.AccountViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;

using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace DioLive.Cache.WebUI.Controllers
{
	[Authorize]
	public class AccountController : BaseController
	{
		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly AppUserManager _userManager;
		private readonly IUsersLogic _usersLogic;

		public AccountController(ICurrentContext currentContext,
		                         SignInManager<IdentityUser> signInManager,
		                         AppUserManager userManager,
		                         IUsersLogic usersLogic)
			: base(currentContext)
		{
			_signInManager = signInManager;
			_userManager = userManager;
			_usersLogic = usersLogic;
		}

		//
		// GET: /Account/Login
		[HttpGet]
		[AllowAnonymous]
		public IActionResult Login(string? returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			return View();
		}

		//
		// POST: /Account/Login
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginVM model, string? returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			SignInResult result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
			if (result.Succeeded)
			{
				return RedirectToLocal(returnUrl);
			}

			ModelState.AddModelError(string.Empty, "Invalid login attempt.");

			return View(model);
		}

		//
		// GET: /Account/Register
		[HttpGet]
		[AllowAnonymous]
		public IActionResult Register(string? returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			return View();
		}

		//
		// POST: /Account/Register
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterVM model, string? returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = new IdentityUser { UserName = model.Email, Email = model.Email };
			IdentityResult result = await _userManager.CreateAsync(user, model.Password);
			if (result.Succeeded)
			{
				await _signInManager.SignInAsync(user, false);
				_usersLogic.Register(_userManager.GetUserId(User), _userManager.GetUserName(User));

				return RedirectToLocal(returnUrl);
			}

			AddErrors(result);

			return View(model);
		}

		//
		// POST: /Account/LogOff
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> LogOff()
		{
			HttpContext.Session.Clear();
			await _signInManager.SignOutAsync();

			return RedirectToAction(nameof(HomeController.Index), "Home");
		}
		#region Helpers

		private void AddErrors(IdentityResult result)
		{
			foreach (IdentityError error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}
		}

		private IActionResult RedirectToLocal(string? returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}

			return RedirectToAction(nameof(HomeController.Index), "Home");
		}

		#endregion Helpers
	}
}
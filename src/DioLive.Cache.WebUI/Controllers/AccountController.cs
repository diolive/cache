﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using DioLive.Cache.Auth;
using DioLive.Cache.Common;
using DioLive.Cache.CoreLogic.Contacts;
using DioLive.Cache.WebUI.Models.AccountViewModels;
using DioLive.Cache.WebUI.Services;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace DioLive.Cache.WebUI.Controllers
{
	[Authorize]
	public class AccountController : BaseController
	{
		private readonly IEmailSender _emailSender;
		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly ISmsSender _smsSender;
		private readonly AppUserManager _userManager;
		private readonly IUsersLogic _usersLogic;

		public AccountController(ICurrentContext currentContext,
		                         SignInManager<IdentityUser> signInManager,
		                         AppUserManager userManager,
		                         IUsersLogic usersLogic,
		                         IEmailSender emailSender,
		                         ISmsSender smsSender)
			: base(currentContext)
		{
			_signInManager = signInManager;
			_userManager = userManager;
			_usersLogic = usersLogic;
			_emailSender = emailSender;
			_smsSender = smsSender;
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

			// This doesn't count login failures towards account lockout
			// To enable password failures to trigger account lockout, set lockoutOnFailure: true
			SignInResult result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
			if (result.Succeeded)
			{
				return RedirectToLocal(returnUrl);
			}

			if (result.RequiresTwoFactor)
			{
				return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, model.RememberMe });
			}

			if (result.IsLockedOut)
			{
				return View("Lockout");
			}

			ModelState.AddModelError(string.Empty, "Invalid login attempt.");

			// If we got this far, something failed, redisplay form
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
				// For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
				// Send an email with this link
				//var code = await _helper.UserManager.GenerateEmailConfirmationTokenAsync(user);
				//var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
				//await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
				//    $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");
				await _signInManager.SignInAsync(user, false);
				_usersLogic.Register(_userManager.GetUserId(User), _userManager.GetUserName(User));
				return RedirectToLocal(returnUrl);
			}

			AddErrors(result);

			// If we got this far, something failed, redisplay form
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

		//
		// POST: /Account/ExternalLogin
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public IActionResult ExternalLogin(string provider, string? returnUrl = null)
		{
			// Request a redirect to the external login provider.
			string redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
			AuthenticationProperties properties =
				_signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
			return Challenge(properties, provider);
		}

		//
		// GET: /Account/ExternalLoginCallback
		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
		{
			if (remoteError != null)
			{
				ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
				return View(nameof(Login));
			}

			ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();
			if (info == null)
			{
				return RedirectToAction(nameof(Login));
			}

			// Sign in the user with this external login provider if the user already has a login.
			SignInResult result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
			if (result.Succeeded)
			{
				return RedirectToLocal(returnUrl);
			}

			if (result.RequiresTwoFactor)
			{
				return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl });
			}

			if (result.IsLockedOut)
			{
				return View("Lockout");
			}

			// If the user does not have an account, then ask the user to create an account.
			ViewData["ReturnUrl"] = returnUrl;
			ViewData["LoginProvider"] = info.LoginProvider;
			string email = info.Principal.FindFirstValue(ClaimTypes.Email);
			return View("ExternalLoginConfirmation", new ExternalLoginConfirmationVM { Email = email });
		}

		//
		// POST: /Account/ExternalLoginConfirmation
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationVM model, string? returnUrl = null)
		{
			if (ModelState.IsValid)
			{
				// Get the information about the user from the external login provider
				ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();
				if (info == null)
				{
					return View("ExternalLoginFailure");
				}

				var user = new IdentityUser { UserName = model.Email, Email = model.Email };
				IdentityResult result = await _userManager.CreateAsync(user);
				if (result.Succeeded)
				{
					result = await _userManager.AddLoginAsync(user, info);
					if (result.Succeeded)
					{
						await _signInManager.SignInAsync(user, false);
						return RedirectToLocal(returnUrl);
					}
				}

				AddErrors(result);
			}

			ViewData["ReturnUrl"] = returnUrl;
			return View(model);
		}

		// GET: /Account/ConfirmEmail
		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> ConfirmEmail(string userId, string code)
		{
			if (userId == null || code == null)
			{
				return View("Error");
			}

			IdentityUser user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return View("Error");
			}

			IdentityResult result = await _userManager.ConfirmEmailAsync(user, code);
			return View(result.Succeeded ? "ConfirmEmail" : "Error");
		}

		//
		// GET: /Account/ForgotPassword
		[HttpGet]
		[AllowAnonymous]
		public IActionResult ForgotPassword()
		{
			return View();
		}

		//
		// POST: /Account/ForgotPassword
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			IdentityUser user = await _userManager.FindByNameAsync(model.Email);
			if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
			{
				// Don't reveal that the user does not exist or is not confirmed
				return View("ForgotPasswordConfirmation");
			}

			// For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
			// Send an email with this link
			//var code = await _helper.UserManager.GeneratePasswordResetTokenAsync(user);
			//var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
			//await _emailSender.SendEmailAsync(model.Email, "Reset Password",
			//   $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
			//return View("ForgotPasswordConfirmation");

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		//
		// GET: /Account/ForgotPasswordConfirmation
		[HttpGet]
		[AllowAnonymous]
		public IActionResult ForgotPasswordConfirmation()
		{
			return View();
		}

		//
		// GET: /Account/ResetPassword
		[HttpGet]
		[AllowAnonymous]
		public IActionResult ResetPassword(string? code = null)
		{
			return code == null ? View("Error") : View();
		}

		//
		// POST: /Account/ResetPassword
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			IdentityUser user = await _userManager.FindByNameAsync(model.Email);
			if (user == null)
			{
				// Don't reveal that the user does not exist
				return RedirectToAction(nameof(ResetPasswordConfirmation), "Account");
			}

			IdentityResult result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
			if (result.Succeeded)
			{
				return RedirectToAction(nameof(ResetPasswordConfirmation), "Account");
			}

			AddErrors(result);
			return View();
		}

		//
		// GET: /Account/ResetPasswordConfirmation
		[HttpGet]
		[AllowAnonymous]
		public IActionResult ResetPasswordConfirmation()
		{
			return View();
		}

		//
		// GET: /Account/SendCode
		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> SendCode(string? returnUrl = null, bool rememberMe = false)
		{
			IdentityUser user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
			if (user == null)
			{
				return View("Error");
			}

			IList<string> userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
			List<SelectListItem> factorOptions = userFactors
				.Select(purpose => new SelectListItem { Text = purpose, Value = purpose })
				.ToList();
			return View(new SendCodeVM { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
		}

		//
		// POST: /Account/SendCode
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SendCode(SendCodeVM model)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}

			IdentityUser user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
			if (user == null)
			{
				return View("Error");
			}

			// Generate the token and send it
			string code = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
			if (string.IsNullOrWhiteSpace(code))
			{
				return View("Error");
			}

			string message = "Your security code is: " + code;
			switch (model.SelectedProvider)
			{
				case "Email":
					await _emailSender.SendEmailAsync(await _userManager.GetEmailAsync(user), "Security Code",
						message);
					break;

				case "Phone":
					await _smsSender.SendSmsAsync(await _userManager.GetPhoneNumberAsync(user), message);
					break;
			}

			return RedirectToAction(nameof(VerifyCode),
				new { Provider = model.SelectedProvider, model.ReturnUrl, model.RememberMe });
		}

		//
		// GET: /Account/VerifyCode
		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string? returnUrl = null)
		{
			// Require that the user has already logged in via username/password or external login
			IdentityUser user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
			return user == null
				? View("Error")
				: View(new VerifyCodeVM { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
		}

		//
		// POST: /Account/VerifyCode
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> VerifyCode(VerifyCodeVM model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			// The following code protects for brute force attacks against the two factor codes.
			// If a user enters incorrect codes for a specified amount of time then the user account
			// will be locked out for a specified amount of time.
			SignInResult result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code,
				model.RememberMe, model.RememberBrowser);
			if (result.Succeeded)
			{
				return RedirectToLocal(model.ReturnUrl);
			}

			if (result.IsLockedOut)
			{
				return View("Lockout");
			}

			ModelState.AddModelError(string.Empty, "Invalid code.");
			return View(model);
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
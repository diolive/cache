using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;
using DioLive.Cache.WebUI.Models;
using DioLive.Cache.WebUI.Models.ManageViewModels;
using DioLive.Cache.WebUI.Services;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DioLive.Cache.WebUI.Controllers
{
	[Authorize]
	public class ManageController : BaseController
	{
		private static readonly Dictionary<ManageMessageId, string> StatusMessages;

		private readonly IOptionsStorage _optionsStorage;

		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly ISmsSender _smsSender;
		private readonly AppUserManager _userManager;

		static ManageController()
		{
			StatusMessages = new Dictionary<ManageMessageId, string>
			{
				[ManageMessageId.ChangePasswordSuccess] = "Your password has been changed.",
				[ManageMessageId.SetPasswordSuccess] = "Your password has been set.",
				[ManageMessageId.SetTwoFactorSuccess] = "Your two-factor authentication provider has been set.",
				[ManageMessageId.Error] = "An error has occurred.",
				[ManageMessageId.AddPhoneSuccess] = "Your phone number was added.",
				[ManageMessageId.RemovePhoneSuccess] = "Your phone number was removed.",
				[ManageMessageId.RemoveLoginSuccess] = "The external login was removed.",
				[ManageMessageId.AddLoginSuccess] = "The external login was added.",
				[ManageMessageId.Error] = "An error has occurred."
			};
		}

		public ManageController(ICurrentContext currentContext,
		                        SignInManager<IdentityUser> signInManager,
		                        AppUserManager userManager,
		                        ISmsSender smsSender,
		                        IOptionsStorage optionsStorage)
			: base(currentContext)
		{
			_signInManager = signInManager;
			_userManager = userManager;
			_smsSender = smsSender;
			_optionsStorage = optionsStorage;
		}

		//
		// GET: /Manage/Index
		[HttpGet]
		public async Task<IActionResult> Index(ManageMessageId? message = null)
		{
			ViewData["StatusMessage"] = message.HasValue &&
			                            StatusMessages.TryGetValue(message.Value, out string msgText)
				? msgText
				: string.Empty;

			IdentityUser user = await _userManager.GetUserAsync(HttpContext.User);
			if (user is null)
			{
				return View("Error");
			}

			Options options = await _optionsStorage.GetAsync();

			var model = new IndexVM
			{
				HasPassword = await _userManager.HasPasswordAsync(user),
				PhoneNumber = await _userManager.GetPhoneNumberAsync(user),
				TwoFactor = await _userManager.GetTwoFactorEnabledAsync(user),
				Logins = await _userManager.GetLoginsAsync(user),

				BrowserRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user),

				PurchaseGrouping = options.PurchaseGrouping,
				ShowPlanList = options.ShowPlanList
			};
			return View(model);
		}

		//
		// POST: /Manage/RemoveLogin
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> RemoveLogin(RemoveLoginVM account)
		{
			ManageMessageId? message = ManageMessageId.Error;
			IdentityUser user = await GetCurrentUserAsync();

			if (user == null)
			{
				return RedirectToAction(nameof(ManageLogins), new { Message = message });
			}

			IdentityResult result =
				await _userManager.RemoveLoginAsync(user, account.LoginProvider, account.ProviderKey);

			if (!result.Succeeded)
			{
				return RedirectToAction(nameof(ManageLogins), new { Message = message });
			}

			await _signInManager.SignInAsync(user, false);
			message = ManageMessageId.RemoveLoginSuccess;
			return RedirectToAction(nameof(ManageLogins), new { Message = message });
		}

		//
		// GET: /Manage/AddPhoneNumber
		public IActionResult AddPhoneNumber()
		{
			return View();
		}

		//
		// POST: /Manage/AddPhoneNumber
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddPhoneNumber(AddPhoneNumberVM model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			// Generate the token and send it
			IdentityUser user = await GetCurrentUserAsync();
			if (user == null)
			{
				return View("Error");
			}

			string code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, model.PhoneNumber);
			await _smsSender.SendSmsAsync(model.PhoneNumber, "Your security code is: " + code);
			return RedirectToAction(nameof(VerifyPhoneNumber), new { model.PhoneNumber });
		}

		//
		// POST: /Manage/EnableTwoFactorAuthentication
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EnableTwoFactorAuthentication()
		{
			IdentityUser user = await GetCurrentUserAsync();

			if (user == null)
			{
				return RedirectToAction(nameof(Index), "Manage");
			}

			await _userManager.SetTwoFactorEnabledAsync(user, true);
			await _signInManager.SignInAsync(user, false);
			return RedirectToAction(nameof(Index), "Manage");
		}

		//
		// POST: /Manage/DisableTwoFactorAuthentication
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DisableTwoFactorAuthentication()
		{
			IdentityUser user = await GetCurrentUserAsync();
			if (user == null)
			{
				return RedirectToAction(nameof(Index), "Manage");
			}

			await _userManager.SetTwoFactorEnabledAsync(user, false);
			await _signInManager.SignInAsync(user, false);
			return RedirectToAction(nameof(Index), "Manage");
		}

		//
		// GET: /Manage/VerifyPhoneNumber
		[HttpGet]
		public async Task<IActionResult> VerifyPhoneNumber(string phoneNumber)
		{
			IdentityUser user = await GetCurrentUserAsync();
			if (user == null)
			{
				return View("Error");
			}

			// var code = await _helper.UserManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
			// Send an SMS to verify the phone number
			return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberVM { PhoneNumber = phoneNumber });
		}

		//
		// POST: /Manage/VerifyPhoneNumber
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> VerifyPhoneNumber(VerifyPhoneNumberVM model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			IdentityUser user = await GetCurrentUserAsync();
			if (user != null)
			{
				IdentityResult result =
					await _userManager.ChangePhoneNumberAsync(user, model.PhoneNumber, model.Code);
				if (result.Succeeded)
				{
					await _signInManager.SignInAsync(user, false);
					return RedirectToAction(nameof(Index), new { Message = ManageMessageId.AddPhoneSuccess });
				}
			}

			// If we got this far, something failed, redisplay the form
			ModelState.AddModelError(string.Empty, "Failed to verify phone number");
			return View(model);
		}

		//
		// POST: /Manage/RemovePhoneNumber
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> RemovePhoneNumber()
		{
			IdentityUser user = await GetCurrentUserAsync();
			if (user == null)
			{
				return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
			}

			IdentityResult result = await _userManager.SetPhoneNumberAsync(user, null);
			if (!result.Succeeded)
			{
				return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
			}

			await _signInManager.SignInAsync(user, false);
			return RedirectToAction(nameof(Index), new { Message = ManageMessageId.RemovePhoneSuccess });
		}

		//
		// GET: /Manage/ChangePassword
		[HttpGet]
		public IActionResult ChangePassword()
		{
			return View();
		}

		//
		// POST: /Manage/ChangePassword
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			IdentityUser user = await GetCurrentUserAsync();
			if (user == null)
			{
				return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
			}

			IdentityResult result =
				await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
			if (result.Succeeded)
			{
				await _signInManager.SignInAsync(user, false);
				return RedirectToAction(nameof(Index), new { Message = ManageMessageId.ChangePasswordSuccess });
			}

			AddErrors(result);
			return View(model);
		}

		//
		// GET: /Manage/SetPassword
		[HttpGet]
		public IActionResult SetPassword()
		{
			return View();
		}

		//
		// POST: /Manage/SetPassword
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SetPassword(SetPasswordVM model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			IdentityUser user = await GetCurrentUserAsync();
			if (user == null)
			{
				return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
			}

			IdentityResult result = await _userManager.AddPasswordAsync(user, model.NewPassword);
			if (result.Succeeded)
			{
				await _signInManager.SignInAsync(user, false);
				return RedirectToAction(nameof(Index), new { Message = ManageMessageId.SetPasswordSuccess });
			}

			AddErrors(result);
			return View(model);
		}

		//GET: /Manage/ManageLogins
		[HttpGet]
		public async Task<IActionResult> ManageLogins(ManageMessageId? message = null)
		{
			ViewData["StatusMessage"] = message.HasValue &&
			                            StatusMessages.TryGetValue(message.Value, out string msgText)
				? msgText
				: string.Empty;

			IdentityUser user = await GetCurrentUserAsync();
			if (user == null)
			{
				return View("Error");
			}

			IList<UserLoginInfo> userLogins = await _userManager.GetLoginsAsync(user);
			List<AuthenticationScheme> otherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
				.Where(auth => userLogins.All(ul => auth.Name != ul.LoginProvider))
				.ToList();
			ViewData["ShowRemoveButton"] = user.PasswordHash != null || userLogins.Count > 1;

			return View(new ManageLoginsVM
			{
				CurrentLogins = userLogins,
				OtherLogins = otherLogins
			});
		}

		//
		// POST: /Manage/LinkLogin
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult LinkLogin(string provider)
		{
			// Request a redirect to the external login provider to link a login for the current user
			string redirectUrl = Url.Action("LinkLoginCallback", "Manage");
			AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
			return Challenge(properties, provider);
		}

		//
		// GET: /Manage/LinkLoginCallback
		[HttpGet]
		public async Task<ActionResult> LinkLoginCallback()
		{
			IdentityUser user = await GetCurrentUserAsync();
			if (user == null)
			{
				return View("Error");
			}

			ExternalLoginInfo info =
				await _signInManager.GetExternalLoginInfoAsync(await _userManager.GetUserIdAsync(user));
			if (info == null)
			{
				return RedirectToAction(nameof(ManageLogins), new { Message = ManageMessageId.Error });
			}

			IdentityResult result = await _userManager.AddLoginAsync(user, info);
			ManageMessageId message = result.Succeeded ? ManageMessageId.AddLoginSuccess : ManageMessageId.Error;
			return RedirectToAction(nameof(ManageLogins), new { Message = message });
		}

		[HttpGet]
		public async Task<IActionResult> Photo(string id)
		{
			IdentityUser user;
			if (id == null)
			{
				user = await GetCurrentUserAsync();
			}
			else
			{
				user = await _userManager.FindByIdAsync(id);
			}

			return Redirect(GravatarHelper.GetAvatarUrl(user.Email, 16));
		}

		[HttpPost]
		public async Task<IActionResult> UpdateOptions(int? purchaseGrouping = null, bool? showPlanList = null)
		{
			await _optionsStorage.SetAsync(purchaseGrouping, showPlanList);

			return Ok();
		}

		#region Helpers

		private void AddErrors(IdentityResult result)
		{
			foreach (IdentityError error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}
		}

		public enum ManageMessageId
		{
			AddPhoneSuccess,
			AddLoginSuccess,
			ChangePasswordSuccess,
			SetTwoFactorSuccess,
			SetPasswordSuccess,
			RemoveLoginSuccess,
			RemovePhoneSuccess,
			Error
		}

		private Task<IdentityUser> GetCurrentUserAsync()
		{
			return _userManager.GetUserAsync(HttpContext.User);
		}

		#endregion Helpers
	}
}
﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Models;
using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.WebUI.Models;
using DioLive.Cache.WebUI.Models.ManageViewModels;
using DioLive.Cache.WebUI.Services;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DioLive.Cache.WebUI.Controllers
{
	[Authorize]
	public class ManageController : BaseController
	{
		private static readonly Dictionary<ManageMessageId, string> StatusMessages;
		private readonly IApplicationUsersStorage _applicationUsersStorage;
		private readonly ILogger _logger;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly ISmsSender _smsSender;

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

		public ManageController(DataHelper dataHelper,
								SignInManager<ApplicationUser> signInManager,
								ISmsSender smsSender,
								IApplicationUsersStorage applicationUsersStorage)
			: base(dataHelper)
		{
			_signInManager = signInManager;
			_smsSender = smsSender;
			_applicationUsersStorage = applicationUsersStorage;
			_logger = DataHelper.LoggerFactory.CreateLogger<ManageController>();
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

			string userId = UserId;
			ApplicationUser user = await _applicationUsersStorage.GetWithOptionsAsync(userId);
			if (user == null)
			{
				return View("Error");
			}

			var model = new IndexVM
			{
				HasPassword = await DataHelper.UserManager.HasPasswordAsync(user),
				PhoneNumber = await DataHelper.UserManager.GetPhoneNumberAsync(user),
				TwoFactor = await DataHelper.UserManager.GetTwoFactorEnabledAsync(user),
				Logins = await DataHelper.UserManager.GetLoginsAsync(user),
				BrowserRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user),
				PurchaseGrouping = user.Options.PurchaseGrouping,
				ShowPlanList = user.Options.ShowPlanList
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
			ApplicationUser user = await GetCurrentUserAsync();

			if (user == null)
			{
				return RedirectToAction(nameof(ManageLogins), new { Message = message });
			}

			IdentityResult result =
				await DataHelper.UserManager.RemoveLoginAsync(user, account.LoginProvider, account.ProviderKey);

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
			ApplicationUser user = await GetCurrentUserAsync();
			if (user == null)
			{
				return View("Error");
			}

			string code = await DataHelper.UserManager.GenerateChangePhoneNumberTokenAsync(user, model.PhoneNumber);
			await _smsSender.SendSmsAsync(model.PhoneNumber, "Your security code is: " + code);
			return RedirectToAction(nameof(VerifyPhoneNumber), new { model.PhoneNumber });
		}

		//
		// POST: /Manage/EnableTwoFactorAuthentication
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EnableTwoFactorAuthentication()
		{
			ApplicationUser user = await GetCurrentUserAsync();

			if (user == null)
			{
				return RedirectToAction(nameof(Index), "Manage");
			}

			await DataHelper.UserManager.SetTwoFactorEnabledAsync(user, true);
			await _signInManager.SignInAsync(user, false);
			_logger.LogInformation(1, "User enabled two-factor authentication.");
			return RedirectToAction(nameof(Index), "Manage");
		}

		//
		// POST: /Manage/DisableTwoFactorAuthentication
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DisableTwoFactorAuthentication()
		{
			ApplicationUser user = await GetCurrentUserAsync();
			if (user == null)
			{
				return RedirectToAction(nameof(Index), "Manage");
			}

			await DataHelper.UserManager.SetTwoFactorEnabledAsync(user, false);
			await _signInManager.SignInAsync(user, false);
			_logger.LogInformation(2, "User disabled two-factor authentication.");
			return RedirectToAction(nameof(Index), "Manage");
		}

		//
		// GET: /Manage/VerifyPhoneNumber
		[HttpGet]
		public async Task<IActionResult> VerifyPhoneNumber(string phoneNumber)
		{
			ApplicationUser user = await GetCurrentUserAsync();
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

			ApplicationUser user = await GetCurrentUserAsync();
			if (user != null)
			{
				IdentityResult result =
					await DataHelper.UserManager.ChangePhoneNumberAsync(user, model.PhoneNumber, model.Code);
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
			ApplicationUser user = await GetCurrentUserAsync();
			if (user == null)
			{
				return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
			}

			IdentityResult result = await DataHelper.UserManager.SetPhoneNumberAsync(user, null);
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

			ApplicationUser user = await GetCurrentUserAsync();
			if (user == null)
			{
				return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
			}

			IdentityResult result =
				await DataHelper.UserManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
			if (result.Succeeded)
			{
				await _signInManager.SignInAsync(user, false);
				_logger.LogInformation(3, "User changed their password successfully.");
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

			ApplicationUser user = await GetCurrentUserAsync();
			if (user == null)
			{
				return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
			}

			IdentityResult result = await DataHelper.UserManager.AddPasswordAsync(user, model.NewPassword);
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

			ApplicationUser user = await GetCurrentUserAsync();
			if (user == null)
			{
				return View("Error");
			}

			IList<UserLoginInfo> userLogins = await DataHelper.UserManager.GetLoginsAsync(user);
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
			AuthenticationProperties properties =
				_signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl,
					UserId);
			return Challenge(properties, provider);
		}

		//
		// GET: /Manage/LinkLoginCallback
		[HttpGet]
		public async Task<ActionResult> LinkLoginCallback()
		{
			ApplicationUser user = await GetCurrentUserAsync();
			if (user == null)
			{
				return View("Error");
			}

			ExternalLoginInfo info =
				await _signInManager.GetExternalLoginInfoAsync(await DataHelper.UserManager.GetUserIdAsync(user));
			if (info == null)
			{
				return RedirectToAction(nameof(ManageLogins), new { Message = ManageMessageId.Error });
			}

			IdentityResult result = await DataHelper.UserManager.AddLoginAsync(user, info);
			ManageMessageId message = result.Succeeded ? ManageMessageId.AddLoginSuccess : ManageMessageId.Error;
			return RedirectToAction(nameof(ManageLogins), new { Message = message });
		}

		[HttpGet]
		public async Task<IActionResult> Photo(string id)
		{
			ApplicationUser user;
			if (id == null)
			{
				user = await DataHelper.UserManager.GetUserAsync(User);
			}
			else
			{
				user = await DataHelper.UserManager.FindByIdAsync(id);
			}

			return Redirect(GravatarHelper.GetAvatarUrl(user.Email, 16));
		}

		[HttpPost]
		public async Task<IActionResult> UpdateOptions(int? purchaseGrouping = null, bool? showPlanList = null)
		{
			await _applicationUsersStorage.UpdateOptionsAsync(UserId, purchaseGrouping, showPlanList);

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

		private Task<ApplicationUser> GetCurrentUserAsync()
		{
			return DataHelper.UserManager.GetUserAsync(HttpContext.User);
		}

		#endregion Helpers
	}
}
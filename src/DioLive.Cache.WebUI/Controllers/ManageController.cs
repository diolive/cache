using DioLive.Cache.Auth;
using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Contacts;
using DioLive.Cache.WebUI.Models;
using DioLive.Cache.WebUI.Models.ManageViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace DioLive.Cache.WebUI.Controllers
{
	[Authorize]
	public class ManageController : BaseController
	{
		private static readonly Dictionary<ManageMessageId, string> _statusMessages;
		private readonly IOptionsLogic _optionsLogic;

		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly AppUserManager _userManager;

		static ManageController()
		{
			_statusMessages = new Dictionary<ManageMessageId, string>
			{
				[ManageMessageId.ChangePasswordSuccess] = "Your password has been changed.",
				[ManageMessageId.Error] = "An error has occurred.",
			};
		}

		public ManageController(ICurrentContext currentContext,
		                        SignInManager<IdentityUser> signInManager,
		                        AppUserManager userManager,
		                        IOptionsLogic optionsLogic)
			: base(currentContext)
		{
			_signInManager = signInManager;
			_userManager = userManager;
			_optionsLogic = optionsLogic;
		}

		//
		// GET: /Manage/Index
		[HttpGet]
		public async Task<IActionResult> Index(ManageMessageId? message = null)
		{
			ViewData["StatusMessage"] = message.HasValue &&
			                            _statusMessages.TryGetValue(message.Value, out string? msgText)
				? msgText
				: string.Empty;

			IdentityUser user = await _userManager.GetUserAsync(HttpContext.User);
			if (user is null)
			{
				return View("Error");
			}

			Result<Options> result = _optionsLogic.Get();

			return ProcessResult(result, options =>
			{
				var model = new ProfileVM
				{
					PurchaseGrouping = options.PurchaseGrouping,
					ShowPlanList = options.ShowPlanList
				};

				return View(model);
			});
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
			ChangePasswordSuccess,
			Error
		}

		private Task<IdentityUser> GetCurrentUserAsync()
		{
			return _userManager.GetUserAsync(HttpContext.User);
		}

		#endregion Helpers
	}
}
using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.Models;
using DioLive.Cache.Storage.Contracts;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DioLive.Cache.WebUI.ViewComponents
{
	public class UserBudgetsViewComponent : ViewComponent
	{
		private readonly IBudgetsStorage _budgetsStorage;
		private readonly UserManager<ApplicationUser> _userManager;

		public UserBudgetsViewComponent(UserManager<ApplicationUser> userManager,
										IBudgetsStorage budgetsStorage)
		{
			_userManager = userManager;
			_budgetsStorage = budgetsStorage;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			string userId = _userManager.GetUserId(HttpContext.User);
			List<Budget> budgets = await _budgetsStorage.GetForUserBudgetsComponentAsync(userId);
			ViewBag.UserId = userId;

			return View("Index", budgets);
		}
	}
}
using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Legacy.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Budget = DioLive.Cache.Storage.Entities.Budget;

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
			IReadOnlyCollection<Budget> budgets = await _budgetsStorage.GetAllAvailableAsync();
			ViewBag.UserId = userId;

			return View("Index", budgets);
		}
	}
}
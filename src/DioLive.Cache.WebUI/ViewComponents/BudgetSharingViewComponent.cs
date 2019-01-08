using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Storage;
using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;
using DioLive.Cache.Storage.Legacy;
using DioLive.Cache.Storage.Legacy.Models;
using DioLive.Cache.WebUI.Models.BudgetSharingViewModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Budget = DioLive.Cache.Storage.Entities.Budget;

namespace DioLive.Cache.WebUI.ViewComponents
{
	public class BudgetSharingViewComponent : ViewComponent
	{
		private static SelectList _accessSelectList;
		private readonly ApplicationUsersStorage _applicationUsersStorage;
		private readonly IBudgetsStorage _budgetsStorage;

		public BudgetSharingViewComponent(IBudgetsStorage budgetsStorage,
										  ApplicationUsersStorage applicationUsersStorage)
		{
			_budgetsStorage = budgetsStorage;
			_applicationUsersStorage = applicationUsersStorage;

			_accessSelectList = new SelectList(new[]
			{
				new { Value = ShareAccess.ReadOnly, Title = "Read only" },
				new { Value = ShareAccess.Purchases, Title = "Purchases" },
				new { Value = ShareAccess.Purchases | ShareAccess.Categories, Title = "Purchases and categories" },
				new { Value = ShareAccess.FullAccess, Title = "Unlimited access" }
			}, "Value", "Title");
		}

		public async Task<IViewComponentResult> InvokeAsync(Guid budgetId)
		{
			(Result result, Budget budget) = await _budgetsStorage.GetAsync(budgetId, ShareAccess.Manage);

			if (result != Result.Success)
			{
				throw new ArgumentException("Cannot read budget shares");
			}

			ViewData["Access"] = _accessSelectList;

			ApplicationUser owner = await _applicationUsersStorage.GetAsync(budget.AuthorId);
			IReadOnlyCollection<ShareVM> shares = await Task.WhenAll((await _budgetsStorage.GetSharesAsync(budgetId))
				.Select(async sh => new ShareVM
				{
					BudgetId = budgetId,
					UserName = (await _applicationUsersStorage.GetAsync(sh.UserId)).UserName,
					Access = sh.Access
				}));

			var model = new BudgetSharingsVM { BudgetId = budgetId, Owner = owner, Shares = shares };
			return View("Index", model);
		}
	}
}
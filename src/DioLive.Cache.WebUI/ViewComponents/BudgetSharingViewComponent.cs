using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Storage;
using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;
using DioLive.Cache.WebUI.Models.BudgetSharingViewModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DioLive.Cache.WebUI.ViewComponents
{
	public class BudgetSharingViewComponent : ViewComponent
	{
		private static SelectList _accessSelectList;
		private readonly IUsersStorage _usersStorage;
		private readonly IBudgetsStorage _budgetsStorage;

		public BudgetSharingViewComponent(IBudgetsStorage budgetsStorage,
		                                  IUsersStorage usersStorage)
		{
			_budgetsStorage = budgetsStorage;
			_usersStorage = usersStorage;

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
			(Result result, _) = await _budgetsStorage.GetAsync(budgetId, ShareAccess.Manage);

			if (result != Result.Success)
			{
				throw new ArgumentException("Cannot read budget shares");
			}

			ViewData["Access"] = _accessSelectList;

			IReadOnlyCollection<ShareVM> shares = await Task.WhenAll((await _budgetsStorage.GetSharesAsync(budgetId))
				.Select(async sh => new ShareVM
				{
					BudgetId = budgetId,
					UserName = await _usersStorage.GetUserNameAsync(sh.UserId),
					Access = sh.Access
				}));

			var model = new BudgetSharingsVM { BudgetId = budgetId, Shares = shares };
			return View("Index", model);
		}
	}
}
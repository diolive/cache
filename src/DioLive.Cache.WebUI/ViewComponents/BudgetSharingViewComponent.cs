using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Storage;
using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;
using DioLive.Cache.WebUI.Models;
using DioLive.Cache.WebUI.Models.BudgetSharingViewModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DioLive.Cache.WebUI.ViewComponents
{
	public class BudgetSharingViewComponent : ViewComponent
	{
		private static SelectList _accessSelectList;
		private readonly IBudgetsStorage _budgetsStorage;
		private readonly AppUserManager _userManager;

		public BudgetSharingViewComponent(IBudgetsStorage budgetsStorage,
		                                  AppUserManager userManager)
		{
			_budgetsStorage = budgetsStorage;
			_userManager = userManager;

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
			Result result = await _budgetsStorage.CheckAccessAsync(budgetId, ShareAccess.Manage);

			if (result != Result.Success)
			{
				throw new ArgumentException("Cannot read budget shares");
			}

			ViewData["Access"] = _accessSelectList;

			IReadOnlyCollection<ShareVM> shares = await Task.WhenAll((await _budgetsStorage.GetSharesAsync(budgetId))
				.Select(async share => new ShareVM
				{
					BudgetId = budgetId,
					UserName = await _userManager.GetUserNameByIdAsync(share.UserId),
					Access = share.Access
				}));

			var model = new BudgetSharingsVM { BudgetId = budgetId, Shares = shares };
			return View("Index", model);
		}
	}
}
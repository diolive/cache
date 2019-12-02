using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Contacts;
using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.WebUI.Models.BudgetSharingViewModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DioLive.Cache.WebUI.ViewComponents
{
	public class BudgetSharingViewComponent : ViewComponent
	{
		private static readonly SelectList _accessSelectList;
		private readonly IBudgetsLogic _budgetsLogic;

		private readonly ICurrentContext _currentContext;
		private readonly IPermissionsValidator _permissionsValidator;

		static BudgetSharingViewComponent()
		{
			_accessSelectList = new SelectList(new[]
			{
				new { Value = ShareAccess.ReadOnly, Title = "Read only" },
				new { Value = ShareAccess.Purchases, Title = "Purchases" },
				new { Value = ShareAccess.Purchases | ShareAccess.Categories, Title = "Purchases and categories" },
				new { Value = ShareAccess.FullAccess, Title = "Unlimited access" }
			}, "Value", "Title");
		}

		public BudgetSharingViewComponent(ICurrentContext currentContext,
		                                  IBudgetsLogic budgetsLogic,
		                                  IPermissionsValidator permissionsValidator)
		{
			_currentContext = currentContext;
			_budgetsLogic = budgetsLogic;
			_permissionsValidator = permissionsValidator;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			if (!_currentContext.BudgetId.HasValue)
			{
				throw new InvalidOperationException("No current budget");
			}

			Guid budgetId = _currentContext.BudgetId.Value;

			ResultStatus result = await _permissionsValidator.CheckUserRightsForBudgetAsync(budgetId, _currentContext.UserId, ShareAccess.Manage);
			if (result != ResultStatus.Success)
			{
				throw new ArgumentException("User don't have access to sharing this budget");
			}

			ViewData["Access"] = _accessSelectList;

			Result<IReadOnlyCollection<ShareItem>> getSharesResult = _budgetsLogic.GetShares();
			if (!getSharesResult.IsSuccess)
			{
				throw new ArgumentException("Cannot get budget shares: " + getSharesResult.ErrorMessage);
			}

			IReadOnlyCollection<ShareVM> shares = getSharesResult.Data
				.Select(share => new ShareVM
				{
					BudgetId = budgetId,
					UserName = share.UserName,
					Access = share.Access
				})
				.ToList()
				.AsReadOnly();

			var model = new BudgetSharingsVM { BudgetId = budgetId, Shares = shares };

			return View(model);
		}
	}
}
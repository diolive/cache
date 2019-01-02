using System;
using System.Threading.Tasks;

using DioLive.Cache.Models;
using DioLive.Cache.Storage.Contracts;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DioLive.Cache.WebUI.ViewComponents
{
	public class BudgetSharingViewComponent : ViewComponent
	{
		private static SelectList _accessSelectList;
		private readonly IBudgetsStorage _budgetsStorage;

		public BudgetSharingViewComponent(IBudgetsStorage budgetsStorage)
		{
			_budgetsStorage = budgetsStorage;

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
			Budget budget = await _budgetsStorage.GetForBudgetSharingComponentAsync(budgetId);
			ViewData["Access"] = _accessSelectList;

			return View("Index", budget);
		}
	}
}
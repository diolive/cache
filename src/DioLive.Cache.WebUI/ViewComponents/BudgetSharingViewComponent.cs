using System;
using System.Threading.Tasks;

using DioLive.Cache.WebUI.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DioLive.Cache.WebUI.ViewComponents
{
	public class BudgetSharingViewComponent : ViewComponent
	{
		private static SelectList _accessSelectList;
		private readonly DataHelper _helper;

		public BudgetSharingViewComponent(DataHelper helper)
		{
			_helper = helper;

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
			Budget budget = await _helper.Db.Budget
				.Include(b => b.Author)
				.Include(b => b.Shares)
				.ThenInclude(s => s.User)
				.SingleOrDefaultAsync(b => b.Id == budgetId);
			ViewData["Access"] = _accessSelectList;
			return View("Index", budget);
		}
	}
}
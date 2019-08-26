using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

using Microsoft.AspNetCore.Mvc;

namespace DioLive.Cache.WebUI.ViewComponents
{
	public class UserBudgetsViewComponent : ViewComponent
	{
		private readonly IBudgetsStorage _budgetsStorage;
		private readonly ICurrentContext _currentContext;

		public UserBudgetsViewComponent(ICurrentContext currentContext,
		                                IBudgetsStorage budgetsStorage)
		{
			_currentContext = currentContext;
			_budgetsStorage = budgetsStorage;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			string userId = _currentContext.UserId;
			IReadOnlyCollection<Budget> budgets = await _budgetsStorage.GetAllAvailableAsync();
			ViewBag.UserId = userId;

			return View("Index", budgets);
		}
	}
}
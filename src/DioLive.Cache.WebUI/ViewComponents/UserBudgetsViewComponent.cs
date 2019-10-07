using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.Common;
using DioLive.Cache.CoreLogic;
using DioLive.Cache.Storage.Entities;

using Microsoft.AspNetCore.Mvc;

namespace DioLive.Cache.WebUI.ViewComponents
{
	public class UserBudgetsViewComponent : ViewComponent
	{
		private readonly BudgetsLogic _budgetsLogic;
		private readonly ICurrentContext _currentContext;

		public UserBudgetsViewComponent(ICurrentContext currentContext,
		                                BudgetsLogic budgetsLogic)
		{
			_currentContext = currentContext;
			_budgetsLogic = budgetsLogic;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			string userId = _currentContext.UserId;
			Result<IReadOnlyCollection<Budget>> result = _budgetsLogic.GetAllAvailable();

			if (!result.IsSuccess)
			{
				return Content(result.ErrorMessage);
			}

			ViewBag.UserId = userId;
			return View("Index", result.Data);

		}
	}
}
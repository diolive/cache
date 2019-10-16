using System.Collections.Generic;

using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Contacts;

using Microsoft.AspNetCore.Mvc;

namespace DioLive.Cache.WebUI.ViewComponents
{
	public class UserBudgetsViewComponent : ViewComponent
	{
		private readonly IBudgetsLogic _budgetsLogic;
		private readonly ICurrentContext _currentContext;

		public UserBudgetsViewComponent(ICurrentContext currentContext,
		                                IBudgetsLogic budgetsLogic)
		{
			_currentContext = currentContext;
			_budgetsLogic = budgetsLogic;
		}

		public IViewComponentResult Invoke()
		{
			string userId = _currentContext.UserId;
			Result<IReadOnlyCollection<Budget>> result = _budgetsLogic.GetAllAvailable();

			if (!result.IsSuccess)
			{
				return Content(result.ErrorMessage);
			}

			ViewBag.UserId = userId;
			return View(result.Data);
		}
	}
}
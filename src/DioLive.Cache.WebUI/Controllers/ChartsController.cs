using System;
using System.Collections.Generic;

using DioLive.Cache.Common;
using DioLive.Cache.CoreLogic;
using DioLive.Cache.CoreLogic.Entities;
using DioLive.Cache.Storage.Entities;

using Microsoft.AspNetCore.Mvc;

namespace DioLive.Cache.WebUI.Controllers
{
	public class ChartsController : BaseController
	{
		private readonly ChartsLogic _chartsLogic;

		public ChartsController(ICurrentContext currentContext,
		                        ChartsLogic chartsLogic)
			: base(currentContext)
		{
			_chartsLogic = chartsLogic;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult PieData(int days = 0)
		{
			Result<IReadOnlyCollection<CategoryWithTotals>> result = _chartsLogic.GetWithTotals(days);
			return ProcessResult(result, () => Json(result.Data));
		}

		public IActionResult SunburstData(int days = 0)
		{
			Result<IReadOnlyCollection<CategoryWithTotals>> result = _chartsLogic.GetWithTotals(days);
			return ProcessResult(result, () => Json(new { DisplayName = "Total", Children = result.Data, Color = "FFF" }));
		}

		public IActionResult StatData(int days, int depth, int step)
		{
			Guid? budgetId = CurrentContext.BudgetId;
			if (!budgetId.HasValue)
			{
				return BadRequest();
			}

			Result<ChartData> result = _chartsLogic.Get(days, depth, step);

			return ProcessResult(result, () => Json(result.Data));
		}
	}
}
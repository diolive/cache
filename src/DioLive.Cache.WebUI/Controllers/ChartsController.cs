using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Contacts;
using DioLive.Cache.CoreLogic.Entities;

using DioRed.Common;

using Microsoft.AspNetCore.Mvc;

namespace DioLive.Cache.WebUI.Controllers;

public class ChartsController(
    ICurrentContext currentContext,
    IChartsLogic chartsLogic
) : BaseController(currentContext)
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult PieData(int days = 0)
    {
        Result<IReadOnlyCollection<CategoryWithTotals>> result = chartsLogic.GetWithTotals(days);

        return ProcessResult(result, Json);
    }

    public IActionResult SunburstData(int days = 0)
    {
        Result<IReadOnlyCollection<CategoryWithTotals>> result = chartsLogic.GetWithTotals(days);

        return ProcessResult(
            result,
            data => Json(
                new
                {
                    Name = "Total",
                    Children = data,
                    Color = "FFF"
                }
            )
        );
    }

    public IActionResult StatData(
        int days,
        int depth,
        int step
    )
    {
        Guid? budgetId = CurrentContext.BudgetId;

        if (!budgetId.HasValue)
        {
            return BadRequest();
        }

        Result<ChartData> result = chartsLogic.Get(
            days,
            depth,
            step
        );

        return ProcessResult(
            result,
            Json
        );
    }
}
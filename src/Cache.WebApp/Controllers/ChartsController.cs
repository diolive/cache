using DioRed.Cache.Domain;
using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Core.Contracts;
using DioRed.Cache.Core.Entities;

using DioRed.Common;

using Microsoft.AspNetCore.Mvc;

namespace DioRed.Cache.WebApp.Controllers;

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
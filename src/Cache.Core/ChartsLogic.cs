using DioRed.Cache.Domain;
using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Core.Contracts;
using DioRed.Cache.Core.Entities;
using DioRed.Cache.Core.Jobs;
using DioRed.Cache.Core.Jobs.Charts;

using DioRed.Common;

namespace DioRed.Cache.Core;

public class ChartsLogic(
    ICurrentContext currentContext,
    JobSettings jobSettings
) : LogicBase(currentContext, jobSettings), IChartsLogic
{
    public Result<ChartData> Get(int days, int depth, int step)
    {
        var job = new GetJob(days, depth, step);
        return GetJobResult(job);
    }

    public Result<IReadOnlyCollection<CategoryWithTotals>> GetWithTotals(int days)
    {
        var job = new GetWithTotalsJob(days);
        return GetJobResult(job);
    }
}
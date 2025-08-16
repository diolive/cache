using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Contacts;
using DioLive.Cache.CoreLogic.Entities;
using DioLive.Cache.CoreLogic.Jobs;
using DioLive.Cache.CoreLogic.Jobs.Charts;

using DioRed.Common;

namespace DioLive.Cache.CoreLogic;

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
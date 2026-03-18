using DioRed.Cache.Domain;
using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Core.Contracts;
using DioRed.Cache.Core.Jobs;
using DioRed.Cache.Core.Jobs.Options;

using DioRed.Common;

namespace DioRed.Cache.Core;

public class OptionsLogic(
    ICurrentContext currentContext,
    JobSettings jobSettings
) : LogicBase(currentContext, jobSettings), IOptionsLogic
{
    public Result<Options> Get()
    {
        var job = new GetJob();
        return GetJobResult(job);
    }

    public Result Update(int? purchaseGrouping, bool? showPlanList)
    {
        var job = new UpdateJob(purchaseGrouping, showPlanList);
        return GetJobResult(job);
    }
}
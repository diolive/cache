using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Contacts;
using DioLive.Cache.CoreLogic.Jobs;
using DioLive.Cache.CoreLogic.Jobs.Options;

using DioRed.Common;

namespace DioLive.Cache.CoreLogic;

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
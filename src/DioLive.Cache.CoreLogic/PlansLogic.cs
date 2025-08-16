using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Contacts;
using DioLive.Cache.CoreLogic.Jobs;
using DioLive.Cache.CoreLogic.Jobs.Plans;

using DioRed.Common;

namespace DioLive.Cache.CoreLogic;

public class PlansLogic(
    ICurrentContext currentContext,
    JobSettings jobSettings
) : LogicBase(currentContext, jobSettings), IPlansLogic
{
    public Result<IReadOnlyCollection<Plan>> GetAll()
    {
        var job = new GetAllJob();
        return GetJobResult(job);
    }

    public Result<string> GetName(int planId)
    {
        var job = new GetNameJob(planId);
        return GetJobResult(job);
    }

    public Result<Plan> Create(string name)
    {
        var job = new CreateJob(name);
        return GetJobResult(job);
    }

    public Result Delete(int id)
    {
        var job = new DeleteJob(id);
        return GetJobResult(job);
    }
}
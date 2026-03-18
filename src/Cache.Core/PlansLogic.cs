using DioRed.Cache.Domain;
using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Core.Contracts;
using DioRed.Cache.Core.Jobs;
using DioRed.Cache.Core.Jobs.Plans;

using DioRed.Common;

namespace DioRed.Cache.Core;

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
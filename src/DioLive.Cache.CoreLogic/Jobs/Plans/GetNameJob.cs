using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Plans;

[Authenticated]
[HasAnyRights]
public class GetNameJob(int planId) : Job<string>
{
    protected override async Task<string> ExecuteAsync()
    {
        IStorageCollection storageCollection = Settings.StorageCollection;

        Plan plan = await storageCollection.Plans.FindAsync(planId)
            ?? throw new ApplicationException("Cannot load current plan");

        return plan.Name;
    }
}
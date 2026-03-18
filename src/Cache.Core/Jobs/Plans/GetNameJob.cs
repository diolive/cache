using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Core.Attributes;
using DioRed.Cache.Domain.Repositories;

namespace DioRed.Cache.Core.Jobs.Plans;

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
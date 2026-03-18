using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Core.Attributes;
using DioRed.Cache.Domain.Repositories;

namespace DioRed.Cache.Core.Jobs.Plans;

[Authenticated]
[HasRights(ShareAccess.Purchases)]
public class CreateJob(string name) : Job<Plan>
{
    protected override async Task<Plan> ExecuteAsync()
    {
        IStorageCollection storageCollection = Settings.StorageCollection;

        return await storageCollection.Plans.AddAsync(name, CurrentBudget);
    }
}
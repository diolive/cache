using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Plans;

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
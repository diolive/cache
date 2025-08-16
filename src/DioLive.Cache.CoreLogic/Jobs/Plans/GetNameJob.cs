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

        return (await storageCollection.Plans.FindAsync(planId)).Name;
    }
}
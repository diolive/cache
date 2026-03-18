using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Core.Attributes;
using DioRed.Cache.Domain.Repositories;

namespace DioRed.Cache.Core.Jobs.Plans;

[Authenticated]
[HasAnyRights]
public class GetAllJob : Job<IReadOnlyCollection<Plan>>
{
    protected override async Task<IReadOnlyCollection<Plan>> ExecuteAsync()
    {
        IStorageCollection storageCollection = Settings.StorageCollection;

        return (await storageCollection.Plans.FindAllAsync(CurrentBudget))
            .OrderBy(p => p.Name)
            .ToList()
            .AsReadOnly();
    }
}
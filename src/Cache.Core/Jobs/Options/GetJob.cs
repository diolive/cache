using DioRed.Cache.Core.Attributes;
using DioRed.Cache.Domain.Repositories;

namespace DioRed.Cache.Core.Jobs.Options;

using DomainOptions = DioRed.Cache.Domain.Entities.Options;

[Authenticated]
public class GetJob : Job<DomainOptions>
{
    protected override async Task<DomainOptions> ExecuteAsync()
    {
        IStorageCollection storageCollections = Settings.StorageCollection;

        DomainOptions? options = await storageCollections.Options.GetAsync();
        if (options is null)
        {
            options = GetDefaultOptions();
            await storageCollections.Options.CreateAsync(options);
        }

        return options;
    }

    private DomainOptions GetDefaultOptions()
    {
        string userId = CurrentContext.GetUserId()
            ?? throw new ApplicationException("Cannot load current user id");

        return new DomainOptions
        {
            UserId = userId,
            PurchaseGrouping = 2,
            ShowPlanList = true
        };
    }
}

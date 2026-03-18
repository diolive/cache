using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Core.Attributes;
using DioRed.Cache.Domain.Repositories;

namespace DioRed.Cache.Core.Jobs.Budgets;

[Authenticated]
[HasAnyRights]
public class GetNameAndAuthorJob : Job<(string name, string authorName)>
{
    protected override async Task<(string name, string authorName)> ExecuteAsync()
    {
        IStorageCollection storageCollection = Settings.StorageCollection;

        Budget budget = await storageCollection.Budgets.GetAsync(CurrentBudget)
            ?? throw new ApplicationException("Cannot load current budget");

        string author = await storageCollection.Users.GetNameByIdAsync(budget.AuthorId)
            ?? string.Empty;

        return (name: budget.Name, authorName: author);
    }
}
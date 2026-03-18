using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Core.Attributes;
using DioRed.Cache.Domain.Repositories;

namespace DioRed.Cache.Core.Jobs.Budgets;

[Authenticated]
public class OpenJob(Guid budgetId) : Job<BudgetSlim>
{
    protected override void CustomValidation()
    {
        AssertUserHasAccessForBudget(budgetId, ShareAccess.ReadOnly);
    }

    protected override async Task<BudgetSlim> ExecuteAsync()
    {
        IStorageCollection storageCollection = Settings.StorageCollection;

        byte version = await storageCollection.Budgets.GetVersionAsync(budgetId);
        if (version == 1)
        {
            string userId = CurrentContext.GetUserId()
                ?? throw new ApplicationException("Cannot load current user id");

            await storageCollection.Categories.CloneCommonCategories(userId, budgetId);
            await storageCollection.Budgets.SetVersionAsync(budgetId, 2);
        }

        string currencySign = await storageCollection.Budgets.GetCurrencyAsync(budgetId);

        return new BudgetSlim
        {
            Id = budgetId,
            Currency = currencySign
        };
    }
}
using DioLive.Cache.Common.Entities;

namespace DioLive.Cache.Common;

public interface ICurrentContext
{
    string GetCulture();
    string? GetUserId();

    BudgetSlim? GetBudget();
    void SetBudget(BudgetSlim value);

    Guid? BudgetId { get; }
}
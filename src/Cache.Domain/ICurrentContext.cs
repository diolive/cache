using DioRed.Cache.Domain.Entities;

namespace DioRed.Cache.Domain;

public interface ICurrentContext
{
    string GetCulture();
    string? GetUserId();

    BudgetSlim? GetBudget();
    void SetBudget(BudgetSlim value);

    Guid? BudgetId { get; }
}
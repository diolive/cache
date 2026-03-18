using DioRed.Cache.Domain.Entities;

using DioRed.Common;

namespace DioRed.Cache.Core.Contracts;

public interface IBudgetsLogic
{
    Result<Guid> Create(string budgetName, string currencyId);
    Result Delete();
    Result<string> GetName();
    Result<(string name, string authorName)> GetNameAndAuthor();
    Result<BudgetSlim> Open(Guid budgetId);
    Result Rename(string newBudgetName);
    Result Share(string targetUserName, ShareAccess targetAccess);
    Result<IReadOnlyCollection<ShareItem>> GetShares();
    Result<IReadOnlyCollection<Budget>> GetAllAvailable();
    Result<string> GetCurrencySign();
}
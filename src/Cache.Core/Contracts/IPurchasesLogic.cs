using DioRed.Cache.Domain.Entities;

using DioRed.Common;

namespace DioRed.Cache.Core.Contracts;

public interface IPurchasesLogic
{
    Result<IReadOnlyCollection<(Purchase purchase, Category category)>> FindWithCategories(string? filter);
    Result Create(string name, int categoryId, DateTime date, decimal cost, string? shop, string? comments, int? planId);
    Result<Purchase> Get(Guid id);
    Result<PurchaseWithNames> GetWithNames(Guid id);
    Result Update(Guid id, string name, int categoryId, DateTime date, decimal cost, string? shop, string? comments);
    Result Delete(Guid id);
    Result<IReadOnlyCollection<string>> GetShops();
    Result<IReadOnlyCollection<string>> GetNames(string filter);
}
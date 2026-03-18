using DioRed.Cache.Domain.Entities;

namespace DioRed.Cache.Domain.Repositories;

public interface IPlansStorage
{
    Task<Plan?> FindAsync(int planId);
    Task<IReadOnlyCollection<Plan>> FindAllAsync(Guid budgetId);
    Task BuyAsync(int planId);
    Task<Plan> AddAsync(string name, Guid budgetId);
    Task RemoveAsync(int planId);
}
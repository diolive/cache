using DioRed.Cache.Domain.Entities;

namespace DioRed.Cache.Domain.Repositories;

public interface IOptionsStorage
{
    Task<Options?> GetAsync();
    Task UpdateAsync(int? purchaseGrouping, bool? showPlanList);
    Task CreateAsync(Options options);
}
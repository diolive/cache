using DioRed.Cache.Domain.Entities;

namespace DioRed.Cache.Domain.Repositories;

public interface ICurrenciesStorage
{
    Task<IReadOnlyCollection<Currency>> GetAllAsync();
}

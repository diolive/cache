using DioRed.Cache.Domain.Entities;

using DioRed.Common;

namespace DioRed.Cache.Core.Contracts;

public interface ICurrenciesLogic
{
    Result<IReadOnlyCollection<Currency>> GetAll();
}
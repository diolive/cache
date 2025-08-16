using DioLive.Cache.Common.Entities;

using DioRed.Common;

namespace DioLive.Cache.CoreLogic.Contacts;

public interface ICurrenciesLogic
{
    Result<IReadOnlyCollection<Currency>> GetAll();
}
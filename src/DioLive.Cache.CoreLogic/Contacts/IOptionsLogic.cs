using DioLive.Cache.Common.Entities;

using DioRed.Common;

namespace DioLive.Cache.CoreLogic.Contacts;

public interface IOptionsLogic
{
    Result<Options> Get();
    Result Update(int? purchaseGrouping, bool? showPlanList);
}
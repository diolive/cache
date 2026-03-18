using DioRed.Cache.Domain.Entities;

using DioRed.Common;

namespace DioRed.Cache.Core.Contracts;

public interface IOptionsLogic
{
    Result<Options> Get();
    Result Update(int? purchaseGrouping, bool? showPlanList);
}
using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Entities;

using DioRed.Common;

namespace DioLive.Cache.CoreLogic.Contacts;

public interface IChartsLogic
{
    Result<ChartData> Get(int days, int depth, int step);
    Result<IReadOnlyCollection<CategoryWithTotals>> GetWithTotals(int days);
}
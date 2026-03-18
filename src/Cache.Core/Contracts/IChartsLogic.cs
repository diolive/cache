using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Core.Entities;

using DioRed.Common;

namespace DioRed.Cache.Core.Contracts;

public interface IChartsLogic
{
    Result<ChartData> Get(int days, int depth, int step);
    Result<IReadOnlyCollection<CategoryWithTotals>> GetWithTotals(int days);
}
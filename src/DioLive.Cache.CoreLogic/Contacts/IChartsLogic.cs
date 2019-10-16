using System.Collections.Generic;

using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Entities;

namespace DioLive.Cache.CoreLogic.Contacts
{
	public interface IChartsLogic
	{
		Result<ChartData> Get(int days, int depth, int step);
		Result<IReadOnlyCollection<CategoryWithTotals>> GetWithTotals(int days);
	}
}
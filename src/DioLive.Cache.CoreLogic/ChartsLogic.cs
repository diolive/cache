using System.Collections.Generic;

using DioLive.Cache.Common;
using DioLive.Cache.CoreLogic.Entities;
using DioLive.Cache.CoreLogic.Jobs;
using DioLive.Cache.CoreLogic.Jobs.Charts;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.CoreLogic
{
	public class ChartsLogic : LogicBase
	{
		public ChartsLogic(ICurrentContext currentContext,
		                   JobSettings jobSettings)
			: base(currentContext, jobSettings)
		{
		}

		public Result<ChartData> Get(int days, int depth, int step)
		{
			var job = new GetJob(days, depth, step);
			return GetJobResult(job);
		}

		public Result<IReadOnlyCollection<CategoryWithTotals>> GetWithTotals(int days)
		{
			var job = new GetWithTotalsJob(days);
			return GetJobResult(job);
		}
	}
}
using DioLive.Cache.Common;
using DioLive.Cache.CoreLogic.Jobs;
using DioLive.Cache.CoreLogic.Jobs.Options;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.CoreLogic
{
	public class OptionsLogic : LogicBase
	{
		public OptionsLogic(ICurrentContext currentContext,
		                    JobSettings jobSettings)
			: base(currentContext, jobSettings)
		{
		}

		public Result<Options> Get()
		{
			var job = new GetJob();
			return GetJobResult(job);
		}

		public Result Update(int? purchaseGrouping, bool? showPlanList)
		{
			var job = new UpdateJob(purchaseGrouping, showPlanList);
			return GetJobResult(job);
		}
	}
}
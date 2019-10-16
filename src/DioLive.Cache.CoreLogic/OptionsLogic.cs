using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Contacts;
using DioLive.Cache.CoreLogic.Jobs;
using DioLive.Cache.CoreLogic.Jobs.Options;

namespace DioLive.Cache.CoreLogic
{
	public class OptionsLogic : LogicBase, IOptionsLogic
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
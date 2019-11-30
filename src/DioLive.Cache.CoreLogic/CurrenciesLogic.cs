using System.Collections.Generic;

using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Contacts;
using DioLive.Cache.CoreLogic.Jobs;
using DioLive.Cache.CoreLogic.Jobs.Currencies;

namespace DioLive.Cache.CoreLogic
{
	public class CurrenciesLogic : LogicBase, ICurrenciesLogic
	{
		public CurrenciesLogic(ICurrentContext currentContext,
		                       JobSettings jobSettings)
			: base(currentContext, jobSettings)
		{
		}

		public Result<IReadOnlyCollection<Currency>> GetAll()
		{
			var job = new GetAllJob();
			return GetJobResult(job);
		}
	}
}
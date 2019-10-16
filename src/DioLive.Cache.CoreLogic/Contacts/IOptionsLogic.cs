using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;

namespace DioLive.Cache.CoreLogic.Contacts
{
	public interface IOptionsLogic
	{
		Result<Options> Get();
		Result Update(int? purchaseGrouping, bool? showPlanList);
	}
}
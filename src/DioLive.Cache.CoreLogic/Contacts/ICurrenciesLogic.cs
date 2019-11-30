using System.Collections.Generic;

using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;

namespace DioLive.Cache.CoreLogic.Contacts
{
	public interface ICurrenciesLogic
	{
		Result<IReadOnlyCollection<Currency>> GetAll();
	}
}

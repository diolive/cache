using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.Common.Entities;

namespace DioLive.Cache.Storage.Contracts
{
	public interface ICurrenciesStorage
	{
		Task<IReadOnlyCollection<Currency>> GetAllAsync();
	}
}

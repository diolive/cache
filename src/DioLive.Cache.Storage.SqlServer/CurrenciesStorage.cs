using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.Storage.SqlServer
{
	public class CurrenciesStorage : StorageBase, ICurrenciesStorage
	{
		public CurrenciesStorage(IConnectionInfo connectionInfo,
		                         ICurrentContext currentContext)
			: base(connectionInfo, currentContext)
		{
		}

		public async Task<IReadOnlyCollection<Currency>> GetAllAsync()
		{
			return (await Connection.QueryAsync<Currency>(Queries.Currencies.SelectAll))
				.ToList()
				.AsReadOnly();
		}		
	}
}
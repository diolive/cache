using System;
using System.Data;

using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.Storage.SqlServer
{
	public abstract class StorageBase
	{
		private readonly Func<IDbConnection> _connectionAccessor;
		private readonly ICurrentContext _currentContext;

		protected StorageBase(Func<IDbConnection> connectionAccessor,
		                      ICurrentContext currentContext)
		{
			_connectionAccessor = connectionAccessor;
			_currentContext = currentContext;
		}

		protected Guid CurrentBudgetId => _currentContext.BudgetId.Value;
		protected string CurrentUserId => _currentContext.UserId;

		protected IDbConnection OpenConnection()
		{
			return _connectionAccessor();
		}
	}
}
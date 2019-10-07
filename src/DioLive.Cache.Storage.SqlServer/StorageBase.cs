using System;
using System.Data;

using DioLive.Cache.Common;

using Microsoft.Data.SqlClient;

namespace DioLive.Cache.Storage.SqlServer
{
	public abstract class StorageBase : IDisposable
	{
		private bool _isDisposed;

		protected StorageBase(IConnectionInfo connectionInfo,
		                      ICurrentContext currentContext)
		{
			CurrentUserId = currentContext.UserId;
			Connection = new SqlConnection(connectionInfo.ConnectionString);
			Connection.Open();
		}

		protected string CurrentUserId { get; }
		protected IDbConnection Connection { get; }


		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_isDisposed)
			{
				return;
			}

			if (disposing)
			{
				Connection?.Dispose();
			}

			_isDisposed = true;
		}

		~StorageBase()
		{
			Dispose(false);
		}
	}
}
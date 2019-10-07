namespace DioLive.Cache.Storage.SqlServer
{
	public class ConnectionInfo : IConnectionInfo
	{
		public ConnectionInfo(string connectionString)
		{
			ConnectionString = connectionString;
		}

		public string ConnectionString { get; }
	}
}
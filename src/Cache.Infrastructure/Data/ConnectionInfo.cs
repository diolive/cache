namespace DioRed.Cache.Infrastructure.Data;

public interface IConnectionInfo
{
    string ConnectionString { get; }
}

public class ConnectionInfo(string connectionString) : IConnectionInfo
{
    public string ConnectionString { get; } = connectionString;
}
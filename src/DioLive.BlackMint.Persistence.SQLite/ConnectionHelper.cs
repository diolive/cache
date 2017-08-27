using System.Collections.Generic;

using Microsoft.Data.Sqlite;

namespace DioLive.BlackMint.Persistence.SQLite
{
    internal static class ConnectionHelper
    {
        private static readonly Dictionary<string, SqliteConnection> _connections =
            new Dictionary<string, SqliteConnection>();

        public static SqliteConnection GetConnection(string connectionString)
        {
            if (_connections.TryGetValue(connectionString, out SqliteConnection connection))
                return connection;

            connection = new SqliteConnection(connectionString);
            _connections.Add(connectionString, connection);

            return connection;
        }
    }
}
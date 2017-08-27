using System;
using System.Threading.Tasks;

using Dapper;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace DioLive.BlackMint.Persistence.SQLite
{
    public class Logger : ILogger
    {
        private readonly SqliteConnection _connection;

        public Logger(IOptions<DataSettings> dataOptions)
        {
            _connection = ConnectionHelper.GetConnection(dataOptions.Value.ConnectionString);
        }

        public async Task WriteLog(string description)
        {
            await WriteLog(DateTime.UtcNow, description);
        }

        public async Task WriteLog(DateTime timeStamp, string description)
        {
            string sql = "INSERT INTO `Log` (`TimeStamp`, `Description`) " +
                         "VALUES (@timeStamp, @description)";
            var parameters = new { timeStamp, description };

            await _connection.ExecuteAsync(sql, parameters);
        }

        public async Task WritePurchaseLog(int purchaseId, int userId)
        {
            await WritePurchaseLog(purchaseId, DateTime.UtcNow, userId);
        }

        public async Task WritePurchaseLog(int purchaseId, DateTime timeStamp, int userId)
        {
            string sql = "INSERT INTO `PurchasesLog` (`PurchaseId`, `TimeStamp`, `UserId`) " +
                         "VALUES (@purchaseId, @timeStamp, @userId)";
            var parameters = new { purchaseId, timeStamp, userId };

            await _connection.ExecuteAsync(sql, parameters);
        }

        public async Task WriteIncomeLog(int incomeId, int userId)
        {
            await WriteIncomeLog(incomeId, DateTime.UtcNow, userId);
        }

        public async Task WriteIncomeLog(int incomeId, DateTime timeStamp, int userId)
        {
            string sql = "INSERT INTO `IncomesLog` (`IncomeId`, `TimeStamp`, `UserId`) " +
                         "VALUES (@incomeId, @timeStamp, @userId)";
            var parameters = new { incomeId, timeStamp, userId };

            await _connection.ExecuteAsync(sql, parameters);
        }
    }
}
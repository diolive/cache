using System;
using System.Threading.Tasks;

namespace DioLive.BlackMint.Persistence
{
    public interface ILogger
    {
        Task WriteLog(string description);

        Task WriteLog(DateTime timeStamp, string description);

        Task WritePurchaseLog(int purchaseId, int userId);

        Task WritePurchaseLog(int purchaseId, DateTime timeStamp, int userId);

        Task WriteIncomeLog(int incomeId, int userId);

        Task WriteIncomeLog(int incomeId, DateTime timeStamp, int userId);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.BlackMint.Entities;

namespace DioLive.BlackMint.Persistence
{
    public interface IDomainStorage
    {
        Task<int> AddBook(string name, int authorId);

        Task<int> AddIncome(Income income);

        Task<int> AddPurchase(Purchase purchase);

        Task<int> AddPurchaseItem(PurchaseItem purchaseItem);

        Task<IEnumerable<Book>> GetAccessibleBooks(int userId);

        Task<IEnumerable<Currency>> GetAllCurrencies();

        Task<Book> GetBookById(int id);

        Task<IEnumerable<Income>> GetIncomesPage(int bookId, Window window, SelectOrder order);

        Task<Purchase> GetPurchaseById(int id);

        Task<IEnumerable<Purchase>> GetPurchasesPage(int bookId, Window window, SelectOrder order);

        Task<AccessRole> GetBookAccess(int bookId, int userId);

        Task<AccessRole> GetPurchaseAccess(int purchaseId, int userId);

        Task<AccessRole> GetPurchaseItemAccess(int purchaseItemId, int userId);

        Task SetBookAccess(int bookId, int userId, AccessRole role);

        Task<bool> UpdateBookName(int id, string name);

        Task<bool> RemoveBook(int bookId);

        Task<Currency> GetCurrencyByCode(string code);

        Task<bool> UpdatePurchase(Purchase purchase);

        Task<PurchaseItem> GetPurchaseItemById(int id);

        Task<IEnumerable<PurchaseItem>> GetPurchaseItems(int purchaseId);
    }
}
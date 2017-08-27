using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.BlackMint.Entities;

namespace DioLive.BlackMint.Logic
{
    public interface IDomainLogic
    {
        Task CreateBook(Book book);

        Task<IEnumerable<Book>> GetAccessibleBooks(int userId);

        Task<Response<Book>> GetBook(int bookId, int userId);

        Task<ResponseStatus> UpdateBookName(int bookId, string newName, int userId);

        Task<ResponseStatus> DeleteBook(int bookId, int userId);

        Task<IEnumerable<Currency>> GetAllCurrencies();

        Task<Currency> GetCurrency(string code);

        Task<Response<IEnumerable<Income>>> GetIncomes(int bookId, int pageNumber, int pageSize, SelectOrder order,
                                                       int userId);

        Task<Response<IEnumerable<Purchase>>> GetPurchases(int bookId, int pageNumber, int pageSize, SelectOrder order,
                                                           int userId);

        Task<Response<Purchase>> GetPurchase(int purchaseId, int userId);

        Task<ResponseStatus> CreatePurchase(Purchase purchase, int userId);

        Task<ResponseStatus> UpdatePurchase(Purchase purchase, int userId);

        Task<Response<IEnumerable<PurchaseItem>>> GetPurchaseItems(int purchaseId, int userId);

        Task<Response<PurchaseItem>> GetPurchaseItem(int purchaseItemId, int userId);

        Task<ResponseStatus> CreatePurchaseItem(PurchaseItem purchaseItem, int userId);
    }
}
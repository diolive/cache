using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Dapper;

using DioLive.BlackMint.Entities;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace DioLive.BlackMint.Persistence.SQLite
{
    public class DomainStorage : IDomainStorage
    {
        private readonly SqliteConnection _connection;

        public DomainStorage(IOptions<DataSettings> dataOptions)
        {
            _connection = ConnectionHelper.GetConnection(dataOptions.Value.ConnectionString);
        }

        #region Book

        public async Task<int> AddBook(string name, int authorId)
        {
            var parameters = new { name, authorId };

            int bookId = await _connection.ExecuteScalarAsync<int>(Queries.Book.Add, parameters);

            return bookId;
        }

        public async Task<IEnumerable<Book>> GetAccessibleBooks(int userId)
        {
            var parameters = new { userId };

            return await _connection.QueryAsync<Book>(Queries.Book.GetAccessible, parameters);
        }

        public async Task<Book> GetBookById(int id)
        {
            var parameters = new { id };

            return await _connection.QueryFirstOrDefaultAsync<Book>(Queries.Book.Get, parameters);
        }

        public async Task<AccessRole> GetBookAccess(int bookId, int userId)
        {
            var parameters = new { bookId, userId };

            return await _connection.QueryFirstOrDefaultAsync<AccessRole>(Queries.BookAccess.GetRole, parameters);
        }

        public async Task SetBookAccess(int bookId, int userId, AccessRole role)
        {
            AccessRole currentRole = await GetBookAccess(bookId, userId);

            if (currentRole == role)
            {
                return;
            }

            if (role == AccessRole.None)
            {
                var parameters = new { bookId, userId };

                await _connection.ExecuteAsync(Queries.BookAccess.Delete, parameters);
            }
            else
            {
                var parameters = new { bookId, userId, role };
                string query = currentRole == AccessRole.None ? Queries.BookAccess.Add : Queries.BookAccess.Update;

                await _connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task<bool> UpdateBookName(int id, string name)
        {
            var parameters = new { id, name };

            int records = await _connection.ExecuteAsync(Queries.Book.UpdateName, parameters);

            if (records > 1)
            {
                throw new InvalidOperationException("Critical behavior: several books were updated.");
            }

            return records == 1;
        }

        public async Task<bool> RemoveBook(int bookId)
        {
            string sql = Queries.Book.Delete;
            var parameters = new { id = bookId };

            int records = await _connection.ExecuteAsync(sql, parameters);

            if (records > 1)
            {
                throw new InvalidOperationException("Critical behavior: several books were removed.");
            }

            return records == 1;
        }

        #endregion

        #region Income

        public async Task<int> AddIncome(Income income)
        {
            var parameters = new
            {
                bookId = income.BookId,
                source = income.Source,
                date = income.Date,
                value = income.Amount.Value,
                currency = income.Amount.Currency,
                comments = income.Comments
            };

            int incomeId = await _connection.ExecuteScalarAsync<int>(Queries.Income.Add, parameters);
            income.Id = incomeId;

            return incomeId;
        }

        public async Task<IEnumerable<Income>> GetIncomesPage(int bookId, Window window, SelectOrder order)
        {
            string query = string.Format(Queries.Income.GetOrdered, order.ToSql("`Date`"));
            var parameters = new { bookId, window.Limit, window.Offset };

            return await _connection.QueryAsync<Income>(query, parameters);
        }

        #endregion

        #region Purchase

        public async Task<int> AddPurchase(Purchase purchase)
        {
            int purchaseId = await _connection.ExecuteScalarAsync<int>(Queries.Purchase.Add, purchase);
            purchase.Id = purchaseId;

            return purchaseId;
        }

        public async Task<Purchase> GetPurchaseById(int id)
        {
            var parameters = new { id };

            return await _connection.QueryFirstOrDefaultAsync<Purchase>(Queries.Purchase.Get, parameters);
        }

        public async Task<AccessRole> GetPurchaseAccess(int purchaseId, int userId)
        {
            var parameters = new { purchaseId, userId };

            return await _connection.QueryFirstOrDefaultAsync<AccessRole>(Queries.Purchase.GetRole, parameters);
        }

        public async Task<IEnumerable<Purchase>> GetPurchasesPage(int bookId, Window window, SelectOrder order)
        {
            string query = string.Format(Queries.Purchase.GetOrdered, order.ToSql("`Date`"));
            var parameters = new { bookId, window.Limit, window.Offset };

            return await _connection.QueryAsync<Purchase>(query, parameters);
        }

        public async Task<bool> UpdatePurchase(Purchase purchase)
        {
            int records = await _connection.ExecuteAsync(Queries.Purchase.Update, purchase);

            if (records > 1)
            {
                throw new InvalidOperationException("Critical behavior: several purchases were updated.");
            }

            return records == 1;
        }

        #endregion

        #region PurchaseItem

        public async Task<int> AddPurchaseItem(PurchaseItem purchaseItem)
        {
            int purchaseItemId = await _connection.ExecuteScalarAsync<int>(Queries.PurchaseItem.Add, purchaseItem);
            purchaseItem.Id = purchaseItemId;

            return purchaseItemId;
        }

        public async Task<PurchaseItem> GetPurchaseItemById(int id)
        {
            var parameters = new { id };

            return await _connection.QueryFirstOrDefaultAsync<PurchaseItem>(Queries.PurchaseItem.Get, parameters);
        }

        public async Task<IEnumerable<PurchaseItem>> GetPurchaseItems(int purchaseId)
        {
            var parameters = new { purchaseId };

            return await _connection.QueryAsync<PurchaseItem>(Queries.PurchaseItem.GetByPurchase, parameters);
        }

        public async Task<AccessRole> GetPurchaseItemAccess(int purchaseItemId, int userId)
        {
            var parameters = new { purchaseItemId, userId };

            return await _connection.QueryFirstOrDefaultAsync<AccessRole>(Queries.PurchaseItem.GetRole, parameters);
        }

        #endregion

        #region Currency

        public async Task<IEnumerable<Currency>> GetAllCurrencies()
        {
            return await _connection.QueryAsync<Currency>(Queries.Currency.GetAll);
        }

        public async Task<Currency> GetCurrencyByCode(string code)
        {
            var parameters = new { code };

            return await _connection.QueryFirstOrDefaultAsync<Currency>(Queries.Currency.Get, parameters);
        }

        #endregion
    }
}
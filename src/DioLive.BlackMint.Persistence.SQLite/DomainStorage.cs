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

        private static string GetOrderClause(SelectOrder order)
        {
            switch (order)
            {
                case SelectOrder.Ascending:
                    return "ASC";

                case SelectOrder.Descending:
                    return "DESC";

                default:
                    throw new ArgumentException($"Unsupported value: {order}", nameof(order));
            }
        }

        #region Book

        public async Task<int> AddBook(string name, int authorId)
        {
            string sql = "INSERT INTO `Books` (`Name`, `AuthorId`) " +
                         "VALUES (@name, @authorId)" +
                         SqlHelper.SelectIdentity;
            var parameters = new { name, authorId };

            int bookId = await _connection.ExecuteScalarAsync<int>(sql, parameters);

            return bookId;
        }

        public async Task<IEnumerable<Book>> GetAccessibleBooks(int userId)
        {
            string sql = "SELECT b.* FROM `Books` b " +
                         "INNER JOIN `BookAccess` ba ON b.`Id`=ba.`BookId` " +
                         "WHERE ba.`UserId`=@userId";
            var parameters = new { userId };

            return await _connection.QueryAsync<Book>(sql, parameters);
        }

        public async Task<Book> GetBookById(int id)
        {
            string sql = "SELECT * FROM `Books` " +
                         "WHERE `Id`=@id";
            var parameters = new { id };

            return await _connection.QueryFirstOrDefaultAsync<Book>(sql, parameters);
        }

        public async Task<AccessRole> GetBookAccess(int bookId, int userId)
        {
            string sql = "SELECT `Role` FROM `BookAccess` " +
                         "WHERE `BookId`=@bookId AND `UserId`=@userId " +
                         "LIMIT 1";
            var parameters = new { bookId, userId };

            return await _connection.QueryFirstOrDefaultAsync<AccessRole>(sql, parameters);
        }

        public async Task SetBookAccess(int bookId, int userId, AccessRole role)
        {
            string sql;
            object param;

            if (role != AccessRole.None)
            {
                sql = "INSERT INTO `BookAccess` (`BookId`, `UserId`, `Role`) " +
                      "VALUES (@bookId, @userId, @role)";
                param = new { bookId, userId, role };
            }
            else
            {
                sql = "DELETE FROM `BookAccess` " +
                      "WHERE `BookId`=@bookId AND `UserId`=@userId";
                param = new { bookId, userId };
            }

            await _connection.ExecuteAsync(sql, param);
        }

        public async Task<bool> UpdateBookName(Book book)
        {
            string sql = "UPDATE `Books` " +
                         "SET `Name`=@Name " +
                         "WHERE `Id`=@Id";

            int records = await _connection.ExecuteAsync(sql, book);

            if (records > 1)
                throw new InvalidOperationException("Critical behavior: several books were updated.");

            return records == 1;
        }

        public async Task<bool> RemoveBook(int bookId)
        {
            string sql = "DELETE FROM `Books` " +
                         "WHERE `Id`=@id";
            var parameters = new { id = bookId };

            int records = await _connection.ExecuteAsync(sql, parameters);

            if (records > 1)
                throw new InvalidOperationException("Critical behavior: more than 1 books were removed.");

            return records == 1;
        }

        #endregion

        #region Income

        public async Task<int> AddIncome(Income income)
        {
            string sql = "INSERT INTO `Incomes` (`BookId`, `Source`, `Date`, `Value`, `Currency`, `Comments`) " +
                         "VALUES (@bookId, @source, @date, @value, @currency, @comments)" +
                         SqlHelper.SelectIdentity;
            var parameters = new
            {
                bookId = income.BookId,
                source = income.Source,
                date = income.Date,
                value = income.Amount.Value,
                currency = income.Amount.Currency,
                comments = income.Comments
            };

            int incomeId = await _connection.ExecuteScalarAsync<int>(sql, parameters);
            income.Id = incomeId;

            return incomeId;
        }

        public async Task<IEnumerable<Income>> GetIncomesPage(int bookId, Window window, SelectOrder order)
        {
            string sql = "SELECT * FROM `Incomes` " +
                         "WHERE `BookId`=@bookId " +
                         "ORDER BY `Date` " + GetOrderClause(order) +
                         " LIMIT @Limit OFFSET @Offset";
            var parameters = new { bookId, window.Limit, window.Offset };

            return await _connection.QueryAsync<Income>(sql, parameters);
        }

        #endregion

        #region Purchase

        public async Task<int> AddPurchase(Purchase purchase)
        {
            string sql = "INSERT INTO `Purchases` (`BookId`, `Seller`, `Date`, `TotalCost`, `Currency`, `Comments`) " +
                         "VALUES (@BookId, @Seller, @Date, 0, @Currency, @Comments)" +
                         SqlHelper.SelectIdentity;

            int purchaseId = await _connection.ExecuteScalarAsync<int>(sql, purchase);
            purchase.Id = purchaseId;

            return purchaseId;
        }

        public async Task<Purchase> GetPurchaseById(int id)
        {
            string sql = "SELECT * FROM `Purchases` " +
                         "WHERE `Id`=@id";
            var parameters = new { id };

            return await _connection.QueryFirstOrDefaultAsync<Purchase>(sql, parameters);
        }

        public async Task<AccessRole> GetPurchaseAccess(int purchaseId, int userId)
        {
            string sql = "SELECT ba.`Role` FROM `BookAccess` ba " +
                         "INNER JOIN `Purchases` p ON p.`BookId`=ba.`BookId` " +
                         "WHERE p.`Id`=@purchaseId AND ba.`UserId`=@userId " +
                         "LIMIT 1";
            var parameters = new { purchaseId, userId };

            return await _connection.QueryFirstOrDefaultAsync<AccessRole>(sql, parameters);
        }

        public async Task<IEnumerable<Purchase>> GetPurchasesPage(int bookId, Window window, SelectOrder order)
        {
            string sql = "SELECT * FROM `Purchases` " +
                         "WHERE `BookId`=@bookId " +
                         "ORDER BY `Date` " + GetOrderClause(order) +
                         " LIMIT @Limit OFFSET @Offset";
            var parameters = new { bookId, window.Limit, window.Offset };

            return await _connection.QueryAsync<Purchase>(sql, parameters);
        }

        #endregion

        #region PurchaseItem

        public async Task<int> AddPurchaseItem(PurchaseItem purchaseItem)
        {
            string sql = "INSERT INTO `PurchaseItems` (`PurchaseId`, `Name`, `Price`, `Count`) " +
                         "VALUES (@PurchaseId, @Name, @Price, @Count)" +
                         SqlHelper.SelectIdentity;

            int purchaseItemId = await _connection.ExecuteScalarAsync<int>(sql, purchaseItem);

            return purchaseItemId;
        }

        public async Task<IEnumerable<PurchaseItem>> GetPurchaseItems(int purchaseId)
        {
            string sql = "SELECT * FROM `PurchaseItems` " +
                         "WHERE `PurchaseId`=@purchaseId";
            var parameters = new { purchaseId };

            return await _connection.QueryAsync<PurchaseItem>(sql, parameters);
        }

        #endregion

        #region Currency

        public async Task<IEnumerable<Currency>> GetAllCurrencies()
        {
            var sql = "SELECT * FROM `Currencies`";

            return await _connection.QueryAsync<Currency>(sql);
        }

        public async Task<Currency> GetCurrencyByCode(string code)
        {
            string sql = "SELECT * FROM `Currencies` " +
                         "WHERE `Code`=@code " +
                         "LIMIT 1";
            var parameters = new { code };

            return await _connection.QueryFirstOrDefaultAsync<Currency>(sql, parameters);
        }

        #endregion
    }
}
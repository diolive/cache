using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

using Dapper;
using Dapper.Contrib.Extensions;

using DioLive.BlackMint.WebApp.Models;

namespace DioLive.BlackMint.WebApp.Data
{
    public static class Database
    {
        public static async Task<bool> AddNewBook(SqlConnection connection, Book book)
        {
            return await connection.InsertAsync(book) > 0;
        }

        public static async Task<IEnumerable<Book>> GetAccessibleBooks(SqlConnection connection, int userId)
        {
            return await connection.QueryAsync<Book>(
                "SELECT b.[Id], b.[Name], b.[AuthorId] FROM [Books] b INNER JOIN [BookAccess] ba ON b.[Id] = ba.[BookId] WHERE ba.[UserId] = @userId",
                new { userId });
        }

        public static async Task<IEnumerable<Currency>> GetAllCurrencies(SqlConnection connection)
        {
            return await connection.GetAllAsync<Currency>();
        }

        public static async Task<Book> GetBookById(SqlConnection connection, int bookId)
        {
            return await connection.GetAsync<Book>(bookId);
        }

        public static async Task<Currency> GetCurrencyByCode(SqlConnection connection, string code)
        {
            return await connection.GetAsync<Currency>(code);
        }

        public static async Task<IEnumerable<IncomeInfo>> GetIncomesPageByBook(
            SqlConnection connection, int bookId, int pageNumber, int pageSize, bool asc)
        {
            string order = asc ? "ASC" : "DESC";
            int offset = pageNumber * pageSize;

            return await connection.QueryAsync<IncomeInfo>(
                $"SELECT [Id], [Source], [Date], [Value], [Currency] FROM [Incomes] WHERE [BookId] = @bookId ORDER BY [Date] {order} OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY",
                new { bookId, offset, pageSize });
        }

        public static async Task<IEnumerable<PurchaseInfo>> GetPurchasesPageByBook(
            SqlConnection connection, int bookId, int pageNumber, int pageSize, bool asc)
        {
            string order = asc ? "ASC" : "DESC";
            int offset = pageNumber * pageSize;

            return await connection.QueryAsync<PurchaseInfo>(
                $"SELECT [Id], [Seller], [Date], [TotalCost], [Currency] FROM [Purchases] WHERE [BookId] = @bookId ORDER BY [Date] {order} OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY",
                new { bookId, offset, pageSize });
        }

        public static async Task<string> GetUserAccessForBook(SqlConnection connection, int userId, int bookId)
        {
            return await connection.ExecuteScalarAsync<string>(
                "SELECT TOP 1 [Role] FROM [BookAccess] WHERE [BookId] = @bookId AND [UserId] = @userId",
                new { bookId, userId });
        }

        public static async Task<string> GetUserDisplayNameById(SqlConnection connection, int id)
        {
            return await connection.ExecuteScalarAsync<string>("SELECT TOP 1 [DisplayName] FROM [Users] WHERE [Id]=@id",
                new { id });
        }

        public static async Task<int?> GetUserIdByNameIdentity(SqlConnection connection, string nameIdentity)
        {
            return await connection.ExecuteScalarAsync<int?>(
                "SELECT TOP 1 [UserId] FROM [UserIdentities] WHERE [NameIdentity] = @nameIdentity",
                new { nameIdentity });
        }
    }
}
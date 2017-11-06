using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.BlackMint.Entities;

namespace DioLive.BlackMint.Logic.Implementation
{
    public partial class DomainLogic
    {
        public async Task CreateBook(Book book)
        {
            if (book is null)
                throw new ArgumentNullException(nameof(book));

            Validators.ValidateBookName(book.Name);

            int bookId = await _domainStorage.AddBook(book.Name, book.AuthorId);
            book.Id = bookId;
            await _logger.WriteLog($"Create book id:{bookId} by user id:{book.AuthorId}");

            await _domainStorage.SetBookAccess(bookId, book.AuthorId, AccessRole.Unlimited);
            await _logger.WriteLog($"Granted Unlimited access for user id:{book.AuthorId} on book id:{bookId}");
        }

        public async Task<IEnumerable<Book>> GetAccessibleBooks(int userId)
        {
            Validators.ValidateUserId(userId);

            return await _domainStorage.GetAccessibleBooks(userId);
        }

        public async Task<Response<Book>> GetBook(int bookId, int userId)
        {
            Validators.ValidateUserId(userId);

            ResponseStatus result = await ValidateBookAccess(bookId, userId, AccessRole.Read);

            if (result != ResponseStatus.Success)
                return new Response<Book>(result);

            Book book = await _domainStorage.GetBookById(bookId);
            return Response<Book>.Success(book);
        }

        public async Task<ResponseStatus> UpdateBookName(int bookId, string newName, int userId)
        {
            Validators.ValidateUserId(userId);
            Validators.ValidateBookName(newName);

            ResponseStatus result = await ValidateBookAccess(bookId, userId, AccessRole.Unlimited);
            if (result != ResponseStatus.Success)
                return result;

            return await _domainStorage.UpdateBookName(bookId, newName)
                ? ResponseStatus.Success
                : ResponseStatus.NotFound;
        }

        public async Task<ResponseStatus> DeleteBook(int bookId, int userId)
        {
            Validators.ValidateUserId(userId);

            ResponseStatus result = await ValidateBookAccess(bookId, userId, AccessRole.Unlimited);
            if (result != ResponseStatus.Success)
                return result;

            return await _domainStorage.RemoveBook(bookId)
                ? ResponseStatus.Success
                : ResponseStatus.NotFound;
        }
    }
}
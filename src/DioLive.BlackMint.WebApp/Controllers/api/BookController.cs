using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.BlackMint.WebApp.Data;
using DioLive.BlackMint.WebApp.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DioLive.BlackMint.WebApp.Controllers.api
{
    [Authorize]
    public class BookController : ApiControllerBase
    {
        public BookController(IOptions<DataSettings> dataOptions)
            : base(dataOptions)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            if (!HasUserId) return Logout();

            IEnumerable<Book> books = await Database.GetAccessibleBooks(Db, UserId);
            return Json(books);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            if (!HasUserId) return Logout();

            string access = await Database.GetUserAccessForBook(Db, UserId, id);
            if (access is null)
            {
                return Forbid();
            }

            Book book = await Database.GetBookById(Db, id);
            return JsonOrNotFound(book);
        }

        [HttpPost]
        public async Task<IActionResult> Post(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Field 'name' is mandatory.");
            }

            if (name.Length > 100)
            {
                return BadRequest("Field 'name' is too long. Shouldn't be longer than 100 chars.");
            }

            if (!HasUserId) return Logout();

            var book = new Book
            {
                Name = name,
                AuthorId = UserId
            };
            bool result = await Database.AddNewBook(Db, book);
            if (result)
            {
                return Created(Url.Action("Get", new { id = book.Id }), book);
            }

            return BadRequest("Cannot create a new book");
        }
    }
}
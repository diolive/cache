using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.BlackMint.Entities;
using DioLive.BlackMint.Logic;
using DioLive.BlackMint.WebApp.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DioLive.BlackMint.WebApp.Controllers.api
{
    [Authorize]
    public class BookController : ApiControllerBase
    {
        private readonly IDomainLogic _domainLogic;

        public BookController(IDomainLogic domainLogic)
        {
            _domainLogic = domainLogic;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAccessible()
        {
            IEnumerable<Book> books = await _domainLogic.GetAccessibleBooks(UserId);
            return Json(books);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            Response<Book> response = await _domainLogic.GetBook(id, UserId);
            return ResponseToResult(response);
        }

        [HttpPost]
        public async Task<IActionResult> Post(NewBookVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var book = new Book
            {
                Name = model.Name.Trim(),
                AuthorId = UserId
            };

            await _domainLogic.CreateBook(book);
            return Created(Url.Action(nameof(Get), new { id = book.Id }), book);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateBookVM model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ResponseStatus responseStatus = await _domainLogic.UpdateBookName(model.Id, model.Name.Trim(), UserId);
            return ResponseStatusToResult(responseStatus);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ResponseStatus responseStatus = await _domainLogic.DeleteBook(id, UserId);
            return ResponseStatusToResult(responseStatus);
        }
    }
}
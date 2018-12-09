using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.BlackMint.Entities;
using DioLive.BlackMint.Logic;
using DioLive.BlackMint.WebApp.Extensions;

using Microsoft.AspNetCore.Mvc;

namespace DioLive.BlackMint.WebApp.Controllers
{
    public class HomeController : ControllerBase
    {
        private readonly IDomainLogic _domainLogic;

        public HomeController(IDomainLogic domainLogic)
        {
            _domainLogic = domainLogic;
        }

        public async Task<IActionResult> Index()
        {
            if (UserId == 0)
                return View("Index");

            ViewBag.CurrentUser = HttpContext.GetCurrentUser();

            Book book = CurrentBook;
            if (book is null)
            {
                IEnumerable<Book> booksModel = await _domainLogic.GetAccessibleBooks(UserId);
                return View("BookSelect", booksModel);
            }

            return RedirectToAction("Purchases", new { bookId = book.Id });
        }

        public IActionResult Error()
        {
            return View();
        }

        public async Task<IActionResult> SelectBook(int id)
        {
            Response<Book> response = await _domainLogic.GetBook(id, UserId);
            switch (response.Status)
            {
                case ResponseStatus.NotFound:
                    return NotFound();

                case ResponseStatus.Forbidden:
                    return Forbid();

                case ResponseStatus.Success:
                    CurrentBook = response.Result;
                    return RedirectToAction("Index");

                default:
                    return BadRequest();
            }
        }

        public async Task<IActionResult> Purchases(int bookId)
        {
            Response<IEnumerable<Purchase>> response =
                await _domainLogic.GetPurchases(bookId, 0, 10000, SelectOrder.Ascending, UserId);

            switch (response.Status)
            {
                case ResponseStatus.Forbidden:
                    return Forbid();

                case ResponseStatus.NotFound:
                    return NotFound();

                case ResponseStatus.Success:
                    return View("Purchases", response.Result);

                default:
                    return BadRequest();
            }
        }
    }
}
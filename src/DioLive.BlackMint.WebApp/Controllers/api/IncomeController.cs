using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.BlackMint.Entities;
using DioLive.BlackMint.Logic;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DioLive.BlackMint.WebApp.Controllers.api
{
    [Authorize]
    public class IncomeController : ApiControllerBase
    {
        private readonly IDomainLogic _domainLogic;

        public IncomeController(IDomainLogic domainLogic)
        {
            _domainLogic = domainLogic;
        }

        [HttpGet("/api/book/{bookId:int}/incomes")]
        public async Task<IActionResult> Get(int bookId, int pageNumber = 0, int pageSize = 20, bool asc = true)
        {
            if (pageNumber < 0 || pageSize <= 0)
                return BadRequest();

            SelectOrder order = asc ? SelectOrder.Ascending : SelectOrder.Descending;
            Response<IEnumerable<Income>> response =
                await _domainLogic.GetIncomes(bookId, pageNumber, pageSize, order, UserId);

            return ResponseToResult(response);
        }
    }
}
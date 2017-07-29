﻿using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.BlackMint.WebApp.Data;
using DioLive.BlackMint.WebApp.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DioLive.BlackMint.WebApp.Controllers.api
{
    [Authorize]
    public class IncomeController : ApiControllerBase
    {
        public IncomeController(IOptions<DataSettings> dataOptions)
            : base(dataOptions)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Get(int bookId, int pageNumber, int pageSize, bool asc = false)
        {
            if (!HasUserId) return Logout();

            string access = await Database.GetUserAccessForBook(Db, UserId, bookId);

            if (access is null) return Forbid();

            Task<IEnumerable<IncomeInfo>> incomes =
                Database.GetIncomesPageByBook(Db, bookId, pageNumber, pageSize, asc);
            return Json(incomes);
        }
    }
}
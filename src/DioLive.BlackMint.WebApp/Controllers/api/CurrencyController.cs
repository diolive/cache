using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.BlackMint.WebApp.Data;
using DioLive.BlackMint.WebApp.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DioLive.BlackMint.WebApp.Controllers.api
{
    public class CurrencyController : ApiControllerBase
    {
        public CurrencyController(IOptions<DataSettings> dataOptions)
            : base(dataOptions)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            IEnumerable<Currency> currencies = await Database.GetAllCurrencies(Db);
            return Json(currencies);
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> Get(string code)
        {
            Currency currency = await Database.GetCurrencyByCode(Db, code);
            return JsonOrNotFound(currency);
        }
    }
}
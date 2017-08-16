using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.BlackMint.Entities;
using DioLive.BlackMint.Logic;

using Microsoft.AspNetCore.Mvc;

namespace DioLive.BlackMint.WebApp.Controllers.api
{
    public class CurrencyController : ApiControllerBase
    {
        private readonly IDomainLogic _domainLogic;

        public CurrencyController(IDomainLogic domainLogic)
        {
            _domainLogic = domainLogic;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<Currency> currencies = await _domainLogic.GetAllCurrencies();
            return Json(currencies);
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> Get(string code)
        {
            Currency currency = await _domainLogic.GetCurrency(code);
            return JsonOrNotFound(currency);
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.BlackMint.Entities;

namespace DioLive.BlackMint.Logic.Implementation
{
    public partial class DomainLogic
    {
        public async Task<IEnumerable<Currency>> GetAllCurrencies()
        {
            return await _domainStorage.GetAllCurrencies();
        }

        public async Task<Currency> GetCurrency(string code)
        {
            Validators.ValidateCurrency(code);

            return await _domainStorage.GetCurrencyByCode(code);
        }
    }
}
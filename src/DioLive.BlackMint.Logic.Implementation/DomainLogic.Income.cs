using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.BlackMint.Entities;

namespace DioLive.BlackMint.Logic.Implementation
{
    public partial class DomainLogic
    {
        public async Task<Response<IEnumerable<Income>>> GetIncomes(int bookId, int pageNumber, int pageSize,
                                                                    SelectOrder order, int userId)
        {
            Validators.ValidateUserId(userId);

            ResponseStatus result = await ValidateBookAccess(bookId, userId, AccessRole.Read);
            if (result != ResponseStatus.Success)
                return new Response<IEnumerable<Income>>(result);

            IEnumerable<Income> incomes = await _domainStorage.GetIncomesPage(bookId,
                new Window { Limit = pageSize, Offset = pageNumber * pageSize },
                order);

            return Response<IEnumerable<Income>>.Success(incomes);
        }
    }
}
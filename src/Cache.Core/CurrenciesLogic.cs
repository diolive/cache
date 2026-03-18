using DioRed.Cache.Domain;
using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Core.Contracts;
using DioRed.Cache.Core.Jobs;
using DioRed.Cache.Core.Jobs.Currencies;

using DioRed.Common;

namespace DioRed.Cache.Core;

public class CurrenciesLogic(
    ICurrentContext currentContext,
    JobSettings jobSettings
) : LogicBase(currentContext, jobSettings), ICurrenciesLogic
{
    public Result<IReadOnlyCollection<Currency>> GetAll()
    {
        var job = new GetAllJob();
        return GetJobResult(job);
    }
}
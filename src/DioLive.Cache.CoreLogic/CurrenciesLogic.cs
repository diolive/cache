using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Contacts;
using DioLive.Cache.CoreLogic.Jobs;
using DioLive.Cache.CoreLogic.Jobs.Currencies;

using DioRed.Common;

namespace DioLive.Cache.CoreLogic;

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
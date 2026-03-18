using DioRed.Cache.Domain;
using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Core.Contracts;
using DioRed.Cache.Core.Jobs;
using DioRed.Cache.Core.Jobs.Budgets;

using DioRed.Common;

namespace DioRed.Cache.Core;

public class BudgetsLogic(
    ICurrentContext currentContext,
    JobSettings jobSettings
) : LogicBase(currentContext, jobSettings), IBudgetsLogic
{
    public Result<Guid> Create(string budgetName, string currencyId)
    {
        var job = new CreateJob(budgetName, currencyId);
        return GetJobResult(job);
    }

    public Result Delete()
    {
        var job = new DeleteJob();
        return GetJobResult(job);
    }

    public Result<string> GetName()
    {
        var job = new GetNameJob();
        return GetJobResult(job);
    }

    public Result<(string name, string authorName)> GetNameAndAuthor()
    {
        var job = new GetNameAndAuthorJob();
        return GetJobResult(job);
    }

    public Result<BudgetSlim> Open(Guid budgetId)
    {
        var job = new OpenJob(budgetId);
        return GetJobResult(job);
    }

    public Result Rename(string newBudgetName)
    {
        var job = new RenameJob(newBudgetName);
        return GetJobResult(job);
    }

    public Result Share(string targetUserName, ShareAccess targetAccess)
    {
        var job = new ShareJob(targetUserName, targetAccess);
        return GetJobResult(job);
    }

    public Result<IReadOnlyCollection<ShareItem>> GetShares()
    {
        var job = new GetSharesJob();
        return GetJobResult(job);
    }

    public Result<IReadOnlyCollection<Budget>> GetAllAvailable()
    {
        var job = new GetAllAvailableJob();
        return GetJobResult(job);
    }

    public Result<string> GetCurrencySign()
    {
        var job = new GetCurrencySignJob();
        return GetJobResult(job);
    }
}
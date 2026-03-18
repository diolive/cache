using System.ComponentModel.DataAnnotations;

using DioRed.Cache.Domain;
using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Core.Contracts;
using DioRed.Cache.Core.Exceptions;
using DioRed.Cache.Core.Jobs;
using DioRed.Cache.Core.Jobs.Purchases;

using DioRed.Common;

namespace DioRed.Cache.Core;

public class PurchasesLogic(
    ICurrentContext currentContext,
    JobSettings jobSettings
) : LogicBase(currentContext, jobSettings), IPurchasesLogic
{
    public Result<IReadOnlyCollection<(Purchase purchase, Category category)>> FindWithCategories(string? filter)
    {
        var job = new FindWithCategoriesJob(filter);
        return GetJobResult(job);
    }

    public Result Create(string name, int categoryId, DateTime date, decimal cost, string? shop, string? comments, int? planId)
    {
        var job = new CreateJob(name, categoryId, date, cost, shop, comments, planId);
        return GetJobResult(job);
    }

    public Result<Purchase> Get(Guid id)
    {
        var job = new GetJob(id);
        return GetJobResult(job).NotFoundIfNull();
    }

    public Result<PurchaseWithNames> GetWithNames(Guid id)
    {
        var job = new GetWithNamesJob(id);
        return GetJobResult(job).NotFoundIfNull();
    }

    public Result Update(Guid id, string name, int categoryId, DateTime date, decimal cost, string? shop, string? comments)
    {
        var job = new UpdateJob(id, name, categoryId, date, cost, shop, comments);
        return GetJobResult(job);
    }

    public Result Delete(Guid id)
    {
        var job = new DeleteJob(id);
        return GetJobResult(job);
    }

    public Result<IReadOnlyCollection<string>> GetShops()
    {
        var job = new GetShopsJob();
        return GetJobResult(job);
    }

    public Result<IReadOnlyCollection<string>> GetNames(string filter)
    {
        var job = new GetNamesJob(filter);
        return GetJobResult(job);
    }
}
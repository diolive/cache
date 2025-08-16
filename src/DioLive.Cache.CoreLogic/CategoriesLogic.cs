using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Contacts;
using DioLive.Cache.CoreLogic.Jobs;
using DioLive.Cache.CoreLogic.Jobs.Categories;

using DioRed.Common;

namespace DioLive.Cache.CoreLogic;

public class CategoriesLogic(
    ICurrentContext currentContext,
    JobSettings jobSettings
) : LogicBase(currentContext, jobSettings), ICategoriesLogic
{
    public Result<IReadOnlyCollection<Category>> GetAll()
    {
        var job = new GetAllJob();
        return GetJobResult(job);
    }

    public Result<int> Create(string newCategoryName)
    {
        var job = new CreateJob(newCategoryName);
        return GetJobResult(job);
    }

    public Result Update(int categoryId, int? parentCategoryId, string name, string color)
    {
        var job = new UpdateJob(categoryId, parentCategoryId, name, color);
        return GetJobResult(job);
    }

    public Result<Category> Get(int categoryId)
    {
        var job = new GetJob(categoryId);
        return GetJobResult(job).NotFoundIfNull();
    }

    public Result Delete(int categoryId)
    {
        var job = new DeleteJob(categoryId);
        return GetJobResult(job);
    }

    public Result<int> GetPrevious(string purchaseName)
    {
        var job = new GetPreviousJob(purchaseName);
        return GetJobResult(job).NotFoundIfNull();
    }

    public Result<int?> GetMostPopularId()
    {
        var job = new GetMostPopularJob();
        return GetJobResult(job);
    }
}
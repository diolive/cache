using System.Collections.Generic;
using System.Linq;

using DioLive.Cache.Common;
using DioLive.Cache.CoreLogic.Jobs;
using DioLive.Cache.CoreLogic.Jobs.Categories;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.CoreLogic
{
	public class CategoriesLogic : LogicBase
	{
		public CategoriesLogic(ICurrentContext currentContext,
		                       JobSettings jobSettings)
			: base(currentContext, jobSettings)
		{
		}

		public Result<IReadOnlyCollection<Category>> GetAll()
		{
			var job = new GetAllJob(CurrentContext.Culture);
			return GetJobResult(job);
		}

		public Result<(Hierarchy<Category, int> hierarchy, ILookup<int, LocalizedName> localizations)> GetHierarchyAndLocalizations()
		{
			var job = new GetHierarchyAndLocalizationsJob();
			return GetJobResult(job);
		}

		public Result<int> Create(string newCategoryName)
		{
			var job = new CreateJob(newCategoryName);
			return GetJobResult(job);
		}

		public Result Update(int categoryId, int? parentCategoryId, LocalizedName[] translates, string color)
		{
			var job = new UpdateJob(categoryId, parentCategoryId, translates, color);
			return GetJobResult(job);
		}

		public Result<Category> Get(int categoryId)
		{
			var job = new GetJob(categoryId);
			return GetJobResult(job);
		}

		public Result Delete(int categoryId)
		{
			var job = new DeleteJob(categoryId);
			return GetJobResult(job);
		}

		public Result<int> GetPrevious(string purchaseName)
		{
			var job = new GetPreviousJob(purchaseName);
			return GetJobResult(job).NullMeansNotFound();
		}

		public Result<int?> GetMostPopularId()
		{
			var job = new GetMostPopularJob();
			return GetJobResult(job);
		}
	}
}
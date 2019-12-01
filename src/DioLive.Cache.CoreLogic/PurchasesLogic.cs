using System;
using System.Collections.Generic;

using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Contacts;
using DioLive.Cache.CoreLogic.Jobs;
using DioLive.Cache.CoreLogic.Jobs.Purchases;

namespace DioLive.Cache.CoreLogic
{
	public class PurchasesLogic : LogicBase, IPurchasesLogic
	{
		public PurchasesLogic(ICurrentContext currentContext,
		                      JobSettings jobSettings)
			: base(currentContext, jobSettings)
		{
		}

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
			return GetJobResult(job).NullMeansNotFound();
		}

		public Result<PurchaseWithNames> GetWithNames(Guid id)
		{
			var job = new GetWithNamesJob(id);
			return GetJobResult(job).NullMeansNotFound();
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
}
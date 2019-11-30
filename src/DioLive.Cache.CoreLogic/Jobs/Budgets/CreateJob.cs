using System;
using System.Threading.Tasks;

using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Budgets
{
	[Authenticated]
	public class CreateJob : Job<Guid>
	{
		private readonly string _name;
		private readonly string _currencyId;

		public CreateJob(string name, string currencyId)
		{
			_name = name;
			_currencyId = currencyId;
		}

		protected override async Task<Guid> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			Guid budgetId = await storageCollection.Budgets.AddAsync(_name, _currencyId);
			await storageCollection.Categories.InitializeCategoriesAsync(budgetId);

			return budgetId;
		}
	}
}
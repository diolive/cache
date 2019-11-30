using System;
using System.Threading.Tasks;

using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Budgets
{
	[Authenticated]
	public class OpenJob : Job<BudgetSlim>
	{
		private readonly Guid _budgetId;

		public OpenJob(Guid budgetId)
		{
			_budgetId = budgetId;
		}

		protected override void CustomValidation()
		{
			AssertUserHasAccessForBudget(_budgetId, ShareAccess.ReadOnly);
		}

		protected override async Task<BudgetSlim> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			byte version = await storageCollection.Budgets.GetVersionAsync(_budgetId);
			if (version == 1)
			{
				await storageCollection.Categories.CloneCommonCategories(CurrentContext.UserId, _budgetId);
				await storageCollection.Budgets.SetVersionAsync(_budgetId, 2);
			}

			string currencySign = await storageCollection.Budgets.GetCurrencyAsync(_budgetId);

			return new BudgetSlim
			{
				Id = _budgetId,
				Currency = currencySign
			};
		}
	}
}
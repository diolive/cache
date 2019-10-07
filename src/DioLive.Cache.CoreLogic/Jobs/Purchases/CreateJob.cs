using System;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.CoreLogic.Jobs.Purchases
{
	public class CreateJob : Job
	{
		private readonly int _categoryId;
		private readonly string? _comments;
		private readonly int _cost;
		private readonly DateTime _date;
		private readonly string _name;
		private readonly int? _planId;
		private readonly string? _shop;

		public CreateJob(string name, int categoryId, DateTime date, int cost, string? shop, string? comments, int? planId)
		{
			_name = name;
			_categoryId = categoryId;
			_date = date;
			_cost = cost;
			_shop = shop;
			_comments = comments;
			_planId = planId;
		}

		protected override void Validation()
		{
			AssertUserIsAuthenticated();
			AssertUserHasAccessForBudget(CurrentBudget, ShareAccess.Purchases);
		}

		protected override async Task ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			await storageCollection.Purchases.AddAsync(CurrentBudget, _name, _categoryId, _date, _cost, _shop, _comments);

			if (_planId.HasValue)
			{
				await storageCollection.Plans.BuyAsync(_planId.Value);
			}
		}
	}
}
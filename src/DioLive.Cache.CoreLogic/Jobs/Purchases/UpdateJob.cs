﻿using System;
using System.Threading.Tasks;

using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Purchases
{
	[Authenticated]
	[HasRights(ShareAccess.Purchases)]
	public class UpdateJob : Job
	{
		private readonly int _categoryId;
		private readonly string? _comments;
		private readonly int _cost;
		private readonly DateTime _date;
		private readonly Guid _id;
		private readonly string _name;
		private readonly string? _shop;

		public UpdateJob(Guid id, string name, int categoryId, DateTime date, int cost, string? shop, string? comments)
		{
			_id = id;
			_name = name;
			_categoryId = categoryId;
			_date = date;
			_cost = cost;
			_shop = shop;
			_comments = comments;
		}

		protected override void CustomValidation()
		{
			AssertCategoryIsInCurrentBudget(_categoryId);
			AssertPurchaseIsInCurrentBudget(_id);
		}

		protected override async Task ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			await storageCollection.Purchases.UpdateAsync(_id, _categoryId, _date, _name, _cost, _shop, _comments);
		}
	}
}
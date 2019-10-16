﻿using System;
using System.Threading.Tasks;

using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Budgets
{
	[Authenticated]
	public class CreateJob : Job<Guid>
	{
		private readonly string _name;

		public CreateJob(string name)
		{
			_name = name;
		}

		protected override async Task<Guid> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			Guid budgetId = await storageCollection.Budgets.AddAsync(_name);
			await storageCollection.Categories.InitializeCategoriesAsync(budgetId);

			return budgetId;
		}
	}
}
﻿using System.Threading.Tasks;

using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Categories
{
	[Authenticated]
	[HasRights(ShareAccess.Categories)]
	public class UpdateJob : Job
	{
		private readonly int _categoryId;
		private readonly string _color;
		private readonly int? _parentCategoryId;
		private readonly LocalizedName[] _translates;

		public UpdateJob(int categoryId, int? parentCategoryId, LocalizedName[] translates, string color)
		{
			_categoryId = categoryId;
			_parentCategoryId = parentCategoryId;
			_translates = translates;
			_color = color;
		}

		protected override void CustomValidation()
		{
			AssertCategoryIsInCurrentBudget(_categoryId);
			if (_parentCategoryId.HasValue)
			{
				AssertCategoryIsInCurrentBudget(_parentCategoryId.Value);
			}
		}

		protected override async Task ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			await storageCollection.Categories.UpdateAsync(_categoryId, _parentCategoryId, _translates, _color);
		}
	}
}
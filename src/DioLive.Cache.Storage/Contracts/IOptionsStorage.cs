﻿using System.Threading.Tasks;

using DioLive.Cache.Common.Entities;

namespace DioLive.Cache.Storage.Contracts
{
	public interface IOptionsStorage
	{
		Task<Options?> GetAsync();
		Task UpdateAsync(int? purchaseGrouping, bool? showPlanList);
		Task CreateAsync(Options options);
	}
}
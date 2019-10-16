using System;
using System.Collections.Generic;

using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;

namespace DioLive.Cache.CoreLogic.Contacts
{
	public interface IPurchasesLogic
	{
		Result<IReadOnlyCollection<(Purchase purchase, Category category)>> FindWithCategories(string? filter);
		Result Create(string name, int categoryId, DateTime date, int cost, string? shop, string? comments, int? planId);
		Result<Purchase> Get(Guid id);
		Result Update(Guid id, string name, int categoryId, DateTime date, int cost, string? shop, string? comments);
		Result Delete(Guid id);
		Result<IReadOnlyCollection<string>> GetShops();
		Result<IReadOnlyCollection<string>> GetNames(string filter);
	}
}
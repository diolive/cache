using System.Collections.Generic;
using System.Linq;

using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;

namespace DioLive.Cache.CoreLogic.Contacts
{
	public interface ICategoriesLogic
	{
		Result<IReadOnlyCollection<Category>> GetAll();
		Result<(Hierarchy<Category, int> hierarchy, ILookup<int, LocalizedName> localizations)> GetHierarchyAndLocalizations();
		Result<int> Create(string newCategoryName);
		Result Update(int categoryId, int? parentCategoryId, LocalizedName[] translates, string color);
		Result<Category> Get(int categoryId);
		Result Delete(int categoryId);
		Result<int> GetPrevious(string purchaseName);
		Result<int?> GetMostPopularId();
	}
}
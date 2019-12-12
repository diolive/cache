using System.Collections.Generic;

using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;

namespace DioLive.Cache.CoreLogic.Contacts
{
	public interface ICategoriesLogic
	{
		Result<IReadOnlyCollection<Category>> GetAll();
		Result<int> Create(string newCategoryName);
		Result Update(int categoryId, int? parentCategoryId, string name, string color);
		Result<Category> Get(int categoryId);
		Result Delete(int categoryId);
		Result<int> GetPrevious(string purchaseName);
		Result<int?> GetMostPopularId();
	}
}
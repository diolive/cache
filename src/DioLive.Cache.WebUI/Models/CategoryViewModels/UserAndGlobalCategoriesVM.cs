using System.Collections.Generic;

using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.WebUI.Models.CategoryViewModels
{
	public class UserAndGlobalCategoriesVM
	{
		public ICollection<Category> UserCategories { get; set; }

		public ICollection<Category> GlobalCategories { get; set; }
	}
}
using System.Collections.Generic;

namespace DioLive.Cache.WebUI.Models.CategoryViewModels
{
    public class UserAndGlobalCategoriesVM
    {
        public IEnumerable<Category> UserCategories { get; set; }

        public IEnumerable<Category> GlobalCategories { get; set; }
    }
}
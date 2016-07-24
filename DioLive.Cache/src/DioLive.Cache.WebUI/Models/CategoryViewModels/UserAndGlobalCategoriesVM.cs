using System.Collections.Generic;

namespace DioLive.Cache.WebUI.Models.CategoryViewModels
{
    public class UserAndGlobalCategoriesVM
    {
        public ICollection<Category> UserCategories { get; set; }

        public ICollection<Category> GlobalCategories { get; set; }
    }
}
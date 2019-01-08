using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.WebUI.Models.CategoryViewModels
{
	public class CategoryVM
	{
		public CategoryVM()
		{
		}

		public CategoryVM(Category category)
		{
			Id = category.Id;
			DisplayName = category.Name;
			Color = category.Color.ToString("X6");
		}

		public int Id { get; set; }

		public string DisplayName { get; set; }

		public string Color { get; set; }
	}
}
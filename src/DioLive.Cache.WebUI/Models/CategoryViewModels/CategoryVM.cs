using DioLive.Cache.Common.Entities;

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
			Name = category.Name;
			Color = category.Color.ToString("X6");
		}

		public int Id { get; set; }

		public string Name { get; set; } = default!;

		public string Color { get; set; } = default!;
	}
}
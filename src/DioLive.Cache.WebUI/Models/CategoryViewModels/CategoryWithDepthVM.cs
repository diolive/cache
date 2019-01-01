using DioLive.Cache.Models;

namespace DioLive.Cache.WebUI.Models.CategoryViewModels
{
	public class CategoryWithDepthVM
	{
		public CategoryWithDepthVM(Category category)
		{
			Category = category;
			Depth = 0;
			Category current = category;
			while (current.ParentId.HasValue)
			{
				current = current.Parent;
				Depth++;
			}
		}

		public Category Category { get; }

		public int Depth { get; }
	}
}
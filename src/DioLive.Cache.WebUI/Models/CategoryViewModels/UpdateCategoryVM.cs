namespace DioLive.Cache.WebUI.Models.CategoryViewModels
{
	public class UpdateCategoryVM
	{
		public int Id { get; set; }

		public string[] Translates { get; set; }

		public string Color { get; set; }

		public int? ParentId { get; set; }
	}
}
using System.ComponentModel.DataAnnotations;

namespace DioLive.Cache.WebUI.Models.CategoryViewModels
{
	public class UpdateCategoryVM
	{
		public int Id { get; set; }

		[Required]
		public string Name { get; set; } = default!;

		[Required]
		public string Color { get; set; } = default!;

		public int? ParentId { get; set; }
	}
}
using System.ComponentModel.DataAnnotations;

namespace DioLive.Cache.WebUI.Models.CategoryViewModels
{
	public class CreateCategoryVM
	{
		[Required]
		[StringLength(300)]
		public string Name { get; set; }
	}
}
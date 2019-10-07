using System.ComponentModel.DataAnnotations;

namespace DioLive.Cache.WebUI.Models.AccountViewModels
{
	public class LoginVM
	{
		[Required(ErrorMessage = "The Email field is required")]
		[EmailAddress(ErrorMessage = "The Email field is not a valid e-mail address")]
		public string Email { get; set; } = default!;

		[Required(ErrorMessage = "The Password field is required")]
		[DataType(DataType.Password)]
		public string Password { get; set; } = default!;

		public bool RememberMe { get; set; }
	}
}
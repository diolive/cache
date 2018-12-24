using System.ComponentModel.DataAnnotations;

namespace DioLive.Cache.WebUI.Models.AccountViewModels
{
	public class LoginVM
	{
		[Required(ErrorMessage = "The Email field is required")]
		[EmailAddress(ErrorMessage = "The Email field is not a valid e-mail address")]
		public string Email { get; set; }

		[Required(ErrorMessage = "The Password field is required")]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		public bool RememberMe { get; set; }
	}
}
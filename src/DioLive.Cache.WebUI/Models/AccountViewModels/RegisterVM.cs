using System.ComponentModel.DataAnnotations;

namespace DioLive.Cache.WebUI.Models.AccountViewModels
{
	public class RegisterVM
	{
		[Required(ErrorMessage = "The Email field is required")]
		[EmailAddress(ErrorMessage = "The Email field is not a valid e-mail address")]
		public string Email { get; set; } = "";

		[Required(ErrorMessage = "The Password field is required")]
		[StringLength(100, ErrorMessage = "The Password must be at least {1} and at max {2} characters long", MinimumLength = 6)]
		[DataType(DataType.Password)]
		public string Password { get; set; } = "";

		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
		public string ConfirmPassword { get; set; } = "";
	}
}
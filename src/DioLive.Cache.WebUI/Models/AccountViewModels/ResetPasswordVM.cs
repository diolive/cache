using System.ComponentModel.DataAnnotations;

namespace DioLive.Cache.WebUI.Models.AccountViewModels
{
	public class ResetPasswordVM
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; } = default!;

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		public string Password { get; set; } = default!;

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; } = default!;

		public string Code { get; set; } = default!;
	}
}
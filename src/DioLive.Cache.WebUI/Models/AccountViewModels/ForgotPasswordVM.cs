using System.ComponentModel.DataAnnotations;

namespace DioLive.Cache.WebUI.Models.AccountViewModels
{
	public class ForgotPasswordVM
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; } = default!;
	}
}
using System.ComponentModel.DataAnnotations;

namespace DioLive.Cache.WebUI.Models.AccountViewModels
{
	public class ExternalLoginConfirmationVM
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }
	}
}
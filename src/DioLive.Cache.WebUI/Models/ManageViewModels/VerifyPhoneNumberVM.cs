using System.ComponentModel.DataAnnotations;

namespace DioLive.Cache.WebUI.Models.ManageViewModels
{
	public class VerifyPhoneNumberVM
	{
		[Required]
		public string Code { get; set; }

		[Required]
		[Phone]
		[Display(Name = "Phone number")]
		public string PhoneNumber { get; set; }
	}
}
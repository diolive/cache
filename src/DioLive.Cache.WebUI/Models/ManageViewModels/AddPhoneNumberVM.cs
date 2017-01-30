using System.ComponentModel.DataAnnotations;

namespace DioLive.Cache.WebUI.Models.ManageViewModels
{
    public class AddPhoneNumberVM
    {
        [Required]
        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
    }
}

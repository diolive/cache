using System.ComponentModel.DataAnnotations;

namespace DioLive.BlackMint.WebApp.ViewModels
{
    public class NewBookVM
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }
}
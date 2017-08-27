using System.ComponentModel.DataAnnotations;

namespace DioLive.BlackMint.WebApp.ViewModels
{
    public class NewPurchaseItemVM
    {
        [Required(AllowEmptyStrings = true)]
        public string Name { get; set; }

        public decimal Price { get; set; }

        public int Count { get; set; }
    }
}
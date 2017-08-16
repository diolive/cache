using System.ComponentModel.DataAnnotations;

namespace DioLive.BlackMint.WebApp.ViewModels
{
    public class UpdateBookVM
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }
}
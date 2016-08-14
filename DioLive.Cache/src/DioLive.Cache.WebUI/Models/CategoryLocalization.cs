using System.ComponentModel.DataAnnotations;

namespace DioLive.Cache.WebUI.Models
{
    public class CategoryLocalization
    {
        public int CategoryId { get; set; }

        [Required, StringLength(10)]
        public string Culture { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; }

        public virtual Category Category { get; set; }
    }
}
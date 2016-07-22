using System.ComponentModel.DataAnnotations;

namespace DioLive.Cache.WebUI.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required, StringLength(300)]
        public string Name { get; set; }

        public string OwnerId { get; set; }

        public virtual ApplicationUser Owner { get; set; }
    }
}
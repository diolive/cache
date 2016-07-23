using System;
using System.ComponentModel.DataAnnotations;

namespace DioLive.Cache.WebUI.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required, StringLength(300)]
        public string Name { get; set; }

        public string OwnerId { get; set; }

        public Guid? BudgetId { get; set; }

        public virtual ApplicationUser Owner { get; set; }

        public virtual Budget Budget { get; set; }
    }
}
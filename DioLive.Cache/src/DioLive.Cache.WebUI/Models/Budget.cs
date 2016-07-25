using System;
using System.Collections.Generic;

namespace DioLive.Cache.WebUI.Models
{
    public class Budget
    {
        public Budget()
        {
            Categories = new HashSet<Category>();
            Purchases = new HashSet<Purchase>();
            Shares = new HashSet<Share>();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string AuthorId { get; set; }

        public virtual ApplicationUser Author { get; set; }

        public virtual ICollection<Category> Categories { get; set; }

        public virtual ICollection<Purchase> Purchases { get; set; }

        public virtual ICollection<Share> Shares { get; set; }
    }
}
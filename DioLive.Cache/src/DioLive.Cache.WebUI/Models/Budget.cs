using System;

namespace DioLive.Cache.WebUI.Models
{
    public class Budget
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string AuthorId { get; set; }

        public virtual ApplicationUser Author { get; set; }
    }
}
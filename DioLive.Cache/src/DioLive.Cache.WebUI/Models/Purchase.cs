using System;
using System.ComponentModel.DataAnnotations;

namespace DioLive.Cache.WebUI.Models
{
    public class Purchase
    {
        public Guid Id { get; set; }

        [Required, StringLength(300)]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        public int CategoryId { get; set; }

        public DateTime Date { get; set; }

        public virtual Category Category { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace DioLive.Cache.WebUI.Models.PurchaseViewModels
{
    public class CreatePurchaseVM
    {
        [Required, StringLength(300)]
        public string Name { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [DisplayFormat(DataFormatString = Constants.DateDisplayFormat, ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Required]
        public int? Cost { get; set; }

        public string Shop { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        public int? PlanId { get; set; }
    }
}
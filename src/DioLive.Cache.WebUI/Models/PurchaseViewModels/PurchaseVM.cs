using System;
using System.ComponentModel.DataAnnotations;

using DioLive.Cache.WebUI.Models.CategoryViewModels;

namespace DioLive.Cache.WebUI.Models.PurchaseViewModels
{
    public class PurchaseVM
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public CategoryVM Category { get; set; }

        [DisplayFormat(DataFormatString = Constants.DateDisplayFormat, ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [DisplayFormat(DataFormatString = Constants.CostDisplayFormat)]
        public int Cost { get; set; }

        [DisplayFormat(NullDisplayText = "N/A")]
        public string Shop { get; set; }

        public string Comments { get; set; }
    }
}
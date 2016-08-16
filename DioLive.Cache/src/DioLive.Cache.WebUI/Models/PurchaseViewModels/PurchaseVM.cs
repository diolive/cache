using System;
using System.ComponentModel.DataAnnotations;

using DioLive.Cache.WebUI.Models.CategoryViewModels;

namespace DioLive.Cache.WebUI.Models.PurchaseViewModels
{
    public class PurchaseVM
    {
        public const string CostFormat = "{0:N0} ₽";

        public Guid Id { get; set; }

        public string Name { get; set; }

        public CategoryVM Category { get; set; }

        [DisplayFormat(DataFormatString = "{0:" + Binders.DateTimeModelBinder.DateFormat + "}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [DisplayFormat(DataFormatString = CostFormat)]
        public int Cost { get; set; }

        [DisplayFormat(NullDisplayText = "N/A")]
        public string Shop { get; set; }

        public string Comments { get; set; }
    }
}
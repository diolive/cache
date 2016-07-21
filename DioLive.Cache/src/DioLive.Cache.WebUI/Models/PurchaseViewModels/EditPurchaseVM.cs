using System;
using System.ComponentModel.DataAnnotations;

namespace DioLive.Cache.WebUI.Models.PurchaseViewModels
{
    public class EditPurchaseVM
    {
        public Guid Id { get; set; }

        [Required, StringLength(300)]
        public string Name { get; set; }

        public int CategoryId { get; set; }

        [DisplayFormat(DataFormatString = "{0:" + Binders.DateTimeModelBinder.DateFormat + "}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        public int Amount { get; set; }

        public string Shop { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }
    }
}
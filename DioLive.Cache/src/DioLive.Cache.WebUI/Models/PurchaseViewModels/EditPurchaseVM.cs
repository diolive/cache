using System;
using System.ComponentModel.DataAnnotations;

namespace DioLive.Cache.WebUI.Models.PurchaseViewModels
{
    public class EditPurchaseVM
    {
        public Guid Id { get; set; }

        [Required, StringLength(300)]
        public string Name { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [DisplayFormat(DataFormatString = "{0:" + Binders.DateTimeModelBinder.DateFormat + "}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        public int Cost { get; set; }

        public string Shop { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        public UserVM Author { get; set; }

        public UserVM LastEditor { get; set; }
    }
}
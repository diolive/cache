using System;
using System.ComponentModel.DataAnnotations;

using DioLive.Cache.Storage;
using DioLive.Cache.Storage.Legacy.Models;

using Purchase = DioLive.Cache.Storage.Entities.Purchase;

namespace DioLive.Cache.WebUI.Models.PurchaseViewModels
{
	public class EditPurchaseVM
	{
		public EditPurchaseVM()
		{
		}

		public EditPurchaseVM(Purchase purchase, ApplicationUser author, ApplicationUser lastEditor)
		{
			Id = purchase.Id;
			Name = purchase.Name;
			CategoryId = purchase.CategoryId;
			Date = purchase.Date;
			Cost = purchase.Cost;
			Shop = purchase.Shop;
			Comments = purchase.Comments;
			Author = new UserVM(author);
			LastEditor = new UserVM(lastEditor);
		}

		public Guid Id { get; set; }

		[Required]
		[StringLength(300)]
		public string Name { get; set; }

		[Display(Name = "Category")]
		public int CategoryId { get; set; }

		[DisplayFormat(DataFormatString = Constants.DateDisplayFormat, ApplyFormatInEditMode = true)]
		[DataType(DataType.Text)]
		public DateTime Date { get; set; }

		public int Cost { get; set; }

		public string Shop { get; set; }

		[DataType(DataType.MultilineText)]
		public string Comments { get; set; }

		public UserVM Author { get; set; }

		public UserVM LastEditor { get; set; }
	}
}
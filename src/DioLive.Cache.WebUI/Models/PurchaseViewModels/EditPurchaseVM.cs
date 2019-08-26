using System;
using System.ComponentModel.DataAnnotations;

using DioLive.Cache.Storage;
using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.WebUI.Models.PurchaseViewModels
{
	public class EditPurchaseVM
	{
		public EditPurchaseVM()
		{
		}

		public EditPurchaseVM(Purchase purchase, UserVM author, UserVM lastEditor)
		{
			Id = purchase.Id;
			Name = purchase.Name;
			CategoryId = purchase.CategoryId;
			Date = purchase.Date;
			Cost = purchase.Cost;
			Shop = purchase.Shop;
			Comments = purchase.Comments;
			Author = author;
			LastEditor = lastEditor;
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
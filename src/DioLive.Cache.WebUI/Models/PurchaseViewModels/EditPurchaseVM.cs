using System;
using System.ComponentModel.DataAnnotations;

using DioLive.Cache.Common.Entities;
using DioLive.Cache.Storage;

namespace DioLive.Cache.WebUI.Models.PurchaseViewModels
{
	public class EditPurchaseVM
	{
		public EditPurchaseVM()
		{
		}

		public EditPurchaseVM(Purchase purchase, string authorName, string? lastEditorName)
		{
			Id = purchase.Id;
			Name = purchase.Name;
			CategoryId = purchase.CategoryId;
			Date = purchase.Date;
			Cost = purchase.Cost;
			Shop = purchase.Shop;
			Comments = purchase.Comments;
			AuthorId = purchase.AuthorId;
			AuthorName = authorName;
			LastEditorId = purchase.LastEditorId;
			LastEditorName = lastEditorName;
		}

		public Guid Id { get; set; }

		[Required]
		[StringLength(300)]
		public string Name { get; set; } = default!;

		[Display(Name = "Category")]
		public int CategoryId { get; set; }

		[DisplayFormat(DataFormatString = Constants.DateDisplayFormat, ApplyFormatInEditMode = true)]
		[DataType(DataType.Text)]
		public DateTime Date { get; set; }

		public int Cost { get; set; }

		public string? Shop { get; set; }

		[DataType(DataType.MultilineText)]
		public string? Comments { get; set; }

		public string AuthorId { get; set; } = default!;

		public string AuthorName { get; set; } = default!;

		public string? LastEditorId { get; set; }

		public string? LastEditorName { get; set; }
	}
}
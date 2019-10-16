using System;
using System.ComponentModel.DataAnnotations;

using DioLive.Cache.Common.Entities;
using DioLive.Cache.Storage;

namespace DioLive.Cache.WebUI.Models.PurchaseViewModels
{
	public class DeletePurchaseVM
	{
		public DeletePurchaseVM()
		{
		}

		public DeletePurchaseVM(Purchase purchase)
		{
			Id = purchase.Id;
			Name = purchase.Name;
			Date = purchase.Date;
			Cost = purchase.Cost;
			Shop = purchase.Shop;
			Comments = purchase.Comments;
		}

		public Guid Id { get; set; }

		public string Name { get; set; } = default!;

		[DisplayFormat(DataFormatString = Constants.DateDisplayFormat, ApplyFormatInEditMode = true)]
		public DateTime Date { get; set; }

		[DisplayFormat(DataFormatString = Constants.CostDisplayFormat)]
		public int Cost { get; set; }

		[DisplayFormat(NullDisplayText = "N/A")]
		public string? Shop { get; set; }

		public string? Comments { get; set; }
	}
}
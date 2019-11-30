using System;
using System.ComponentModel.DataAnnotations;

using DioLive.Cache.Common.Entities;
using DioLive.Cache.Storage;

namespace DioLive.Cache.WebUI.Models.PurchaseViewModels
{
	public class DeletePurchaseVM
	{
		public DeletePurchaseVM(Purchase purchase, string currency)
		{
			Id = purchase.Id;
			Name = purchase.Name;
			Date = purchase.Date;
			Cost = string.Format(Constants.CostDisplayFormat, purchase.Cost, currency);
			Shop = purchase.Shop;
			Comments = purchase.Comments;
		}

		public Guid Id { get; set; }

		public string Name { get; set; } = default!;

		[DisplayFormat(DataFormatString = Constants.DateDisplayFormat, ApplyFormatInEditMode = true)]
		public DateTime Date { get; set; }

		public string Cost { get; set; }

		[DisplayFormat(NullDisplayText = "N/A")]
		public string? Shop { get; set; }

		public string? Comments { get; set; }
	}
}
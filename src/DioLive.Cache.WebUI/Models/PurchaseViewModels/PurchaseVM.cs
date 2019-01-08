using System;
using System.ComponentModel.DataAnnotations;

using DioLive.Cache.Storage;
using DioLive.Cache.Storage.Entities;
using DioLive.Cache.WebUI.Models.CategoryViewModels;

namespace DioLive.Cache.WebUI.Models.PurchaseViewModels
{
	public class PurchaseVM
	{
		public PurchaseVM()
		{
		}

		public PurchaseVM(Purchase purchase, Category category)
		{
			Id = purchase.Id;
			Name = purchase.Name;
			Category = new CategoryVM(category);
			Date = purchase.Date;
			Cost = purchase.Cost;
			Shop = purchase.Shop;
			Comments = purchase.Comments;
		}

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
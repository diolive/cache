using System;
using System.Collections.Generic;

using DioLive.Cache.WebUI.Models.PlanViewModels;

namespace DioLive.Cache.WebUI.Models.PurchaseViewModels
{
	public class PurchasesPageVM
	{
		public IReadOnlyCollection<PurchaseVM> Purchases { get; set; } = default!;
		public Guid BudgetId { get; set; } = default!;
		public string BudgetName { get; set; } = default!;
		public string BudgetAuthor { get; set; } = default!;
		public int PurchaseGrouping { get; set; } = default!;
		public IReadOnlyCollection<PlanVM>? Plans { get; set; }
	}
}
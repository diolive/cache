using System;
using System.Collections.Generic;

namespace DioLive.Cache.WebUI.Models.BudgetSharingViewModels
{
	public class BudgetSharingsVM
	{
		public Guid BudgetId { get; set; }
		public IReadOnlyCollection<ShareVM> Shares { get; set; }
	}
}
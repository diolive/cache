using System;
using System.Collections.Generic;

using DioLive.Cache.Storage.Entities;

using ApplicationUser = DioLive.Cache.Storage.Legacy.Models.ApplicationUser;

namespace DioLive.Cache.WebUI.Models.BudgetSharingViewModels
{
	public class BudgetSharingsVM
	{
		public Guid BudgetId { get; set; }
		public IReadOnlyCollection<ShareVM> Shares { get; set; }

		public ApplicationUser Owner { get; set; }
	}
}
using System.Collections.Generic;

using Microsoft.AspNetCore.Identity;

namespace DioLive.Cache.WebUI.Models.ManageViewModels
{
	public class IndexVM
	{
		public bool HasPassword { get; set; }

		public IList<UserLoginInfo> Logins { get; set; } = default!;

		public string? PhoneNumber { get; set; }

		public bool TwoFactor { get; set; }

		public bool BrowserRemembered { get; set; }

		public int PurchaseGrouping { get; set; }

		public bool ShowPlanList { get; set; }
	}
}
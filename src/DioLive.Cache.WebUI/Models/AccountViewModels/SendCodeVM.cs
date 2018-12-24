using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace DioLive.Cache.WebUI.Models.AccountViewModels
{
	public class SendCodeVM
	{
		public string SelectedProvider { get; set; }

		public ICollection<SelectListItem> Providers { get; set; }

		public string ReturnUrl { get; set; }

		public bool RememberMe { get; set; }
	}
}
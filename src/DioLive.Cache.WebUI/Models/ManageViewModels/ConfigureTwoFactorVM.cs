using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace DioLive.Cache.WebUI.Models.ManageViewModels
{
	public class ConfigureTwoFactorVM
	{
		public string SelectedProvider { get; set; } = default!;

		public ICollection<SelectListItem> Providers { get; set; } = default!;
	}
}
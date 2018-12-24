using System.Collections.Generic;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace DioLive.Cache.WebUI.Models.ManageViewModels
{
	public class ManageLoginsVM
	{
		public IList<UserLoginInfo> CurrentLogins { get; set; }

		public IList<AuthenticationScheme> OtherLogins { get; set; }
	}
}
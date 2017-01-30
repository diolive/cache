using System.Collections.Generic;

using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;

namespace DioLive.Cache.WebUI.Models.ManageViewModels
{
    public class ManageLoginsVM
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }

        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }
}

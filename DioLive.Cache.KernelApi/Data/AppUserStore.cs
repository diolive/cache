using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DioLive.Cache.KernelApi.Data
{
	public class AppUserStore : UserStore<IdentityUser>
	{
		public AppUserStore(DbContext context)
			: base(context)
		{
		}
	}
}
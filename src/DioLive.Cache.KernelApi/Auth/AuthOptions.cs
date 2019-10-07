using System.Text;

using Microsoft.IdentityModel.Tokens;

namespace DioLive.Cache.KernelApi.Auth
{
	public class AuthOptions
	{
		public string Issuer { get; set; } = default!;

		public string Audience { get; set; } = default!;

		public string Key { get; set; } = default!;

		public int LifeTime { get; set; } = default!;

		public SymmetricSecurityKey GetSymmetricSecurityKey()
		{
			return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
		}
	}
}
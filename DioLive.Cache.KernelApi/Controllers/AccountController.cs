using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

using DioLive.Cache.KernelApi.Auth;
using DioLive.Cache.KernelApi.Data;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DioLive.Cache.KernelApi.Controllers
{
	public class AccountController : Controller
	{
		private readonly AuthOptions _authOptions;

		public AccountController(IOptions<AuthOptions> authOptions)
		{
			_authOptions = authOptions.Value;
		}

		[HttpPost("/token")]
		public async Task<IActionResult> Token(string username, string password)
		{
			ClaimsIdentity identity = await AppUserManager.Instance.GetIdentityAsync(username, password);
			if (identity == null)
			{
				return BadRequest("Invalid username or password.");
			}

			DateTime now = DateTime.UtcNow;

			var jwt = new JwtSecurityToken(
				_authOptions.Issuer,
				_authOptions.Audience,
				notBefore: now,
				claims: identity.Claims,
				expires: now.Add(TimeSpan.FromMinutes(_authOptions.LifeTime)),
				signingCredentials: new SigningCredentials(_authOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
			string encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

			var response = new
			{
				access_token = encodedJwt,
				username = identity.Name
			};

			return Json(response);
		}
	}
}
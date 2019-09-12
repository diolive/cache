using DioLive.Cache.KernelStorage.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DioLive.Cache.KernelApi.Controllers
{
	[Route("api/[controller]")]
	public class ValuesController : Controller
	{
		private readonly AppUserManager _userManager;

		public ValuesController(AppUserManager userManager)
		{
			_userManager = userManager;
		}

		[Authorize]
		[Route("getlogin")]
		public IActionResult GetLogin()
		{
			return Ok($"Ваш логин: {User.Identity.Name}, id:{_userManager.GetUserId(User)}");
		}

		[Authorize(Roles = "admin")]
		[Route("getrole")]
		public IActionResult GetRole()
		{
			return Ok("Ваша роль: администратор");
		}
	}
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DioLive.BlackMint.WebApp.Controllers
{
    public class HomeController : ControllerBase
    {
        public HomeController(IOptions<DataSettings> dataOptions)
            : base(dataOptions)
        {
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated && !HasUserId) return Logout();

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
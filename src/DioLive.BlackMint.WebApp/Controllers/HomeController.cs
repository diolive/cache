using Microsoft.AspNetCore.Mvc;

namespace DioLive.BlackMint.WebApp.Controllers
{
    public class HomeController : ControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
using Microsoft.AspNetCore.Mvc;

namespace DioLive.BlackMint.WebApp.Controllers.api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    {
    }
}
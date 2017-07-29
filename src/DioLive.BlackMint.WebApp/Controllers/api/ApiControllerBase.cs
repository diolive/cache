using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DioLive.BlackMint.WebApp.Controllers.api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    {
        public ApiControllerBase(IOptions<DataSettings> dataOptions)
            : base(dataOptions)
        {
        }

        protected IActionResult JsonOrNotFound(object @object)
        {
            return @object is null
                ? (IActionResult)NotFound()
                : Json(@object);
        }
    }
}
using System.Threading.Tasks;

using DioLive.BlackMint.Entities;
using DioLive.BlackMint.Logic;

using Microsoft.AspNetCore.Mvc;

namespace DioLive.BlackMint.WebApp.Controllers.api
{
    public class UserController : ApiControllerBase
    {
        private readonly IIdentityLogic _identityLogic;

        public UserController(IIdentityLogic identityLogic)
        {
            _identityLogic = identityLogic;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            if (id <= 0)
                return BadRequest();

            User user = await _identityLogic.GetUser(id);
            return JsonOrNotFound(user);
        }

        [HttpGet("{id:int}/displayName")]
        public async Task<IActionResult> GetDisplayName(int id)
        {
            if (id <= 0)
                return BadRequest();

            string displayName = await _identityLogic.GetUserDisplayName(id);
            return JsonOrNotFound(displayName);
        }

        [HttpGet("{nameIdentity}")]
        public async Task<IActionResult> GetByIdentity(string nameIdentity)
        {
            if (string.IsNullOrWhiteSpace(nameIdentity))
                return BadRequest();

            User user = await _identityLogic.GetUser(nameIdentity);
            return JsonOrNotFound(user);
        }
    }
}
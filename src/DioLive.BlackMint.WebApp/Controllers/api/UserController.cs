using System.Threading.Tasks;

using Dapper.Contrib.Extensions;

using DioLive.BlackMint.WebApp.Data;
using DioLive.BlackMint.WebApp.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DioLive.BlackMint.WebApp.Controllers.api
{
    public class UserController : ApiControllerBase
    {
        public UserController(IOptions<DataSettings> dataOptions)
            : base(dataOptions)
        {
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            User user = await Db.GetAsync<User>(id);
            return JsonOrNotFound(user);
        }

        [HttpGet("{id:int}/displayName")]
        public async Task<IActionResult> GetDisplayName(int id)
        {
            string displayName = await Database.GetUserDisplayNameById(Db, id);
            return JsonOrNotFound(displayName);
        }

        [HttpGet("{nameIdentity}")]
        public async Task<IActionResult> GetByIdentity(string nameIdentity)
        {
            int? userId = await Database.GetUserIdByNameIdentity(Db, nameIdentity);

            if (userId.HasValue)
            {
                return await Get(userId.Value);
            }

            return NotFound();
        }
    }
}
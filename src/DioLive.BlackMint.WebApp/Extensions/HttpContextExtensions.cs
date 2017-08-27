using System.Linq;
using System.Security.Claims;

using DioLive.BlackMint.Entities;
using DioLive.BlackMint.Logic;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DioLive.BlackMint.WebApp.Extensions
{
    public static class HttpContextExtensions
    {
        private static IIdentityLogic _identityLogic;

        public static User GetCurrentUser(this HttpContext httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
                return null;

            var user = httpContext.Session.GetObject<User>("user");
            if (user != null)
                return user;

            if (_identityLogic is null)
                _identityLogic = httpContext.RequestServices.GetService<IIdentityLogic>();

            Claim nameIdentifierClaim =
                httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            user = _identityLogic.GetUser(nameIdentifierClaim?.Value).GetAwaiter().GetResult();
            if (user != null)
            {
                httpContext.Session.SetObject("user", user);
                return user;
            }

            return null;
        }
    }
}
using System;
using System.Linq;
using System.Security.Claims;

using DioLive.BlackMint.Entities;
using DioLive.BlackMint.Logic;
using DioLive.BlackMint.WebApp.Extensions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace DioLive.BlackMint.WebApp.Controllers
{
    public abstract class ControllerBase : Controller
    {
        private const string NameIdentifierClaimType =
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

        private readonly Lazy<IIdentityLogic> _identityLogic;
        private int? _userId;

        protected ControllerBase()
        {
            _identityLogic = new Lazy<IIdentityLogic>(() => HttpContext.RequestServices.GetService<IIdentityLogic>());
        }

        protected int UserId
        {
            get
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return 0;
                }

                if (_userId.HasValue)
                {
                    return _userId.Value;
                }

                var user = HttpContext.Session.GetObject<User>("user");
                if (user != null)
                {
                    _userId = user.Id;
                    return user.Id;
                }

                Claim nameIdentifierClaim = User.Claims.FirstOrDefault(c => c.Type == NameIdentifierClaimType);

                user = _identityLogic.Value.GetUser(nameIdentifierClaim?.Value).GetAwaiter().GetResult();
                if (user != null)
                {
                    HttpContext.Session.SetObject("user", user);
                    _userId = user.Id;
                    return user.Id;
                }

                throw new InvalidOperationException("Cannot retrieve user id");
            }
        }

        protected IActionResult Logout()
        {
            return RedirectToAction("Logout", "Account");
        }

        protected IActionResult JsonOrNotFound(object value)
        {
            return value is null
                ? (IActionResult)NotFound()
                : Json(value);
        }

        protected IActionResult ResponseToResult<T>(Response<T> response)
        {
            switch (response.Status)
            {
                case ResponseStatus.Success:
                    return Json(response.Result);

                case ResponseStatus.NotFound:
                    return NotFound();

                case ResponseStatus.Forbidden:
                    return Forbid();

                default:
                    return BadRequest();
            }
        }

        protected IActionResult ResponseStatusToResult(ResponseStatus responseStatus)
        {
            switch (responseStatus)
            {
                case ResponseStatus.Success:
                    return Ok();

                case ResponseStatus.NotFound:
                    return NotFound();

                case ResponseStatus.Forbidden:
                    return Forbid();

                default:
                    return BadRequest();
            }
        }
    }
}
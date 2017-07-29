using System;
using System.Data.SqlClient;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DioLive.BlackMint.WebApp.Controllers
{
    public abstract class ControllerBase : Controller
    {
        private readonly Lazy<int?> _userId;

        protected ControllerBase(IOptions<DataSettings> dataOptions)
        {
            Db = new SqlConnection(dataOptions.Value.ConnectionString);
            _userId = new Lazy<int?>(() => HttpContext.Session.GetInt32("userId"));
        }

        protected SqlConnection Db { get; }

        protected bool HasUserId => _userId.Value.HasValue;

        // ReSharper disable once PossibleInvalidOperationException
        protected int UserId => _userId.Value.Value;

        protected IActionResult Logout()
        {
            return RedirectToAction("Logout", "Account");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Db?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
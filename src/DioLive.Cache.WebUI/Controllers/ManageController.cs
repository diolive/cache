using DioLive.Cache.Auth;
using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Contacts;
using DioLive.Cache.WebUI.Models;
using DioLive.Cache.WebUI.Models.ManageViewModels;

using DioRed.Common;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DioLive.Cache.WebUI.Controllers;

[Authorize]
public class ManageController(
    ICurrentContext currentContext,
    SignInManager<IdentityUser> signInManager,
    AppUserManager userManager,
    IOptionsLogic optionsLogic
) : BaseController(currentContext)
{
    private static readonly Dictionary<ManageMessageId, string> _statusMessages;

    static ManageController()
    {
        _statusMessages = new Dictionary<ManageMessageId, string>
        {
            [ManageMessageId.ChangePasswordSuccess] = "Your password has been changed.",
            [ManageMessageId.Error] = "An error has occurred.",
        };
    }

    [HttpGet]
    public async Task<IActionResult> Index(ManageMessageId? message = null)
    {
        ViewData["StatusMessage"] = message.HasValue &&
                                    _statusMessages.TryGetValue(message.Value, out string? msgText)
            ? msgText
            : string.Empty;

        IdentityUser? user = await userManager.GetUserAsync(HttpContext.User);

        if (user is null)
        {
            return View("Error");
        }

        Result<Options> result = optionsLogic.Get();

        return ProcessResult(result, options =>
        {
            var model = new ProfileVM
            {
                PurchaseGrouping = options.PurchaseGrouping,
                ShowPlanList = options.ShowPlanList
            };

            return View(model);
        });
    }

    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        IdentityUser? user = await GetCurrentUserAsync();

        if (user == null)
        {
            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        IdentityResult result = await userManager.ChangePasswordAsync(
            user,
            model.OldPassword,
            model.NewPassword
        );

        if (result.Succeeded)
        {
            await signInManager.SignInAsync(
                user,
                false
            );

            return RedirectToAction(
                nameof(Index),
                new
                {
                    Message = ManageMessageId.ChangePasswordSuccess
                }
            );
        }

        AddErrors(result);

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Photo(string id)
    {
        IdentityUser? user;
        if (id == null)
        {
            user = await GetCurrentUserAsync();
        }
        else
        {
            user = await userManager.FindByIdAsync(id);
        }

        if (user?.Email is null)
        {
            return Ok();
        }

        return Redirect(GravatarHelper.GetAvatarUrl(user.Email, 16));
    }

    #region Helpers

    private void AddErrors(IdentityResult result)
    {
        foreach (IdentityError error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    public enum ManageMessageId
    {
        ChangePasswordSuccess,
        Error
    }

    private async Task<IdentityUser> GetCurrentUserAsync()
    {
        return await userManager.GetUserAsync(HttpContext.User)
            ?? throw new ApplicationException("Cannot load current user");
    }

    #endregion Helpers
}
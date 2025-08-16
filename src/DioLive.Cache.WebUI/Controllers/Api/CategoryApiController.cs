using DioLive.Cache.Common;
using DioLive.Cache.CoreLogic.Contacts;
using DioLive.Cache.WebUI.Models.CategoryViewModels;

using DioRed.Common;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DioLive.Cache.WebUI.Controllers.Api;

[ApiController]
[Authorize]
[Route("api/category")]
public class CategoryApiController(
    ICurrentContext currentContext,
    ICategoriesLogic categoriesLogic
) : BaseController(currentContext)
{
    [HttpPost]
    [Route("update")]
    public IActionResult Update([FromBody] UpdateCategoryVM model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        Result result = categoriesLogic.Update(
            model.Id,
            model.ParentId,
            model.Name,
            model.Color
        );

        return ProcessResult(
            result,
            Ok
        );
    }
}
using DioRed.Cache.Domain;
using DioRed.Cache.Core.Contracts;
using DioRed.Cache.WebApp.Models.CategoryViewModels;

using DioRed.Common;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DioRed.Cache.WebApp.Controllers.Api;

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
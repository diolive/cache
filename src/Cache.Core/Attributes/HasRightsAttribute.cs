using DioRed.Cache.Domain;
using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Core.Exceptions;
using DioRed.Cache.Domain.Repositories;

using DioRed.Common;

namespace DioRed.Cache.Core.Attributes;

public class HasRightsAttribute(ShareAccess shareAccess) : ValidationAttribute
{
    public IPermissionsValidator? PermissionsValidator { get; set; }

    public override void Validate(ICurrentContext currentContext)
    {
        if (PermissionsValidator is null)
        {
            throw new InvalidOperationException("Property PermissionValidator should be initialized before using this attribute for context validation");
        }

        Result result = PermissionsValidator.CheckUserRightsForBudget(
            CurrentBudget(currentContext),
            currentContext.GetUserId(),
            shareAccess
        );

        ValidationException.RaiseIfNeeded(result);
    }
}
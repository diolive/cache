using DioRed.Cache.Domain;
using DioRed.Cache.Core.Exceptions;

namespace DioRed.Cache.Core.Attributes;

public class AuthenticatedAttribute : ValidationAttribute
{
    public override void Validate(ICurrentContext currentContext)
    {
        if (string.IsNullOrEmpty(currentContext.GetUserId()))
        {
            throw new ValidationException("User should be authenticated");
        }
    }
}
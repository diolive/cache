using DioLive.Cache.Common;
using DioLive.Cache.CoreLogic.Exceptions;

namespace DioLive.Cache.CoreLogic.Attributes
{
	public class AuthenticatedAttribute : ValidationAttribute
	{
		public override void Validate(ICurrentContext currentContext)
		{
			if (string.IsNullOrEmpty(currentContext.UserId))
			{
				throw new ValidationException("User should be authenticated");
			}
		}
	}
}
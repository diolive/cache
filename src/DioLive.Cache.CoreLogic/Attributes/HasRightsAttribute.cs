using System;

using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Exceptions;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Attributes
{
	public class HasRightsAttribute : ValidationAttribute
	{
		private readonly ShareAccess _shareAccess;

		public HasRightsAttribute(ShareAccess shareAccess)
		{
			_shareAccess = shareAccess;
		}

		public IPermissionsValidator? PermissionsValidator { get; set; }

		public override void Validate(ICurrentContext currentContext)
		{
			if (PermissionsValidator is null)
			{
				throw new InvalidOperationException("Property PermissionValidator should be initialized before using this attribute for context validation");
			}

			ResultStatus result = PermissionsValidator.CheckUserRightsForBudget(CurrentBudget(currentContext), currentContext.UserId, _shareAccess);

			ValidationException.RaiseIfNeeded(result);
		}
	}
}
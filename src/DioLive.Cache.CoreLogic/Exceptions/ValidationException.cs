using System;
using System.Security;

using DioLive.Cache.Common;

namespace DioLive.Cache.CoreLogic.Exceptions
{
	public class ValidationException : SecurityException
	{
		public ValidationException()
		{
		}

		public ValidationException(string message) : base(message)
		{
		}

		public ValidationException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public static void RaiseIfNeeded(ResultStatus result)
		{
			switch (result)
			{
				case ResultStatus.NotFound:
					throw new NotFoundException("Not found");

				case ResultStatus.Success:
					return;

				case ResultStatus.Forbidden:
					throw new ValidationException("User has no permissions");

				case ResultStatus.Error:
					throw new InvalidOperationException("Unexpected error occured");

				default:
					throw new ArgumentOutOfRangeException(nameof(result));
			}
		}
	}
}
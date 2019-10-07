using System;
using System.Security;

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
	}
}
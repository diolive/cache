using System;

namespace DioLive.Cache.CoreLogic.Exceptions
{
	public class NotFoundException : InvalidOperationException
	{
		public NotFoundException()
		{
		}

		public NotFoundException(string message) : base(message)
		{
		}

		public NotFoundException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
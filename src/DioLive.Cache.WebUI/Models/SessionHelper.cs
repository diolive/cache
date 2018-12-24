using System;

namespace Microsoft.AspNetCore.Http
{
	public static class SessionHelper
	{
		public static Guid? GetGuid(this ISession session, string key)
		{
			byte[] bytes = session.Get(key);
			return bytes != null ? new Guid(bytes) : default(Guid?);
		}

		public static void SetGuid(this ISession session, string key, Guid value)
		{
			session.Set(key, value.ToByteArray());
		}
	}
}
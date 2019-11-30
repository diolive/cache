using System;

using Microsoft.AspNetCore.Http;

namespace DioLive.Cache.WebUI.Models
{
	public static class SessionExtensions
	{
		public static Guid? GetGuid(this ISession session, string key)
		{
			return session.TryGetValue(key, out byte[] value)
				? new Guid(value)
				: default(Guid?);
		}

		public static void SetGuid(this ISession session, string key, Guid value)
		{
			session.Set(key, value.ToByteArray());
		}
	}
}
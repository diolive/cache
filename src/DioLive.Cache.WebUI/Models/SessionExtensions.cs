using System;

using Microsoft.AspNetCore.Http;

namespace DioLive.Cache.WebUI.Models
{
	public static class SessionExtensions
	{
		public static Guid? SafeGetGuid(this ISession session, string key)
		{
			return session.TryGetValue(key, out byte[] value)
				? new Guid(value)
				: default(Guid?);
		}

		public static void SetOrRemoveGuid(this ISession session, string key, Guid? value)
		{
			if (value.HasValue)
			{
				session.Set(key, value.Value.ToByteArray());
			}
			else
			{
				session.Remove(key);
			}
		}
	}
}
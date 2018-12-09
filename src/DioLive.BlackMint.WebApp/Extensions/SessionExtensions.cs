using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;

namespace DioLive.BlackMint.WebApp.Extensions
{
    public static class SessionExtensions
    {
        public static void SetObject<T>(this ISession session, string key, T value)
            where T : new()
        {
            string stringValue = JsonConvert.SerializeObject(value, Formatting.None);
            session.SetString(key, stringValue);
        }

        public static T GetObject<T>(this ISession session, string key)
            where T : new()
        {
            string stringValue = session.GetString(key);

            if (stringValue is null)
                return default(T);

            return JsonConvert.DeserializeObject<T>(stringValue);
        }
    }
}
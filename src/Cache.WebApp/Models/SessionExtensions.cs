namespace DioRed.Cache.WebApp.Models;

public static class SessionExtensions
{
    public static Guid? GetGuid(this ISession session, string key)
    {
        return session.TryGetValue(key, out byte[]? value)
            ? new Guid(value)
            : null;
    }

    public static void SetGuid(this ISession session, string key, Guid value)
    {
        session.Set(key, value.ToByteArray());
    }
}
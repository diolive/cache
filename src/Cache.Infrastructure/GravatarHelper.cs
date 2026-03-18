using System.Security.Cryptography;
using System.Text;

using DioRed.Cache.Domain;

namespace DioRed.Cache.Infrastructure;

public class GravatarHelper : IAvatarUrlProvider
{
    private static readonly MD5 Md5 = MD5.Create();

    public string GetAvatarUrl(string email, int size)
    {
        string hash = string.Join(
            "",
            Md5.ComputeHash(
                Encoding.ASCII.GetBytes(
                    email.ToLowerInvariant()
                )
            ).Select(b => b.ToString("x2"))
        );

        return $"https://www.gravatar.com/avatar/{hash}?d=identicon&s={size}";
    }
}

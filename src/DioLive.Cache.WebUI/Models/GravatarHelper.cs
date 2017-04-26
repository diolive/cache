using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DioLive.Cache.WebUI.Models
{
    public class GravatarHelper
    {
        private static readonly MD5 Md5;

        static GravatarHelper()
        {
            Md5 = MD5.Create();
        }

        public static string GetAvatarUrl(string email, int size)
        {
            string hash = string.Join("", Md5.ComputeHash(Encoding.ASCII.GetBytes(email.ToLowerInvariant())).Select(b => b.ToString("x2")));
            return $"https://www.gravatar.com/avatar/{hash}?d=identicon&s={size}";
        }
    }
}

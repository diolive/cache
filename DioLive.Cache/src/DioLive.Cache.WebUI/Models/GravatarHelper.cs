using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DioLive.Cache.WebUI.Models
{
    public class GravatarHelper
    {
        private readonly static MD5 _md5;

        static GravatarHelper()
        {
            _md5 = MD5.Create();
        }

        public static string GetAvatarUrl(string email, int size)
        {
            string hash = string.Join("", _md5.ComputeHash(Encoding.ASCII.GetBytes(email.ToLowerInvariant())).Select(b => b.ToString("x2")));
            return $"https://www.gravatar.com/avatar/{hash}?d=identicon&s={size}";
        }
    }
}

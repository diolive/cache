using Dapper.Contrib.Extensions;

namespace DioLive.BlackMint.WebApp.Models
{
    [Table("UserIdentities")]
    public class UserIdentity
    {
        [ExplicitKey]
        public int UserId { get; set; }

        public string NameIdentity { get; set; }
    }
}
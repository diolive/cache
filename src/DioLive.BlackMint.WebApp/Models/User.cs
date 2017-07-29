using Dapper.Contrib.Extensions;

namespace DioLive.BlackMint.WebApp.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string DisplayName { get; set; }
    }
}
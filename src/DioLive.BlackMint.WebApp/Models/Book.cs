using Dapper.Contrib.Extensions;

namespace DioLive.BlackMint.WebApp.Models
{
    [Table("books")]
    public class Book
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int AuthorId { get; set; }
    }
}
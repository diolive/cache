using Dapper.Contrib.Extensions;

namespace DioLive.BlackMint.WebApp.Models
{
    [Table("Currencies")]
    public class Currency
    {
        [ExplicitKey]
        public string Code { get; set; }

        public string Name { get; set; }

        public string Format { get; set; }
    }
}
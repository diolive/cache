using System;

using Dapper.Contrib.Extensions;

namespace DioLive.BlackMint.WebApp.Models
{
    [Table("Purchases")]
    public class PurchaseInfo
    {
        [Key]
        public int Id { get; set; }

        public string Seller { get; set; }

        public DateTime Date { get; set; }

        public decimal TotalCost { get; set; }

        public string Currency { get; set; }
    }
}
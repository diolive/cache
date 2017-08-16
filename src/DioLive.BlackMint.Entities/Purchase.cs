using System;

namespace DioLive.BlackMint.Entities
{
    public class Purchase
    {
        public int Id { get; set; }

        public int BookId { get; set; }

        public string Seller { get; set; }

        public DateTime Date { get; set; }

        public decimal TotalCost { get; set; }

        public string Currency { get; set; }

        public string Comments { get; set; }
    }
}
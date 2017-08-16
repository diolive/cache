using System;

namespace DioLive.BlackMint.Entities
{
    public class Income
    {
        public int Id { get; set; }

        public int BookId { get; set; }

        public string Source { get; set; }

        public DateTime Date { get; set; }

        public Money Amount { get; set; }

        public string Comments { get; set; }
    }
}
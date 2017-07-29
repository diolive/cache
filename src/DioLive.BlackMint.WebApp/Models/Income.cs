using System;

namespace DioLive.BlackMint.WebApp.Models
{
    public class IncomeInfo
    {
        public int Id { get; set; }

        public string Source { get; set; }

        public DateTime Date { get; set; }

        public decimal Value { get; set; }

        public string Currency { get; set; }
    }
}
using System;

namespace DioLive.BlackMint.WebApp.ViewModels
{
    public class UpdatePurchaseVM
    {
        public string Seller { get; set; }

        public DateTime? Date { get; set; }

        public string Currency { get; set; }

        public string Comments { get; set; }
    }
}
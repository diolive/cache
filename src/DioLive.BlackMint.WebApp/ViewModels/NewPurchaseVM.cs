﻿using System;
using System.ComponentModel.DataAnnotations;

namespace DioLive.BlackMint.WebApp.ViewModels
{
    public class NewPurchaseVM
    {
        [Required(AllowEmptyStrings = true)]
        public string Seller { get; set; }

        public DateTime Date { get; set; }

        [Required]
        public string Currency { get; set; }

        public string Comments { get; set; }
    }
}
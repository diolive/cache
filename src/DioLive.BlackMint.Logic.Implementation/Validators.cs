using System;

namespace DioLive.BlackMint.Logic.Implementation
{
    internal static class Validators
    {
        public static void ValidateCurrency(string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException($"Bad currency value: '{currency}'");
        }

        public static void ValidateNameIdentity(string nameIdentity)
        {
            if (string.IsNullOrWhiteSpace(nameIdentity))
                throw new ArgumentNullException(nameof(nameIdentity));
        }

        public static void ValidatePurchaseSeller(string seller)
        {
            if (seller is null)
                throw new ArgumentNullException(nameof(seller));
        }

        public static void ValidateBookName(string bookName)
        {
            if (string.IsNullOrWhiteSpace(bookName))
                throw new ArgumentException($"Invalid book name: '{bookName}'");
        }

        public static void ValidateUserId(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException($"Bad user id: {userId}", nameof(userId));
        }

        public static void ValidatePurchaseItemName(string purchaseItemName)
        {
            if (string.IsNullOrWhiteSpace(purchaseItemName))
                throw new ArgumentException($"Invalid purchase item name: '{purchaseItemName}'");
        }

        public static void ValidatePurchaseItemPrice(decimal purchaseItemPrice)
        {
            if (purchaseItemPrice < 0)
                throw new ArgumentException($"Bad purchase item price: {purchaseItemPrice}", nameof(purchaseItemPrice));
        }

        public static void ValidatePurchaseItemCount(int purchaseItemCount)
        {
            if (purchaseItemCount < 0)
                throw new ArgumentException($"Bad purchase item count: {purchaseItemCount}", nameof(purchaseItemCount));
        }
    }
}
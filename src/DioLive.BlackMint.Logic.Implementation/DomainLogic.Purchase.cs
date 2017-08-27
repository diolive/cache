using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.BlackMint.Entities;

namespace DioLive.BlackMint.Logic.Implementation
{
    public partial class DomainLogic
    {
        public async Task<Response<IEnumerable<Purchase>>> GetPurchases(int bookId, int pageNumber, int pageSize,
                                                                        SelectOrder order, int userId)
        {
            Validators.ValidateUserId(userId);

            ResponseStatus result = await ValidateBookAccess(bookId, userId, AccessRole.Read);
            if (result != ResponseStatus.Success)
                return new Response<IEnumerable<Purchase>>(result);

            IEnumerable<Purchase> purchases = await _domainStorage.GetPurchasesPage(bookId,
                new Window { Limit = pageSize, Offset = pageNumber * pageSize },
                order);

            return Response<IEnumerable<Purchase>>.Success(purchases);
        }

        public async Task<Response<Purchase>> GetPurchase(int purchaseId, int userId)
        {
            Validators.ValidateUserId(userId);

            ResponseStatus result = await ValidatePurchaseAccess(purchaseId, userId, AccessRole.Read);
            if (result != ResponseStatus.Success)
                return new Response<Purchase>(result);

            Purchase purchase = await _domainStorage.GetPurchaseById(purchaseId);

            return purchase is null
                ? Response<Purchase>.NotFound()
                : Response<Purchase>.Success(purchase);
        }

        public async Task<ResponseStatus> CreatePurchase(Purchase purchase, int userId)
        {
            Validators.ValidateUserId(userId);

            if (purchase is null)
                throw new ArgumentNullException(nameof(purchase));

            Validators.ValidatePurchaseSeller(purchase.Seller);
            Validators.ValidateCurrency(purchase.Currency);

            ResponseStatus result = await ValidateBookAccess(purchase.BookId, userId, AccessRole.Write);
            if (result != ResponseStatus.Success)
                return result;

            int purchaseId = await _domainStorage.AddPurchase(purchase);
            purchase.Id = purchaseId;
            await _logger.WritePurchaseLog(purchaseId, userId);

            return ResponseStatus.Success;
        }

        public async Task<ResponseStatus> UpdatePurchase(Purchase purchase, int userId)
        {
            Validators.ValidateUserId(userId);

            if (purchase is null)
                throw new ArgumentNullException(nameof(purchase));

            Validators.ValidatePurchaseSeller(purchase.Seller);
            Validators.ValidateCurrency(purchase.Currency);

            ResponseStatus result = await ValidatePurchaseAccess(purchase.Id, userId, AccessRole.Write);
            if (result != ResponseStatus.Success)
                return result;

            bool done = await _domainStorage.UpdatePurchase(purchase);
            await _logger.WritePurchaseLog(purchase.Id, userId);

            return done ? ResponseStatus.Success : ResponseStatus.NotFound;
        }

        public async Task<Response<IEnumerable<PurchaseItem>>> GetPurchaseItems(int purchaseId, int userId)
        {
            Validators.ValidateUserId(userId);

            ResponseStatus responseStatus = await ValidatePurchaseAccess(purchaseId, userId, AccessRole.Read);
            if (responseStatus != ResponseStatus.Success)
                return new Response<IEnumerable<PurchaseItem>>(responseStatus);

            IEnumerable<PurchaseItem> result = await _domainStorage.GetPurchaseItems(purchaseId);

            return Response<IEnumerable<PurchaseItem>>.Success(result);
        }

        public async Task<Response<PurchaseItem>> GetPurchaseItem(int purchaseItemId, int userId)
        {
            Validators.ValidateUserId(userId);

            ResponseStatus responseStatus = await ValidatePurchaseItemAccess(purchaseItemId, userId, AccessRole.Read);
            if (responseStatus != ResponseStatus.Success)
                return new Response<PurchaseItem>(responseStatus);

            PurchaseItem result = await _domainStorage.GetPurchaseItemById(purchaseItemId);

            return Response<PurchaseItem>.Success(result);
        }

        public async Task<ResponseStatus> CreatePurchaseItem(PurchaseItem purchaseItem, int userId)
        {
            Validators.ValidateUserId(userId);

            if (purchaseItem is null)
                throw new ArgumentNullException(nameof(purchaseItem));

            Validators.ValidatePurchaseItemName(purchaseItem.Name);
            Validators.ValidatePurchaseItemPrice(purchaseItem.Price);
            Validators.ValidatePurchaseItemCount(purchaseItem.Count);

            ResponseStatus result = await ValidatePurchaseAccess(purchaseItem.PurchaseId, userId, AccessRole.Write);
            if (result != ResponseStatus.Success)
                return result;

            int purchaseItemId = await _domainStorage.AddPurchaseItem(purchaseItem);
            purchaseItem.Id = purchaseItemId;
            await _logger.WritePurchaseLog(purchaseItem.PurchaseId, userId);

            return ResponseStatus.Success;
        }
    }
}
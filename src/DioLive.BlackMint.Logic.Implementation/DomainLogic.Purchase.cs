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
    }
}
using System.Threading.Tasks;

using DioLive.BlackMint.Entities;

namespace DioLive.BlackMint.Logic.Implementation
{
    public partial class DomainLogic
    {
        private async Task<ResponseStatus> ValidateBookAccess(int bookId, int userId, AccessRole minimalRole)
        {
            AccessRole role = await _domainStorage.GetBookAccess(bookId, userId);
            return AccessRoleToResponseStatus(role, minimalRole);
        }

        private async Task<ResponseStatus> ValidatePurchaseAccess(int purchaseId, int userId, AccessRole minimalRole)
        {
            AccessRole role = await _domainStorage.GetPurchaseAccess(purchaseId, userId);
            return AccessRoleToResponseStatus(role, minimalRole);
        }
    }
}
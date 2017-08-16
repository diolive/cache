using DioLive.BlackMint.Entities;
using DioLive.BlackMint.Persistence;

namespace DioLive.BlackMint.Logic.Implementation
{
    public partial class DomainLogic : IDomainLogic
    {
        private readonly IDomainStorage _domainStorage;
        private readonly ILogger _logger;

        public DomainLogic(IDomainStorage domainStorage, ILogger logger)
        {
            _domainStorage = domainStorage;
            _logger = logger;
        }

        private static ResponseStatus AccessRoleToResponseStatus(AccessRole role, AccessRole minimalRole)
        {
            if (role == AccessRole.None)
                return ResponseStatus.NotFound;

            if (role < minimalRole)
                return ResponseStatus.Forbidden;

            return ResponseStatus.Success;
        }
    }
}
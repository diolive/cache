using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.BlackMint.Entities;
using DioLive.BlackMint.Logic;
using DioLive.BlackMint.WebApp.ViewModels;

using Microsoft.AspNetCore.Mvc;

namespace DioLive.BlackMint.WebApp.Controllers.api
{
    public class PurchaseItemController : ApiControllerBase
    {
        private readonly IDomainLogic _domainLogic;

        public PurchaseItemController(IDomainLogic domainLogic)
        {
            _domainLogic = domainLogic;
        }

        [HttpGet("/api/purchase/{purchaseId:int}/items")]
        public async Task<IActionResult> GetAllAsync(int purchaseId)
        {
            Response<IEnumerable<PurchaseItem>> response = await _domainLogic.GetPurchaseItems(purchaseId, UserId);

            return ResponseToResult(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            Response<PurchaseItem> response = await _domainLogic.GetPurchaseItem(id, UserId);

            return ResponseToResult(response);
        }

        [HttpPost("/api/purchase/{purchaseId:int}/items")]
        public async Task<IActionResult> Post(int purchaseId, [FromBody]NewPurchaseItemVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var purchaseItem = new PurchaseItem
            {
                PurchaseId = purchaseId,
                Name = model.Name?.Trim(),
                Price = model.Price,
                Count = model.Count
            };

            ResponseStatus responseStatus = await _domainLogic.CreatePurchaseItem(purchaseItem, UserId).ConfigureAwait(false);

            if (responseStatus == ResponseStatus.Success)
            {
                return CreatedAtAction(nameof(Get), new { purchaseItem.Id }, purchaseItem);
            }

            return ResponseStatusToResult(responseStatus);
        }
    }
}
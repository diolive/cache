﻿using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.BlackMint.Entities;
using DioLive.BlackMint.Logic;
using DioLive.BlackMint.WebApp.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DioLive.BlackMint.WebApp.Controllers.api
{
    [Authorize]
    public class PurchaseController : ApiControllerBase
    {
        private readonly IDomainLogic _domainLogic;

        public PurchaseController(IDomainLogic domainLogic)
        {
            _domainLogic = domainLogic;
        }

        [HttpGet("/api/book/{bookId}/purchases")]
        public async Task<IActionResult> Find(int bookId, int pageNumber = 0, int pageSize = 20, bool asc = true)
        {
            if (pageNumber < 0 || pageSize <= 0)
                return BadRequest();

            SelectOrder order = asc ? SelectOrder.Ascending : SelectOrder.Descending;
            Response<IEnumerable<Purchase>> response =
                await _domainLogic.GetPurchases(bookId, pageNumber, pageSize, order, UserId);

            return ResponseToResult(response);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            Response<Purchase> response = await _domainLogic.GetPurchase(id, UserId);
            return ResponseToResult(response);
        }

        [HttpPost("/api/book/{bookId}/purchases")]
        public async Task<IActionResult> Post(int bookId, [FromBody]NewPurchaseVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var purchase = new Purchase
            {
                BookId = bookId,
                Seller = model.Seller?.Trim(),
                Date = model.Date,
                Currency = model.Currency,
                Comments = model.Comments
            };

            ResponseStatus responseStatus = await _domainLogic.CreatePurchase(purchase, UserId);

            if (responseStatus == ResponseStatus.Success)
            {
                return CreatedAtAction(nameof(Get), new { purchase.Id }, purchase);
            }

            return ResponseStatusToResult(responseStatus);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody]UpdatePurchaseVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Response<Purchase> getResponse = await _domainLogic.GetPurchase(id, UserId);
            if (getResponse.Status != ResponseStatus.Success)
                return ResponseStatusToResult(getResponse.Status);

            Purchase purchase = getResponse.Result;

            purchase.Seller = model.Seller?.Trim() ?? purchase.Seller;
            purchase.Date = model.Date ?? purchase.Date;
            purchase.Currency = model.Currency ?? purchase.Currency;
            purchase.Comments = model.Comments ?? purchase.Comments;

            var updateStatus = await _domainLogic.UpdatePurchase(purchase, UserId);

            return ResponseStatusToResult(updateStatus);
        }
    }
}
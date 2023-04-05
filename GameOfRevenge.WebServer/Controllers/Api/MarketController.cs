using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Common.Models.Quest;
using Google.Apis.Auth.OAuth2;
using Google.Apis.AndroidPublisher.v3;
using Google.Apis.Services;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common;
using System;

namespace GameOfRevenge.WebServer.Controllers.Api
{
    public class MarketController : BaseApiController
    {
        private readonly IUserStructureManager userStructureManager;
        private readonly IUserMarketManager userMarketManager;

        public MarketController(IUserStructureManager userStructureManager, IUserMarketManager userMarketManager)
        {
            this.userStructureManager = userStructureManager;
            this.userMarketManager = userMarketManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGifts() => ReturnResponse(await userMarketManager.GetAllGifts(Token.PlayerId));


        [HttpPost]
        public async Task<IActionResult> RedeemGiftResource(int giftId) => ReturnResponse(await userMarketManager.RedeemGiftResource(giftId));


        [HttpPost]
        public async Task<IActionResult> GiftResource(int toPlayerId, int food, int wood, int ore) => ReturnResponse(await userStructureManager.GiftResource(Token.PlayerId, toPlayerId,  food,  wood,  ore));


        [HttpGet]
        public IActionResult GetAllProducts()
        {
            var productList = CacheProductDataManager.ProductList;
            return ReturnResponse(productList);
        }

        [HttpPost]
        public async Task<IActionResult> BuyProduct(int food, int wood, int ore) => ReturnResponse(await userMarketManager.BuyProduct(Token.PlayerId, food, wood, ore));


        [HttpGet]
        public IActionResult GetAllIAPProducts()
        {
            var iapProducts = CacheProductDataManager.GetIAPProducts(Token.PlayerId);
            return ReturnResponse(new Response<List<IAPProduct>>(iapProducts, CaseType.Success, "Available Products"));
        }

        [HttpPost]
        public async Task<IActionResult> ValidatePurchase(int version, string productId, string transactionId, string store, string package, string data)
        {
            var playerId = Token.PlayerId;

            var valid = true;
            try
            {
                if (valid)
                {
                    var response = await userMarketManager.RedeemPurchaseProduct(playerId, productId);
                    if (!response.IsSuccess) throw new InvalidModelExecption("Error when collecting products");
                }
            }
            catch (InvalidModelExecption ex)
            {
                return ReturnResponse(new Response<bool>(valid, 200, ex.Message));
            }
            catch (Exception ex)
            {
                return ReturnResponse(new Response<bool>(valid, 201, "Unknown error."));
            }

            var msg = valid ? "Valid product purchase" : "Invalid product purchase";
            return ReturnResponse(new Response<bool>(valid, CaseType.Success, msg));
        }
    }
}

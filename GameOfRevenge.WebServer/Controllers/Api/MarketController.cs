using System;
using System.Linq;
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
using GameOfRevenge.Common;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Services;

namespace GameOfRevenge.WebServer.Controllers.Api
{
    public class MarketController : BaseApiController
    {
        private readonly IPlayerDataManager manager;
        private readonly IUserStructureManager userStructureManager;
        private readonly IUserMarketManager userMarketManager;
        private readonly IUserResourceManager resourceManager;
        private readonly List<IAPProduct> iapProducts;

        public MarketController(IPlayerDataManager playerDataManager, IUserStructureManager userStructureManager,
                                IUserMarketManager userMarketManager, IUserResourceManager resourceManager)
        {
            manager = playerDataManager;
            this.userStructureManager = userStructureManager;
            this.userMarketManager = userMarketManager;
            this.resourceManager = resourceManager;

            iapProducts = new List<IAPProduct>()
            {
                new IAPProduct("t_a002", 500), new IAPProduct("t_a004", 1000),
                new IAPProduct("t_a014", 5000), new IAPProduct("t_a024", 10000)
            };
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


        [HttpGet]//obsolete
        public IActionResult GetAllIAPProducts()
        {
            var dummy = new List<IAPProduct>();
            return ReturnResponse(new Response<List<IAPProduct>>(dummy, CaseType.Success, "Available IAP Products"));

//            var iapProducts = CacheProductDataManager.GetPackageProducts(Token.PlayerId);
//            return ReturnResponse(new Response<IReadOnlyList<ProductPackage>>(iapProducts, CaseType.Success, "Available Products"));
        }

        [HttpGet]
        public IActionResult GetIAPProducts()
        {
            return ReturnResponse(new Response<List<IAPProduct>>(iapProducts, CaseType.Success, "Available IAP Products"));
        }

        [HttpGet]
        public IActionResult GetShopProducts()
        {
            var packageProducts = CacheProductDataManager.GetPackageProducts(Token.PlayerId);
            return ReturnResponse(new Response<IReadOnlyList<ProductPackage>>(packageProducts, CaseType.Success, "Available Products"));
        }

        [HttpPost]
        public async Task<IActionResult> PurchaseShopProduct(string productId)
        {
            var playerId = Token.PlayerId;
            try
            {
                var packageProducts = CacheProductDataManager.GetPackageProducts(Token.PlayerId);
                var product = packageProducts.FirstOrDefault(x => (x.ProductId == productId));
                if (product == null) throw new InvalidModelExecption("Invalid product");

                var response = await manager.GetAllPlayerData(playerId, DataType.Resource);
                if (!response.IsSuccess) throw new DataNotExistExecption(response.Message);

                var golddata = response.Data.Find(x => (x.DataType == DataType.Resource) && (x.ValueId == (int)ResourceType.Gold));
                if (golddata == null) throw new DataNotExistExecption("No resource gold");

                var reqGold = product.Cost;
                long.TryParse(golddata.Value, out long plyGold);
                if (plyGold < reqGold) throw new DataNotExistExecption("Not enough gold");

                var reddemResponse = await userMarketManager.RedeemPurchaseProduct(playerId, productId);
                if (!reddemResponse.IsSuccess) throw new DataNotExistExecption("Error when collecting products");

                await manager.RemovePlayerResourceData(playerId, 0, 0, 0, 0, reqGold);

                return ReturnResponse(new Response<bool>(true, 100, "Success"));
            }
            catch (InvalidModelExecption ex)
            {
                return ReturnResponse(new Response<bool>(true, 200, ErrorManager.ShowError(ex)));
            }
            catch (DataNotExistExecption ex)
            {
                return ReturnResponse(new Response<bool>(true, 201, ErrorManager.ShowError(ex)));
            }
            catch (Exception ex)
            {
                return ReturnResponse(new Response<bool>(true, 202, ErrorManager.ShowError(ex)));
            }
        }

        [HttpPost]
        public async Task<IActionResult> ValidatePurchase(int version, string productId, string transactionId, string store, string package, string data)
        {
            var playerId = Token.PlayerId;

            var approved = true;

            var product = iapProducts.Find(x => (x.ProductId == productId));
            try
            {
                if (product == null) throw new InvalidModelExecption("Product not found");

                if (approved)
                {
                    var response = await resourceManager.SumGoldResource(playerId, product.Value);
                    if (!response.IsSuccess) throw new InvalidModelExecption(response.Message);
                }
                else
                {
                    throw new InvalidModelExecption("Product not valid");
                }
            }
            catch (InvalidModelExecption ex)
            {
                return ReturnResponse(new Response<bool>(approved, 200, ex.Message));
            }
            catch (Exception ex)
            {
                return ReturnResponse(new Response<bool>(approved, 201, "Unknown error."));
            }

            var msg = approved ? "Product purchase approved" : "Product purchase already approved";
            return ReturnResponse(new Response<bool>(approved, CaseType.Success, msg));
        }
    }
}

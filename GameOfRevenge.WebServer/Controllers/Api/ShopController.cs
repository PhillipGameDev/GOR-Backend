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
    public class ShopController : BaseApiController
    {
        private readonly IPlayerDataManager manager;
        private readonly IUserShopManager userShopManager;
        private readonly IUserResourceManager resourceManager;
        private readonly List<IAPProduct> iapProducts;
        private readonly List<IAPProduct> subscriptionPlans;

        public ShopController(IPlayerDataManager playerDataManager, IUserShopManager userShopManager,
                            IUserResourceManager resourceManager)
        {
            this.manager = playerDataManager;
            this.userShopManager = userShopManager;
            this.resourceManager = resourceManager;

            iapProducts = new List<IAPProduct>()
            {
                new IAPProduct("t_a001", 110), new IAPProduct("t_a005", 500),
                new IAPProduct("t_a010", 1200), new IAPProduct("t_a014", 2300),
                new IAPProduct("t_a020", 4800), new IAPProduct("t_a025", 10000),
                new IAPProduct("t_a035", 25000), new IAPProduct("t_a055", 60000)
            };
            subscriptionPlans = new List<IAPProduct>()
            {
                new IAPProduct("s_d007", 7), new IAPProduct("s_d030", 30)
            };
        }

        [HttpGet]
        public IActionResult GetIAPProducts()
        {
            return ReturnResponse(new Response<List<IAPProduct>>(iapProducts, CaseType.Success, "Available IAP Products"));
        }

        [HttpGet]
        public IActionResult GetSubscriptionPlans()
        {
            return ReturnResponse(new Response<List<IAPProduct>>(subscriptionPlans, CaseType.Success, "Available Subscription Plans"));
        }

        [HttpGet]
        public async Task<IActionResult> GetSubscription()
        {
            return ReturnResponse(await userShopManager.GetSubscription(Token.PlayerId));
        }

        [HttpPost]
        public async Task<IActionResult> ValidateSubscription(string transactionId, bool active)
        {
            return ReturnResponse(await userShopManager.ValidateSubscription(Token.PlayerId, transactionId, active));
        }

        [HttpPost]
        public async Task<IActionResult> ValidatePurchase(int version, string productId, string transactionId, string store, string signature, string data)
        {
            var playerId = Token.PlayerId;

            var approved = false;
            var subscription = false;

            var product = iapProducts.Find(x => (x.ProductId == productId));
            if (product == null)
            {
                product = subscriptionPlans.Find(x => (x.ProductId == productId));
                if (product != null) subscription = true;
            }
            try
            {
                if (product == null) throw new DataNotExistExecption("Product not found");

                approved = true;
                if (approved)
                {
                    if (subscription)
                    {
                        var packageLists = CacheShopDataManager.AllSubscriptionPackages;
                        var package = packageLists.FirstOrDefault(x => x.Name == productId);
                        if (package == null) throw new DataNotExistExecption("Package list not found");

                        var transactionDate = DateTime.UtcNow;
                        var getResp = await userShopManager.GetSubscription(playerId);
                        if (getResp.IsSuccess && getResp.HasData)
                        {
                            transactionDate = getResp.Data.TransactionDate;
                        }
                        var response = await userShopManager.AddSubscription(playerId, store, transactionId, transactionDate, productId, product.Value);
                        if (!response.IsSuccess) throw new InvalidModelExecption(response.Message);

                        var reddemResponse = await userShopManager.CollectPackage(playerId, package);
                        if (reddemResponse != ReturnCode.OK) throw new DataNotExistExecption("Error when collecting package");
                    }
                    else
                    {
                        var response = await resourceManager.SumGoldResource(playerId, product.Value);
                        if (!response.IsSuccess) throw new InvalidModelExecption(response.Message);
                    }
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
            catch (DataNotExistExecption ex)
            {
                return ReturnResponse(new Response<bool>(approved, 201, ex.Message));
            }
            catch (Exception ex)
            {
                return ReturnResponse(new Response<bool>(approved, 202, "Unknown error."));
            }

            var msg = approved ? "Product purchase approved" : "Product purchase already approved";
            return ReturnResponse(new Response<bool>(approved, CaseType.Success, msg));
        }

        [HttpGet]
        public IActionResult GetAllShopCategories() => ReturnResponse(new Response<IReadOnlyList<ShopCategory>>(CacheShopDataManager.AllShopCategories, CaseType.Success, "Available Shop Categories"));

        [HttpGet]
        public IActionResult GetAllPackageLists() => ReturnResponse(new Response<IReadOnlyList<PackageList>>(CacheShopDataManager.AllPackageLists, CaseType.Success, "Available Package Lists"));

        [HttpGet]
        public IActionResult GetAllSubscriptionPackages() => ReturnResponse(new Response<IReadOnlyList<PackageList>>(CacheShopDataManager.AllSubscriptionPackages, CaseType.Success, "Available Subscription Packages"));

        [HttpPost]
        public async Task<IActionResult> PurchaseShopItem(int itemId)
        {
            var playerId = Token.PlayerId;
            try
            {
                var shopItems = CacheShopDataManager.AllShopItems;
                var shopItem = shopItems.FirstOrDefault(item => item.Id == itemId);
                if (shopItem == null) throw new InvalidModelExecption("Invalid Shop Item");

                var response = await manager.GetAllPlayerData(playerId, DataType.Resource);
                if (!response.IsSuccess) throw new DataNotExistExecption(response.Message);

                var golddata = response.Data.Find(x => (x.DataType == DataType.Resource) && (x.ValueId == (int)ResourceType.Gold));
                if (golddata == null) throw new DataNotExistExecption("No resource gold");

                var reqGold = shopItem.Cost;
                long.TryParse(golddata.Value, out long plyGold);
                if (plyGold < reqGold) throw new DataNotExistExecption("Not enough gold");

                var reddemResponse = await userShopManager.RedeemPurchaseShopItem(playerId, shopItem);
                if (!reddemResponse.IsSuccess) throw new DataNotExistExecption("Error when collecting shop items");

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
        public async Task<IActionResult> PurchasePackage(int packageId)
        {
            var playerId = Token.PlayerId;
            try
            {
                var packageLists = CacheShopDataManager.AllPackageLists;
                var packageList = packageLists.FirstOrDefault(item => item.ListId == packageId);
                if (packageList == null) throw new InvalidModelExecption("Invalid Package List");

                var response = await manager.GetAllPlayerData(playerId, DataType.Resource);
                if (!response.IsSuccess) throw new DataNotExistExecption(response.Message);

                var golddata = response.Data.Find(x => (x.DataType == DataType.Resource) && (x.ValueId == (int)ResourceType.Gold));
                if (golddata == null) throw new DataNotExistExecption("No resource gold");

                var reqGold = packageList.Cost;
                long.TryParse(golddata.Value, out long plyGold);
                if (plyGold < reqGold) throw new DataNotExistExecption("Not enough gold");

                var reddemResponse = await userShopManager.RedeemPurchasePackage(playerId, packageId);
                if (!reddemResponse.IsSuccess) throw new DataNotExistExecption("Error when collecting package");

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
    }
}

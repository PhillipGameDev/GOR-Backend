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
        private readonly IShopManager shopManager;
        private readonly IUserShopManager userShopManager;

        public ShopController(IPlayerDataManager playerDataManager, IShopManager shopManager,
                                IUserShopManager userShopManager)
        {
            this.manager = playerDataManager;
            this.shopManager = shopManager;
            this.userShopManager = userShopManager;
        }

        [HttpGet]
        public IActionResult GetAllShopCategories() => ReturnResponse(new Response<IReadOnlyList<ShopCategory>>(CacheShopDataManager.AllShopCategories, CaseType.Success, "Available Shop Categories"));

        [HttpGet]
        public IActionResult GetAllPackageLists() => ReturnResponse(new Response<IReadOnlyList<PackageList>>(CacheShopDataManager.AllPackageLists, CaseType.Success, "Available Package Lists"));

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
    }
}

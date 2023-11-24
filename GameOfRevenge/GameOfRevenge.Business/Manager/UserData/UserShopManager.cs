﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models;

namespace GameOfRevenge.Business.Manager.UserData
{
    public class UserShopManager : BaseManager, IUserShopManager
    {
        protected static readonly IPlayerDataManager manager = new PlayerDataManager();
        public async Task<Response> BuyPackage(int playerId, int packageId)
        {
            var spParam = new Dictionary<string, object>()
            {
                { "PlayerId", playerId },
                { "TypeId", 1 },
                { "ItemId", packageId }
            };

            return await Db.ExecuteSPNoData("BuyShopItem", spParam);
        }

        public async Task<Response> BuyShopItem(int playerId, int itemId)
        {
            var spParam = new Dictionary<string, object>()
            {
                { "PlayerId", playerId },
                { "TypeId", 0 },
                { "ItemId", itemId }
            };

            return await Db.ExecuteSPNoData("BuyShopItem", spParam);
        }

        public async Task<ReturnCode> CollectShopItem(int playerId, IReadOnlyShopItemTable shopItem)
        {
            if (shopItem == null) return ReturnCode.OK;

            var resp = await manager.GetAllPlayerData(playerId, DataType.Item);
            if (!resp.IsSuccess || !resp.HasData) return ReturnCode.InvalidOperation;

            var noErrors = true;
            var userShopItems = resp.Data;

            var data = userShopItems.Find(x => (x.ValueId == shopItem.ItemId));
            if (data != null)
            {
                if (int.TryParse(data.Value, out int userCount))
                {
                    userCount ++;
                    data.Value = userCount.ToString();
                    var saveResp = await manager.UpdatePlayerDataID(playerId, data.Id, data.Value);
                    if (!saveResp.IsSuccess) noErrors = false;
                }
                else
                {
                    noErrors = false;
                }
            }
            else
            {
                var value = "1";
                var saveResp = await manager.AddOrUpdatePlayerData(playerId, DataType.Item, shopItem.ItemId, value);
                if (saveResp.IsSuccess && saveResp.HasData)
                {
                    userShopItems.Add(saveResp.Data.ToPlayerDataTable);
                }
                else
                {
                    noErrors = false;
                }
            }

            return noErrors ? ReturnCode.OK : ReturnCode.Failed;
        }

        public async Task<ReturnCode> CollectPackage(int playerId, PackageList package)
        {
            if (package == null) return ReturnCode.OK;

            var resp = await manager.GetAllPlayerData(playerId, DataType.Item);
            if (!resp.IsSuccess || !resp.HasData) return ReturnCode.InvalidOperation;

            var noErrors = true;
            var userShopItems = resp.Data;

            foreach (var packageItem in package.Items)
            {
                var data = userShopItems.Find(x => (x.ValueId == packageItem.ItemId));
                if (data != null)
                {
                    if (int.TryParse(data.Value, out int userCount))
                    {
                        userCount += packageItem.Count;
                        data.Value = userCount.ToString();
                        var saveResp = await manager.UpdatePlayerDataID(playerId, data.Id, data.Value);
                        if (!saveResp.IsSuccess) noErrors = false;
                    }
                    else
                    {
                        noErrors = false;
                    }
                }
                else
                {
                    var value = packageItem.Count.ToString();
                    var saveResp = await manager.AddOrUpdatePlayerData(playerId, DataType.Item, packageItem.ItemId, value);
                    if (saveResp.IsSuccess && saveResp.HasData)
                    {
                        userShopItems.Add(saveResp.Data.ToPlayerDataTable);
                    }
                    else
                    {
                        noErrors = false;
                    }
                }
            }

            return noErrors ? ReturnCode.OK : ReturnCode.Failed;
        }

        public async Task<Response> RedeemPurchasePackage(int playerId, int packageId)
        {
            var packageLists = CacheShopDataManager.AllPackageLists;
            var package = packageLists.FirstOrDefault(item => item.ListId == packageId);

            var resp = await CollectPackage(playerId, package);
            if (resp != ReturnCode.OK)
            {
                return new Response(CaseType.Error, "Failed to redeem package.");
            }
            else
            {
                return new Response(CaseType.Success, "Package redeemed");
            }
        }

        public async Task<Response> RedeemPurchaseShopItem(int playerId, IReadOnlyShopItemTable shopItem)
        {
            var resp = await CollectShopItem(playerId, shopItem);
            if (resp != ReturnCode.OK)
            {
                return new Response(CaseType.Error, "Failed to redeem shop item.");
            }
            else
            {
                //TODO: register transacton
                /*                var redemeedResp = await Db.ExecuteSPNoData("RedeemChapterReward", new Dictionary<string, object>()
                                {
                                    { "PlayerChapterUserId", chapterData.ChapterUserDataId }
                                });*/

                return new Response(CaseType.Success, "Shop item redeemed");
            }
        }
    }
}
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Inventory;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameOfRevenge.Business.Manager.UserData
{
    public class UserInventoryManager : BaseUserDataManager, IUserInventoryManager
    {
        private readonly IUserResourceManager userResourceManager = new UserResourceManager();
        private readonly IUserActiveBoostsManager userActiveBoostManager = new UserActiveBoostManager();

        /*        public async Task<Response<UserInventoryDataUpdated>> UpdateItem(int playerId, int playerDataId, int value)
                {
                    try
                    {
        //                int valueId = CacheInventoryDataManager.GetFullInventoryItemData(type).Id;
                        var response = await manager.UpdatePlayerData(playerId, playerDataId, value.ToString());
                        if (response.IsSuccess)
                        {
                            return new Response<UserInventoryDataUpdated>(PlayerDataToUserInventoryData(response.Data), response.Case, response.Message);
                        }
                        else
                        {
                            return new Response<UserInventoryDataUpdated>(response.Case, response.Message);
                        }
                    }
                    catch (InvalidModelExecption ex)
                    {
                        return new Response<UserInventoryDataUpdated>()
                        {
                            Case = 200,
                            Message = ErrorManager.ShowError(ex)
                        };
                    }
                    catch (DataNotExistExecption ex)
                    {
                        return new Response<UserInventoryDataUpdated>()
                        {
                            Case = 201,
                            Message = ErrorManager.ShowError(ex)
                        };
                    }
                    catch (RequirementExecption ex)
                    {
                        return new Response<UserInventoryDataUpdated>()
                        {
                            Case = 202,
                            Message = ErrorManager.ShowError(ex)
                        };
                    }
                    catch (Exception)
                    {
                        return new Response<UserInventoryDataUpdated>()
                        {
                            Case = 0,
                            Message = ErrorManager.ShowError()
                        };
                    }
                }*/

        #region origin
        public async Task<Response<UserInventoryDataUpdated>> UpdateItem(int playerId, InventoryItemType itemType, long playerDataId, int value)
        {
            try
            {
                if ((itemType == InventoryItemType.Unknown) && (playerDataId <= 0))
                {
                    throw new DataNotExistExecption("Invalid parameters");
                }

                Response<PlayerDataTableUpdated> response;
                if (playerDataId <= 0)
                {
                    var item = CacheInventoryDataManager.GetFullInventoryItemData(itemType);
                    if (item == null) throw new DataNotExistExecption("Item does not exist");

                    response = await manager.AddOrUpdatePlayerData(playerId, DataType.Inventory, item.Id, value.ToString());
                }
                else
                {
                    response = await manager.UpdatePlayerDataID(playerId, playerDataId, value.ToString());
                }

                if (response.IsSuccess)
                {
                    return new Response<UserInventoryDataUpdated>(PlayerData.PlayerDataToUserInventoryData(response.Data), response.Case, response.Message);
                }
                else
                {
                    return new Response<UserInventoryDataUpdated>(response.Case, response.Message);
                }
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<UserInventoryDataUpdated>()
                {
                    Case = 200,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<UserInventoryDataUpdated>()
                {
                    Case = 201,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (RequirementExecption ex)
            {
                return new Response<UserInventoryDataUpdated>()
                {
                    Case = 202,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (Exception)
            {
                return new Response<UserInventoryDataUpdated>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError()
                };
            }
        }

        //        public async Task<Response<UserInventoryData>> AddUniqueItem(int playerId, InventoryItemType itemType)
        //        {
        //            return await AddItem(playerId, itemType, 1, true);
        //        }

        public async Task<Response<UserInventoryData>> AddUniqueItem(int playerId, InventoryItemType itemType, int value = 1)
        {
            return await AddItem(playerId, itemType, value, true);
        }

        //        public async Task<Response<UserInventoryData>> AddItem(int playerId, InventoryItemType itemType)
        //        {
        //            return await AddItem(playerId, itemType, 1, false);
        //        }

        public async Task<Response<UserInventoryData>> AddItem(int playerId, InventoryItemType itemType, int value = 1, bool unique = false)
        {
            try
            {
                if (itemType == InventoryItemType.Unknown)
                {
                    throw new DataNotExistExecption("Invalid parameters");
                }

                var item = CacheInventoryDataManager.GetFullInventoryItemData(itemType);
                if (item == null) throw new DataNotExistExecption("Item does not exist");

                var response = await manager.AddOrUpdatePlayerData(playerId, DataType.Inventory, item.Id, value.ToString(), unique);
                if (response.IsSuccess)
                {
                    return new Response<UserInventoryData>(PlayerData.PlayerDataToUserInventoryData(response.Data), response.Case, response.Message);
                }
                else
                {
                    return new Response<UserInventoryData>(response.Case, response.Message);
                }


                /*                int total = 0;
                //                int valueId = CacheInventoryDataManager.GetFullInventoryItemData(type).Id;
                //                var existingData = await manager.GetPlayerData(playerId, DataType.Inventory, valueId);
                //                if (existingData.IsSuccess && existingData.HasData)
                //                {
                //                    var invData = PlayerDataToUserInventoryData(existingData.Data);
                //                    total = invData.Value;
                //                }
                                total += count;

                                return await UpdateItem(playerId, type, total);*/
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<UserInventoryData>()
                {
                    Case = 200,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<UserInventoryData>()
                {
                    Case = 201,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (RequirementExecption ex)
            {
                return new Response<UserInventoryData>()
                {
                    Case = 202,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (Exception)
            {
                return new Response<UserInventoryData>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError()
                };
            }
        }

        public async Task<Response<UserInventoryDataUpdated>> AddItem(int playerId, InventoryItemType itemType, long playerDataId, int value)
        {
            try
            {
                if ((itemType == InventoryItemType.Unknown) && (playerDataId <= 0))
                {
                    throw new DataNotExistExecption("Invalid parameters");
                }

                Response<PlayerDataTableUpdated> response;
                if (playerDataId <= 0)
                {
                    var item = CacheInventoryDataManager.GetFullInventoryItemData(itemType);
                    if (item == null) throw new DataNotExistExecption("Item does not exist");

                    response = await manager.IncrementPlayerData(playerId, DataType.Inventory, item.Id, value);
                }
                else
                {
                    response = await manager.SumPlayerData(playerId, playerDataId, value);
                }

                if (response.IsSuccess)
                {
                    return new Response<UserInventoryDataUpdated>(PlayerData.PlayerDataToUserInventoryData(response.Data), response.Case, response.Message);
                }
                else
                {
                    return new Response<UserInventoryDataUpdated>(response.Case, response.Message);
                }
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<UserInventoryDataUpdated>()
                {
                    Case = 200,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<UserInventoryDataUpdated>()
                {
                    Case = 201,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (RequirementExecption ex)
            {
                return new Response<UserInventoryDataUpdated>()
                {
                    Case = 202,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (Exception)
            {
                return new Response<UserInventoryDataUpdated>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError()
                };
            }
        }




        public async Task<Response<UserInventoryData>> RemoveItem(int playerId, InventoryItemType itemType, long playerDataId)
        {
            try
            {
                if ((itemType == InventoryItemType.Unknown) && (playerDataId <= 0))
                {
                    throw new DataNotExistExecption("Invalid parameters");
                }

                Response<PlayerDataTableUpdated> response;
                if (playerDataId <= 0)
                {
                    var item = CacheInventoryDataManager.GetFullInventoryItemData(itemType);
                    if (item == null) throw new DataNotExistExecption("Item does not exist");

                    response = await manager.RemovePlayerData(playerId, DataType.Inventory, item.Id);
                }
                else
                {
                    response = await manager.RemovePlayerData(playerId, playerDataId);
                }

                if (response.IsSuccess)
                {
                    return new Response<UserInventoryData>(PlayerData.PlayerDataToUserInventoryData(response.Data), response.Case, response.Message);
                }
                else
                {
                    return new Response<UserInventoryData>(response.Case, response.Message);
                }
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<UserInventoryData>()
                {
                    Case = 200,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<UserInventoryData>()
                {
                    Case = 201,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (RequirementExecption ex)
            {
                return new Response<UserInventoryData>()
                {
                    Case = 202,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (Exception)
            {
                return new Response<UserInventoryData>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError()
                };
            }
        }

        /*        public async Task<Response<UserInventoryData>> RemoveItem(int playerId, InventoryItemType itemType, int count)
                {
                    try
                    {
                        int valueId = CacheInventoryDataManager.GetFullInventoryItemData(itemType).Id;
                        var existingData = await manager.GetPlayerData(playerId, DataType.Inventory, valueId);
        / *                if (existingData.IsSuccess && existingData.HasData)
                        {
                            var invData = PlayerDataToUserInventoryData(existingData.Data);
                            return await UpdateItem(playerId, type, invData.Value - count);
                        }
                        else return await UpdateItem(playerId, type, count);* /
                        return null;
                    }
                    catch (InvalidModelExecption ex)
                    {
                        return new Response<UserInventoryData>()
                        {
                            Case = 200,
                            Message = ErrorManager.ShowError(ex)
                        };
                    }
                    catch (DataNotExistExecption ex)
                    {
                        return new Response<UserInventoryData>()
                        {
                            Case = 201,
                            Message = ErrorManager.ShowError(ex)
                        };
                    }
                    catch (RequirementExecption ex)
                    {
                        return new Response<UserInventoryData>()
                        {
                            Case = 202,
                            Message = ErrorManager.ShowError(ex)
                        };
                    }
                    catch (Exception)
                    {
                        return new Response<UserInventoryData>()
                        {
                            Case = 0,
                            Message = ErrorManager.ShowError()
                        };
                    }
                }*/

        /*        public async Task<Response<UserInventoryData>> BuyItem(int playerId, InventoryItemType itemType, int count)
                {
                    try
                    {
                        var item = CacheInventoryDataManager.GetFullInventoryItemData(type);
                        if (item == null) throw new DataNotExistExecption("Item does not exist");
                        var playerData = await GetPlayerData(playerId);
        //                var hasReq = HasRequirements(item.Requirements, playerData?.Data?.Resources, count);
        //                if (!hasReq) throw new DataNotExistExecption("Not enought resource");
        //                var removed = await userResourceManager.RemoveResourceByRequirement(playerId, item.Requirements, count);
        //                if (!removed) throw new DataNotExistExecption("Not enought resource");
                        var response = await AddItem(playerId, type, count);

                        if (!response.IsSuccess && !response.HasData)
                        {
        //                    await userResourceManager.RefundResourceByRequirement(playerId, item.Requirements, count);
                            throw new RequirementExecption("Not enought resource");
                        }

                        return response;
                    }
                    catch (InvalidModelExecption ex)
                    {
                        return new Response<UserInventoryData>()
                        {
                            Case = 200,
                            Message = ErrorManager.ShowError(ex)
                        };
                    }
                    catch (DataNotExistExecption ex)
                    {
                        return new Response<UserInventoryData>()
                        {
                            Case = 201,
                            Message = ErrorManager.ShowError(ex)
                        };
                    }
                    catch (RequirementExecption ex)
                    {
                        return new Response<UserInventoryData>()
                        {
                            Case = 202,
                            Message = ErrorManager.ShowError(ex)
                        };
                    }
                    catch (Exception)
                    {
                        return new Response<UserInventoryData>()
                        {
                            Case = 0,
                            Message = ErrorManager.ShowError()
                        };
                    }
                }*/

        public async Task<Response<UserInventoryData>> UseItem(int playerId, InventoryItemType itemType, int count)
        {
            try
            {
                var item = CacheInventoryDataManager.GetFullInventoryItemData(itemType);
                if (item == null) throw new DataNotExistExecption("Item does not exist");

                var existingItem = await manager.GetPlayerData(playerId, DataType.Inventory, item.Id);
                if (!existingItem.IsSuccess && !existingItem.HasData) throw new RequirementExecption(existingItem.Message);

                var data = PlayerData.PlayerDataToUserInventoryData(existingItem.Data);
                if (data.Value < count) throw new RequirementExecption("Not enought items");

                var response = await RemoveItem(playerId, itemType, count);

                if (!response.IsSuccess && !response.HasData)
                {
                    //                    await userResourceManager.RefundResourceByRequirement(playerId, item.Requirements, count);
                    throw new RequirementExecption("Not enought resource");
                }
                else
                {
                    //                    var boostResp = await userActiveBoostManager.AddBoost(playerId, type, count);
                    //                    if (!boostResp.IsSuccess && !boostResp.HasData)
                    {
                        //                        await userResourceManager.RefundResourceByRequirement(playerId, item.Requirements, count);
                        //                        throw new RequirementExecption(boostResp.Message);
                        throw new RequirementExecption("Not supported");
                    }

                    //                    return new Response<UserInventoryData>(response.Data, boostResp.Case, boostResp.Message);
                }
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<UserInventoryData>()
                {
                    Case = 200,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<UserInventoryData>()
                {
                    Case = 201,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (RequirementExecption ex)
            {
                return new Response<UserInventoryData>()
                {
                    Case = 202,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (Exception)
            {
                return new Response<UserInventoryData>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError()
                };
            }
        }
        #endregion

        public async Task<Response<List<InventoryUserDataTable>>> GetAllUserInventory(int playerId)
        {
            try
            {
                if (playerId <= 0) throw new InvalidModelExecption("Invalid player id");

                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId }
                };

                var result = new List<InventoryUserDataTable>();

                var inventoryResp = await Db.ExecuteSPMultipleRow<InventoryUserDataTable>("GetPlayerAllInventory", spParams);
                if (!inventoryResp.IsSuccess || !inventoryResp.HasData)
                    throw new DataNotExistExecption("Error when collecting player inventory data: " + inventoryResp.Message);

                return new Response<List<InventoryUserDataTable>>()
                {
                    Case = 100,
                    Data = inventoryResp.Data,
                    Message = "Data fetched successfully"
                };
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<InventoryUserDataTable>>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<InventoryUserDataTable>>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response> AddNewInventory(int playerId, int inventoryId)
        {
            var spParams = new Dictionary<string, object>()
            {
                { "PlayerId", playerId },
                { "InventoryId", inventoryId }
            };

            try
            {
                return await Db.ExecuteSPNoData("AddInventoryUserData", spParams);
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response> UpdateInventoryOrder(int playerId, int itemId, int order)
        {
            var spParams = new Dictionary<string, object>()
            {
                { "PlayerId", playerId },
                { "ItemId", itemId },
                { "Order", order }
            };

            try
            {
                return await Db.ExecuteSPNoData("UpdateUserInventoryOrder", spParams);
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<InventoryUserDataTable>> UpgradeInventory(int playerId, int inventoryId, int level, DateTime startTime)
        {
            var inventoryData = CacheInventoryDataManager.AllInventoryData.FirstOrDefault(e => e.InventoryId == inventoryId && e.InventoryLevel == level);
            if (inventoryData == null)
            {
                return new Response<InventoryUserDataTable>()
                {
                    Case = 0,
                    Data = null,
                    Message = "Error on fetching inventory data"
                };
            }

            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId },
                    { "InventoryId", inventoryId },
                    { "Level", level + 1 },
                    { "StartTime", startTime },
                    { "Duration", inventoryData.TimeToUpgrade }
                };

                foreach (var req in inventoryData.RequirementValues)
                {
                    if (req.Type == RawResourceType.Steel) await userResourceManager.SumSteelResource(playerId, -req.Value);
                    else if (req.Type == RawResourceType.Stone) await userResourceManager.SumStoneResource(playerId, -req.Value);
                    else if (req.Type == RawResourceType.Ruby) await userResourceManager.SumRubyResource(playerId, -req.Value);
                }

                return await Db.ExecuteSPSingleRow<InventoryUserDataTable>("UpgradeUserInventory", spParams);
            }
            catch (Exception ex)
            {
                return new Response<InventoryUserDataTable>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }

            // inventoryData.RequirementValues
            /*var spParams = new Dictionary<string, object>()
            {
                { "PlayerId", playerId },
                { "InventoryId", inventoryId },
                { "Order", order }
            };

            try
            {
                return await Db.ExecuteSPNoData("UpdateUserInventoryOrder", spParams);
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
            }*/
        }
    }
}

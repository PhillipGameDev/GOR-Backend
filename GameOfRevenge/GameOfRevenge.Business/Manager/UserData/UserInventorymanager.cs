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
                    return new Response<UserInventoryData>(PlayerDataToUserInventoryData(response.Data), response.Case, response.Message);
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

        public async Task<Response<UserInventoryDataUpdated>> IncrementItem(int playerId, InventoryItemType itemType, long playerDataId, int value)
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
                    response = await manager.IncrementPlayerData(playerId, playerDataId, value);
                }

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
                    return new Response<UserInventoryData>(PlayerDataToUserInventoryData(response.Data), response.Case, response.Message);
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

                var data = PlayerDataToUserInventoryData(existingItem.Data);
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
    }
}

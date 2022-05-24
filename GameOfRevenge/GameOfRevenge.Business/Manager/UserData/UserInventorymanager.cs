using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Interface.UserData;
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
        private readonly IUserActiveBuffsManager userActiveBuffsManager = new UserActiveBuffsManager();

        public async Task<Response<UserInventoryData>> UpdateItem(int playerId, InventoryItemType type, int newValue)
        {
            try
            {
                int valueId = CacheInventoryDataManager.GetFullInventoryItemData(type).Info.Id;
                var respone = await manager.AddOrUpdatePlayerData(playerId, DataType.Inventory, valueId, newValue.ToString());
                if (respone.IsSuccess) return new Response<UserInventoryData>(PlayerDataToUserInventoryData(respone.Data), respone.Case, respone.Message);
                else return new Response<UserInventoryData>(respone.Case, respone.Message);
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

        public async Task<Response<UserInventoryData>> AddItem(int playerId, InventoryItemType type, int count)
        {
            try
            {
                int valueId = CacheInventoryDataManager.GetFullInventoryItemData(type).Info.Id;
                var existingData = await manager.GetPlayerData(playerId, DataType.Inventory, valueId);
                if (existingData.IsSuccess && existingData.HasData)
                {
                    var invData = PlayerDataToUserInventoryData(existingData.Data);
                    return await UpdateItem(playerId, type, invData.Value + count);
                }
                else return await UpdateItem(playerId, type, count);
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

        public async Task<Response<UserInventoryData>> RemoveItem(int playerId, InventoryItemType type, int count)
        {
            try
            {
                int valueId = CacheInventoryDataManager.GetFullInventoryItemData(type).Info.Id;
                var existingData = await manager.GetPlayerData(playerId, DataType.Inventory, valueId);
                if (existingData.IsSuccess && existingData.HasData)
                {
                    var invData = PlayerDataToUserInventoryData(existingData.Data);
                    return await UpdateItem(playerId, type, invData.Value - count);
                }
                else return await UpdateItem(playerId, type, count);
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

        public async Task<Response<UserInventoryData>> BuyItem(int playerId, InventoryItemType type, int count)
        {
            try
            {
                var item = CacheInventoryDataManager.GetFullInventoryItemData(type);
                if (item == null) throw new DataNotExistExecption("Item does not exist");
                var playerData = await GetPlayerData(playerId);
                var hasReq = HasRequirements(item.Requirements, playerData?.Data?.Resources, count);
                if (!hasReq) throw new DataNotExistExecption("Not enought resource");
                var removed = await userResourceManager.RemoveResourceByRequirement(playerId, item.Requirements, count);
                if (!removed) throw new DataNotExistExecption("Not enought resource");
                var response = await AddItem(playerId, type, count);

                if (!response.IsSuccess && !response.HasData)
                {
                    await userResourceManager.RefundResourceByRequirement(playerId, item.Requirements, count);
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
        }

        public async Task<Response<UserInventoryData>> UseItem(int playerId, InventoryItemType type, int count)
        {
            try
            {
                var item = CacheInventoryDataManager.GetFullInventoryItemData(type);
                if (item == null) throw new DataNotExistExecption("Item does not exist");
                var existingItem = await manager.GetPlayerData(playerId, DataType.Inventory, item.Info.Id);
                if (!existingItem.IsSuccess && !existingItem.HasData) throw new RequirementExecption(existingItem.Message);
                var data = PlayerDataToUserInventoryData(existingItem.Data);
                if (data.Value < count) throw new RequirementExecption("Not enought items");
                var response = await RemoveItem(playerId, type, count);

                if (!response.IsSuccess && !response.HasData)
                {
                    await userResourceManager.RefundResourceByRequirement(playerId, item.Requirements, count);
                    throw new RequirementExecption("Not enought resource");
                }
                else
                {
                    var buggResp = await userActiveBuffsManager.AddBuff(playerId, type, count);
                    if (!buggResp.IsSuccess && !buggResp.HasData)
                    {
                        await userResourceManager.RefundResourceByRequirement(playerId, item.Requirements, count);
                        throw new RequirementExecption(buggResp.Message);
                    }

                    return new Response<UserInventoryData>(response.Data, buggResp.Case, buggResp.Message);
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

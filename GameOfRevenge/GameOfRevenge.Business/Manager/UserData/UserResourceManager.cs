using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Common.Models;

namespace GameOfRevenge.Business.Manager.UserData
{
    public class UserResourceManager : BaseUserDataManager, IUserResourceManager
    {
        private int GetResourceId(ResourceType type)
        {
            var info = CacheResourceDataManager.GetResourceData(type);
            if (info == null) throw new InvalidModelExecption("Unexpected error occured");
            else return info.Id;
        }

        private async Task<float> GetPlayerDataValue(int playerId, int resId)
        {
            var response = await manager.GetPlayerData(playerId, DataType.Resource, resId);
            if (response.IsSuccess && response.HasData && response.Data != null)
            {
                float.TryParse(response.Data.Value, out float retResp);
                return retResp;
            }

            return 0;
        }
        private void Validate(int playerId, int valueId)
        {
            if (playerId <= 0) throw new InvalidModelExecption("Invalid player id");
            if (valueId <= 0) throw new InvalidModelExecption("Invalid value id");
        }

        public async Task<Response<List<UserResourceData>>> AddMainResource(int playerId, float food, float wood, float ore, float gems)
        {
            var foodData = await AddFoodResource(playerId, food);
            var woodData = await AddWoodResource(playerId, wood);
            var oreData = await AddOreResource(playerId, ore);
            var gemData = await AddGemsResource(playerId, gems);

            return new Response<List<UserResourceData>>()
            {
                Case = 100,
                Message = "Update Main Resource",
                Data = new List<UserResourceData>() { foodData.Data, woodData.Data, oreData.Data, gemData.Data }
            };
        }
        public async Task<Response<UserResourceData>> AddResource(int playerId, int resId, float value)
        {
            Validate(playerId, resId);
            var existingValue = await GetPlayerDataValue(playerId, resId);
            return await UpdateResource(playerId, resId, existingValue + value);
        }
        public async Task<Response<UserResourceData>> AddResource(int playerId, ResourceType type, float value) => await AddResource(playerId, GetResourceId(type), value);
        public async Task<Response<UserResourceData>> AddFoodResource(int playerId, float value) => await AddResource(playerId, CacheResourceDataManager.Food.Id, value);
        public async Task<Response<UserResourceData>> AddWoodResource(int playerId, float value) => await AddResource(playerId, CacheResourceDataManager.Wood.Id, value);
        public async Task<Response<UserResourceData>> AddOreResource(int playerId, float value) => await AddResource(playerId, CacheResourceDataManager.Ore.Id, value);
        public async Task<Response<UserResourceData>> AddGemsResource(int playerId, float value) => await AddResource(playerId, CacheResourceDataManager.Gems.Id, value);

        public async Task<Response<List<UserResourceData>>> GetMainResource(int playerId)
        {
            var foodData = await GetFoodResource(playerId);
            var woodData = await GetWoodResource(playerId);
            var oreData = await GetOreResource(playerId);
            var gemData = await GetGemsResource(playerId);

            return new Response<List<UserResourceData>>()
            {
                Case = 100,
                Message = "Player Main Resource",
                Data = new List<UserResourceData>() { foodData.Data, woodData.Data, oreData.Data, gemData.Data }
            };
        }
        public async Task<Response<UserResourceData>> GetResource(int playerId, int resId)
        {
            try
            {
                Validate(playerId, resId);
                var response = await manager.GetPlayerData(playerId, DataType.Resource, resId);

                if (response.IsSuccess && response.HasData && response.Data != null)
                {
                    return new Response<UserResourceData>()
                    {
                        Case = response.Case,
                        Data = PlayerDataToUserResourceData(response.Data),
                        Message = response.Message
                    };
                }

                return new Response<UserResourceData>()
                {
                    Case = response.Case,
                    Data = null,
                    Message = response.Message
                };
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<UserResourceData>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<UserResourceData>()
                {
                    Case = 1,
                    Data = null,
                    Message = ex.Message
                };
            }

        }
        public async Task<Response<UserResourceData>> GetResource(int playerId, ResourceType type) => await GetResource(playerId, GetResourceId(type));
        public async Task<Response<UserResourceData>> GetFoodResource(int playerId) => await GetResource(playerId, CacheResourceDataManager.Food.Id);
        public async Task<Response<UserResourceData>> GetGemsResource(int playerId) => await GetResource(playerId, CacheResourceDataManager.Gems.Id);
        public async Task<Response<UserResourceData>> GetOreResource(int playerId) => await GetResource(playerId, CacheResourceDataManager.Ore.Id);
        public async Task<Response<UserResourceData>> GetWoodResource(int playerId) => await GetResource(playerId, CacheResourceDataManager.Wood.Id);

        public async Task<Response<List<UserResourceData>>> RemoveMainResource(int playerId, float food, float wood, float ore, float gems)
        {
            var foodData = await RemoveFoodResource(playerId, food);
            var woodData = await RemoveWoodResource(playerId, wood);
            var oreData = await RemoveOreResource(playerId, ore);
            var gemData = await RemoveGemsResource(playerId, gems);

            return new Response<List<UserResourceData>>()
            {
                Case = 100,
                Message = "Update Main Resource",
                Data = new List<UserResourceData>() { foodData.Data, woodData.Data, oreData.Data, gemData.Data }
            };
        }
        public async Task<Response<UserResourceData>> RemoveResource(int playerId, int resId, float value)
        {
            Validate(playerId, resId);
            var existingValue = await GetPlayerDataValue(playerId, resId);
            return await UpdateResource(playerId, resId, existingValue - value);
        }
        public async Task<Response<UserResourceData>> RemoveResource(int playerId, ResourceType type, float value) => await RemoveResource(playerId, GetResourceId(type), value);
        public async Task<Response<UserResourceData>> RemoveFoodResource(int playerId, float value) => await RemoveResource(playerId, CacheResourceDataManager.Food.Id, value);
        public async Task<Response<UserResourceData>> RemoveWoodResource(int playerId, float value) => await RemoveResource(playerId, CacheResourceDataManager.Wood.Id, value);
        public async Task<Response<UserResourceData>> RemoveOreResource(int playerId, float value) => await RemoveResource(playerId, CacheResourceDataManager.Ore.Id, value);
        public async Task<Response<UserResourceData>> RemoveGemsResource(int playerId, float value) => await RemoveResource(playerId, CacheResourceDataManager.Gems.Id, value);


        public async Task<Response<List<UserResourceData>>> UpdateMainResource(int playerId, float food, float wood, float ore, float gems)
        {
            var foodData = await UpdateFoodResource(playerId, food);
            var woodData = await UpdateWoodResource(playerId, wood);
            var oreData = await UpdateOreResource(playerId, ore);
            var gemData = await UpdateGemsResource(playerId, gems);

            return new Response<List<UserResourceData>>()
            {
                Case = 100,
                Message = "Update Main Resource",
                Data = new List<UserResourceData>() { foodData.Data, woodData.Data, oreData.Data, gemData.Data }
            };
        }

        public async Task<Response<List<StoredDataTable>>> GetAllPlayerStoredData(int playerId, int structureLocationId = -1)
        {
            return await manager.GetAllPlayerStoredData(playerId, structureLocationId);
/*            if (response.IsSuccess)
            {
                return new Response(<StoredDataTable>()
                {
                    Case = response.Case,
                    Data = response.Data,
                    Message = response.Message
                };
            }
            else
            {
                return new Response()//<UserResourceData>()
                {
                    Case = response.Case,
                    //                    Data = null,
                    Message = response.Message
                };
            }*/
        }

        public async Task<Response> StoreResource(int playerId, int structureLocationId, int valueId, int value)
        {
            return await manager.StoreResource(playerId, structureLocationId, valueId, value);
/*            var response = await manager.StoreResource(playerId, structureLocationId, resId, value);
            if (response.IsSuccess)
            {
                return new Response()//<UserResourceData>()
                {
                    Case = response.Case,
//                    Data = PlayerDataToUserResourceData(response.Data),
                    Message = response.Message
                };
            }
            else
            {
                return new Response()//<UserResourceData>()
                {
                    Case = response.Case,
//                    Data = null,
                    Message = response.Message
                };
            }*/
        }

/*        public async Task<Response> StoreResource(int playerId, int structureLocationId, int resId, int value)
        {
            //            var response = await manager.TransferResource(playerId, structureLocationId, DataType.Resource, resId, value);

            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId },
                    { "StructureLocationId", structureLocationId },
                    { "DataTypeId", DataType.Resource },
                    { "ObjectId", resId },
                    { "Value", value }
                };

                return await Db.ExecuteSPNoData("AddOrUpdatePlayerStoredData", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response()
                {
                    Case = 200,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }*/

        public async Task<Response<UserResourceData>> UpdateResource(int playerId, int resId, float value)
        {
            Validate(playerId, resId);
            var response = await manager.AddOrUpdatePlayerData(playerId, DataType.Resource, resId, value.ToString());

            if (response.IsSuccess && response.HasData && response.Data != null)
            {
                return new Response<UserResourceData>()
                {
                    Case = response.Case,
                    Data = PlayerDataToUserResourceData(response.Data),
                    Message = response.Message
                };
            }

            return new Response<UserResourceData>()
            {
                Case = response.Case,
                Data = null,
                Message = response.Message
            };
        }
        public async Task<Response<UserResourceData>> UpdateResource(int playerId, ResourceType type, float value) => await UpdateResource(playerId, GetResourceId(type), value);
        public async Task<Response<UserResourceData>> UpdateFoodResource(int playerId, float value) => await UpdateResource(playerId, CacheResourceDataManager.Food.Id, value);
        public async Task<Response<UserResourceData>> UpdateGemsResource(int playerId, float value) => await UpdateResource(playerId, CacheResourceDataManager.Gems.Id, value);
        public async Task<Response<UserResourceData>> UpdateOreResource(int playerId, float value) => await UpdateResource(playerId, CacheResourceDataManager.Ore.Id, value);
        public async Task<Response<UserResourceData>> UpdateWoodResource(int playerId, float value) => await UpdateResource(playerId, CacheResourceDataManager.Wood.Id, value);

        public async Task<bool> RemoveResourceByRequirement(int playerId, IReadOnlyList<IReadOnlyDataRequirement> requirements, int count)
        {
            if (count <= 0) return true;

            try
            {
                foreach (var req in requirements)
                {
                    if (req.DataType == DataType.Resource)
                    {
                        Response<UserResourceData> response;
                        ResourceType resourceType = CacheResourceDataManager.GetResourceData(req.ValueId).Code;
                        if (resourceType == ResourceType.Food) response = await RemoveFoodResource(playerId, req.Value * count);
                        else if (resourceType == ResourceType.Wood) response = await RemoveWoodResource(playerId, req.Value * count);
                        else if (resourceType == ResourceType.Ore) response = await RemoveOreResource(playerId, req.Value * count);
                        else response = await RemoveGemsResource(playerId, req.Value);
                        if (!response.IsSuccess) return false;
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> RefundResourceByRequirement(int playerId, IReadOnlyList<IReadOnlyDataRequirement> requirements, int count)
        {
            if (count <= 0) return true;

            try
            {
                foreach (var req in requirements)
                {
                    if (req.DataType == DataType.Resource)
                    {
                        Response<UserResourceData> response;
                        ResourceType resourceType = CacheResourceDataManager.GetResourceData(req.ValueId).Code;
                        if (resourceType == ResourceType.Food) response = await AddFoodResource(playerId, req.Value * count);
                        else if (resourceType == ResourceType.Wood) response = await AddWoodResource(playerId, req.Value * count);
                        else if (resourceType == ResourceType.Ore) response = await AddOreResource(playerId, req.Value * count);
                        else response = await AddGemsResource(playerId, req.Value);
                        if (!response.IsSuccess) return false;
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> RemoveResourceByRequirement(int playerId, IReadOnlyList<IReadOnlyDataRequirement> requirements) => await RemoveResourceByRequirement(playerId, requirements, 1);
        public async Task<bool> RefundResourceByRequirement(int playerId, IReadOnlyList<IReadOnlyDataRequirement> requirements) => await RefundResourceByRequirement(playerId, requirements, 1);

        public async Task<bool> RemoveResourceByRequirement(int playerId, IReadOnlyDataRequirement gems, int count) => await RemoveResourceByRequirement(playerId, new List<IReadOnlyDataRequirement>() { gems }, 1);
        public async Task<bool> RefundResourceByRequirement(int playerId, IReadOnlyDataRequirement gems, int count) => await RefundResourceByRequirement(playerId, new List<IReadOnlyDataRequirement>() { gems }, 1);
    }
}

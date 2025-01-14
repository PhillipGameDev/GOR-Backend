﻿using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common.Interface.Model;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Common.Interface.UserData
{
    public interface IUserResourceManager : IBaseUserManager
    {
        Task<Response<UserResourceData>> GetResource(int playerId, ResourceType type);
        Task<Response<UserResourceData>> GetResource(int playerId, int resId);
        Task<Response<List<ResourceData>>> GetResources(int playerId);
        Task<Response<List<UserResourceData>>> GetMainResource(int playerId);
        Task<Response<UserResourceData>> GetFoodResource(int playerId);
        Task<Response<UserResourceData>> GetWoodResource(int playerId);
        Task<Response<UserResourceData>> GetOreResource(int playerId);
        Task<Response<UserResourceData>> GetGemsResource(int playerId);
        Task<Response<UserResourceData>> GetGoldResource(int playerId);

        Task<Response<UserResourceData>> UpdateResource(int playerId, ResourceType type, long value);
        Task<Response<UserResourceData>> UpdateResource(int playerId, int resId, long value);
        Task<Response<List<UserResourceData>>> UpdateMainResource(int playerId, long food, long wood, long ore, long gems, long gold);
        Task<Response<UserResourceData>> UpdateFoodResource(int playerId, long value);
        Task<Response<UserResourceData>> UpdateWoodResource(int playerId, long value);
        Task<Response<UserResourceData>> UpdateOreResource(int playerId, long value);
        Task<Response<UserResourceData>> UpdateGemsResource(int playerId, long value);
        Task<Response<UserResourceData>> UpdateGoldResource(int playerId, long value);

        Task<Response<UserResourceData>> SumResource(int playerId, ResourceType type, int value);
        Task<Response<UserResourceData>> SumResource(int playerId, int resId, int value);
        Task<Response<List<UserResourceData>>> SumMainResource(int playerId, int food, int wood, int ore, int gems, int gold);
        Task<Response<UserResourceData>> SumGemsResource(int playerId, int value);
        Task<Response<UserResourceData>> SumFoodResource(int playerId, int value);
        Task<Response<UserResourceData>> SumWoodResource(int playerId, int value);
        Task<Response<UserResourceData>> SumOreResource(int playerId, int value);
        Task<Response<UserResourceData>> SumGoldResource(int playerId, int value);

        Task<Response<List<UserResourceData>>> SumRawResource(int playerId, int steel, int stone, int ruby);
        Task<Response<UserResourceData>> SumSteelResource(int playerId, int value);
        Task<Response<UserResourceData>> SumStoneResource(int playerId, int value);
        Task<Response<UserResourceData>> SumRubyResource(int playerId, int value);

        Task<bool> RemoveResourceByRequirement(int playerId, IReadOnlyList<IReadOnlyDataRequirement> requirements);
        Task<bool> RefundResourceByRequirement(int playerId, IReadOnlyList<IReadOnlyDataRequirement> requirements);
        Task<bool> RemoveResourceByRequirement(int playerId, IReadOnlyList<IReadOnlyDataRequirement> requirements, int count);
        Task<bool> SumResourceByRequirement(int playerId, IReadOnlyList<IReadOnlyDataRequirement> requirements, int count);
        Task<bool> RemoveResourceByRequirement(int playerId, IReadOnlyDataRequirement gems, int count);
        Task<bool> RefundResourceByRequirement(int playerId, IReadOnlyDataRequirement gems, int count);


        Task<Response<List<ResourceData>>> GetAllPlayerStoredResource(int playerId, int? locationId = null);
        Task<Response<StoredResourceData>> SetStoredResource(int playerId, int locationId, ResourceType resourceType, int value);
    }
}

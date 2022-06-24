using System.Collections.Generic;
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
        Task<Response<List<UserResourceData>>> GetMainResource(int playerId);
        Task<Response<UserResourceData>> GetGemsResource(int playerId);
        Task<Response<UserResourceData>> GetFoodResource(int playerId);
        Task<Response<UserResourceData>> GetWoodResource(int playerId);
        Task<Response<UserResourceData>> GetOreResource(int playerId);

        Task<Response<UserResourceData>> UpdateResource(int playerId, ResourceType type, float value);
        Task<Response<UserResourceData>> UpdateResource(int playerId, int resId, float value);
        Task<Response<List<UserResourceData>>> UpdateMainResource(int playerId, float food, float wood, float ore, float gems);
        Task<Response<UserResourceData>> UpdateGemsResource(int playerId, float value);
        Task<Response<UserResourceData>> UpdateFoodResource(int playerId, float value);
        Task<Response<UserResourceData>> UpdateWoodResource(int playerId, float value);
        Task<Response<UserResourceData>> UpdateOreResource(int playerId, float value);

        Task<Response<UserResourceData>> AddResource(int playerId, ResourceType type, float value);
        Task<Response<UserResourceData>> AddResource(int playerId, int resId, float value);
        Task<Response<List<UserResourceData>>> AddMainResource(int playerId, float food, float wood, float ore, float gems);
        Task<Response<UserResourceData>> AddGemsResource(int playerId, float value);
        Task<Response<UserResourceData>> AddFoodResource(int playerId, float value);
        Task<Response<UserResourceData>> AddWoodResource(int playerId, float value);
        Task<Response<UserResourceData>> AddOreResource(int playerId, float value);

        Task<Response<UserResourceData>> RemoveResource(int playerId, ResourceType type, float value);
        Task<Response<UserResourceData>> RemoveResource(int playerId, int resId, float value);
        Task<Response<List<UserResourceData>>> RemoveMainResource(int playerId, float food, float wood, float ore, float gems);
        Task<Response<UserResourceData>> RemoveGemsResource(int playerId, float value);
        Task<Response<UserResourceData>> RemoveFoodResource(int playerId, float value);
        Task<Response<UserResourceData>> RemoveWoodResource(int playerId, float value);
        Task<Response<UserResourceData>> RemoveOreResource(int playerId, float value);

        Task<bool> RemoveResourceByRequirement(int playerId, IReadOnlyList<IReadOnlyDataRequirement> requirements);
        Task<bool> RefundResourceByRequirement(int playerId, IReadOnlyList<IReadOnlyDataRequirement> requirements);
        Task<bool> RemoveResourceByRequirement(int playerId, IReadOnlyList<IReadOnlyDataRequirement> requirements, int count);
        Task<bool> RefundResourceByRequirement(int playerId, IReadOnlyList<IReadOnlyDataRequirement> requirements, int count);
        Task<bool> RemoveResourceByRequirement(int playerId, IReadOnlyDataRequirement gems, int count);
        Task<bool> RefundResourceByRequirement(int playerId, IReadOnlyDataRequirement gems, int count);


        Task<Response<List<StoredDataTable>>> GetAllPlayerStoredData(int playerId, int structureLocationId);
        Task<Response> StoreResource(int playerId, int structureLocationId, int valueId, int value);
    }
}

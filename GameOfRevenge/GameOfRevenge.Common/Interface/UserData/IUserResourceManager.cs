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

        Task<Response<UserResourceData>> UpdateResource(int playerId, ResourceType type, long value);
        Task<Response<UserResourceData>> UpdateResource(int playerId, int resId, long value);
        Task<Response<List<UserResourceData>>> UpdateMainResource(int playerId, long food, long wood, long ore, long gems);
        Task<Response<UserResourceData>> UpdateGemsResource(int playerId, long value);
        Task<Response<UserResourceData>> UpdateFoodResource(int playerId, long value);
        Task<Response<UserResourceData>> UpdateWoodResource(int playerId, long value);
        Task<Response<UserResourceData>> UpdateOreResource(int playerId, long value);

        Task<Response<UserResourceData>> SumResource(int playerId, ResourceType type, int value);
        Task<Response<UserResourceData>> SumResource(int playerId, int resId, int value);
        Task<Response<List<UserResourceData>>> SumMainResource(int playerId, int food, int wood, int ore, int gems);
        Task<Response<UserResourceData>> SumGemsResource(int playerId, int value);
        Task<Response<UserResourceData>> SumFoodResource(int playerId, int value);
        Task<Response<UserResourceData>> SumWoodResource(int playerId, int value);
        Task<Response<UserResourceData>> SumOreResource(int playerId, int value);

        Task<bool> RemoveResourceByRequirement(int playerId, IReadOnlyList<IReadOnlyDataRequirement> requirements);
        Task<bool> RefundResourceByRequirement(int playerId, IReadOnlyList<IReadOnlyDataRequirement> requirements);
        Task<bool> RemoveResourceByRequirement(int playerId, IReadOnlyList<IReadOnlyDataRequirement> requirements, int count);
        Task<bool> SumResourceByRequirement(int playerId, IReadOnlyList<IReadOnlyDataRequirement> requirements, int count);
        Task<bool> RemoveResourceByRequirement(int playerId, IReadOnlyDataRequirement gems, int count);
        Task<bool> RefundResourceByRequirement(int playerId, IReadOnlyDataRequirement gems, int count);


        Task<Response<List<StoredDataTable>>> GetAllPlayerStoredData(int playerId, int structureLocationId);
        Task<Response> StoreResource(int playerId, int structureLocationId, ResourceType type, int value);
    }
}

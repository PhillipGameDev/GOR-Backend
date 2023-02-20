using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Hero;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Common.Models.Troop;
using GameOfRevenge.Common.Net;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameOfRevenge.Common.Interface.UserData
{
    public interface IUserTroopManager : IBaseUserManager
    {
        Task<Response<UserTroopData>> TrainTroops(int playerId, int id, int level, int count, int location);
        Task<Response<UserTroopData>> TrainTroops(int playerId, TroopType type, int level, int count, int location);
        Task<Response<UserTroopData>> InstantTrainTroops(int playerId, TroopType type, int level, int count);

        Task<Response<UserTroopData>> AddTroops(int playerId, TroopType type, int level, int count);
        Task<Response<UserTroopData>> AddTroops(int playerId, int id, int level, int count);

        Task<Response<UserTroopData>> UpdateTroops(int playerId, TroopType type, List<TroopDetails> troops);
        Task<Response<UserTroopData>> RecoverWounded(int playerId, TroopType type, List<WoundeAndDeadTroopsUpdate> troops);
        Task<Response<UserTroopData>> AddWoundedAndDeadTroops(int playerId, TroopType type, List<WoundeAndDeadTroopsUpdate> troops);

        Task<Response<PopulationData>> GetPopulationData(int playerId);
        Response<PopulationData> GetPopulationData(IReadOnlyList<StructureInfos> structures, IReadOnlyList<TroopInfos> troops);
        Response<int> GetMaxPopulation(IReadOnlyList<StructureInfos> structures);
        Response<int> GetCurrentPopulation(IReadOnlyList<TroopInfos> troops);
        Response<PopulationData> GetPopulationData(PlayerCompleteData compPlayerData);

        //Task<Response> InstantTrainTroops(int playerId, int buildingLoc);
        //Task<Response> InstantTrainTroops(PlayerCompleteData compPlayerData);
        //Task<Response> InstantTrainTroops(IReadOnlyList<StructureInfos> structures, IReadOnlyList<TroopInfos> troops);
    }
}

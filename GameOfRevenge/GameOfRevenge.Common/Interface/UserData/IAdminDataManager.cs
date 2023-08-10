using System.Threading.Tasks;
using System.Collections.Generic;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Common.Interface.UserData
{
    public interface IAdminDataManager : IBaseUserManager
    {
        Task<Response<List<PlayerID>>> GetPlayers();
        Task<Response<PlayerInfo>> GetPlayerInfo(int playerId);
        Task<Response<List<PlayerInfo>>> GetPlayersInfo(int playerId = 0, int length = 10);

        Task<Response<ChartDataTable>> GetDailyVisits();
        Task<Response<ActiveUsersTable>> GetActiveUsers();
        Task<Response> SaveDailyVisits();

        Task<List<DataReward>> GetAvailableRewards();
        Task<Response<List<StorePackageTable>>> GetPackages(bool? active = null);
        Task<List<ProductPackage>> GetAllProductPackages(bool? active = null);
        Task<Response> UpdatePackage(int packageId, int cost, bool active);
        Task<Response> UpdatePackageReward(int packageId, int rewardId, int count);
        Task<Response> IncrementPackageReward(int packageId, int rewardId, int count);
        Task<Response<IntValue>> AddQuestReward(int questId, DataType dataType, int valueId, int value, int count);

        Task<Response<List<PlayerBackupTable>>> GetPlayerBackups(int playerId);
        Task<Response<PlayerBackupTable>> GetPlayerBackup(long backupId);
        Task<Response> SavePlayerBackup(int playerId, string description, string data);
        Task<Response> RestorePlayerBackup(int playerId, long backupId);

        Task<Response> ResetAllDailyQuests();

        Task<AllPlayerData> GetAllFullPlayerData(int playerId);
        Task<List<PlayerDataReward>> GetAllPlayerRewards(int playerId);
        Task<FullPlayerCompleteData> GetFullPlayerData(int playerId, bool allData = true, bool getBackups = false);
    }
}

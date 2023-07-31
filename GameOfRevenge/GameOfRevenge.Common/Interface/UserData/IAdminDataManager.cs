using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Net;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        Task<Response<List<PlayerBackupTable>>> GetPlayerBackups(int playerId);
        Task<Response<PlayerBackupTable>> GetPlayerBackup(long backupId);
        Task<Response> SavePlayerBackup(int playerId, string description, string data);
        Task<Response> RestorePlayerBackup(int playerId, long backupId);

        Task<Response> ResetAllDailyQuests();
    }
}

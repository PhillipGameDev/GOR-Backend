using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Services;

namespace GameOfRevenge.Business.Manager.UserData
{
    public class AdminDataManager : BaseManager, IAdminDataManager
    {
        private static readonly IAccountManager accountManager = new AccountManager();

        public Task<Response<UserVIPDetails>> ActivateVIPBoosts(int playerId) => throw new NotImplementedException();
        public Task<Response<UserVIPDetails>> AddVIPPoints(int playerId, int points) => throw new NotImplementedException();
        public Task<Response<List<PlayerDataTable>>> GetAllPlayerData(int playerId) => throw new NotImplementedException();
        public int GetInstantBuildCost(int timeLeft) => throw new NotImplementedException();
        public Dictionary<int, UserStructureData> GetMultipleBuildings(UserStructureData structure) => throw new NotImplementedException();


        public async Task<Response<List<PlayerID>>> GetPlayers()
        {
            var spParams = new Dictionary<string, object>()
            {
                { "Length", 0 }
            };
            return await Db.ExecuteSPMultipleRow<PlayerID>("GetPlayerIDs", spParams);
        }

        public async Task<Response<PlayerInfo>> GetPlayerInfo(int playerId) => await accountManager.GetAccountInfo(playerId);

        public async Task<Response<List<PlayerInfo>>> GetPlayersInfo(int playerId = 0, int length = 10)
        {
            List<PlayerInfo> list = null;

            var spParams = new Dictionary<string, object>()
            {
                { "PlayerId", playerId },
                { "Length", length }
            };
            var response = await Db.ExecuteSPMultipleRow<PlayerID>("GetPlayerIDs", spParams);
            if (response.IsSuccess && response.HasData)
            {
                list = new List<PlayerInfo>();

                var players = response.Data;
                foreach (var player in players)
                {
                    var plyRes = await accountManager.GetAccountInfo(player.PlayerId);
                    if (plyRes.IsSuccess && plyRes.HasData)
                    {
                        list.Add(plyRes.Data);
                    }
                    else
                    {
                        list.Add(new PlayerInfo() { PlayerId = player.PlayerId });
                    }
                }
            }

            return new Response<List<PlayerInfo>>()
            {
                Case = response.Case,
                Data = list,
                Message = response.Message
            };
        }

        public async Task<Response<ChartDataTable>> GetDailyVisits()
        {
            try
            {
                return await Db.ExecuteSPSingleRow<ChartDataTable>("GetDailyVisits", null);
            }
            catch (Exception ex)
            {
                return new Response<ChartDataTable>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<ActiveUsersTable>> GetActiveUsers()
        {
            try
            {
                return await Db.ExecuteSPSingleRow<ActiveUsersTable>("GetActiveUsers", null);
            }
            catch (Exception ex)
            {
                return new Response<ActiveUsersTable>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response> SaveDailyVisits()
        {
            try
            {
                return await Db.ExecuteSPNoData("SaveDailyVisits", null);
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

        public async Task<Response<List<PlayerBackupTable>>> GetPlayerBackups(int playerId)
        {
            var spParams = new Dictionary<string, object>()
            {
                { "PlayerId", playerId }
            };

            try
            {
                return await Db.ExecuteSPMultipleRow<PlayerBackupTable>("GetPlayerBackups", spParams);
            }
            catch (Exception ex)
            {
                return new Response<List<PlayerBackupTable>>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<PlayerBackupTable>> GetPlayerBackup(long backupId)
        {
            var spParams = new Dictionary<string, object>()
            {
                { "BackupId", backupId }
            };

            try
            {
                return await Db.ExecuteSPSingleRow<PlayerBackupTable>("GetPlayerBackup", spParams);
            }
            catch (Exception ex)
            {
                return new Response<PlayerBackupTable>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response> RestorePlayerBackup(int playerId, long backupId)
        {
            var spParams = new Dictionary<string, object>()
            {
                { "PlayerId", playerId },
                { "BackupId", backupId }
            };

            try
            {
                return await Db.ExecuteSPNoData("RestorePlayerBackup", spParams);
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

        public async Task<Response> SavePlayerBackup(int playerId, string description, string data)
        {
            var spParams = new Dictionary<string, object>()
            {
                { "PlayerId", playerId },
                { "Description", description },
                { "Data", data }
            };

            try
            {
                return await Db.ExecuteSPNoData("SavePlayerBackup", spParams);
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


        public async Task<Response> ResetAllDailyQuests()
        {
            try
            {
                return await Db.ExecuteSPNoData("ResetAllDailyQuests", null);
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

        public Task<Response<RankingElement>> GetRanking(int playerId)
        {
            throw new NotImplementedException();
        }

        public Task<Response<List<RankingElement>>> GetRankings(long rankId = 0)
        {
            throw new NotImplementedException();
        }

        public bool HasActiveBoostRequirements(IReadOnlyList<IReadOnlyDataRequirement> requirements, List<UserRecordNewBoost> boosts)
        {
            throw new NotImplementedException();
        }

        public bool HasRequirements(IReadOnlyList<IReadOnlyDataRequirement> requirements, PlayerCompleteData playerData)
        {
            throw new NotImplementedException();
        }

        public bool HasRequirements(IReadOnlyList<IReadOnlyDataRequirement> requirements, PlayerCompleteData playerData, int count)
        {
            throw new NotImplementedException();
        }

        public bool HasResourceRequirements(IReadOnlyList<IReadOnlyDataRequirement> requirements, ResourcesList resourcess, int count)
        {
            throw new NotImplementedException();
        }

        public bool HasStructureRequirements(IReadOnlyList<IReadOnlyDataRequirement> requirements, List<StructureInfos> structures)
        {
            throw new NotImplementedException();
        }
    }
}

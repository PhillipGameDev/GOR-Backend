using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Services;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Business.Manager.GameDef;

namespace GameOfRevenge.Business.Manager.UserData
{
    public class AdminDataManager : BaseManager, IAdminDataManager
    {
        private readonly IAccountManager accountManager = new AccountManager();
        private readonly IClanManager userClanManager;
        private readonly IUserQuestManager userQuestManager;
        private readonly IUserItemManager userItemManager;
        private readonly IPlayerDataManager playerDataManager;

        public AdminDataManager(IPlayerDataManager playerManager, IClanManager clanManager, IUserQuestManager questManager, IUserItemManager userItemManager)
        {
            userClanManager = clanManager;
            userQuestManager = questManager;
            playerDataManager = playerManager;
            this.userItemManager = userItemManager;

        }

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

        public async Task<List<DataReward>> GetAvailableRewards()
        {
            var qm = new QuestManager();
            var resp = await qm.GetAllQuestRewards();
            if (resp.IsSuccess && resp.HasData)
            {
                return resp.Data;
            }

            return null;
        }

/*        public async Task<List<ProductPackage>> GetAllPackages()
        {
            var list = new List<ProductPackage>();
            var packagesResp = await GetStorePackages();
            if (packagesResp.IsSuccess)
            {
                foreach (var package in packagesResp.Data)
                {
                    var product = new ProductPackage()
                    {

                    };
                    list.Add(product);
                }
            }

            return list;
        }*/

        public async Task<Response<List<StorePackageTable>>> GetPackages(bool? active = null)
        {
            var sdParams = new Dictionary<string, object>();
            if (active != null) sdParams.Add("Active", active);

            try
            {
                return await Db.ExecuteSPMultipleRow<StorePackageTable>("GetStorePackages", sdParams);
            }
            catch (Exception ex)
            {
                return new Response<List<StorePackageTable>>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<List<ProductPackage>> GetAllProductPackages(bool? active = null)
        {
            List<ProductPackage> list = null;
            try
            {
                var packages = await GetPackages(active);
                if (!packages.IsSuccess || !packages.HasData) throw new Exception();

                var rewards = await GetAvailableRewards();
                if (rewards == null) throw new Exception();

                list = new List<ProductPackage>();
                foreach (var package in packages.Data)
                {
                    var product = new ProductPackage()
                    {
                        PackageId = package.PackageId,
                        QuestId = package.QuestId,
                        ProductId = package.ProductId,
                        Cost = package.Cost,
                        Active = package.Active,
                        Rewards = rewards.FindAll(x => (x.QuestId == package.QuestId))
                    };
                    list.Add(product);
                }
                return list;
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        public async Task<Response> UpdatePackage(int packageId, int cost, bool active)
        {
            var spParams = new Dictionary<string, object>()
            {
                { "PackageId", packageId },
                { "Cost", cost },
                { "Active", active? 1 : 0 }
            };

            try
            {
                return await Db.ExecuteSPNoData("UpdatePackage", spParams);
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

        public async Task<Response> UpdatePackageReward(int packageId, int rewardId, int count)
        {
            var spParams = new Dictionary<string, object>()
            {
                { "PackageId", packageId },
                { "RewardId", rewardId },
                { "Count", count }
            };

            try
            {
                return await Db.ExecuteSPNoData("UpdatePackageReward", spParams);
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

        public async Task<Response> IncrementPackageReward(int packageId, int rewardId, int count)
        {
            var spParams = new Dictionary<string, object>()
            {
                { "PackageId", packageId },
                { "RewardId", rewardId },
                { "Count", count }
            };

            try
            {
                return await Db.ExecuteSPNoData("IncrementPackageReward", spParams);
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

        public async Task<Response<IntValue>> AddQuestReward(int questId, DataType dataType, int valueId, int value, int count)
        {
            var spParams = new Dictionary<string, object>()
            {
                { "QuestId", questId },
                { "DataType", dataType.ToString() },
                { "ValueId", valueId },
                { "Value", value },
                { "Count", count }
            };

            try
            {
                return await Db.ExecuteSPSingleRow<IntValue>("AddQuestReward", spParams);
            }
            catch (Exception ex)
            {
                return new Response<IntValue>()
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

        public async Task<AllPlayerData> GetAllFullPlayerData(int playerId)
        {
            try
            {
                var allPlayerData = new AllPlayerData();

                //PLAYER INFO
                var playerInfo = await GetPlayerInfo(playerId);
                if (!playerInfo.IsSuccess || !playerInfo.HasData) throw new Exception();

                allPlayerData.PlayerInfo = playerInfo.Data;

                //ALL PLAYER DATA
                var allData = await playerDataManager.GetAllPlayerData(playerId);
                if (!allData.IsSuccess || !allData.HasData) throw new Exception();

                allPlayerData.PlayerData = allData.Data;

                //QUESTS
                var questsResp = await userQuestManager.GetAllQuestProgress(playerId);
                if (!questsResp.IsSuccess || !questsResp.HasData) throw new Exception();

                allPlayerData.QuestData = questsResp.Data;

                return allPlayerData;
            }
            catch
            {
            }

            return null;
        }

        public async Task<List<PlayerDataReward>> GetAllPlayerRewards(int playerId)
        {
            var rewardsResp = await userItemManager.GetUserAllItems(playerId);
            if (rewardsResp.IsSuccess && rewardsResp.HasData)
            {
                var allPlayerRewards = new List<PlayerDataReward>();
                var playerDataResp = await playerDataManager.GetAllPlayerData(playerId, DataType.Reward);
                if (playerDataResp.IsSuccess && playerDataResp.HasData)
                {
                    foreach (var userReward in rewardsResp.Data)
                    {
                        var reward = new PlayerDataReward()
                        {
                            PlayerDataId = userReward.PlayerDataId,
                            DataType = userReward.DataType,
                            ValueId = userReward.ValueId,
                            Value = userReward.Value,
                            Count = userReward.Count,
                            RewardId = playerDataResp.Data.Find(x => (x.Id == userReward.PlayerDataId)).ValueId
                        };
                        allPlayerRewards.Add(reward);
                    }
                    allPlayerRewards = allPlayerRewards.OrderBy(x => x.DataType).ThenBy(x => x.RewardId).ThenBy(x => x.ValueId).ThenBy(x => x.Value).ToList();
                }

                return allPlayerRewards;
            }

            return null;
        }

        public async Task<FullPlayerCompleteData> GetFullPlayerData(int playerId, bool allData = true, bool getBackups = false)
        {
            FullPlayerCompleteData fullPlayerData = null;

            var resp = await BaseUserDataManager.GetFullPlayerData(playerId);
            if (resp.IsSuccess && resp.HasData)
            {
                fullPlayerData = new FullPlayerCompleteData(resp.Data);

                //CLAN
                var clanResp = userClanManager.GetClanData(fullPlayerData.ClanId);
                if (clanResp.Result.IsSuccess && clanResp.Result.HasData)
                {
                    fullPlayerData.Clan = clanResp.Result.Data;
                }

                if (allData)
                {
                    //REWARDS
                    fullPlayerData.Rewards = await GetAllPlayerRewards(playerId);

                    //QUESTS
                    var questsResp = await userQuestManager.GetAllQuestProgress(playerId);
                    if (questsResp.IsSuccess && questsResp.HasData)
                    {
                        fullPlayerData.Quests = questsResp.Data;
                    }
                }

                //BACKUPS
                if (getBackups)
                {
                    var backupResp = await GetPlayerBackups(playerId);
                    if (backupResp.IsSuccess)
                    {
                        fullPlayerData.Backups = backupResp.Data;
                    }
                }
            }
            else
            {
                Console.WriteLine(resp.Message);
            }

            return fullPlayerData;
        }


        public Task<Response<UserVIPDetails>> ActivateVIPBoosts(int playerId, int d) => throw new NotImplementedException();
        public Task<Response<UserVIPDetails>> AddVIPPoints(int playerId, int points) => throw new NotImplementedException();
        public Task<Response<List<PlayerDataTable>>> GetAllPlayerData(int playerId) => throw new NotImplementedException();
        public int GetInstantBuildCost(int timeLeft) => throw new NotImplementedException();
        public Dictionary<int, UserStructureData> GetMultipleBuildings(UserStructureData structure) => throw new NotImplementedException();
        public Task<Response<RankingElement>> GetRanking(int playerId) => throw new NotImplementedException();
        public Task<Response<List<RankingElement>>> GetRankings(long rankId = 0) => throw new NotImplementedException();
        public bool HasActiveBoostRequirements(IReadOnlyList<IReadOnlyDataRequirement> requirements, List<UserRecordNewBoost> boosts) => throw new NotImplementedException();
        public bool HasRequirements(IReadOnlyList<IReadOnlyDataRequirement> requirements, PlayerCompleteData playerData) => throw new NotImplementedException();
        public bool HasRequirements(IReadOnlyList<IReadOnlyDataRequirement> requirements, PlayerCompleteData playerData, int count) => throw new NotImplementedException();
        public bool HasResourceRequirements(IReadOnlyList<IReadOnlyDataRequirement> requirements, ResourcesList resourcess, int count) => throw new NotImplementedException();
        public bool HasStructureRequirements(IReadOnlyList<IReadOnlyDataRequirement> requirements, List<StructureInfos> structures) => throw new NotImplementedException();
    }
}

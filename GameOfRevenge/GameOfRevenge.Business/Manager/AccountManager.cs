using System;
using System.Threading.Tasks;
using GameOfRevenge.Common.Services;
using GameOfRevenge.Common;
using GameOfRevenge.Business.Manager.UserData;
using System.Collections.Generic;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Structure;
using System.Linq;
using Newtonsoft.Json;
using GameOfRevenge.Common.Models.Quest.Template;
using GameOfRevenge.Common.Models.Quest;

namespace GameOfRevenge.Business.Manager
{
    public class AccountManager : BaseManager, IAccountManager
    {
        private readonly IUserResourceManager resManager = new UserResourceManager();
        private readonly IUserStructureManager strManager = new UserStructureManager();
        private readonly IUserQuestManager questManager = new UserQuestManager();

        public async Task<Response<Player>> TryLoginOrRegister(string identifier, string name, bool accepted, int version)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(identifier))
                {
                    throw new InvalidModelExecption("Invalid identifier was provided");
                }

                identifier = identifier.Trim();
                name = string.IsNullOrWhiteSpace(name) ? "Guest" : name.Trim();

                if (!accepted) throw new InvalidModelExecption("Kindly accept terms and condition");

                var spParams = new Dictionary<string, object>()
                {
                    { "Identifier", identifier },
                    { "Name", name },
                    { "Accepted", accepted },
                    { "Version", version }
                };
                var response = await Db.ExecuteSPSingleRow<Player>("TryLoginOrRegister", spParams);

                if (!response.IsSuccess) throw new InvalidModelExecption(response.Message);
                if (!response.HasData) throw new InvalidModelExecption("Unable to validate your user account.");

                int playerId = response.Data.PlayerId;
                string testx = "";
                if (response.Case == 100)// new account
                {
                    //todo add in database
                    var structureData = CacheData.CacheStructureDataManager.GetFullStructureData(StructureType.CityCounsel);
                    var cityCounselHealth = structureData.Levels.OrderBy(x => x.Data.Level).FirstOrDefault().Data.HitPoint;
                    var cityCounselLoc = structureData.Locations.FirstOrDefault();

                    structureData = CacheData.CacheStructureDataManager.GetFullStructureData(StructureType.Gate);
                    var gateHealth = structureData.Levels.OrderBy(x => x.Data.Level).FirstOrDefault().Data.HitPoint;
                    var gateLoc = structureData.Locations.FirstOrDefault();

                    structureData = CacheData.CacheStructureDataManager.GetFullStructureData(StructureType.WatchTower);
                    var wtHealth = structureData.Levels.OrderBy(x => x.Data.Level).FirstOrDefault().Data.HitPoint;
                    var wtLoc = structureData.Locations.FirstOrDefault();

                    int thHealth = 0;
                    int thLoc = 0;
                    if (version > 0)
                    {
                        structureData = CacheData.CacheStructureDataManager.GetFullStructureData(StructureType.TrainingHeroes);
                        thHealth = structureData.Levels.OrderBy(x => x.Data.Level).FirstOrDefault().Data.HitPoint;
                        thLoc = structureData.Locations.FirstOrDefault();
                    }
#if DEBUG
                    await resManager.SumMainResource(playerId, 100000, 100000, 100000, 10000);
#else
                    await resManager.SumMainResource(playerId, 10000, 10000, 10000, 100);
#endif
                    var dataManager = new PlayerDataManager();
                    var json = JsonConvert.SerializeObject(new UserKingDetails());
                    await dataManager.AddOrUpdatePlayerData(playerId, DataType.Custom, 1, json);

                    json = JsonConvert.SerializeObject(new UserBuilderDetails());
                    await dataManager.AddOrUpdatePlayerData(playerId, DataType.Custom, 2, json);

                    json = JsonConvert.SerializeObject(new UserVIPDetails(100));
                    await dataManager.AddOrUpdatePlayerData(playerId, DataType.Custom, 3, json);

                    var timestamp = DateTime.UtcNow;
                    var dataList = new List<StructureDetails>();
                    dataList.Add(new StructureDetails()
                    {
                        Level = 1,
                        StartTime = timestamp,
                        Location = cityCounselLoc,
                        HitPoints = cityCounselHealth,
                    });
                    json = JsonConvert.SerializeObject(dataList);
                    await dataManager.AddOrUpdatePlayerData(playerId, DataType.Structure, (int)StructureType.CityCounsel, json);

                    dataList[0].Location = gateLoc;
                    dataList[0].HitPoints = gateHealth;
                    json = JsonConvert.SerializeObject(dataList);
                    await dataManager.AddOrUpdatePlayerData(playerId, DataType.Structure, (int)StructureType.Gate, json);

                    dataList[0].Location = wtLoc;
                    dataList[0].HitPoints = wtHealth;
                    json = JsonConvert.SerializeObject(dataList);
                    await dataManager.AddOrUpdatePlayerData(playerId, DataType.Structure, (int)StructureType.WatchTower, json);

                    if (version > 0)
                    {
                        dataList[0].Location = thLoc;
                        dataList[0].HitPoints = thHealth;
                        json = JsonConvert.SerializeObject(dataList);
                        await dataManager.AddOrUpdatePlayerData(playerId, DataType.Structure, (int)StructureType.TrainingHeroes, json);
                    }
                }
                else if ((response.Case == 101) && (version > 0))
                {
                    try
                    {
                        var dataManager = new PlayerDataManager();
                        var resp = await dataManager.GetPlayerData(playerId, DataType.Structure, (int)StructureType.TrainingHeroes);
                        if (resp.IsSuccess)
                        {
                            var found = false;
                            if (resp.HasData)
                            {
                                var structures = JsonConvert.DeserializeObject<List<StructureDetails>>(resp.Data.Value);
                                if (structures != null)
                                {
                                    var bld = structures.FirstOrDefault();
                                    if (bld != null)
                                    {
                                        found = true;
                                        var structureData = CacheData.CacheStructureDataManager.GetFullStructureData(StructureType.TrainingHeroes);
                                        int thLoc = structureData.Locations.FirstOrDefault();

                                        if (bld.Location != thLoc)
                                        {
                                            bld.Location = thLoc;
                                            var json = JsonConvert.SerializeObject(structures);
                                            await dataManager.UpdatePlayerDataID(playerId, resp.Data.Id, json);
                                        }
                                    }
                                }
                            }
                            if (!found)
                            {
                                testx += "nofound ";
                                var structureData = CacheData.CacheStructureDataManager.GetFullStructureData(StructureType.TrainingHeroes);
                                int thHealth = structureData.Levels.OrderBy(x => x.Data.Level).FirstOrDefault().Data.HitPoint;
                                int thLoc = structureData.Locations.FirstOrDefault();

                                var timestamp = DateTime.UtcNow;
                                var dataList = new List<StructureDetails>();
                                dataList.Add(new StructureDetails()
                                {
                                    Level = 1,
                                    Location = thLoc,
                                    StartTime = timestamp,
                                    HitPoints = thHealth,
                                });
                                var json = JsonConvert.SerializeObject(dataList);
                                await dataManager.AddOrUpdatePlayerData(playerId, DataType.Structure, (int)StructureType.TrainingHeroes, json);
                            }
                        }

                        resp = await dataManager.GetPlayerData(playerId, DataType.Custom, 3);
                        if (playerId == 71)
                        {
                            testx += resp.IsSuccess ? "ok" : "ng";
                            testx += resp.HasData ? " si" : " no";
                        }
                        if (resp.IsSuccess && !resp.HasData)
                        {
                            var json = JsonConvert.SerializeObject(new UserVIPDetails(100));
                            var x = await dataManager.AddOrUpdatePlayerData(playerId, DataType.Custom, 3, json);
                            if (playerId == 71)
                            {
                                testx += " " + x.IsSuccess + "  " + x.Case + "  " + x.Message;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        response.Message = ex.Message;
                    }
                }
                await CompleteAccountQuest(playerId, AccountTaskType.SignIn);
                if (playerId == 71)
                {
                    response.Message = response.Case+"  "+version+testx;
                }

                return response;
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<Player>() { Case = 200, Data = null, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new Response<Player>() { Case = 0, Data = null, Message = ErrorManager.ShowError(ex) };
            }
        }

        public async Task<Response<string[]>> ChangeName(int playerId, string name)
        {
            try
            {
                if (playerId <= 0) throw new InvalidModelExecption("Invalid player id was provided");
                if (string.IsNullOrWhiteSpace(name) || (name.Length < 3)) throw new InvalidModelExecption("Invalid name was provided");

                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId },
                    { "Name", name }
                };

                var response = await Db.ExecuteSPSingleRow<Player>("UpdatePlayerProperties", spParams);
                if (response.IsSuccess)
                {
                    var updated = await CompleteAccountQuest(playerId, AccountTaskType.ChangeName);
                    if (updated) response.Case = 101;
                }

                return new Response<string[]>() { Case = response.Case, Data = null, Message = response.Message };
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<string[]>() { Case = 200, Data = null, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new Response<string[]>() { Case = 0, Data = null, Message = ErrorManager.ShowError(ex) };
            }
        }

        public async Task<Response<PlayerInfo>> GetAccountInfo(string identifier)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(identifier)) throw new InvalidModelExecption("Invalid identifier was provided");

                var spParams = new Dictionary<string, object>()
                {
                    { "Identifier", identifier },
                };

                return await Db.ExecuteSPSingleRow<PlayerInfo>("GetPlayerDetailsByIdentifier", spParams);

            }
            catch (InvalidModelExecption ex)
            {
                return new Response<PlayerInfo>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<PlayerInfo>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<PlayerInfo>> GetAccountInfo(int playerId)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId },
                };

                var resp = await Db.ExecuteSPSingleRow<PlayerInfo>("GetPlayerDetailsById", spParams);
                if (!resp.IsSuccess) resp.Data = null;

                return resp;
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<PlayerInfo>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<PlayerInfo>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }


        public async Task<Response<PlayerTutorialData>> GetTutorialInfo(string identifier)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerIdentifier", identifier },
                };

                return await Db.ExecuteSPSingleRow<PlayerTutorialData>("GetPlayerTutorial", spParams);
            }

            catch (InvalidModelExecption ex)
            {
                return new Response<PlayerTutorialData>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<PlayerTutorialData>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<PlayerTutorialData>> UpdateTutorialInfo(string identifier, string playerData, bool isComplete)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerIdentifier", identifier },
                    { "ProgressData", playerData },
                    { "IsComplete", isComplete },
                };
                return await Db.ExecuteSPSingleRow<PlayerTutorialData>("UpdateTutorialInfo", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<PlayerTutorialData>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<PlayerTutorialData>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        private async Task<List<PlayerQuestDataTable>> AllUserQuests(int playerId)
        {
            var response = await questManager.GetAllQuestProgress(playerId);
            if (response.IsSuccess && response.HasData) return response.Data;

            throw new Exception();
        }

        private async Task<bool> CompleteAccountQuest(int playerId, AccountTaskType taskType)
        {
            var quests = CacheData.CacheQuestDataManager.AllQuestRewards
                                .Where(x => (x.Quest.QuestType == QuestType.Account))?.ToList();
            if (quests == null) return false;

            var questUpdated = false;
            List<PlayerQuestDataTable> allUserQuests = null;
            try
            {
                foreach (var quest in quests)
                {
                    QuestAccountData questData = JsonConvert.DeserializeObject<QuestAccountData>(quest.Quest.DataString);
                    if ((questData == null) || (questData.AccountTaskType != taskType)) continue;

                    if (allUserQuests == null) allUserQuests = await AllUserQuests(playerId);
                    var userQuest = allUserQuests.Find(x => (x.QuestId == quest.Quest.QuestId));
                    string initialString = (userQuest == null) ? quest.Quest.DataString : null;
                    if ((userQuest == null) || !userQuest.Completed)
                    {
                        var resp = await questManager.UpdateQuestData(playerId, quest.Quest.QuestId, true, initialString);
                        if (resp.IsSuccess) questUpdated = true;
                    }
                }
            }
            catch { }

            return questUpdated;
        }
    }
}

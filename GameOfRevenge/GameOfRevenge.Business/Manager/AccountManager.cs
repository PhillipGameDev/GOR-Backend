﻿using System;
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

        public async Task<Response<Player>> TryLoginOrRegister(string identifier, bool accepted, int version)
        {
            try
            {
                if (version < 903) throw new DataNotExistExecption("Update Required");
                if (string.IsNullOrWhiteSpace(identifier))
                {
                    throw new InvalidModelExecption("Invalid identifier was provided");
                }

                identifier = identifier.Trim();
                /*                if ((identifier == "21886937eabae07847de33d5ad5bbf20") ||
                                    (identifier == "Bu3gC5191gftKiUYF0QmgSmRHDk2"))
                                {
                                    throw new DataNotExistExecption("Under Maintenance");
                                }
                                if (identifier == "test") identifier = "21886937eabae07847de33d5ad5bbf20";*/

                if (!accepted) throw new RequirementExecption("Kindly accept terms and condition");

                var spParams = new Dictionary<string, object>()
                {
                    { "Identifier", identifier },
                    { "Accepted", accepted },
                    { "Version", version }
                };
                var response = await Db.ExecuteSPSingleRow<Player>("TryLoginOrRegister", spParams);

                if (!response.IsSuccess) throw new InvalidModelExecption(response.Message);
                if (!response.HasData) throw new InvalidModelExecption("Unable to validate your user account.");

                int playerId = response.Data.PlayerId;
                if (response.Case == 100)// new account
                {
                    //todo add in database
                    var structureData = CacheData.CacheStructureDataManager.GetFullStructureData(StructureType.CityCounsel);
                    var cityCounselHealth = structureData.Levels.OrderBy(x => x.Data.Level).FirstOrDefault().Data.HitPoint;
                    var cityCounselLoc = 1;//structureData.Locations.FirstOrDefault();

                    structureData = CacheData.CacheStructureDataManager.GetFullStructureData(StructureType.Gate);
                    var gateHealth = structureData.Levels.OrderBy(x => x.Data.Level).FirstOrDefault().Data.HitPoint;
                    var gateLoc = 2;

                    structureData = CacheData.CacheStructureDataManager.GetFullStructureData(StructureType.WatchTower);
                    var wtHealth = structureData.Levels.OrderBy(x => x.Data.Level).FirstOrDefault().Data.HitPoint;
                    var wtLoc = 4;

                    structureData = CacheData.CacheStructureDataManager.GetFullStructureData(StructureType.TrainingHeroes);
                    var thHealth = structureData.Levels.OrderBy(x => x.Data.Level).FirstOrDefault().Data.HitPoint;
                    var thLoc = 3;

                    int fhHealth = 0;
                    int fhLoc = 0;

                    if (version > 901)
                    {
                        structureData = CacheData.CacheStructureDataManager.GetFullStructureData(StructureType.FriendshipHall);
                        fhHealth = structureData.Levels.OrderBy(x => x.Data.Level).FirstOrDefault().Data.HitPoint;
                        fhLoc = 5;
                    }
                    //#if DEBUG
                    //                    await resManager.SumMainResource(playerId, 100000, 100000, 100000, 10000);
                    //#else
                    await resManager.SumMainResource(playerId, 10000, 10000, 10000, 500);
                    //#endif
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

                    dataList[0].Location = thLoc;
                    dataList[0].HitPoints = thHealth;
                    json = JsonConvert.SerializeObject(dataList);
                    await dataManager.AddOrUpdatePlayerData(playerId, DataType.Structure, (int)StructureType.TrainingHeroes, json);

                    if (version > 901)
                    {
                        dataList[0].Location = fhLoc;
                        dataList[0].HitPoints = fhHealth;
                        json = JsonConvert.SerializeObject(dataList);
                        await dataManager.AddOrUpdatePlayerData(playerId, DataType.Structure, (int)StructureType.FriendshipHall, json);
                    }

                    await CreateBackup(playerId);
                }
                else if (response.Case == 101)// && (version > 0))
                {
                    await CreateBackup(playerId);

                    try
                    {
                        var dataManager = new PlayerDataManager();
                        var resp = await dataManager.GetPlayerData(playerId, DataType.Structure, (int)StructureType.FriendshipHall);
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
                                        var structureData = CacheData.CacheStructureDataManager.GetFullStructureData(StructureType.FriendshipHall);
                                        int fhLoc = 5;

                                        if (bld.Location != fhLoc)
                                        {
                                            bld.Location = fhLoc;
                                            var json = JsonConvert.SerializeObject(structures);
                                            await dataManager.UpdatePlayerDataID(playerId, resp.Data.Id, json);
                                        }
                                    }
                                }
                            }
                            if (!found)
                            {
                                var structureData = CacheData.CacheStructureDataManager.GetFullStructureData(StructureType.FriendshipHall);
                                int fhHealth = structureData.Levels.OrderBy(x => x.Data.Level).FirstOrDefault().Data.HitPoint;
                                int fhLoc = 5;// structureData.Locations.FirstOrDefault();

                                var timestamp = DateTime.UtcNow;
                                var dataList = new List<StructureDetails>();
                                dataList.Add(new StructureDetails()
                                {
                                    Level = 1,
                                    Location = fhLoc,
                                    StartTime = timestamp,
                                    HitPoints = fhHealth,
                                });
                                var json = JsonConvert.SerializeObject(dataList);
                                await dataManager.AddOrUpdatePlayerData(playerId, DataType.Structure, (int)StructureType.FriendshipHall, json);
                            }
                        }

                        resp = await dataManager.GetPlayerData(playerId, DataType.Custom, 3);
                        if (resp.IsSuccess && !resp.HasData)
                        {
                            var json = JsonConvert.SerializeObject(new UserVIPDetails(100));
                            var x = await dataManager.AddOrUpdatePlayerData(playerId, DataType.Custom, 3, json);
                        }
                    }
                    catch (Exception ex)
                    {
                        response.Message = ex.Message;
                    }
                }
                else
                {
                    await CreateBackup(playerId);
                }
                await CompleteAccountQuest(playerId, AccountTaskType.SignIn);

                return response;
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<Player>() { Case = 202, Data = null, Message = ex.Message };
            }
            catch (RequirementExecption ex)
            {
                return new Response<Player>() { Case = 201, Data = null, Message = ex.Message };
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

        private async Task<Response> CreateBackup(int playerId)
        {
            string json = null;
            try
            {
                var allPlayerData = new AllPlayerData();
                var dataManager = new PlayerDataManager();

                //PLAYER INFO
                var playerInfo = await GetAccountInfo(playerId);
                if (!playerInfo.IsSuccess || !playerInfo.HasData) throw new Exception("Error retrieving player info");

                allPlayerData.PlayerInfo = playerInfo.Data;

                //ALL PLAYER DATA
                var allData = await dataManager.GetAllPlayerData(playerId);
                if (!allData.IsSuccess || !allData.HasData) throw new Exception("Error retrieving all player data");

                allPlayerData.PlayerData = allData.Data;

                //QUESTS
                var questsResp = await questManager.GetAllQuestProgress(playerId);
                if (!questsResp.IsSuccess || !questsResp.HasData) throw new Exception("Error retrieving all quest data");

                allPlayerData.QuestData = questsResp.Data;

                json = Newtonsoft.Json.JsonConvert.SerializeObject(allPlayerData);
            }
            catch (Exception ex)
            {
                return new Response<AllPlayerData>() { Case = 0, Data = null, Message = ErrorManager.ShowError(ex) };
            }

            return await SavePlayerBackup(playerId, "Auto backup", json);
        }

        private async Task<Response> SavePlayerBackup(int playerId, string description, string data)
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

        public async Task<Response<Player>> SetProperties(int playerId, string firebaseId = null, bool? terms = null, int? worldTileId = null, string name = null, int? vipPoints = null)
        {
            try
            {
                if (playerId <= 0) throw new InvalidModelExecption("Invalid player id was provided");
                if ((name != null) && (string.IsNullOrWhiteSpace(name) || (name.Length < 3)))
                {
                    throw new InvalidModelExecption("Invalid name was provided");
                }

                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId }
                };
                if (firebaseId != null) spParams.Add("FirebaseId", firebaseId);
                if (terms != null) spParams.Add("Terms", (bool)terms ? 1 : 0);
                if (worldTileId != null) spParams.Add("WorldTileId", worldTileId);
                if (name != null) spParams.Add("Name", name);
                if (vipPoints != null) spParams.Add("VIPPoints", vipPoints);

                return await Db.ExecuteSPSingleRow<Player>("UpdatePlayerProperties", spParams);
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

                var response = await SetProperties(playerId, name: name);
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

        public async Task<Response> Debug(int playerId, int dip)
        {
            try
            {
                if (playerId <= 0) throw new InvalidModelExecption("Invalid player id was provided");

                var dataManager = new PlayerDataManager();
                var refill = (dip & 1) == 1;
                if (refill)
                {
                    var resval = "777777777";
                    await dataManager.AddOrUpdatePlayerData(playerId, DataType.Resource, (int)ResourceType.Food, resval);
                    await dataManager.AddOrUpdatePlayerData(playerId, DataType.Resource, (int)ResourceType.Wood, resval);
                    await dataManager.AddOrUpdatePlayerData(playerId, DataType.Resource, (int)ResourceType.Ore, resval);
                    await dataManager.AddOrUpdatePlayerData(playerId, DataType.Resource, (int)ResourceType.Gems, resval);
                }
                var destroyBld = (dip & 64) == 64;
                if (destroyBld)
                {
                    var structResp = await dataManager.GetAllPlayerData(playerId, DataType.Structure);
                    if (structResp.IsSuccess && structResp.HasData)
                    {
                        var list = new List<PlayerDataTable>();
                        var structures = structResp.Data;
                        foreach (var structure in structures)
                        {
                            if ((structure.ValueId <= 3) || (structure.ValueId == 8))
                            {
                                try
                                {
                                    var userData = PlayerData.PlayerDataToUserStructureData(structure);
                                    userData.Value[0].Level = 1;
                                    userData.Value[0].Duration = 0;
                                    var json = JsonConvert.SerializeObject(userData.Value);
                                    await dataManager.UpdatePlayerDataID(playerId, structure.Id, json);
                                }
                                catch { }
                            }
                            else
                            {
                                await dataManager.UpdatePlayerDataID(playerId, structure.Id, null);
                            }
                        }
                    }
                    var troopResp = await dataManager.GetAllPlayerData(playerId, DataType.Troop);
                    if (troopResp.IsSuccess && troopResp.HasData)
                    {
                        var list = new List<PlayerDataTable>();
                        var troops = troopResp.Data;
                        foreach (var troop in troops)
                        {
                            await dataManager.UpdatePlayerDataID(playerId, troop.Id, null);
                        }
                    }
                    await UpdateTutorialInfo(playerId.ToString(), "0", false);
                }

                return new Response() { Case = 100, Message = "success" };
            }
            catch (InvalidModelExecption ex)
            {
                return new Response() { Case = 200, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new Response() { Case = 0, Message = ErrorManager.ShowError(ex) };
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


        public async Task<Response<PlayerTutorialData>> GetTutorialInfo(string playerId)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId },
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

        public async Task<Response<PlayerTutorialData>> UpdateTutorialInfo(string playerId, string playerData, bool isComplete)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId },
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

        private async Task<bool> CompleteAccountQuest(int playerId, AccountTaskType taskType)
        {
            var quests = CacheData.CacheQuestDataManager.AllQuestRewards
                                .Where(x => (x.Quest.QuestType == QuestType.Account))?.ToList();
            if (quests == null) return false;

            var questUpdated = false;
            List<PlayerQuestDataTable> allUserQuests = null;
            foreach (var quest in quests)
            {
                QuestAccountData questData = null;
                try
                {
                    questData = JsonConvert.DeserializeObject<QuestAccountData>(quest.Quest.DataString);
                }
                catch { }
                if ((questData == null) || (questData.AccountTaskType != taskType)) continue;

                if (allUserQuests == null)
                {
                    var response = await questManager.GetAllQuestProgress(playerId);
                    if (response.IsSuccess && response.HasData) allUserQuests = response.Data;
                    else break;
                }
                var userQuest = allUserQuests.Find(x => (x.QuestId == quest.Quest.QuestId));
                if ((userQuest == null) || !userQuest.Completed)
                {
                    string initialString = (userQuest == null) ? quest.Quest.DataString : null;
                    var resp = await questManager.UpdateQuestData(playerId, quest.Quest.QuestId, true, initialString);
                    if (resp.IsSuccess) questUpdated = true;
                }
            }

            return questUpdated;
        }
    }
}

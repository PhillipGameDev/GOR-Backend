using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Services;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Quest;
using GameOfRevenge.Common.Models.Quest.Template;
using GameOfRevenge.Common.Models.Kingdom;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Business.Manager.UserData;
using GameOfRevenge.Business.Manager.Kingdom;
using GameOfRevenge.Common.Email;
using Newtonsoft.Json.Linq;

namespace GameOfRevenge.Business.Manager
{
    public class AccountManager : BaseManager, IAccountManager
    {
        private readonly IUserResourceManager resManager = new UserResourceManager();
        private readonly IUserInventoryManager inventoryManager = new UserInventoryManager();
        private readonly IUserStructureManager strManager = new UserStructureManager();
        private readonly IUserQuestManager questManager = new UserQuestManager();
        private readonly IKingdomManager kingdomManager = new KingdomManager();
        private readonly IPlayerDataManager dataManager = new PlayerDataManager();
        private readonly IUserMailManager mailManager = new UserMailManager();

        private const bool underMaintenance = false;
        private const int devVersion = 906;
        private const int recommendVersion = 905;
        private const int requireMinVersion = 904;

        private async Task<(WorldTable, bool, Exception)> ValidateAccess(string identifier, bool accepted, int version)
        {
            var updateAvailable = false;
            WorldTable world = null;
            Exception exp = null;

            try
            {
                if (underMaintenance && (version < devVersion))
                {
                    throw new InvalidModelExecption("Server under maintenance, sorry for the inconvenience!");
                }
                if (version <= requireMinVersion)
                {
                    updateAvailable = true;
                    throw new InvalidModelExecption("Update Required");
                }
                if (version <= recommendVersion) updateAvailable = true;

                if (string.IsNullOrWhiteSpace(identifier))
                {
                    throw new InvalidModelExecption("Invalid identifier was provided");
                }
                if (!accepted) throw new RequirementExecption("Kindly accept terms and condition");

                var worldResp = await kingdomManager.GetWorld(Config.DefaultWorldCode);
                if (!worldResp.IsSuccess) throw new DataNotExistExecption();
                if (!worldResp.HasData) throw new DataNotExistExecption("Unable to assign world");
                world = worldResp.Data;

                identifier = identifier.Trim();
                var respInfo = await GetAccountInfo(identifier);
                if (!respInfo.IsSuccess)
                {
                    var respCount = await kingdomManager.GetWorldTileCount(world.Id);
                    if (!respCount.IsSuccess || !respCount.HasData)
                    {
                        throw new DataNotExistExecption("Unable to assign world");
                    }

                    var zoneCapacity = (world.ZoneSize * world.ZoneSize) - 4;
                    if (respCount.Data.Value >= (zoneCapacity * world.TotalZones))
                    {
                        throw new InvalidModelExecption("Server capacity reached.");
                    }
                }
            }
            catch (Exception ex)
            {
                exp = ex;
            }

            return (world, updateAvailable, exp);
        }

        public async Task<Response<Player>> Handshake(string identifier, bool accepted, int version, string platform)
        {
            var updateAvailable = false;

            try
            {
                WorldTable world;
                Exception exp;
                (world, updateAvailable, exp) = await ValidateAccess(identifier, accepted, version);
                if (exp != null) throw exp;

                var spParams = new Dictionary<string, object>()
                {
                    { "Identifier", identifier.Trim() },
                    { "Version", version }
                };
                var response = await Db.ExecuteSPSingleRow<Player>("TryLoginOrRegister", spParams);
                if (!response.IsSuccess) throw new InvalidModelExecption(response.Message);
                if (!response.HasData)
                {
                    throw new InvalidModelExecption("Unable to validate your user account.");
                }

                int playerId = response.Data.PlayerId;
                if (response.Case == 100)// new account
                {
                    //TODO: move initial data for stored procedure
                    await SetNewAccount(playerId, version);
                    await CreateBackup(playerId);
                }
                if (updateAvailable) response.Case += 50;

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
                var caseCode = updateAvailable ? 250 : 200;
                return new Response<Player>() { Case = caseCode, Data = null, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new Response<Player>() { Case = 0, Data = null, Message = ErrorManager.ShowError(ex) };
            }
        }

        public async Task<Response<Player>> TryLoginOrRegister(string identifier, bool accepted, int version, string platform, bool isReferred = false)
        {
            var updateAvailable = false;

            try
            {
                WorldTable world;
                Exception exp;
                (world, updateAvailable, exp) = await ValidateAccess(identifier, accepted, version);
                if (exp != null) throw exp;
                var zoneCapacity = (world.ZoneSize * world.ZoneSize) - 4;

                var spParams = new Dictionary<string, object>()
                {
                    { "Identifier", identifier.Trim() },
                    { "Version", version }
                };
                var response = await Db.ExecuteSPSingleRow<Player>("TryLoginOrRegister", spParams);
                if (!response.IsSuccess) throw new InvalidModelExecption(response.Message);
                if (!response.HasData)
                {
                    throw new InvalidModelExecption("Unable to validate your user account.");
                }

                int playerId = response.Data.PlayerId;
                if (response.Data.WorldTileId == 0)
                {
                    var offsetX = (world.CurrentZone % world.ZoneX) * world.ZoneSize;
                    var offsetY = (world.CurrentZone / world.ZoneY) * world.ZoneSize;
                    var maxX = (offsetX + world.ZoneSize) - 1;
                    var maxY = (offsetY + world.ZoneSize) - 1;
                    var zonePlyResp = await kingdomManager.GetWorldZonePlayers(offsetX, offsetY, maxX, maxY);
                    if (!zonePlyResp.IsSuccess || !zonePlyResp.HasData)
                    {
                        throw new DataNotExistExecption("Unable to assign world");
                    }

                    var zonePlayers = zonePlyResp.Data;
                    foreach (var zonePly in zonePlayers)
                    {
                        zonePly.X -= offsetX;
                        zonePly.Y -= offsetY;
                    }
                    var data = AddPlayerToZone(playerId, world.ZoneSize, zonePlayers);
                    if (data == null) throw new Exception("Unable to assign world position");

                    var finalX = offsetX + data.X;
                    var finalY = offsetY + data.Y;
                    var updateResp = await kingdomManager.UpdateWorldTileData(finalX, finalY, worldId: world.Id);
                    if (!updateResp.IsSuccess) throw new Exception("Unable to assign world position");

                    var updatePlyResp = await SetProperties(playerId, worldTileId: updateResp.Data.WorldTileId);
                    if (!updatePlyResp.IsSuccess) throw new Exception("Unable to assign world position");

                    response.Data.WorldTileId = updatePlyResp.Data.WorldTileId;
                    response.Data.X = finalX;
                    response.Data.Y = finalY;

                    if (zonePlayers.Count >= zoneCapacity)
                    {
                        int zoneId = world.CurrentZone + 1;
                        await kingdomManager.UpdateWorld(world.Id, zoneId);
                        await kingdomManager.AddZoneFortress(world.Id, zoneId);
                        await kingdomManager.AddMonsters(world, zoneId);
                    }
                    zonePlayers.Clear();
                }

                if (response.Case == 100)// new account
                {
                    //TODO: move initial data for stored procedure
                    await SetNewAccount(playerId, version);
                    await CreateBackup(playerId);
                    var infoResp = await GetAccountInfo(playerId);
                    response.Data.Info = infoResp.Data;

                    if (!isReferred)
                    {
                        // Add records to the table
                        var addResp = await AddPlayerReferredData(response.Data.PlayerId, null);
                        if (!addResp.IsSuccess)
                        {
                            response.Case = addResp.Case; response.Message = addResp.Message;
                        }
                    }

                    return response;
                }
                else if (response.Case == 101)// && (version > 0))
                {
                    await CreateBackup(playerId);

                    try
                    {
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

                                var dataList = new List<StructureDetails>();
                                dataList.Add(new StructureDetails()
                                {
                                    Level = 1,
                                    Location = fhLoc,
                                    StartTime = DateTime.UtcNow,
                                    HitPoints = fhHealth,
                                });
                                var json = JsonConvert.SerializeObject(dataList);
                                await dataManager.AddOrUpdatePlayerData(playerId, DataType.Structure, (int)StructureType.FriendshipHall, json);
                            }
                        }

                        resp = await dataManager.GetPlayerData(playerId, DataType.Custom, (int)CustomValueType.VIPPoints);
                        if (resp.IsSuccess && !resp.HasData)
                        {
                            var json = JsonConvert.SerializeObject(new UserVIPDetails(100));
                            var x = await dataManager.AddOrUpdatePlayerData(playerId, DataType.Custom, (int)CustomValueType.VIPPoints, json);
                        }
                    }
                    catch (Exception ex)
                    {
                        response.Message = ex.Message;
                    }
                    await CompleteAccountQuest(playerId, AccountTaskType.SignIn);
                }
                else
                {
                    await CreateBackup(playerId);
                    await CompleteAccountQuest(playerId, AccountTaskType.SignIn);
                }
                if (updateAvailable) response.Case += 50;

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
                var caseCode = updateAvailable? 250 : 200;
                return new Response<Player>() { Case = caseCode, Data = null, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new Response<Player>() { Case = 0, Data = null, Message = ErrorManager.ShowError(ex) };
            }
        }

        public async Task<Response<Player>> TryLoginOrRegister(string identifier, int referredPlayerId, bool accepted, int version, string platform)
        {
            try
            {
                var loginResp = await this.TryLoginOrRegister(identifier, accepted, version, platform, true);

                if (loginResp.Case != 100) return loginResp;

                // Create new account by refered playerId

                // 1. Add 1,000 Gold to two players
                var resp = await dataManager.AddPlayerResourceData(loginResp.Data.PlayerId, 0, 0, 0, 0, 1000);
                if (!resp.IsSuccess)
                {
                    loginResp.Case = resp.Case; loginResp.Message = resp.Message; return loginResp;
                }

                resp = await dataManager.AddPlayerResourceData(referredPlayerId, 0, 0, 0, 0, 1000);
                if (!resp.IsSuccess)
                {
                    loginResp.Case = resp.Case; loginResp.Message = resp.Message; return loginResp;
                }

                // 2. Send Notification Messages
                var msgResp = await mailManager.SendMail(referredPlayerId, MailType.Message, JsonConvert.SerializeObject(new MailMessage()
                {
                    Subject = "Referred Reward",
                    Message = "You received a bonus of 1,000 gold by referring a new player.",
                    SenderId = loginResp.Data.PlayerId,
                    SenderName = loginResp.Data.Name
                }));
                if (!msgResp.IsSuccess)
                {
                    loginResp.Case = msgResp.Case; loginResp.Message = msgResp.Message; return loginResp;
                }

                msgResp = await mailManager.SendMail(loginResp.Data.PlayerId, MailType.Message, JsonConvert.SerializeObject(new MailMessage()
                {
                    Subject = "Referred Reward",
                    Message = "You received a bonus of 1,000 gold for entering this game through a referred player.",
                    SenderId = loginResp.Data.PlayerId,
                    SenderName = loginResp.Data.Name
                }));
                if (!msgResp.IsSuccess)
                {
                    loginResp.Case = msgResp.Case; loginResp.Message = msgResp.Message; return loginResp;
                }

                // 3. Add records to the table
                var addResp = await AddPlayerReferredData(loginResp.Data.PlayerId, referredPlayerId);
                if (!addResp.IsSuccess)
                {
                    loginResp.Case = addResp.Case; loginResp.Message = addResp.Message; return loginResp;
                }

                return loginResp;
            } catch (Exception ex)
            {
                return new Response<Player>() { Case = 0, Data = null, Message = ErrorManager.ShowError(ex) };
            }
        }

        private async Task<Response> AddPlayerReferredData(int playerId, int? referredPlayerId)
        {
            var spParams = new Dictionary<string, object>()
            {
                { "PlayerId", playerId },
                { "ReferredPlayerId", referredPlayerId }
            };

            try
            {
                return await Db.ExecuteSPNoData("AddPlayerReferredData", spParams);
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

        async Task SetNewAccount(int playerId, int version)
        {
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
            int farmHealth = 0;
            int sawmillHealth = 0;
            int mineHealth = 0;

            if (version > 901)
            {
                structureData = CacheData.CacheStructureDataManager.GetFullStructureData(StructureType.FriendshipHall);
                fhHealth = structureData.Levels.OrderBy(x => x.Data.Level).FirstOrDefault().Data.HitPoint;
                fhLoc = 5;

                structureData = CacheData.CacheStructureDataManager.GetFullStructureData(StructureType.Farm);
                farmHealth = structureData.Levels.OrderBy(x => x.Data.Level).FirstOrDefault().Data.HitPoint;

                structureData = CacheData.CacheStructureDataManager.GetFullStructureData(StructureType.Sawmill);
                sawmillHealth = structureData.Levels.OrderBy(x => x.Data.Level).FirstOrDefault().Data.HitPoint;

                structureData = CacheData.CacheStructureDataManager.GetFullStructureData(StructureType.Mine);
                mineHealth = structureData.Levels.OrderBy(x => x.Data.Level).FirstOrDefault().Data.HitPoint;
            }
#if DEBUG
            await resManager.SumMainResource(playerId, 100000, 100000, 100000, 10000, 10000);
            await resManager.SumRawResource(playerId, 100, 100, 100);

            for (int i = 1; i <= 3; i ++)
                await inventoryManager.AddNewInventory(playerId, i);

#else
            await resManager.SumMainResource(playerId, 10000, 10000, 10000, 500, 200);
#endif
            var dataManager = new PlayerDataManager();
            var json = JsonConvert.SerializeObject(new UserKingDetails());
            await dataManager.AddOrUpdatePlayerData(playerId, DataType.Custom, (int)CustomValueType.KingDetails, json);

            json = JsonConvert.SerializeObject(new UserBuilderDetails());
            await dataManager.AddOrUpdatePlayerData(playerId, DataType.Custom, (int)CustomValueType.BuildingWorker, json);

            json = JsonConvert.SerializeObject(new UserVIPDetails(100));
            await dataManager.AddOrUpdatePlayerData(playerId, DataType.Custom, (int)CustomValueType.VIPPoints, json);

            var dataList = new List<StructureDetails>();
            dataList.Add(new StructureDetails()
            {
                Level = 1,
                StartTime = DateTime.UtcNow,
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

                /*dataList[0].Location = 50;
                dataList[0].HitPoints = farmHealth;
                json = JsonConvert.SerializeObject(dataList);
                await dataManager.AddOrUpdatePlayerData(playerId, DataType.Structure, (int)StructureType.Farm, json);

                dataList[0].Location = 51;
                dataList[0].HitPoints = sawmillHealth;
                json = JsonConvert.SerializeObject(dataList);
                await dataManager.AddOrUpdatePlayerData(playerId, DataType.Structure, (int)StructureType.Sawmill, json);

                dataList[0].Location = 52;
                dataList[0].HitPoints = mineHealth;
                json = JsonConvert.SerializeObject(dataList);
                await dataManager.AddOrUpdatePlayerData(playerId, DataType.Structure, (int)StructureType.Mine, json);*/
            }
        }

        public PlayerID AddPlayerToZone(int playerId, int zoneSize, List<PlayerID> list)
        {
            var random = new Random();
            var half = zoneSize / 2;
            var x = 0;
            var y = 0;
            var tries = 1000;
            do
            {
                x = random.Next(zoneSize);
                y = random.Next(zoneSize);
                tries--;
                if (tries == 0) break;
            } while (Invalid(x, y, half, list));

            if (tries == 0)
            {
                var total = zoneSize * zoneSize;
                int pos = random.Next(0, total);
                do
                {
                    pos++;
                    x = pos % zoneSize;
                    y = (int)Math.Ceiling(pos / (float)zoneSize);
                    if (!Invalid(x, y, half, list)) break;
                    total--;
                } while (total > 0);
                if (total == 0) return null;
            }

            var data = new PlayerID(playerId, x, y);
            list.Add(data);

            return data;
        }

        bool Invalid(int x, int y, int half, List<PlayerID> list)
        {
            return list.Exists(ply => (ply.X == x) && (ply.Y == y)) ||
                    ((x >= (half - 1)) && (x <= half) && (y >= (half - 1)) && (y <= half));
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
            return new Response() { Case = 100, Message = "success" };

/*            try
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
            }*/
        }

        public async Task<Response<List<PlayerID>>> GetAllPlayerIDs(int? playerId = null, int length = 0, bool includeTileId = false)
        {
            var spParams = new Dictionary<string, object>()
            {
                { "Length", length },
                { "GetCoords", 1 }
            };
            if (playerId != null) spParams.Add("PlayerId", playerId);
            if (includeTileId) spParams.Add("GetTileId", 1);

            return await Db.ExecuteSPMultipleRow<PlayerID>("GetPlayerIDs", spParams);
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

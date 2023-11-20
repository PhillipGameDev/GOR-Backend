using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Business.Manager.GameDef;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Boost;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Common.Models.Quest;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Net;
using Newtonsoft.Json;

namespace GameOfRevenge.Business.Manager.UserData
{
    public class UserStructureManager : BaseUserDataManager, IUserStructureManager
    {
        private readonly UserResourceManager userResourceManager;
        //        private readonly UserQuestManager userQuestManager;
        private readonly UserMarketManager marketManager;

        public UserStructureManager()
        {
            userResourceManager = new UserResourceManager();
            //            userQuestManager = new UserQuestManager();
            marketManager = new UserMarketManager();
        }

        public async Task<Response<UserStructureData>> CheckBuildingStatus(int playerId, StructureType structureType)
        {
            //            var structId = CacheStructureDataManager.GetFullStructureData(structureType).Info.Id;
            var playerData = await manager.GetPlayerData(playerId, DataType.Structure, (int)structureType);
            return new Response<UserStructureData>()
            {
                Case = playerData.Case,
                Data = playerData.HasData ? PlayerData.PlayerDataToUserStructureData(playerData.Data) : null,
                Message = playerData.Message
            };
        }

        public async Task<Response<UserStructureData>> CheckBuildingStatus(int playerId, int location)
        {
            var allPlayerData = await manager.GetAllPlayerData(playerId, DataType.Structure);
            if (allPlayerData.IsSuccess && allPlayerData.HasData)
            {
                var userStructures = allPlayerData.Data;
                foreach (var structures in userStructures)
                {
                    if (structures == null) continue;

                    var userStructureData = PlayerData.PlayerDataToUserStructureData(structures);
                    if (!userStructureData.Value.Exists(x => (x.Location == location))) continue;

                    return new Response<UserStructureData>()
                    {
                        Case = 100,
                        Data = userStructureData,
                        Message = "Fetched building data"
                    };
                }

                allPlayerData.Message = "User building not found";
            }

            return new Response<UserStructureData>()
            {
                Case = allPlayerData.Case,
                Data = null,
                Message = allPlayerData.Message
            };
        }

        public async Task<Response<BuildingStructureData>> UpdateBuildingData(int playerId, StructureType structureType, int location, Dictionary<string, int> changes)
        {
            var existing = await CheckBuildingStatus(playerId, structureType);
            List<StructureDetails> dataList = null;
            StructureDetails building = null;
            if (existing.IsSuccess && existing.HasData)
            {
                dataList = existing.Data.Value;
                building = dataList.Find(x => (x.Location == location));
            }
            if (building == null)
            {
                return new Response<BuildingStructureData>(200, "Structure not found at location");
            }

            if (changes.ContainsKey("level")) building.Level = changes["level"];

            if (changes.ContainsKey("upgrading"))
            {
                var duration = changes["upgrading"];
                if ((duration > 0) && (building.TimeLeft == 0)) building.StartTime = DateTime.UtcNow;
                building.Duration = duration;
            }

            if (changes.ContainsKey("boost"))
            {
                var duration = changes["boost"];
                if (duration == 0)
                {
                    building.Boost = null;
                }
                else
                {
                    int castleLvl = 1;
                    var userCastle = await CheckBuildingStatus(playerId, StructureType.CityCounsel);
                    if (userCastle.Data.Value.Count > 0) castleLvl = userCastle.Data.Value[0].CurrentLevel;

                    var (seconds, percentage) = CacheStructureDataManager.GetBoostResourceGenerationTime(building.Location, castleLvl);
                    building.Boost = new BoostUpData()
                    {
                        StartTime = DateTime.UtcNow,
                        Duration = duration,
                        Value = percentage
                    };
                }
            }

            var json = JsonConvert.SerializeObject(dataList);
            var respModel = await manager.UpdatePlayerDataID(playerId, existing.Data.Id, json);
            if (respModel.IsSuccess)
            {
                return new Response<BuildingStructureData>(new BuildingStructureData(existing.Data), 100, "Structure updated succesfully");
            }
            else
            {
                return new Response<BuildingStructureData>(201, respModel.Message);
            }
        }

        private async Task<Response<UserRecordBuilderDetails>> SetWorkerLocation(int playerId, List<StructureInfos> structures, List<UserRecordBuilderDetails> workers, int location, bool createWorker)
        {
            UserRecordBuilderDetails freeWorker = null;
            UserRecordBuilderDetails currWorker = null;
            foreach (var worker in workers)
            {
                if (worker.TimeLeft(structures) > 0) continue;

                if (freeWorker == null) freeWorker = worker;
                if (worker.Location == location)
                {
                    currWorker = worker;
                    break;
                }
            }
            if (currWorker == null) currWorker = freeWorker;

            if ((currWorker == null) && createWorker && (workers.Count < 2))
            {
                currWorker = new UserRecordBuilderDetails();
            }
            if (currWorker == null) return new Response<UserRecordBuilderDetails>(200, "No builder available");

            currWorker.Location = location;
            var json = JsonConvert.SerializeObject((UserBuilderDetails)currWorker);
//            System.Console.WriteLine("builder " + currWorker.Id + " data = " + json);

            Response<PlayerDataTableUpdated> respBuilder;
            if (currWorker.Id == 0)
            {
                respBuilder = await manager.AddOrUpdatePlayerData(playerId, DataType.Custom, (int)CustomValueType.BuildingWorker, json, false);
                if (respBuilder.IsSuccess)
                {
                    currWorker.Id = respBuilder.Data.Id;
                    workers.Add(currWorker);
                }
            }
            else
            {
                respBuilder = await manager.UpdatePlayerDataID(playerId, currWorker.Id, json);
            }
            if (!respBuilder.IsSuccess)
            {
                return new Response<UserRecordBuilderDetails>(respBuilder.Case, respBuilder.Message);
            }

            return new Response<UserRecordBuilderDetails>(currWorker, respBuilder.Case, respBuilder.Message);
        }

/*        private async Task<Response<UserRecordBuilderDetails>> SetWorkerLocation(int playerId, List<UserRecordBuilderDetails> workers, int location, bool createWorker)
        {
            UserRecordBuilderDetails currWorker = null;
            var response = await manager.GetAllPlayerData(playerId, DataType.Structure);
            if (response.IsSuccess && response.HasData)
            {
                var allStructures = response.Data;
                int len = allStructures.Count;
                UserStructureData[] userStructures = new UserStructureData[len];
                foreach (var worker in workers)
                {
                    for (int num = 0; num < len; num++)
                    {
                        if (userStructures[num] == null) userStructures[num] = PlayerData.PlayerDataToUserStructureData(allStructures[num]);
                        var build = userStructures[num].Value.Find(x => (x.Location == location));
                        if (build.TimeLeft > 0) continue;

                        currWorker = worker;
                        break;
                    }

                    if (currWorker != null) break;
                }
                if ((currWorker == null) && createWorker && (workers.Count < 2))
                {
                    currWorker = new UserRecordBuilderDetails();
                }
                if (currWorker == null) return new Response<UserRecordBuilderDetails>(200, "No builder available");

                currWorker.Location = location;
                var json = JsonConvert.SerializeObject((UserBuilderDetails)currWorker);
                System.Console.WriteLine("builder " + currWorker.Id + " data = " + json);

                Response<PlayerDataTableUpdated> respBuilder;
                if (currWorker.Id == 0)
                {
                    respBuilder = await manager.AddOrUpdatePlayerData(playerId, DataType.Custom, 2, json, false);
                    if (respBuilder.IsSuccess)
                    {
                        currWorker.Id = respBuilder.Data.Id;
                        workers.Add(currWorker);
                    }
                }
                else
                {
                    respBuilder = await manager.UpdatePlayerDataID(playerId, currWorker.Id, json);
                }
                if (!respBuilder.IsSuccess)
                {
                    return new Response<UserRecordBuilderDetails>(respBuilder.Case, respBuilder.Message);
                }

                return new Response<UserRecordBuilderDetails>(currWorker, respBuilder.Case, respBuilder.Message);
            }

            return new Response<UserRecordBuilderDetails>(200, response.Message);
        }*/

        public async Task<Response<BuildingStructureData>> CreateBuilding(int playerId, StructureType structureType, int location) => await CreateBuilding(playerId, structureType, location, true, false, false);
        public async Task<Response<BuildingStructureData>> CreateBuilding(int playerId, StructureType structureType, int location, bool removeRes, bool createWorker, bool instantBuild)
        {
            var timestamp = DateTime.UtcNow;
            var existing = await CheckBuildingStatus(playerId, location);
            List<StructureDetails> dataList = null;
            var foundAtLocation = false;
            if (existing.IsSuccess && existing.HasData)
            {
                foundAtLocation = true;
            }
            else
            {
                existing = await CheckBuildingStatus(playerId, structureType);
                if (existing.IsSuccess && existing.HasData)
                {
                    dataList = existing.Data.Value;
                    foundAtLocation = dataList.Exists(x => (x.Location == location));
                }
            }
            if (foundAtLocation)
            {
                return new Response<BuildingStructureData>(new BuildingStructureData(existing.Data), 200, "Structure already exists at location");
            }

            if (dataList == null) dataList = new List<StructureDetails>();

            int limit = 0;
            var structureInfo = CacheStructureDataManager.StructureInfos.FirstOrDefault(x => (x.Info.Code == structureType));
            if (structureInfo != null)
            {
                int castleLvl = 0;
                var userCastle = await CheckBuildingStatus(playerId, StructureType.CityCounsel);
                if (userCastle.Data.Value.Count > 0) castleLvl = userCastle.Data.Value[0].CurrentLevel;

                for (int num = castleLvl; num > 0; num--)
                {
                    var lvl = num.ToString();
                    if (!structureInfo.BuildLimit.ContainsKey(lvl)) continue;

                    limit = structureInfo.BuildLimit[lvl];
                    break;
                }
            }
            if (dataList.Count(x => x.Location < 50) >= limit)
            {
                if (limit == 0)
                {
                    return new Response<BuildingStructureData>(201, "Structure not available, upgrade castle");
                }
                else
                {
                    return new Response<BuildingStructureData>(202, "Structure max limit reached");
                }
            }

            float timeReduced = 0;
            var playerData = await GetFullPlayerData(playerId);
            if (!instantBuild)
            {
                var technology = playerData.Data.Boosts.Find(x => (byte)x.Type == (byte)TechnologyType.ConstructionTechnology);
                if (technology != null)
                {
                    var specBoostData = CacheBoostDataManager.SpecNewBoostDatas.FirstOrDefault(x => x.Type == technology.Type);
                    if (specBoostData.Table > 0)
                    {
                        float.TryParse(specBoostData.Levels[technology.Level].ToString(), out float levelVal);
                        //                            int reducePercentage = technology.Level;
                        timeReduced = levelVal;// reducePercentage.HasValue ? reducePercentage.Value : 0;
                    }
                }
            }

            var structureData = CacheStructureDataManager.GetFullStructureData(structureType);
            var firstLevel = structureData.Levels.Min(x => x.Data.Level);
            if (firstLevel != 1) throw new CacheDataNotExistExecption("Structure level data does not exist");

            var structure = structureData.Levels.FirstOrDefault(x => x.Data.Level == 1);
            int secs = 0;
            if (!instantBuild)
            {
                secs = (int)(structure.Data.TimeToBuild * (1 - (timeReduced / 100f)));
                if (secs < 0) secs = 0;
            }

            dataList.Add(new StructureDetails()
            {
                Level = 1,
                LastCollected = timestamp,
                Location = location,
                StartTime = timestamp,
                Duration = secs,
                HitPoints = structure.Data.HitPoint,
                Helped = 0
            });

            if (removeRes)
            {
                var success = userResourceManager.HasResourceRequirements(structure.Requirements, playerData.Data.Resources, 1);
                if (!success) return new Response<BuildingStructureData>(203, "Insufficient player resources");
            }
//            System.Console.WriteLine("building time = " + secs);

            Response<UserRecordBuilderDetails> currWorker = null;
            if (secs > 0)
            {
                currWorker = await SetWorkerLocation(playerId, playerData.Data.Structures, playerData.Data.Workers, location, createWorker);
                if (!currWorker.IsSuccess) return new Response<BuildingStructureData>(204, currWorker.Message);
            }

            if (removeRes)
            {
                var success = await userResourceManager.RemoveResourceByRequirement(playerId, structure.Requirements);
                if (!success) return new Response<BuildingStructureData>(203, "Insufficient player resources");
            }

            // Add king experience
            if (structure.Data.KingEXP != 0)
            {
                var kingResp = await manager.AddKingExperience(playerId, structure.Data.KingEXP);
                if (!kingResp.IsSuccess)
                    return new Response<BuildingStructureData>(204, "Can't add king experience");
            }

            var json = JsonConvert.SerializeObject(dataList);
            var respModel = await manager.AddOrUpdatePlayerData(playerId, DataType.Structure, structureData.Info.Id, json);
            if (respModel.IsSuccess)
            {
                var userStructure = new UserStructureData()
                {
                    Id = respModel.Data.Id,
                    DataType = DataType.Structure,
                    ValueId = structureType,
                    Value = dataList
                };
                return new Response<BuildingStructureData>(new BuildingStructureData(userStructure, currWorker?.Data), 100, "Structure added succesfully");
            }
            else
            {
                if (removeRes)
                {
                    var success = await userResourceManager.RefundResourceByRequirement(playerId, structure.Requirements);
                }
                return new Response<BuildingStructureData>(205, "Error creating structure");
            }
        }

        public async Task<Response<BuildingStructureData>> UpgradeBuilding(int playerId, StructureType type, int location) => await UpgradeBuilding(playerId, type, location, true, false);
        public async Task<Response<BuildingStructureData>> UpgradeBuilding(int playerId, StructureType type, int location, bool removeRes, bool createWorker)
        {
            var timestamp = DateTime.UtcNow;
            var existing = await CheckBuildingStatus(playerId, type);
            if (existing.IsSuccess && existing.HasData)
            {
                var dataList = existing.Data.Value;
                var locData = dataList.Find(x => (x.Location == location));
                if (locData != null)
                {
                    var structureData = CacheStructureDataManager.GetFullStructureData(type);
                    var higherLvl = structureData.Levels.Select(x => x.Data.Level).OrderByDescending(x => x).FirstOrDefault();
                    if (locData.Level < higherLvl)
                    {
                        locData.Level++;
                        var structureSpec = structureData.GetStructureLevelById(locData.Level);
                        var requirements = structureSpec.Requirements;

                        locData.StartTime = timestamp;
                        locData.HitPoints = structureSpec.Data.HitPoint;
                        locData.Helped = 0;

                        float percentage = 0;
                        float seconds = 0;
                        var playerData = await GetFullPlayerData(playerId);
                        var technology = playerData.Data.Boosts.Find(x => (x.Type == (NewBoostType)TechnologyType.ConstructionTechnology));
                        if (technology != null)
                        {
                            var specBoostData = CacheBoostDataManager.SpecNewBoostDatas.FirstOrDefault(x => x.Type == technology.Type);

                            if (specBoostData.Table > 0)
                            {
                                float.TryParse(specBoostData.Levels[technology.Level].ToString(), out float levelVal);
                                //                            int reducePercentage = technology.Level;
                                percentage += levelVal;// reducePercentage.HasValue ? reducePercentage.Value : 0;
                            }
                        }

                        var vip = playerData.Data.VIP;
                        if ((vip != null) && (vip.TimeLeft > 0))
                        {
                            var vipBoostData = CacheBoostDataManager.SpecNewBoostDatas.FirstOrDefault(x => (x.Type == (NewBoostType)VIPBoostType.VIP));
                            if (vipBoostData != null)
                            {
                                var vipTech = vipBoostData.Techs.FirstOrDefault(x => (x.Tech == (NewBoostTech)VIPBoostTech.BuildingTimeBonus));
                                if (vipTech != null)
                                {
                                    seconds += vipTech.GetValue(vip.Level);
                                }
                            }
                        }

                        int secs = structureSpec.Data.TimeToBuild - (int)seconds;
                        float multiplier = (1 - (percentage / 100f));
                        secs = (int)(secs * multiplier);
                        if (secs <= 0) secs = 1;
                        locData.Duration = secs;

                        if (removeRes)
                        {
                            var success = userResourceManager.HasResourceRequirements(requirements, playerData.Data.Resources, 1);
                            if (!success) return new Response<BuildingStructureData>(203, "Insufficient player resources");
                        }
//                        System.Console.WriteLine("building time = " + secs);

                        Response<UserRecordBuilderDetails> currBuilder = null;
                        if (secs > 0)
                        {
                            currBuilder = await SetWorkerLocation(playerId, playerData.Data.Structures, playerData.Data.Workers, location, createWorker);
                            if (!currBuilder.IsSuccess) return new Response<BuildingStructureData>(204, currBuilder.Message);
                        }

                        if (removeRes)
                        {
                            var success = await userResourceManager.RemoveResourceByRequirement(playerId, requirements);
                            if (!success) return new Response<BuildingStructureData>(203, "Insufficient player resources");
                        }
                        if (type == StructureType.Embassy)
                        {
                            var respModel1 = await manager.AddOrUpdatePlayerData(playerId, DataType.Activity, 1, "0");
                        }

                        // Add king experience
                        if (structureSpec.Data.KingEXP != 0)
                        {
                            var kingResp = await manager.AddKingExperience(playerId, structureSpec.Data.KingEXP);
                            if (!kingResp.IsSuccess)
                                return new Response<BuildingStructureData>(204, "Can't add king experience");
                        }

                        var json = JsonConvert.SerializeObject(dataList);
                        var respModel = await manager.AddOrUpdatePlayerData(playerId, DataType.Structure, structureData.Info.Id, json);
                        if (respModel.IsSuccess)
                        {
                            var userStructure = new UserStructureData()
                            {
                                Id = respModel.Data.Id,
                                DataType = DataType.Structure,
                                ValueId = type,
                                Value = dataList
                            };
                            return new Response<BuildingStructureData>(new BuildingStructureData(userStructure, currBuilder?.Data), 100, "Structure upgraded succesfully");
                        }
                        else
                        {
                            if (removeRes)
                            {
                                var success = await userResourceManager.RefundResourceByRequirement(playerId, requirements);
                            }
                            return new Response<BuildingStructureData>(205, "Error upgrading structure");
                        }
                    }
                    else
                    {
                        return new Response<BuildingStructureData>(202, "Max level reached");
                    }
                }
                else
                {
                    return new Response<BuildingStructureData>(201, "Structure does not exist at location");
                }
            }

            return new Response<BuildingStructureData>(200, "Structure does not exists");
        }

        public async Task<Response<UserStructureData>> HelpBuilding(int playerId, int toPlayerId, StructureType type, int location, int seconds)
        {
            var targetStructures = await CheckBuildingStatus(toPlayerId, type);
            if (targetStructures.IsSuccess && targetStructures.HasData)
            {
                var dataList = targetStructures.Data.Value;
                var targetBld = dataList.Find(x => (x.Location == location));
                if (targetBld != null)
                {
                    if ((targetBld.Helped < 10) && (targetBld.TimeLeft > 2))
                    {
                        var playerData = await GetFullPlayerData(playerId);
                        if (!playerData.IsSuccess || (playerData.Data == null)) return new Response<UserStructureData>(202, "Account does not exist");

                        playerData.Data.HelpedBuild++;
                        var respModel1 = await manager.AddOrUpdatePlayerData(playerId, DataType.Activity, 1, playerData.Data.HelpedBuild.ToString());

                        targetBld.Helped++;
                        targetBld.Duration -= seconds;
                        if (targetBld.Duration < 0) targetBld.Duration = 0;

                        var json = JsonConvert.SerializeObject(dataList);
                        var respModel = await manager.UpdatePlayerDataID(toPlayerId, targetStructures.Data.Id, json);

                        return new Response<UserStructureData>(targetStructures.Data, 100, "Structure helped succesfully");
                    }
                    else
                    {
                        return new Response<UserStructureData>(201, "Help not required");
                    }
                }
            }

            return new Response<UserStructureData>(200, "Structure does not exists");
        }

        async Task<List<UserRecordBuilderDetails>> GetBuilders(int playerId)
        {
            var builders = new List<UserRecordBuilderDetails>();

            var resp = await manager.GetAllPlayerData(playerId, DataType.Custom, (int)CustomValueType.BuildingWorker);
            if (resp.IsSuccess)
            {
                foreach (var data in resp.Data)
                {
                    UserRecordBuilderDetails builder = null;
                    try
                    {
                        builder = JsonConvert.DeserializeObject<UserRecordBuilderDetails>(data.Value);
                    }
                    catch { }

                    if (builder != null)
                    {
                        builder.Id = data.Id;
                        builders.Add(builder);
                    }
                }
            }

            return builders;
        }

/*        async Task<UserRecordBuilderDetails> GetWorker(int playerId, int location)
        {
            var resp = await manager.GetAllPlayerData(playerId, DataType.Custom, 2);
            if (resp.IsSuccess)
            {
                foreach (var data in resp.Data)
                {
                    UserRecordBuilderDetails worker = null;
                    try
                    {
                        worker = JsonConvert.DeserializeObject<UserRecordBuilderDetails>(data.Value);
                    }
                    catch { }

                    if ((worker != null) && (worker.Location == location))
                    {
                        worker.Id = data.Id;
                        return worker;
                    }
                }
            }

            return null;
        }*/

        public async Task<Response<UserStructureData>> SpeedupBuilding(int toPlayerId, int location, int seconds)
        {
            var targetStructures = await CheckBuildingStatus(toPlayerId, location);
            if (targetStructures.IsSuccess && targetStructures.HasData)
            {
                var dataList = targetStructures.Data.Value;
                var targetBld = dataList.Find(x => (x.Location == location));
                if (targetBld != null)
                {
                    if (targetBld.TimeLeft > 2)
                    {
                        targetBld.Duration -= seconds;
                        if (targetBld.Duration < 0) targetBld.Duration = 0;

                        var json = JsonConvert.SerializeObject(dataList);
                        var respModel = await manager.UpdatePlayerDataID(toPlayerId, targetStructures.Data.Id, json);

                        return new Response<UserStructureData>(targetStructures.Data, 100, "Building time successfully reduced");
                    }
                    else
                    {
                        return new Response<UserStructureData>(201, "Time reduction not required");
                    }
                }
            }

            return new Response<UserStructureData>(200, "Structure does not exists");
        }

        public async Task<Response<UserStructureData>> DestroyBuilding(int playerId, StructureType type, int location)
        {
            var existing = await CheckBuildingStatus(playerId, type);

            if (existing.IsSuccess && existing.HasData)
            {
                var dataList = existing.Data.Value;
                var locData = dataList.Find(x => (x.Location == location));
                if (locData != null)
                {
                    dataList.Remove(locData);
//                    var respModel = await manager.AddOrUpdatePlayerData(playerId, DataType.Structure, CacheStructureDataManager.GetFullStructureData(type).Info.Id, JsonConvert.SerializeObject(dataList));
                    var json = JsonConvert.SerializeObject(dataList);
                    var respModel = await manager.UpdatePlayerDataID(playerId, existing.Data.Id, json);
                    var userData = new UserStructureData()
                    {
                        Id = respModel.Data.Id,
                        DataType = DataType.Structure,
                        ValueId = type,
                        Value = dataList
                    };
                    return new Response<UserStructureData>(userData, 100, "Structure removed succesfully");
                }
            }

            return new Response<UserStructureData>(100, "Structure does not exists");
        }

        private static readonly List<StructureType> ValidResourceStructures = new List<StructureType>() { StructureType.Farm, StructureType.Sawmill, StructureType.Mine };

        private static (StructureInfos, StructureDetails, IReadOnlyStructureDataRequirement) FindUserBuilding(int locationId, List<PlayerDataTable> playerData, StructureType structureType = StructureType.Unknown)
        {
            var filter = ValidResourceStructures;
            if (structureType != StructureType.Unknown) filter = new List<StructureType>() { structureType };

            var exists = false;
            StructureInfos userBldGroup = null;
            StructureDetails userBld = null;
            foreach (var item in playerData)
            {
                if ((item == null) || !filter.Exists(x => ((int)x == item.ValueId))) continue;

                exists = true;
                var group = new StructureInfos(item);
                var bld = group.Buildings?.Find(x => (x.Location == locationId));
                if (bld != null)
                {
                    userBld = bld;
                    userBldGroup = group;
                    break;
                }
            }
            if (userBldGroup == null)
            {
                throw new InvalidModelExecption(exists? "Invalid location id was provide" : "Invalid structure type");
            }

            var cacheData = CacheStructureDataManager.GetFullStructureData(userBldGroup.StructureType);
            var cacheBuilding = cacheData?.Levels.FirstOrDefault(x => (x.Data.Level == userBld.Level));
            if (cacheBuilding == null) throw new DataNotExistExecption("Invalid structure details");

            return (userBldGroup, userBld, cacheBuilding);
        }

        public async Task<Response<BoostUpResourceResponse>> BoostUpResource(int playerId, int locationId, bool setBoost)
        {
            try
            {
                var allStructures = await manager.GetAllPlayerData(playerId, DataType.Structure);
                if (!allStructures.IsSuccess || !allStructures.HasData)
                {
                    throw new InvalidModelExecption(allStructures.Message);
                }
                var (userBldGroup, userBld, cacheBuilding) = FindUserBuilding(locationId, allStructures.Data);

                if (!setBoost && ((userBld.Boost == null) || (userBld.Boost.TimeLeft == 0)))
                {
                    return new Response<BoostUpResourceResponse>()
                    {
                        Case = 102,
                        Message = "Boost inactive"
                    };
                }

                int respCase;
                string msg;
                if (userBld.Boost == null) userBld.Boost = new BoostUpData();
                if (setBoost)
                {
                    int castleLvl = 1;
                    var userCastle = await CheckBuildingStatus(playerId, StructureType.CityCounsel);
                    if (userCastle.Data.Value.Count > 0) castleLvl = userCastle.Data.Value[0].CurrentLevel;

                    var (seconds, percentage) = CacheStructureDataManager.GetBoostResourceGenerationTime(locationId, castleLvl);
                    userBld.Boost.StartTime = DateTime.UtcNow;
                    userBld.Boost.Duration = seconds;
                    userBld.Boost.Value = percentage;

                    var json = JsonConvert.SerializeObject(userBldGroup.Buildings);
                    var response = await manager.UpdatePlayerDataID(playerId, userBldGroup.Id, json);

                    respCase = 100;
                    msg = "Boost started "+castleLvl+"  "+percentage;
                }
                else
                {
                    respCase = 101;
                    msg = "Boost active";
                }

                var resp = new BoostUpResourceResponse()
                {
                    StartTime = userBld.Boost.StartTime,
                    Duration = userBld.Boost.Duration,
                    Percentage = userBld.Boost.Value
                };

                return new Response<BoostUpResourceResponse>()
                {
                    Case = respCase,
                    Data = resp,
                    Message = msg
                };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<BoostUpResourceResponse>() { Case = 202, Message = ex.Message };
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<BoostUpResourceResponse>() { Case = 201, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new Response<BoostUpResourceResponse>() { Case = 200, Message = ex.Message };
            }
        }

        public async Task<Response<CollectedResourceResponse>> CollectResource(int playerId, int locationId)
        {
            try
            {
                var allStructures = await manager.GetAllPlayerData(playerId, DataType.Structure);
                if (!allStructures.IsSuccess || !allStructures.HasData)
                {
                    throw new InvalidModelExecption(allStructures.Message);
                }
                var (userBldGroup, userBld, cacheBuilding) = FindUserBuilding(locationId, allStructures.Data);

                int resHourlyProduction = 0;
                int resMaxAmount = cacheBuilding.Data.ResourceCapacity;
                ResourceType resType;
                switch (userBldGroup.StructureType)
                {
                    case StructureType.Farm:
                        resHourlyProduction = cacheBuilding.Data.FoodProduction;
                        resType = ResourceType.Food;
                        break;
                    case StructureType.Sawmill:
                        resHourlyProduction = cacheBuilding.Data.WoodProduction;
                        resType = ResourceType.Wood;
                        break;
                    case StructureType.Mine:
                        resHourlyProduction = cacheBuilding.Data.OreProduction;
                        resType = ResourceType.Ore;
                        break;
                    default:
                        throw new DataNotExistExecption("Invalid building type was provided");
                }

                var utcNow = DateTime.UtcNow;
                var timeElapsed = (utcNow - userBld.LastCollected);
                var productionAmount = timeElapsed.TotalSeconds * (resHourlyProduction / 3600f);

                float percentage = 0;
                var resPlayerData = await GetFullPlayerData(playerId);
                if (!resPlayerData.IsSuccess || !resPlayerData.HasData)
                {
                    throw new DataNotExistExecption("PlayerData can't find");
                }
                var playerData = resPlayerData.Data;

                // City Boost
                var boost = playerData.Boosts.Find(x => (x.Type == (NewBoostType)CityBoostType.ProductionBoost));
                if ((boost != null) && (boost.TimeLeft > 0))
                {
                    var specBoostData = CacheBoostDataManager.SpecNewBoostDatas.FirstOrDefault(x => (x.Type == boost.Type));
                    if (specBoostData != null)
                    {
                        var cityTech = specBoostData.Techs.FirstOrDefault(x => (x.Tech == NewBoostTech.ResourceProductionMultiplier));
                        if (cityTech != null) percentage += cityTech.GetValue(playerData.CityLevel);
                    }
                }

                // Technology Boost
                percentage += playerData.UserTechnologies.FindAll(e => e.IsResourceBoost(resType)).Sum(e => e.Effect);

                // VIP Boost
                var vip = playerData.VIP;
                if ((vip != null) && (vip.TimeLeft > 0))
                {
                    var vipBoostData = CacheBoostDataManager.SpecNewBoostDatas.FirstOrDefault(x => (x.Type == (NewBoostType)VIPBoostType.VIP));
                    if (vipBoostData != null)
                    {
                        var vipTech = vipBoostData.Techs.FirstOrDefault(x => (x.Tech == (NewBoostTech)VIPBoostTech.ResourceProductionMultiplier));
                        if (vipTech != null) percentage += vipTech.GetValue(vip.Level);
                    }
                }

                // Building Boost
                var extraMultiplier = 0f;
                if (userBld.Boost != null)
                {
                    if (userBld.Boost.TimeLeft > 0)
                    {
                        extraMultiplier = (userBld.Boost.Value / 100f);
                    }
                    else
                    {
                        userBld.Boost = null;
                    }
                }

                //            var buildingField = peer.Actor.InternalPlayerDataManager.GetPlayerBuilding(operation.StructureType, 50+);
                //TODO: add extra boost if found

                var finalMultiplier = 1 + (percentage / 100f) + extraMultiplier;
                double amount = (productionAmount * finalMultiplier);
                int collectedValue = (amount > resMaxAmount) ? resMaxAmount : (int)amount;

                var resp = await userResourceManager.SumResource(playerId, resType, collectedValue);
                if (!resp.IsSuccess) throw new DataNotExistExecption("Couldnt add resources");

                userBld.LastCollected = utcNow;
                var json = JsonConvert.SerializeObject(userBldGroup.Buildings);
                var response = await manager.UpdatePlayerDataID(playerId, userBldGroup.Id, json);

                var userQuestManager = new UserQuestManager();
                var list = new List<PlayerQuestDataTable>();
                var userQuestData = await userQuestManager.GetUserAllQuestProgress(playerId, true);
                if (!userQuestData.IsSuccess || !userQuestData.HasData)
                {
                    throw new DataNotExistExecption("User Quest Data can't find");
                }
                var playerUserQuestData = new PlayerUserQuestData()
                {
                    PlayerId = playerId,
                    UserData = playerData,
                    QuestData = userQuestData.Data,
                    QuestEventAction = (questProgress) =>
                    {
                        list.Add(questProgress);
                    }
                };
                await userQuestManager.CheckQuestProgressForCollectResourceAsync(playerUserQuestData, resType, collectedValue);

                var collected = new CollectedResourceData(collectedValue, resp.Data);
                var collectedResponse = new CollectedResourceResponse(collected, list);
                return new Response<CollectedResourceResponse>(collectedResponse, resp.Case, resp.Message);
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<CollectedResourceResponse>()
                {
                    Case = 202,
                    Message = ex.Message
                };
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<CollectedResourceResponse>()
                {
                    Case = 201,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<CollectedResourceResponse>()
                {
                    Case = 200,
                    Message = ex.Message
                };
            }
        }

        public async Task<Response<bool>> GiftResource(int playerId, int toPlayerId, int food, int wood, int ore)
        {
            try
            {
                var timestamp = DateTime.UtcNow;
                var compPlayerData = await GetFullPlayerData(playerId);
                if (!compPlayerData.IsSuccess || !compPlayerData.HasData) throw new DataNotExistExecption(compPlayerData.Message);

                if (compPlayerData.Data.Resources.Food < food || compPlayerData.Data.Resources.Wood < wood || compPlayerData.Data.Resources.Ore < ore) 
                    throw new RequirementExecption("Not enough resources");

                var resp = await marketManager.GiftResource(playerId, toPlayerId, food, wood, ore);
                return new Response<bool>(resp.IsSuccess, resp.Case, resp.Message);
            }
            catch (Exception ex)
            {
                return new Response<bool>()
                {
                    Case = 200,
                    Message = ex.Message
                };
            }
        }

        public int GetMaxInfirmaryCapacity(IReadOnlyList<StructureInfos> structures)
        {
            var maxCapacity = 0;
            try
            {

                foreach (var structure in structures)
                {
                    foreach (var building in structure.Buildings)
                    {
                        int lvl = (building.TimeLeft > 0) ? (building.Level - 1) : building.Level;
                        try
                        {
                            maxCapacity += CacheStructureDataManager.GetStructureDataTable(structure.StructureType, lvl).WoundedCapacity;
                        }
                        catch { }
                    }
                }
            }
            catch { }

            return maxCapacity;
        }


#region Gate
        public async Task<Response> UpdateGate(int playerId, int hp)
        {
            var existing = await CheckBuildingStatus(playerId, StructureType.Gate);
            if (!existing.IsSuccess || !existing.HasData) return new Response(200, "Structure does not exists");

            var playerData = await GetFullPlayerData(playerId);
            if (!playerData.IsSuccess) return new Response(200, "Account does not exist");

            //TODO: verify this, we are pulling building data but asked for it before in existing variable
            List<StructureDetails> gateData = null;
            var building = playerData.Data.Structures?.Find(x => (x.StructureType == StructureType.Gate));
            if (building != null) gateData = building.Buildings;
            if ((gateData == null) || (gateData.Count == 0)) return new Response(200, "Structure does not exists");

            gateData[0].HitPoints = hp;

            var gateId = CacheStructureDataManager.GetFullStructureData(StructureType.Gate).Info.Id;
            var json = JsonConvert.SerializeObject(gateData);
            var respModel = await manager.AddOrUpdatePlayerData(playerId, DataType.Structure, gateId, json);
            return new Response<UserStructureData>(respModel.Case, respModel.Message);
        }

        public async Task<Response<int>> RepairGate(int playerId)
        {
            var existing = await CheckBuildingStatus(playerId, StructureType.Gate);
            if (!existing.IsSuccess || !existing.HasData) return new Response<int>(200, "Structure does not exists");

            var requirements = await RepairGateCost(playerId);
            if (!requirements.IsSuccess) return new Response<int>(requirements.Case, requirements.Message);

            var playerData = await GetFullPlayerData(playerId);
            if (!playerData.IsSuccess) return new Response<int>(200, "Account does not exist");

            List<StructureDetails> gateData = null;
            var building = playerData.Data.Structures?.Find(x => (x.StructureType == StructureType.Gate));
            if (building != null) gateData = building.Buildings;
            if ((gateData == null) || (gateData.Count == 0)) return new Response<int>(200, "Structure does not exists");

            var structLvls = CacheStructureDataManager.GetFullStructureData(StructureType.Gate).Levels;
            var lvlData = structLvls.FirstOrDefault(x => (x.Data.Level == gateData[0].Level))?.Data;
            if (lvlData == null) return new Response<int>(200, "Structure cache does not exists");

            var success = await userResourceManager.RemoveResourceByRequirement(playerId, requirements.Data);
            if (success)
            {
                var respModel = await UpdateGate(playerId, lvlData.HitPoint);
                if (respModel.IsSuccess)
                {
                    return new Response<int>(lvlData.HitPoint, respModel.Case, respModel.Message);
                }
                else
                {
                    await userResourceManager.RefundResourceByRequirement(playerId, requirements.Data);
                }
            }

            return new Response<int>(200, "Insufficient player resources.");
        }
        public async Task<Response<List<DataRequirement>>> RepairGateCost(int playerId)
        {
            var playerData = await GetFullPlayerData(playerId);
            if (!playerData.IsSuccess) return new Response<List<DataRequirement>>(200, "Account does not exist");

            List<StructureDetails> gateData = null;
            var building = playerData.Data.Structures?.Find(x => x.StructureType == StructureType.Gate);
            if (building != null) gateData = building.Buildings;
            if ((gateData == null) || (gateData.Count == 0)) return new Response<List<DataRequirement>>(200, "Structure does not exists");

            var structLvls = CacheStructureDataManager.GetFullStructureData(StructureType.Gate).Levels;
            var lvlData = structLvls.FirstOrDefault(x => (x.Data.Level == gateData[0].Level))?.Data;
            if (lvlData == null) return new Response<List<DataRequirement>>(200, "Structure cache does not exists");

            var missingHp = lvlData.HitPoint - gateData[0].HitPoints;
            var requirements = new List<DataRequirement>()
            {
                new DataRequirement() { DataType = DataType.Resource, ValueId = CacheResourceDataManager.Food.Id, Value = missingHp},
                new DataRequirement() { DataType = DataType.Resource, ValueId = CacheResourceDataManager.Wood.Id, Value = missingHp},
                new DataRequirement() { DataType = DataType.Resource, ValueId = CacheResourceDataManager.Ore.Id, Value = missingHp},
            };

            return new Response<List<DataRequirement>>(requirements, 100, "Structure does not exists");
        }
        public async Task<Response<GateHpData>> GetGateHp(int playerId)
        {
            var playerData = await GetFullPlayerData(playerId);
            if (!playerData.IsSuccess) return new Response<GateHpData>(200, "Account does not exist");

            List<StructureDetails> gateData = null;
            var building = playerData.Data.Structures?.Find(x => x.StructureType == StructureType.Gate);
            if (building != null) gateData = building.Buildings;
            if (gateData == null || gateData.Count == 0) return new Response<GateHpData>(200, "Structure does not exists");

            var structLvls = CacheStructureDataManager.GetFullStructureData(StructureType.Gate).Levels;
            var lvlData = structLvls.FirstOrDefault(x => (x.Data.Level == gateData[0].Level))?.Data;
            if (lvlData == null) return new Response<GateHpData>(200, "Structure cache does not exists");

            var missingHp = lvlData.HitPoint - gateData[0].HitPoints;
            var gateHpData = new GateHpData()
            {
                CurrentHp = gateData[0].HitPoints,
                MaxHp = lvlData.HitPoint,
                RepairCost = new List<DataRequirement>()
                {
                    new DataRequirement() { DataType = DataType.Resource, ValueId = CacheResourceDataManager.Food.Id, Value = missingHp},
                    new DataRequirement() { DataType = DataType.Resource, ValueId = CacheResourceDataManager.Wood.Id, Value = missingHp},
                    new DataRequirement() { DataType = DataType.Resource, ValueId = CacheResourceDataManager.Ore.Id, Value = missingHp},
                }
            };

            return new Response<GateHpData>(gateHpData, 100, "Missing hp");
        }
#endregion
    }
}


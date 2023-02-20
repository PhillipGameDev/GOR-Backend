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
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Net;
using Newtonsoft.Json;

namespace GameOfRevenge.Business.Manager.UserData
{
    public class UserStructureManager : BaseUserDataManager, IUserStructureManager
    {
        private readonly UserResourceManager userResourceManager;
        private readonly UserMarketManager marketManager;

        public UserStructureManager()
        {
            userResourceManager = new UserResourceManager();
            marketManager = new UserMarketManager();
        }

        public async Task<Response<UserStructureData>> CheckBuildingStatus(int playerId, StructureType type)
        {
            var structId = CacheStructureDataManager.GetFullStructureData(type).Info.Id;
            var playerData = await manager.GetPlayerData(playerId, DataType.Structure, structId);
            if (playerData.IsSuccess && playerData.HasData)
            {
                return new Response<UserStructureData>()
                {
                    Case = playerData.Case,
                    Data = PlayerDataToUserStructureData(playerData.Data),
                    Message = playerData.Message
                };
            }

            return new Response<UserStructureData>()
            {
                Case = playerData.Case,
                Data = null,
                Message = playerData.Message
            };
        }

        public async Task<Response<UserStructureData>> CheckBuildingStatus(int playerId, int location)
        {
            var allPlayerData = await manager.GetAllPlayerData(playerId);
            if (allPlayerData.IsSuccess)
            {
                var userStructures = allPlayerData.Data.Where(x => (x.DataType == DataType.Structure));
                if (userStructures != null)
                {
                    foreach (var structures in userStructures)
                    {
                        if (structures == null) continue;

                        var userStructureData = PlayerDataToUserStructureData(structures);
                        if (!userStructureData.Value.Exists(x => (x.Location == location))) continue;

                        return new Response<UserStructureData>()
                        {
                            Case = 100,
                            Data = userStructureData,
                            Message = "Fetched building data"
                        };
                    }
                }

                allPlayerData.Message = "User building not found";
            }

            return new Response<UserStructureData>()
            {
                Case = 200,
                Data = null,
                Message = allPlayerData.Message
            };
        }

        private async Task<Response<UserRecordBuilderDetails>> SetBuilderTask(int playerId, bool createBuilder, List<UserRecordBuilderDetails> builders, int duration, int location, DateTime startTime)
        {
            UserRecordBuilderDetails currBuilder = null;
            foreach (var builder in builders)
            {
                System.Console.WriteLine("builder = " + builder.TimeLeft);
                if (builder.TimeLeft > 0) continue;

                currBuilder = builder;
                break;
            }
            if ((currBuilder == null) && (builders.Count < 2) && createBuilder)
            {
                currBuilder = new UserRecordBuilderDetails();
            }
            if (currBuilder == null) return new Response<UserRecordBuilderDetails>(200, "No builder available");

            currBuilder.Duration = duration;

            currBuilder.Location = location;
            currBuilder.StartTime = startTime;
            var json = JsonConvert.SerializeObject((UserBuilderDetails)currBuilder);
            System.Console.WriteLine("builder " + currBuilder.Id + " data = " + json);

            Response<PlayerDataTableUpdated> respBuilder;
            if (currBuilder.Id == 0)
            {
                respBuilder = await manager.AddOrUpdatePlayerData(playerId, DataType.Custom, 2, json, false);
                if (respBuilder.IsSuccess)
                {
                    currBuilder.Id = respBuilder.Data.Id;
                    builders.Add(currBuilder);
                }
            }
            else
            {
                respBuilder = await manager.UpdatePlayerDataID(playerId, currBuilder.Id, json);
            }
            if (!respBuilder.IsSuccess)
            {
                return new Response<UserRecordBuilderDetails>(respBuilder.Case, respBuilder.Message);
            }

            return new Response<UserRecordBuilderDetails>(currBuilder, respBuilder.Case, respBuilder.Message);
        }

        public async Task<Response<UserStructureData>> CreateBuilding(int playerId, StructureType type, int location) => await CreateBuilding(playerId, type, location, true, false, false);
        public async Task<Response<UserStructureData>> CreateBuilding(int playerId, StructureType type, int location, bool removeRes, bool createBuilder, bool instantBuild)
        {
            var timestamp = DateTime.UtcNow;
            var existing = await CheckBuildingStatus(playerId, type);
            List<StructureDetails> dataList = null;
            if (existing.IsSuccess && existing.HasData)
            {
                dataList = existing.Data.Value;
                var structureExists = dataList.Find(x => (x.Location == location));
                if (structureExists != null)
                {
                    return new Response<UserStructureData>(existing.Data, 200, "Structure already exists at location");
                }
            }
            if (dataList == null) dataList = new List<StructureDetails>();

            int limit = 0;
            var structureInfo = CacheStructureDataManager.StructureInfos.FirstOrDefault(x => (x.Info.Code == type));
            if (structureInfo != null)
            {
                int castleLvl = 0;
                var userCastle = await CheckBuildingStatus(playerId, StructureType.CityCounsel);
                if (userCastle.Data.Value.Count > 0) castleLvl = userCastle.Data.Value[0].Level;

                for (int num = castleLvl; num > 0; num--)
                {
                    var lvl = num.ToString();
                    if (!structureInfo.BuildLimit.ContainsKey(lvl)) continue;

                    limit = structureInfo.BuildLimit[lvl];
                    break;
                }
            }
            if (dataList.Count >= limit)
            {
                if (limit == 0) return new Response<UserStructureData>(201, "Structure not available, upgrade castle");
                else return new Response<UserStructureData>(202, "Structure max limit reached");
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

            var structureData = CacheStructureDataManager.GetFullStructureData(type);
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
//                EndTime = timestamp.AddSeconds(totalSec),
                HitPoints = structure.Data.HitPoint,
                Helped = 0
            });

            if (removeRes)
            {
                var success = userResourceManager.HasResourceRequirements(structure.Requirements, playerData.Data.Resources, 1);
                if (!success) return new Response<UserStructureData>(203, "Insufficient player resources");
            }
            System.Console.WriteLine("building time = " + secs);

            Response<UserRecordBuilderDetails> currBuilder = null;
            if (secs > 0)
            {
                currBuilder = await SetBuilderTask(playerId, createBuilder, playerData.Data.Builders, secs, location, timestamp);
                if (!currBuilder.IsSuccess) return new Response<UserStructureData>(204, currBuilder.Message);
            }

            if (removeRes)
            {
                var success = await userResourceManager.RemoveResourceByRequirement(playerId, structure.Requirements);
                if (!success)
                {
                    if (currBuilder != null)
                    {
                        currBuilder.Data.Duration = 0;
                        var json2 = JsonConvert.SerializeObject((UserBuilderDetails)currBuilder.Data);
                        await manager.UpdatePlayerDataID(playerId, currBuilder.Data.Id, json2);
                    }
                    return new Response<UserStructureData>(203, "Insufficient player resources");
                }
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
                return new Response<UserStructureData>(userStructure, 100, "Structure added succesfully");
            }
            else
            {
                if (removeRes)
                {
                    var success = await userResourceManager.RefundResourceByRequirement(playerId, structure.Requirements);
                }
                return new Response<UserStructureData>(205, "Error creating structure");
            }
        }

        public async Task<Response<UserStructureData>> UpgradeBuilding(int playerId, StructureType type, int location) => await UpgradeBuilding(playerId, type, location, true, false);
        public async Task<Response<UserStructureData>> UpgradeBuilding(int playerId, StructureType type, int location, bool removeRes, bool createBuilder)
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
                        if (secs < 0) secs = 0;
                        locData.Duration = secs;

                        if (removeRes)
                        {
                            var success = userResourceManager.HasResourceRequirements(requirements, playerData.Data.Resources, 1);
                            if (!success) return new Response<UserStructureData>(203, "Insufficient player resources");
                        }
//                        System.Console.WriteLine("building time = " + secs);

                        Response<UserRecordBuilderDetails> currBuilder = null;
                        if (secs > 0)
                        {
                            currBuilder = await SetBuilderTask(playerId, createBuilder, playerData.Data.Builders, secs, location, timestamp);
                            if (!currBuilder.IsSuccess) return new Response<UserStructureData>(204, currBuilder.Message);
                        }

                        if (removeRes)
                        {
                            var success = await userResourceManager.RemoveResourceByRequirement(playerId, requirements);
                            if (!success)
                            {
                                if (currBuilder != null)//revert builder task
                                {
                                    currBuilder.Data.Duration = 0;
                                    var json2 = JsonConvert.SerializeObject((UserBuilderDetails)currBuilder.Data);
                                    await manager.UpdatePlayerDataID(playerId, currBuilder.Data.Id, json2);
                                }
                                return new Response<UserStructureData>(203, "Insufficient player resources");
                            }
                        }
                        if (type == StructureType.Embassy)
                        {
                            var respModel1 = await manager.AddOrUpdatePlayerData(playerId, DataType.Activity, 1, "0");
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
                            return new Response<UserStructureData>(userStructure, 100, "Structure upgraded succesfully");
                        }
                        else
                        {
                            if (removeRes)
                            {
                                var success = await userResourceManager.RefundResourceByRequirement(playerId, requirements);
                            }
                            return new Response<UserStructureData>(205, "Error upgrading structure");
                        }
                    }
                    else
                    {
                        return new Response<UserStructureData>(202, "Max level reached");
                    }
                }
                else
                {
                    return new Response<UserStructureData>(201, "Structure does not exist at location");
                }
            }

            return new Response<UserStructureData>(200, "Structure does not exists");
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

                        var builder = await GetBuilder(toPlayerId, location);
                        if (builder != null)
                        {
                            builder.Duration = targetBld.Duration;
                            json = JsonConvert.SerializeObject((UserBuilderDetails)builder);
                            var builderResp = await manager.UpdatePlayerDataID(toPlayerId, builder.Id, json);
                        }

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

            var resp = await manager.GetAllPlayerData(playerId, DataType.Custom, 2);
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

        async Task<UserRecordBuilderDetails> GetBuilder(int playerId, int location)
        {
            var resp = await manager.GetAllPlayerData(playerId, DataType.Custom, 2);
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

                    if ((builder != null) && (builder.Location == location))
                    {
                        builder.Id = data.Id;
                        return builder;
                    }
                }
            }

            return null;
        }

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

                        var builder = await GetBuilder(toPlayerId, location);
                        if (builder != null)
                        {
                            builder.Duration = targetBld.Duration;
                            json = JsonConvert.SerializeObject((UserBuilderDetails)builder);
                            var builderResp = await manager.UpdatePlayerDataID(toPlayerId, builder.Id, json);
                        }

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

        public async Task<Response<int>> CollectResource(int playerId, int locId, float multiplier)
        {
            try
            {
                var compPlayerData = await GetFullPlayerData(playerId);
                if (!compPlayerData.IsSuccess || !compPlayerData.HasData) throw new DataNotExistExecption(compPlayerData.Message);

                var structureValid = ValidateStructureInLocAndBuild(locId, compPlayerData.Data.Structures, new List<StructureType>() { StructureType.Mine, StructureType.Farm, StructureType.Sawmill });
                if (!structureValid) throw new DataNotExistExecption("Invalid location id was provided");
                var structInfo = GetStructureLocation(locId, compPlayerData.Data.Structures);
                if (structInfo == null) throw new DataNotExistExecption("Invalid structure type");
                var structDetails = structInfo.Buildings.Find(x => (x.Location == locId));
                if (structDetails == null) throw new DataNotExistExecption("Invalid location id was provided");

                var structData = CacheStructureDataManager.GetFullStructureData(structInfo.StructureType);
                if (structData == null) throw new DataNotExistExecption("Invalid structure was provided");
                var structDataTable = structData.Levels.FirstOrDefault(x => (x.Data.Level == structDetails.Level));
                if (structDataTable == null) throw new DataNotExistExecption("Invalid structure details");

                int resProduction = 0;
                ResourceType resId;
                switch (structInfo.StructureType)
                {
                    case StructureType.Farm:
                        resProduction = structDataTable.Data.FoodProduction;
                        resId = ResourceType.Food;
                        break;
                    case StructureType.Sawmill:
                        resProduction = structDataTable.Data.WoodProduction;
                        resId = ResourceType.Wood;
                        break;
                    case StructureType.Mine:
                        resProduction = structDataTable.Data.OreProduction;
                        resId = ResourceType.Ore;
                        break;
                    default:
                        throw new DataNotExistExecption("Invalid building type was provided");
                }
                var timestamp = DateTime.UtcNow;
                var timeEscaped = timestamp - structDetails.LastCollected;
                var productionAmount = timeEscaped.TotalSeconds * resProduction;

                float percentage = 0;
                var playerData = await GetFullPlayerData(playerId);
                var boost = playerData.Data.Boosts.Find(x => (x.Type == (NewBoostType)CityBoostType.ProductionBoost));
                if ((boost != null) && (boost.TimeLeft > 0))
                {
                    var specBoostData = CacheBoostDataManager.SpecNewBoostDatas.FirstOrDefault(x => (x.Type == boost.Type));
                    if (specBoostData.Table > 0)
                    {
                        float.TryParse(specBoostData.Levels[boost.Level].ToString(), out float levelVal);
                        percentage += levelVal;
                    }
                }

                var vip = playerData.Data.VIP;
                if ((vip != null) && (vip.TimeLeft > 0))
                {
                    var vipBoostData = CacheBoostDataManager.SpecNewBoostDatas.FirstOrDefault(x => (x.Type == (NewBoostType)VIPBoostType.VIP));
                    if (vipBoostData != null)
                    {
                        var vipTech = vipBoostData.Techs.FirstOrDefault(x => (x.Tech == (NewBoostTech)VIPBoostTech.ResourceProductionMultiplier));
                        if (vipTech != null)
                        {
                            percentage += vipTech.GetValue(vip.Level);
                        }
                    }
                }

                var finalMultiplier = 1 + (percentage / 100f) + multiplier;
                int finalValue = (int)(productionAmount * finalMultiplier);

                var resp = await userResourceManager.SumResource(playerId, resId, finalValue);
                if (!resp.IsSuccess) throw new DataNotExistExecption("Couldnt add resources");

                structDetails.LastCollected = timestamp;
                var json = JsonConvert.SerializeObject(structInfo.Buildings);
                var fresponse = await manager.AddOrUpdatePlayerData(playerId, DataType.Structure, structData.Info.Id, json);

                return new Response<int>(finalValue, resp.Case, resp.Message);
            }
            catch (Exception ex)
            {
                return new Response<int>()
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


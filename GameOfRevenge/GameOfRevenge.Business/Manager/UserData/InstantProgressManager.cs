using ExitGames.Logging;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameOfRevenge.Business.Manager.UserData
{
    public class InstantProgressManager : BaseUserDataManager, IInstantProgressManager
    {
        private readonly IUserResourceManager _userResourceManager = new UserResourceManager();
        private readonly IUserStructureManager _userStructureManager = new UserStructureManager();
        private readonly IUserTroopManager _userTroopManager = new UserTroopManager();

//        private readonly float costPerSecond = 0.08f;
//        private readonly int freeCostSeconds = 60;

/*        public bool IsFreeCost(int timeLeft) => FinalCost(timeLeft) <= 0;
        public int FinalCost(float timeLeft)
        {
            var intTime = (int)timeLeft;
            return FinalCost(intTime);
        }
        public int FinalCost(double timeLeft)
        {
            var intTime = (int)timeLeft;
            return FinalCost(intTime);
        }
        public int FinalCost(int timeLeft)
        {
            var newTime = timeLeft - freeCostSeconds;
            if (newTime <= freeCostSeconds) return 0;
            else return (int)(newTime * costPerSecond);
        }*/
        private int RecruitCost(TroopType type, int troopLevel, int count)
        {
            float singleCost = 1;
            switch (type)
            {
                case TroopType.Other:
                    break;
                case TroopType.Swordsman:
                    singleCost += 1;
                    break;
                case TroopType.Archer:
                    singleCost += 1.5f;
                    break;
                case TroopType.Knight:
                    singleCost += 2f;
                    break;
                case TroopType.Slingshot:
                    singleCost += 5f;
                    break;
                default:
                    break;
            }

            singleCost += (troopLevel / 2);

            return (int)(singleCost * count);
        }

        public Response<int> GetInstantBuildCost(int playerId, StructureType structType, int level)
        {
            try
            {
                var structureData = CacheStructureDataManager.GetFullStructureLevelData(structType, level);
//                var respones = new Response<int>(FinalCost(structureData.Data.TimeToBuild), 100, "Instant build cost");
                var respones = new Response<int>(structureData.Data.InstantBuildCost, 100, "Instant build cost");
                return respones;
            }
            catch (CacheDataNotExistExecption ex) { return new Response<int>(200, ErrorManager.ShowError(ex)); }
            catch (Exception ex) { return new Response<int>(0, ErrorManager.ShowError(ex)); }
        }

        public Response<int> GetInstantRecruitCost(TroopType type, int troopLevel, int count) => new Response<int>(RecruitCost(type, troopLevel, count), 100, "Cost");

        public async Task<Response<int>> GetBuildingSpeedUpCost(int playerId, int locId)
        {
            try
            {
                var playerData = await GetFullPlayerData(playerId);
                if (!playerData.IsSuccess || !playerData.HasData) throw new DataNotExistExecption(playerData.Message);

                foreach (var structure in playerData.Data.Structures)
                {
                    var structureInfo = structure.Buildings.FirstOrDefault(x => x.Location == locId);
                    if (structureInfo != null)
                    {
                        if (structureInfo.TimeLeft <= 0) break;
                        var structureData = CacheStructureDataManager.GetFullStructureLevelData(structure.StructureType, structureInfo.Level);
                        int cost = (int)Math.Ceiling(structureData.Data.InstantBuildCost * ((Math.Ceiling(structureInfo.TimeLeft / 30) * 30) / structureData.Data.TimeToBuild));
                        return new Response<int>(cost, 100, "Speedup building cost");
                    }
                }

                throw new DataNotExistExecption("Is not under construction");
            }
            catch (DataNotExistExecption ex) { return new Response<int>(201, ErrorManager.ShowError(ex)); }
            catch (Exception ex) { return new Response<int>(0, ErrorManager.ShowError(ex)); }
        }

        public async Task<Response<UserStructureData>> InstantBuildStructure(int playerId, StructureType structType, int level, int locId)
        {
            try
            {
                var structureData = CacheStructureDataManager.GetFullStructureData(structType);
                var structureLevelData = CacheStructureDataManager.GetFullStructureLevelData(structType, level);

                var playerDataResp = await GetFullPlayerData(playerId);
                if (!playerDataResp.IsSuccess || !playerDataResp.HasData) throw new DataNotExistExecption(playerDataResp.Message);

                var playerData = playerDataResp.Data;
/*                if (level > 1)
                {
                    foreach (var structure in playerData.Structures)
                    {
                        var structureInfo = structure.Buildings.FirstOrDefault(x => x.Location == locId);
                        if ((structureInfo != null) && (structureInfo.TimeLeft > freeCostSeconds))
                        {
                            return await SpeedUpBuildStructure(playerId, locId, playerData);
                        }
                    }
                }*/

                var cost = structureLevelData.Data.InstantBuildCost;
                if (cost == 0)
                {
                    UserRecordBuilderDetails currBuilder = null;
                    foreach (var builder in playerData.Builders)
                    {
                        if (builder.TimeLeft > 0) continue;

                        currBuilder = builder;
                        break;
                    }

                    if (currBuilder == null)
                    {
                        if (playerData.Builders.Count >= 2) return new Response<UserStructureData>(200, "No builder available");
                        cost = 1;
                    }
                }
                if (cost > 0)
                {
                    log.Info("TAKE GEMS");
                    var playerGems = playerData.Resources.Gems;
                    if (playerGems < cost) throw new RequirementExecption("Not enough gems");

                    var resCut = await _userResourceManager.SumGemsResource(playerId, -cost);
                    if (!resCut.IsSuccess) throw new RequirementExecption("Not enough gems");
                }

                bool removeResources = true;
                bool createBuilder = true;
                Response<UserStructureData> createBuildResponse = null;
                if (level > 1)
                {
                    log.Info("UPGRADE BUILDING");
                    createBuildResponse = await _userStructureManager.UpgradeBuilding(playerId, structType, locId, removeResources, createBuilder);
                }
                else
                {
                    log.Info("CREATE BUILDING");
                    createBuildResponse = await _userStructureManager.CreateBuilding(playerId, structType, locId, removeResources, createBuilder, false);
                }
                if (createBuildResponse == null)
                {
                    createBuildResponse = new Response<UserStructureData>(200, ErrorManager.ShowError());
                }

                if (!createBuildResponse.IsSuccess)
                {
                    log.Info("RETURN GEMS");
                    if (cost > 0) await _userResourceManager.SumGemsResource(playerId, cost);
                    return createBuildResponse;// new Response<UserStructureData>(createBuildResponse.Case, createBuildResponse.Message);
                }

                var buildings = createBuildResponse.Data.Value;
                buildings.Find(x => x.Location == locId).Duration = 0;

                var json = JsonConvert.SerializeObject(buildings);
                log.Info("UPDATING BUILDING DATA "+ createBuildResponse.Data.Id +"  "+ json);
                var response = await manager.UpdatePlayerDataID(playerId, createBuildResponse.Data.Id, json);
                log.Info("RESPONSE "+response.Message);

                    //AddOrUpdatePlayerData(playerId, DataType.Structure, structureData.Info.Id, json);
                if (response.IsSuccess)
                {
                    log.Info("PULL BUILDERS");
                    var builderResp = await manager.GetAllPlayerData(playerId, DataType.Custom, 2);
                    if (builderResp.IsSuccess && builderResp.HasData)
                    {
                        log.Info("BUILDERS = "+builderResp.Data.Count);
                        foreach (var builderData in builderResp.Data)
                        {
                            log.Info(">> "+builderData.Value);
                            var builder = JsonConvert.DeserializeObject<UserBuilderDetails>(builderData.Value);
                            if (builder.Location != locId) continue;

                            builder.Duration = 0;
                            json = JsonConvert.SerializeObject(builder);
                            log.Info("UPDATING BUILDER "+builderData.Id+"  "+json);
                            var respBuilder = await manager.UpdatePlayerDataID(playerId, builderData.Id, json);
                            break;
                        }
                    }

                    var userData = _userStructureManager.PlayerDataToUserStructureData(response.Data);
                    return new Response<UserStructureData>(userData, 100, "Structure upgrade completed");
                }

                if (cost > 0) await _userResourceManager.SumGemsResource(playerId, cost);
                return new Response<UserStructureData>(200, "Structure upgrade could'nt be completed");
            }
            catch (CacheDataNotExistExecption ex)
            {
                return new Response<UserStructureData>(200, ErrorManager.ShowError(ex));
            }
            catch (RequirementExecption ex)
            {
                return new Response<UserStructureData>(202, ErrorManager.ShowError(ex));
            }
            catch (Exception ex)
            {
                return new Response<UserStructureData>(0, ErrorManager.ShowError(ex));
            }
        }
        public static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public async Task<Response<UserStructureData>> SpeedUpBuildStructure(int playerId, int locId, PlayerCompleteData playerData = null)
        {
            try
            {
                if (playerData == null)
                {
                    var fullPlayerData = await GetFullPlayerData(playerId);
                    if (!fullPlayerData.IsSuccess || !fullPlayerData.HasData) throw new DataNotExistExecption(fullPlayerData.Message);

                    playerData = fullPlayerData.Data;
                }

                StructureInfos structureGroup = null;
                StructureDetails selectedBuilding = null;
                foreach (var structure in playerData.Structures)
                {
                    var building = structure.Buildings.Find(x => (x.Location == locId));
                    if (building == null) continue;

                    if (building.TimeLeft <= 0) return new Response<UserStructureData>(101, "Structure already completed");

                    structureGroup = structure;
                    selectedBuilding = building;
                    break;
                }
                if (selectedBuilding == null) return new Response<UserStructureData>(200, "Structure not found");

                var structureData = CacheStructureDataManager.GetFullStructureLevelData(structureGroup.StructureType, selectedBuilding.Level);
                double multiplier = (Math.Ceiling(selectedBuilding.TimeLeft / 30) * 30) / structureData.Data.TimeToBuild;
                int cost = (int)Math.Ceiling(structureData.Data.InstantBuildCost * multiplier);
                if (cost > 0)
                {
                    var playerGems = playerData.Resources.Gems;
                    if (playerGems < cost) throw new RequirementExecption("Not enough gems");

                    var resCut = await _userResourceManager.SumGemsResource(playerId, -cost);
                    if (!resCut.IsSuccess) throw new RequirementExecption("Not enough gems");
                }

//                    var structureData = CacheStructureDataManager.GetFullStructureData(structure.StructureType);
                selectedBuilding.Duration = 0;

                var json = JsonConvert.SerializeObject(structureGroup.Buildings);
                var response = await manager.UpdatePlayerDataID(playerId, structureGroup.Id, json);
//                    var response = await manager.AddOrUpdatePlayerData(playerId, DataType.Structure, structureData.Info.Id, json);
                if (!response.IsSuccess)
                {
                    if (cost > 0) await _userResourceManager.SumGemsResource(playerId, cost);

                    return new Response<UserStructureData>(200, "Structure upgrade could'nt be completed");
                }

                var builder = playerData.Builders.Find(x => (x.Location == selectedBuilding.Location));
                if (builder != null)
                {
                    builder.Duration = 0;
                    json = JsonConvert.SerializeObject((UserBuilderDetails)builder);
                    var builderResp = await manager.UpdatePlayerDataID(playerId, builder.Id, json);
                }

                var userStructure = _userStructureManager.PlayerDataToUserStructureData(response.Data);
                return new Response<UserStructureData>(userStructure, 100, "Structure upgrade completed");
            }
            catch (CacheDataNotExistExecption ex)
            {
                return new Response<UserStructureData>(200, ErrorManager.ShowError(ex));
            }
            catch (RequirementExecption ex)
            {
                return new Response<UserStructureData>(202, ErrorManager.ShowError(ex));
            }
            catch (Exception ex)
            {
                return new Response<UserStructureData>(0, ErrorManager.ShowError(ex));
            }
        }

        public async Task<Response<UserTroopData>> InstantRecruitTroops(int playerId, int locId, TroopType type, int troopLevel, int count)
        {
            try
            {
                var playerData = await GetFullPlayerData(playerId);
                if (!playerData.IsSuccess || !playerData.HasData) throw new DataNotExistExecption(playerData.Message);

                var troopData = CacheTroopDataManager.GetFullTroopLevelData(type, troopLevel);
                var singleTroopTime = troopData.Data.TraningTime;
                var finalTime = singleTroopTime * count;
                var cost = RecruitCost(type, troopLevel, count);

                var playerGems = playerData.Data.Resources.Gems;
                if (playerGems < cost) return new Response<UserTroopData>(200, "Not enough gems");

                var resCut = await _userResourceManager.SumGemsResource(playerId, -cost);
                if (!resCut.IsSuccess) return new Response<UserTroopData>(200, "Not enough gems");

                foreach (var troop in playerData.Data.Troops)
                {
                    if (troop.TroopType == type)
                    {
                        foreach (var troopLvl in troop.TroopData)
                        {
                            if (troopLvl.Level == troopLevel)
                            {
                                troopLvl.Count += count;
                            }

                            return await _userTroopManager.UpdateTroops(playerId, type, troop.TroopData);
                        }
                    }
                }

                var troopDataList = new List<TroopDetails>();
                troopDataList.Add(new TroopDetails()
                {
                    Count = count,
                    Level = troopLevel,
                });

                return await _userTroopManager.UpdateTroops(playerId, type, troopDataList);
            }
            catch (InvalidModelExecption ex) { return new Response<UserTroopData>(200, ErrorManager.ShowError(ex)); }
            catch (DataNotExistExecption ex) { return new Response<UserTroopData>(201, ErrorManager.ShowError(ex)); }
            catch (RequirementExecption ex) { return new Response<UserTroopData>(202, ErrorManager.ShowError(ex)); }
            catch (Exception ex) { return new Response<UserTroopData>(0, ErrorManager.ShowError(ex)); }
        }


    }
}

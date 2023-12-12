using ExitGames.Logging;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Academy;
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
        private readonly IUserAcademyManager _userAcademyManager = new UserAcademyManager();

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
            float soldierCost = 1;
            switch (type)
            {
                case TroopType.Swordsman: soldierCost += 1; break;
                case TroopType.Archer: soldierCost += 1.5f;  break;
                case TroopType.Knight: soldierCost += 2f; break;
                case TroopType.Slingshot: soldierCost += 5f; break;
                default: break;
            }

            soldierCost += (troopLevel / 2f);

            return (int)(soldierCost * count);
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

        public async Task<Response<int>> GetBuildingSpeedUpCost(int playerId, int location)
        {
            try
            {
                var playerData = await GetFullPlayerData(playerId);
                if (!playerData.IsSuccess || !playerData.HasData) throw new DataNotExistExecption(playerData.Message);

                foreach (var structure in playerData.Data.Structures)
                {
                    var structureInfo = structure.Buildings.FirstOrDefault(x => x.Location == location);
                    if (structureInfo != null)
                    {
                        if (structureInfo.TimeLeft <= 0) break;
                        var structureData = CacheStructureDataManager.GetFullStructureLevelData(structure.StructureType, structureInfo.Level);
                        int cost = (int)Math.Ceiling(structureData.Data.InstantBuildCost * (((structureInfo.TimeLeft / 30f) * 30) / structureData.Data.TimeToBuild));
                        return new Response<int>(cost, 100, "Speedup building cost");
                    }
                }

                throw new DataNotExistExecption("Is not under construction");
            }
            catch (DataNotExistExecption ex) { return new Response<int>(201, ErrorManager.ShowError(ex)); }
            catch (Exception ex) { return new Response<int>(0, ErrorManager.ShowError(ex)); }
        }

        public async Task<Response<BuildingStructureData>> InstantBuildStructure(int playerId, StructureType structureType, int level, int location)
        {
            try
            {
//                var structureData = CacheStructureDataManager.GetFullStructureData(structType);
                var structureLevelData = CacheStructureDataManager.GetFullStructureLevelData(structureType, level);

                var playerDataResp = await GetFullPlayerData(playerId);
                if (!playerDataResp.IsSuccess || !playerDataResp.HasData)
                {
                    throw new DataNotExistExecption(playerDataResp.Message);
                }

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

                bool createWorker = false;
                var cost = structureLevelData.Data.InstantBuildCost;
                if (cost != 0)//override cost based on remain time. 1minute = 3 gems
                {
                    cost = (int)Math.Ceiling(structureLevelData.Data.TimeToBuild / 60f) * 3;
                }

                var freeWorkerFound = playerData.Workers.Exists(x => (x.TimeLeft(playerData.Structures) == 0));
                if (!freeWorkerFound)
                {
                    if (playerData.Workers.Count >= 2)
                    {
                        if (cost > 0) return new Response<BuildingStructureData>(200, "No worker available");
                    }
                    else
                    {
                        createWorker = true;
                        if (cost == 0) cost = 99;
                    }
                }

                if (cost > 0)
                {

                    var playerGems = playerData.Resources.Gems;
                    if (playerGems < cost) throw new RequirementExecption("Not enough gems");

                    var resCut = await _userResourceManager.SumGemsResource(playerId, -cost);
                    if (!resCut.IsSuccess) throw new RequirementExecption("Issue removing gems");
                }

                bool removeResources = true;
                Response<BuildingStructureData> createBuildResponse = null;
                if (level > 1)
                {
                    createBuildResponse = await _userStructureManager.UpgradeBuilding(playerId, structureType, location, removeResources, createWorker);
                }
                else
                {
                    createBuildResponse = await _userStructureManager.CreateBuilding(playerId, structureType, location, removeResources, createWorker, true);
                }
                if (createBuildResponse == null)
                {
                    createBuildResponse = new Response<BuildingStructureData>(200, ErrorManager.ShowError());
                }

                if (!createBuildResponse.IsSuccess)
                {
                    if (cost > 0) await _userResourceManager.SumGemsResource(playerId, cost);
                    return createBuildResponse;// new Response<UserStructureData>(createBuildResponse.Case, createBuildResponse.Message);
                }

                var buildings = createBuildResponse.Data.StructureData.Value;
                buildings.Find(x => (x.Location == location)).Duration = 0;

                var json = JsonConvert.SerializeObject(buildings);
                var response = await manager.UpdatePlayerDataID(playerId, createBuildResponse.Data.StructureData.Id, json);

                    //AddOrUpdatePlayerData(playerId, DataType.Structure, structureData.Info.Id, json);
                if (response.IsSuccess)
                {
                    createBuildResponse.Data.StructureData = PlayerData.PlayerDataToUserStructureData(response.Data);
                    return new Response<BuildingStructureData>(createBuildResponse.Data, 100, "Structure upgrade completed");
                }

                if (cost > 0) await _userResourceManager.SumGemsResource(playerId, cost);
                return new Response<BuildingStructureData>(200, "Structure upgrade could'nt be completed");
            }
            catch (CacheDataNotExistExecption ex)
            {
                return new Response<BuildingStructureData>(200, ErrorManager.ShowError(ex));
            }
            catch (RequirementExecption ex)
            {
                return new Response<BuildingStructureData>(202, ErrorManager.ShowError(ex));
            }
            catch (Exception ex)
            {
                return new Response<BuildingStructureData>(0, ErrorManager.ShowError(ex));
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
/*                double multiplier = ((selectedBuilding.TimeLeft / 30f) * 30) / structureData.Data.TimeToBuild;
                int cost = (int)Math.Ceiling(structureData.Data.InstantBuildCost * multiplier);*/
                int cost = (int)Math.Ceiling(selectedBuilding.TimeLeft / 60f) * 3;
                if (cost > 0)
                {
                    var playerGems = playerData.Resources.Gems;
                    if (playerGems < cost) throw new RequirementExecption("Not enough gems");

                    var resCut = await _userResourceManager.SumGemsResource(playerId, -cost);
                    if (!resCut.IsSuccess) throw new RequirementExecption("Issue removing gems");
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

                var userStructure = PlayerData.PlayerDataToUserStructureData(response.Data);
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

        private UnavaliableTroopInfo GetTroopTrainingData(int locationId, TroopType troopType, List<TroopInfos> troops)
        {
            if (!CacheTroopDataManager.TroopTypes.Contains(troopType)) return null;

            UnavaliableTroopInfo data = null;
            List<List<UnavaliableTroopInfo>> troopTraining = null;
            var troopData = troops.FirstOrDefault(x => x.TroopType == troopType)?.TroopData;
            if (troopData?.Count > 0)
            {
                troopTraining = troopData.Select(x => x.InTraning).Where(x => x != null)?.ToList();
            }
            if (troopTraining?.Count > 0)
            {
                foreach (var training in troopTraining)
                {
                    var troopInfo = training.Find(x => (x.BuildingLocId == locationId));
                    if (troopInfo == null) continue;

                    data = troopInfo;
                    break;
                }
            }

            return data;
        }

        private async Task<(Response<PlayerCompleteData>, UnavaliableTroopInfo)> GetTroopStatus(int locationId, TroopType troopType, int playerId)
        {
            UnavaliableTroopInfo data = null;
            var response = await GetFullPlayerData(playerId);
            if (response.IsSuccess)
            {
                data = GetTroopTrainingData(locationId, troopType, response.Data.Troops);
            }

            if (data == null)
            {
                response.Case = 0;
                response.Message = "Invalid troop data";
            }
            //            return peer.SendOperation(operationRequest.OperationCode, ReturnCode.InvalidOperation, debuMsg: "troop not training on location");

            return (response, data);
        }

        public async Task<Response<UserTroopData>> InstantRecruitTroops(int playerId, int locId, TroopType type, int troopLevel, int count)
        {
            try
            {
                var playerData = await GetFullPlayerData(playerId);
                if (!playerData.IsSuccess || !playerData.HasData) throw new DataNotExistExecption(playerData.Message);

                //                var troopData = CacheTroopDataManager.GetFullTroopLevelData(type, troopLevel);
                //                var singleTroopTime = troopData.Data.TraningTime;
                //                var finalTime = singleTroopTime * count;

                var speedUpInstant = (count == 0);
                int trainingCount = count;
                double multiplier = 1;
                var cost = 0;
                if (speedUpInstant)
                {
                    var troopInfo = GetTroopTrainingData(locId, type, playerData.Data.Troops);
//                    multiplier = (Math.Ceiling(troopInfo.TimeLeft / 30f) * 30) / troopInfo.Duration;
                    // trainingCount = troopInfo.Count;
                    cost = (int)Math.Ceiling(troopInfo.TimeLeft / 60f) * 3;
                } else
                {
                    var troopData = CacheTroopDataManager.TroopInfos.First(e => e.Info.Code == type).Levels.First(e => e.Data.Level == troopLevel).Data;
                    cost = (int)Math.Ceiling((troopData.TraningTime * count) / 60f) * 3;
                }

                log.Info("Instant Recruit Gem Count: " + cost);

//                var cost = RecruitCost(type, troopLevel, trainingCount);
                var finalCost = (int)Math.Ceiling(cost * multiplier);


                var playerGems = playerData.Data.Resources.Gems;
                if (playerGems < finalCost) throw new RequirementExecption("Not enough gems");

                var resCut = await _userResourceManager.SumGemsResource(playerId, -finalCost);
                if (!resCut.IsSuccess) throw new RequirementExecption("Issue removing gems");

                List<TroopDetails> troopDataList = null;
                TroopDetails data = null;
                var troops = playerData.Data.Troops.Find(x => (x.TroopType == type));
                if (troops != null)
                {
                    troopDataList = troops.TroopData;
                    data = troopDataList.Find(x => (x.Level == troopLevel));
                }
                else
                {
                    troopDataList = new List<TroopDetails>();
                }

                if (!speedUpInstant)//normal instant
                {
                    if (data == null)
                    {
                        data = new TroopDetails() { Level = troopLevel };
                        troopDataList.Add(data);
                    }
                    data.Count += count;
                }
                else if (data != null)//speed up instant
                {
                    var training = data.InTraning.Find(x => (x.BuildingLocId == locId));
                    if (training != null) data.InTraning.Remove(training);
                    if (data.InTraning.Count == 0) data.InTraning = null;
                }

                return await _userTroopManager.UpdateTroops(playerId, type, troopDataList);
            }
            catch (InvalidModelExecption ex) { return new Response<UserTroopData>(200, ErrorManager.ShowError(ex)); }
            catch (DataNotExistExecption ex) { return new Response<UserTroopData>(201, ErrorManager.ShowError(ex)); }
            catch (RequirementExecption ex) { return new Response<UserTroopData>(202, ErrorManager.ShowError(ex)); }
            catch (Exception ex) { return new Response<UserTroopData>(0, ErrorManager.ShowError(ex)); }
        }

        public async Task<Response<AcademyUserDataTable>> InstantAcademyItemUpgrade(int playerId, int itemId)
        {
            try
            {
                var playerData = await GetFullPlayerData(playerId);
                if (!playerData.IsSuccess || !playerData.HasData) throw new DataNotExistExecption(playerData.Message);

                var items = _userAcademyManager.GetUserAllItems(playerId);
                var item = items.Result.Data.Find(e => e.ItemId == itemId);

                var cost = 0;
                double multiplier = 1;

                if (item != null) {
                    cost = (int)Math.Ceiling(item.TimeLeft / 60f) * 3;
                }

                var finalCost = (int)Math.Ceiling(cost * multiplier);

                var playerGems = playerData.Data.Resources.Gems;
                if (playerGems < finalCost) throw new RequirementExecption("Not enough gems");

                var resCut = await _userResourceManager.SumGemsResource(playerId, -finalCost);
                if (!resCut.IsSuccess) throw new RequirementExecption("Issue removing gems");

                return await _userAcademyManager.InstantUpgradeItem(playerId, itemId);
            }
            catch (InvalidModelExecption ex) { return new Response<AcademyUserDataTable>(200, ErrorManager.ShowError(ex)); }
            catch (DataNotExistExecption ex) { return new Response<AcademyUserDataTable>(201, ErrorManager.ShowError(ex)); }
            catch (RequirementExecption ex) { return new Response<AcademyUserDataTable>(202, ErrorManager.ShowError(ex)); }
            catch (Exception ex) { return new Response<AcademyUserDataTable>(0, ErrorManager.ShowError(ex)); }
        }
    }
}

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

        private readonly float costPerSecond = 0.08f;
        private readonly int freeCostSeconds = 60;

        public bool IsFreeCost(int timeLeft) => FinalCost(timeLeft) <= 0;
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
        }
        private int RecruitCost(TroopType type, int troopLevel, int count)
        {
            float singleCost = 1;
            switch (type)
            {
                case TroopType.Other:
                    break;
                case TroopType.Swordsmen:
                    singleCost += 1;
                    break;
                case TroopType.Archer:
                    singleCost += 1.5f;
                    break;
                case TroopType.Knight:
                    singleCost += 2f;
                    break;
                case TroopType.Seige:
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
                var respones = new Response<int>(FinalCost(structureData.Data.TimeToBuild), 100, "Instant build cost");
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
                var playerData = await GetPlayerData(playerId);
                if (!playerData.IsSuccess || !playerData.HasData) throw new DataNotExistExecption(playerData.Message);

                foreach (var structure in playerData.Data.Structures)
                {
                    var structureInfo = structure.Buildings.FirstOrDefault(x => x.Location == locId);
                    if (structureInfo != null)
                    {
                        if (structureInfo.TimeLeft <= 0) break;
                        var timeLeft = (int)structureInfo.TimeLeft;
                        var cost = FinalCost(timeLeft);
                        return new Response<int>(cost, 100, "Speedup building cost");
                    }
                }

                throw new DataNotExistExecption("Building not found in that location");
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
                var cost = FinalCost(structureLevelData.Data.TimeToBuild);

                var playerData = await GetPlayerData(playerId);
                if (!playerData.IsSuccess || !playerData.HasData) throw new DataNotExistExecption(playerData.Message);

                if (level > 1)
                {
                    foreach (var structure in playerData.Data.Structures)
                    {
                        var structureInfo = structure.Buildings.FirstOrDefault(x => x.Location == locId);
                        if (structureInfo != null && structureInfo.TimeLeft > freeCostSeconds)
                            return await SpeedUpBuildStructure(playerId, locId);
                    }
                }

                if (cost > 0)
                {
                    var playerGems = playerData.Data.Resources.Gems;
                    if (playerGems < cost) throw new RequirementExecption("Not enough gems");

                    var resCut = await _userResourceManager.RemoveGemsResource(playerId, cost);
                    if (!resCut.IsSuccess) throw new RequirementExecption("Not enough gems");
                }

                Response createBuildResponse = null;

                if (level <= 1) createBuildResponse = await _userStructureManager.CreateBuilding(playerId, structType, locId, false);
                else createBuildResponse = await _userStructureManager.UpgradeBuilding(playerId, structType, locId, false);

                if (createBuildResponse == null) return new Response<UserStructureData>(200, ErrorManager.ShowError());

                if (!createBuildResponse.IsSuccess)
                {
                    if (cost > 0) await _userResourceManager.AddGemsResource(playerId, cost);
                    return new Response<UserStructureData>(createBuildResponse.Case, createBuildResponse.Message);
                }

                playerData = await GetPlayerData(playerId);
                if (!playerData.IsSuccess || !playerData.HasData) throw new DataNotExistExecption(playerData.Message);

                foreach (var structure in playerData.Data.Structures)
                {
                    var structureInfo = structure.Buildings.FirstOrDefault(x => x.Location == locId);
                    if (structureInfo != null)
                    {
                        structureInfo.EndTime = DateTime.UtcNow;
                        structureInfo.StartTime = DateTime.UtcNow;

                        var response = await manager.AddOrUpdatePlayerData(playerId, DataType.Structure, structureData.Info.Id, JsonConvert.SerializeObject(structure.Buildings));
                        if (response.IsSuccess) return new Response<UserStructureData>(_userStructureManager.PlayerDataToUserStructureData(response.Data), 100, "Structure upgrade completed");

                        break;
                    }
                }

                if (cost > 0) await _userResourceManager.AddGemsResource(playerId, cost);
                return new Response<UserStructureData>(200, "Structure upgrade could'nt be completed");
            }
            catch (CacheDataNotExistExecption ex) { return new Response<UserStructureData>(200, ErrorManager.ShowError(ex)); }
            catch (RequirementExecption ex) { return new Response<UserStructureData>(202, ErrorManager.ShowError(ex)); }
            catch (Exception ex) { return new Response<UserStructureData>(0, ErrorManager.ShowError(ex)); }
        }

        public async Task<Response<UserStructureData>> SpeedUpBuildStructure(int playerId, int locId)
        {
            try
            {
                var playerData = await GetPlayerData(playerId);
                if (!playerData.IsSuccess || !playerData.HasData) throw new DataNotExistExecption(playerData.Message);

                foreach (var structure in playerData.Data.Structures)
                {
                    var structureInfo = structure.Buildings.FirstOrDefault(x => x.Location == locId);
                    if (structureInfo != null)
                    {
                        if (structureInfo.TimeLeft <= 0) return new Response<UserStructureData>(101, "Structure cd already completed");

                        var cost = FinalCost(structureInfo.TimeLeft);

                        if (cost > 0)
                        {
                            var playerGems = playerData.Data.Resources.Gems;
                            if (playerGems < cost) throw new RequirementExecption("Not enough gems");

                            var resCut = await _userResourceManager.RemoveGemsResource(playerId, cost);
                            if (!resCut.IsSuccess) throw new RequirementExecption("Not enough gems");
                        }

                        var structureData = CacheStructureDataManager.GetFullStructureData(structure.StructureType);
                        structureInfo.EndTime = DateTime.UtcNow;
                        structureInfo.StartTime = DateTime.UtcNow;
                        var response = await manager.AddOrUpdatePlayerData(playerId, DataType.Structure, structureData.Info.Id, JsonConvert.SerializeObject(structure.Buildings));
                        if (response.IsSuccess) return new Response<UserStructureData>(_userStructureManager.PlayerDataToUserStructureData(response.Data), 100, "Structure upgrade completed");

                        if (cost > 0) await _userResourceManager.AddGemsResource(playerId, cost);
                        return new Response<UserStructureData>(200, "Structure upgrade could'nt be completed");
                    }
                }

                return new Response<UserStructureData>(200, "Structure not found");
            }
            catch (CacheDataNotExistExecption ex) { return new Response<UserStructureData>(200, ErrorManager.ShowError(ex)); }
            catch (RequirementExecption ex) { return new Response<UserStructureData>(202, ErrorManager.ShowError(ex)); }
            catch (Exception ex) { return new Response<UserStructureData>(0, ErrorManager.ShowError(ex)); }
        }

        public async Task<Response<UserTroopData>> InstantRecruitTroops(int playerId, int locId, TroopType type, int troopLevel, int count)
        {
            try
            {
                var playerData = await GetPlayerData(playerId);
                if (!playerData.IsSuccess || !playerData.HasData) throw new DataNotExistExecption(playerData.Message);

                var troopData = CacheTroopDataManager.GetFullTroopLevelData(type, troopLevel);
                var singleTroopTime = troopData.Data.TraningTime;
                var finalTime = singleTroopTime * count;
                var cost = RecruitCost(type, troopLevel, count);

                var playerGems = playerData.Data.Resources.Gems;
                if (playerGems < cost) return new Response<UserTroopData>(200, "Not enough gems");

                var resCut = await _userResourceManager.RemoveGemsResource(playerId, cost);
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

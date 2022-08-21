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
            else
            {
                return new Response<UserStructureData>()
                {
                    Case = playerData.Case,
                    Data = null,
                    Message = playerData.Message
                };
            }
        }
        
        public async Task<Response<UserStructureData>> CreateBuilding(int playerId, StructureType type, int position) => await CreateBuilding(playerId, type, position, true);
        public async Task<Response<UserStructureData>> CreateBuilding(int playerId, StructureType type, int position, bool removeRes)
        {
            var timestamp = DateTime.UtcNow;
            var existing = await CheckBuildingStatus(playerId, type);
            var structure = CacheStructureDataManager.GetFullStructureLevelData(type, 1);
            var dataList = new List<StructureDetails>();

            var playerData = await GetFullPlayerData(playerId);
            int timeReduced = 0;
            int? reducePercentage = playerData.Data.Technologies?.Where(x => x.TechnologyType == TechnologyType.ConstructionSpeed)?.FirstOrDefault()?.Level;
            if (reducePercentage.HasValue) timeReduced = reducePercentage.Value;
            else timeReduced = 0;

            var totalSec = structure.Data.TimeToBuild * (1 - (timeReduced / 100));

            var userStructureDetails = new StructureDetails()
            {
                Level = 1,
                LastCollected = timestamp,
                Location = position,
                StartTime = timestamp,
                EndTime = timestamp.AddSeconds(totalSec),
                HitPoints = structure.Data.HitPoint,
                Helped = 0
            };

            if (existing.IsSuccess && existing.HasData)
            {
                dataList = existing.Data.Value;
                var structureExists = dataList.Exists(x => x.Location.Equals(position));
                if (structureExists) return new Response<UserStructureData>(existing.Data, 200, "Structure already exists cannot create in that location");
            }
            dataList.Add(userStructureDetails);

            if (removeRes)
            {
                var success = await userResourceManager.RemoveResourceByRequirement(playerId, structure.Requirements);
                if (!success) return new Response<UserStructureData>( 201, "Insufficient player resources");
            }

            var respModel = await manager.AddOrUpdatePlayerData(playerId, DataType.Structure, CacheStructureDataManager.GetFullStructureData(type).Info.Id, JsonConvert.SerializeObject(dataList));

            return new Response<UserStructureData>(PlayerDataToUserStructureData(respModel.Data), 100, "Structure Added succesfully");
        }

        public async Task<Response<UserStructureData>> UpgradeBuilding(int playerId, StructureType type, int position) => await UpgradeBuilding(playerId, type, position, true);
        public async Task<Response<UserStructureData>> UpgradeBuilding(int playerId, StructureType type, int position, bool removeRes)
        {
            var existing = await CheckBuildingStatus(playerId, type);
            if (existing.IsSuccess && existing.HasData)
            {
                var dataList = existing.Data.Value;
                var locData = dataList.Where(x => x.Location == position).FirstOrDefault();
                if (locData != null)
                {
                    var structLvls = CacheStructureDataManager.GetFullStructureData(type).Levels.Select(x => x.Data.Level).OrderByDescending(x => x).FirstOrDefault();
                    if (locData.Level < structLvls)
                    {
                        locData.Level++;
                        var structure = CacheStructureDataManager.GetFullStructureData(type).GetStructureLevelById(locData.Level);
                        locData.StartTime = DateTime.UtcNow;
                        locData.HitPoints = structure.Data.HitPoint;
                        locData.Helped = 0;

                        var playerData = await GetFullPlayerData(playerId);
                        int timeReduced = 0;
                        int? reducePercentage = playerData.Data.Technologies.Where(x => x.TechnologyType == TechnologyType.ConstructionSpeed)?.FirstOrDefault()?.Level;
                        if (reducePercentage.HasValue) timeReduced = reducePercentage.Value;
                        else timeReduced = 0;

                        var totalSec = structure.Data.TimeToBuild * (1 - (timeReduced / 100));

                        locData.EndTime = DateTime.UtcNow.AddSeconds(totalSec);

                        if (removeRes)
                        {
                            var success = await userResourceManager.RemoveResourceByRequirement(playerId, structure.Requirements);
                            if (!success) return new Response<UserStructureData>(new UserStructureData() { Value = dataList, ValueId = type }, 201, "Insufficient player resources");
                        }
                        if (type == StructureType.Embassy)
                        {
                            var respModel1 = await manager.AddOrUpdatePlayerData(playerId, DataType.Activity, 1, "0");
                        }

                        var respModel = await manager.AddOrUpdatePlayerData(playerId, DataType.Structure, CacheStructureDataManager.GetFullStructureData(type).Info.Id, JsonConvert.SerializeObject(dataList));
                        return new Response<UserStructureData>(new UserStructureData() { Value = dataList, ValueId = type }, 100, "Structure upgraded succesfully");
                    }
                }
            }

            return new Response<UserStructureData>(200, "Structure does not exists");
        }

        public async Task<Response<UserStructureData>> HelpBuilding(int playerId, int toPlayerId, StructureType type, int position, int helpPower)
        {
            var existing = await CheckBuildingStatus(toPlayerId, type);
            if (existing.IsSuccess && existing.HasData)
            {
                var dataList = existing.Data.Value;
                var locData = dataList.Where(x => x.Location == position).FirstOrDefault();
                if (locData != null)
                {
                    if ((locData.TimeLeft > 0) && (locData.Helped < 10))
                    {
                        var playerData = await GetFullPlayerData(playerId);
                        if (!playerData.IsSuccess || (playerData.Data == null)) return new Response<UserStructureData>(200, "Account does not exist");

                        playerData.Data.HelpedBuild++;
                        var respModel1 = await manager.AddOrUpdatePlayerData(playerId, DataType.Activity, 1, playerData.Data.HelpedBuild.ToString());

                        locData.Helped++;
                        locData.EndTime = locData.EndTime.AddMinutes(-helpPower);
                        var respModel = await manager.AddOrUpdatePlayerData(toPlayerId, DataType.Structure, CacheStructureDataManager.GetFullStructureData(type).Info.Id, JsonConvert.SerializeObject(dataList));
                        return new Response<UserStructureData>(new UserStructureData() { Value = dataList, ValueId = type }, 100, "Structure helped succesfully");
                    }
                    else
                    {
                        return new Response<UserStructureData>(200, "Help not required");
                    }
                }
            }

            return new Response<UserStructureData>(200, "Structure does not exists");
        }

        public async Task<Response<UserStructureData>> DestroyBuilding(int playerId, StructureType type, int position)
        {
            var existing = await CheckBuildingStatus(playerId, type);

            if (existing.IsSuccess && existing.HasData)
            {
                var dataList = existing.Data.Value;
                var locData = dataList.Where(x => x.Location == position).FirstOrDefault();
                if (locData != null)
                {
                    dataList.Remove(locData);
                    var respModel = await manager.AddOrUpdatePlayerData(playerId, DataType.Structure, CacheStructureDataManager.GetFullStructureData(type).Info.Id, JsonConvert.SerializeObject(dataList));

                    return new Response<UserStructureData>(new UserStructureData() { Value = dataList, ValueId = type }, 100, "Structure removed succesfully");
                }
            }

            return new Response<UserStructureData>(new UserStructureData() { Value = existing.Data.Value, ValueId = type }, 100, "Structure does not exists");
        }

        public async Task<Response<int>> CollectResource(int playerId, int locId)
        {
            try
            {
                var timestamp = DateTime.UtcNow;
                var compPlayerData = await GetFullPlayerData(playerId);
                if (!compPlayerData.IsSuccess || !compPlayerData.HasData) throw new DataNotExistExecption(compPlayerData.Message);

                var structureValid = ValidateStructureInLocAndBuild(locId, compPlayerData.Data.Structures, new List<StructureType>() { StructureType.Mine, StructureType.Farm, StructureType.Sawmill });
                if (!structureValid) throw new DataNotExistExecption("Invalid location id was provided");
                var structInfo = GetStructureLocation(locId, compPlayerData.Data.Structures);
                if (structInfo == null) throw new DataNotExistExecption("Invalid structure type");
                var structDetails = structInfo.Buildings.Where(x => x.Location == locId).FirstOrDefault();
                if (structDetails == null) throw new DataNotExistExecption("Invalid location id was provided");

                var structData = CacheStructureDataManager.GetFullStructureData(structInfo.StructureType);
                if (structData == null) throw new DataNotExistExecption("Invalid structure was");
                var structDataTable = structData.Levels.Where(x => x.Data.Level == structDetails.Level).FirstOrDefault();
                if (structDataTable == null) throw new DataNotExistExecption("Invalid structure details");
                var timeEscaped = timestamp - structDetails.LastCollected;
                double addValue = 0;
                Response<UserResourceData> addResponse;

                int boostValue = 0;
                int? percentage = compPlayerData.Data.Technologies.Where(x => x.TechnologyType == TechnologyType.ResourceProduction)?.FirstOrDefault()?.Level;
                if (percentage.HasValue) boostValue = percentage.Value;
                else boostValue = 0;

                if (compPlayerData.Data.Boosts.Exists(x => x.BoostType == BoostType.SpeedGathering && x.TimeLeft > 0)) boostValue += 5;
                if (compPlayerData.Data.Boosts.Exists(x => x.BoostType == BoostType.ProductionBoost && x.TimeLeft > 0)) boostValue += 5;

                switch (structInfo.StructureType)
                {
                    case StructureType.Farm:
                        addValue = timeEscaped.TotalSeconds * structDataTable.Data.FoodProduction;
                        addResponse = await userResourceManager.AddFoodResource(playerId, (float)addValue * (1 + (boostValue / 100)));
                        break;
                    case StructureType.Sawmill:
                        addValue = timeEscaped.TotalSeconds * structDataTable.Data.WoodProduction;
                        addResponse = await userResourceManager.AddWoodResource(playerId, (float)addValue * (1 + (boostValue / 100)));
                        break;
                    case StructureType.Mine:
                        addValue = timeEscaped.TotalSeconds * structDataTable.Data.OreProduction;
                        addResponse = await userResourceManager.AddOreResource(playerId, (float)addValue * (1 + (boostValue / 100)));
                        break;
                    default:
                        throw new DataNotExistExecption("Invalid building type was prtovided");
                }

                if (!addResponse.IsSuccess) throw new DataNotExistExecption("Couldnt add resources");

                structDetails.LastCollected = timestamp;
                var fresponse = await manager.AddOrUpdatePlayerData(playerId, DataType.Structure, structData.Info.Id, JsonConvert.SerializeObject(structInfo.Buildings));

                return new Response<int>((int)addValue, addResponse.Case, addResponse.Message);
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
            if (!existing.IsSuccess || !existing.HasData)
                return new Response(200, "Structure does not exists");

            var playerData = await GetFullPlayerData(playerId);
            if (!playerData.IsSuccess) return new Response(200, "Account does not exist");

            var gateData = playerData.Data?.Structures?.Where(x => x.StructureType == StructureType.Gate)?.FirstOrDefault()?.Buildings;
            if (gateData == null || gateData.Count == 0) return new Response(200, "Structure does not exists");

            var currentGateData = gateData.FirstOrDefault();
            currentGateData.HitPoints = hp;

            var respModel = await manager.AddOrUpdatePlayerData(playerId, DataType.Structure, CacheStructureDataManager.GetFullStructureData(StructureType.Gate).Info.Id, JsonConvert.SerializeObject(gateData));
            return new Response<UserStructureData>(respModel.Case, respModel.Message);
        }
        public async Task<Response<int>> RepairGate(int playerId)
        {
            var existing = await CheckBuildingStatus(playerId, StructureType.Gate);
            if (!existing.IsSuccess || !existing.HasData)
                return new Response<int>(200, "Structure does not exists");

            var requirements = await RepairGateCost(playerId);
            if (!requirements.IsSuccess) return new Response<int>(requirements.Case, requirements.Message);

            var playerData = await GetFullPlayerData(playerId);
            if (!playerData.IsSuccess) return new Response<int>(200, "Account does not exist");
            var gateData = playerData.Data?.Structures?.Where(x => x.StructureType == StructureType.Gate)?.FirstOrDefault()?.Buildings;
            if (gateData == null || gateData.Count == 0) return new Response<int>(200, "Structure does not exists");
            var currentGateData = gateData.FirstOrDefault();
            var structLvls = CacheStructureDataManager.GetFullStructureData(StructureType.Gate).Levels.Where(x => x.Data.Level == currentGateData.Level).FirstOrDefault().Data;

            var success = await userResourceManager.RemoveResourceByRequirement(playerId, requirements.Data);
            if (success)
            {
                var respModel = await UpdateGate(playerId, structLvls.HitPoint);
                if (respModel.IsSuccess) return new Response<int>(structLvls.HitPoint, respModel.Case, respModel.Message);
                else await userResourceManager.RefundResourceByRequirement(playerId, requirements.Data);
            }

            return new Response<int>(200, "Insufficient player resources.");
        }
        public async Task<Response<List<DataRequirement>>> RepairGateCost(int playerId)
        {
            var playerData = await GetFullPlayerData(playerId);
            if (!playerData.IsSuccess) return new Response<List<DataRequirement>>(200, "Account does not exist");

            var gateData = playerData.Data?.Structures?.Where(x => x.StructureType == StructureType.Gate)?.FirstOrDefault()?.Buildings;
            if (gateData == null || gateData.Count == 0) return new Response<List<DataRequirement>>(200, "Structure does not exists");

            var currentGateData = gateData.FirstOrDefault();
            var structLvls = CacheStructureDataManager.GetFullStructureData(StructureType.Gate).Levels.Where(x => x.Data.Level == currentGateData.Level).FirstOrDefault().Data;

            var currentHp = currentGateData.HitPoints;
            var missingHp = structLvls.HitPoint - currentGateData.HitPoints;

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

            var gateData = playerData.Data?.Structures?.Where(x => x.StructureType == StructureType.Gate)?.FirstOrDefault()?.Buildings;
            if (gateData == null || gateData.Count == 0) return new Response<GateHpData>(200, "Structure does not exists");

            var currentGateData = gateData.FirstOrDefault();
            var structLvls = CacheStructureDataManager.GetFullStructureData(StructureType.Gate).Levels.Where(x => x.Data.Level == currentGateData.Level).FirstOrDefault().Data;

            var currentHp = currentGateData.HitPoints;
            var maxHp = structLvls.HitPoint;
            var missingHp = maxHp - currentHp;

            var gateHpData = new GateHpData()
            {
                CurrentHp = currentHp,
                MaxHp = maxHp,
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


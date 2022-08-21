using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Interface.Model;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Models.Troop;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Services;

namespace GameOfRevenge.Business.Manager.UserData
{
    public class UserTroopManager : BaseUserDataManager, IUserTroopManager
    {
        private readonly IUserResourceManager userResourceManager;

        public UserTroopManager()
        {
            userResourceManager = new UserResourceManager();
        }

        public async Task<Response<UserTroopData>> RecoverWounded(int playerId, TroopType type, List<WoundeAndDeadTroopsUpdate> troops)
        {
            if (troops == null || troops.Count <= 0 || playerId <= 0 || type == TroopType.Other) return new Response<UserTroopData>(203, "Invalid data was provided");

            var timeStamp = DateTime.UtcNow;

            try
            {
                var troopDbInfo = CacheTroopDataManager.GetFullTroopData(type);
                if (troopDbInfo == null) throw new DataNotExistExecption($"Troop type {type} was not found");

                var compPlayerData = await GetFullPlayerData(playerId);
                if (!compPlayerData.IsSuccess || !compPlayerData.HasData) throw new DataNotExistExecption(compPlayerData.Message);

                var troopDetails = compPlayerData.Data.Troops.Where(x => x.TroopType == type).FirstOrDefault()?.TroopData;

                foreach (var troop in troops)
                {
                    if (troop == null || troop.Level <= 0 || troop.WoundedCount <= 0) continue;
                    var troopDataList = troopDetails?.Where(x => x.Level == troop.Level).FirstOrDefault();

                    if (troopDataList == null) throw new DataNotExistExecption($"Troop type {type} was not found");
                    if (troopDataList.Wounded <= 0 || troopDataList.Wounded < troop.WoundedCount) throw new DataNotExistExecption($"Invalid data was provided");
                    troopDataList.Wounded -= troop.WoundedCount;

                    var troopDbData = troopDbInfo.Levels.Where(x => x.Data.Level == troop.Level).FirstOrDefault();
                    if (troopDbData == null) throw new DataNotExistExecption($"Troop type {type} was not found");
                    if (troopDataList.InRecovery == null) troopDataList.InRecovery = new List<UnavaliableTroopInfo>();

                    int timeReduced = 0;
                    int? reducePercentage = compPlayerData.Data.Technologies.Where(x => x.TechnologyType == TechnologyType.RecoverySpeed)?.FirstOrDefault()?.Level;
                    if (reducePercentage.HasValue) timeReduced = reducePercentage.Value;
                    else timeReduced = 0;

                    var totalSec = troopDbData.Data.WoundedThreshold * troop.WoundedCount * (1 - (timeReduced / 100));

                    troopDataList.InRecovery.Add(new UnavaliableTroopInfo()
                    {
                        BuildingLocId = troop.BuildingLocation,
                        Count = troop.WoundedCount,
                        StartTime = timeStamp,
                        EndTime = timeStamp.AddSeconds(totalSec)
                    });
                }

                var response = await UpdateTroops(playerId, type, troopDetails);
                if (response.IsSuccess && response.HasData) return response;
                throw new RequirementExecption("Requirements is not meet");
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<UserTroopData>()
                {
                    Case = 200,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<UserTroopData>()
                {
                    Case = 201,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (RequirementExecption ex)
            {
                return new Response<UserTroopData>()
                {
                    Case = 202,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (Exception ex)
            {
                return new Response<UserTroopData>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
                //Config.PrintLog(String.Format("Exception RecoverWounded {0} {1} ", ex.Message, ex.StackTrace));
            }


        }

        public async Task<Response<UserTroopData>> AddWoundedAndDeadTroops(int playerId, TroopType type, List<WoundeAndDeadTroopsUpdate> troops)
        {
            if (troops == null || troops.Count <= 0 || playerId <= 0 || type == TroopType.Other) return new Response<UserTroopData>(203, "Invalid data was provided");

            try
            {
                var troopData = CacheTroopDataManager.GetFullTroopData(type);
                if (troopData == null) throw new DataNotExistExecption($"Troop type {type} was not found");

                var compPlayerData = await GetFullPlayerData(playerId);
                if (!compPlayerData.IsSuccess || !compPlayerData.HasData) throw new DataNotExistExecption(compPlayerData.Message);

                var troopDetails = compPlayerData.Data.Troops.Where(x => x.TroopType == type).FirstOrDefault()?.TroopData;

                foreach (var troop in troops)
                {
                    if (troop == null || troop.Level <= 0 || troop.WoundedCount <= 0) continue;
                    var troopDataList = troopDetails?.Where(x => x.Level == troop.Level).FirstOrDefault();
                    if (troopDataList == null) throw new DataNotExistExecption($"Troop type {type} was not found");
                    if (troopDataList.Count < (troop.DeadCount + troop.WoundedCount)) throw new DataNotExistExecption($"Invalid data was provided");
                    troopDataList.Count -= troop.DeadCount;
                    troopDataList.Wounded = troop.WoundedCount;
                }

                var response = await UpdateTroops(playerId, type, troopDetails);
                if (response.IsSuccess && response.HasData) return response;
                throw new RequirementExecption("Requirements is not meet");
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<UserTroopData>()
                {
                    Case = 200,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<UserTroopData>()
                {
                    Case = 201,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (RequirementExecption ex)
            {
                return new Response<UserTroopData>()
                {
                    Case = 202,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (Exception)
            {
                return new Response<UserTroopData>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError()
                };
            }
        }

        public async Task<Response<UserTroopData>> TrainTroops(int playerId, int id, int level, int count, int fromId)
        {
            try
            {
                var troop = CacheTroopDataManager.GetFullTroopData(id);
                return await TrainTroops(playerId, troop.Info.Code, level, count, fromId);
            }
            catch (CacheDataNotExistExecption ex)
            {
                return new Response<UserTroopData>()
                {
                    Case = 203,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (Exception)
            {
                return new Response<UserTroopData>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError()
                };
            }
        }
        public async Task<Response<UserTroopData>> TrainTroops(int playerId, TroopType type, int level, int count, int fromId)
        {
            try
            {
                var troopData = CacheTroopDataManager.GetFullTroopLevelData(type, level);
                var compPlayerData = await GetFullPlayerData(playerId);
                if (!compPlayerData.IsSuccess || !compPlayerData.HasData) throw new DataNotExistExecption(compPlayerData.Message);

                var structureValid = ValidateStructureInLocAndBuild(fromId, type, compPlayerData.Data.Structures);
                if (!structureValid) throw new RequirementExecption($"Structure to train troops was not found or its not a training building for troop type {type}");

                var requirements = troopData.Requirements;
                var hasRequirements = HasRequirements(requirements, compPlayerData.Data.Structures, compPlayerData.Data.Resources, count);
                if (!hasRequirements) throw new RequirementExecption("Requirements is not meet");

                var isReduced = await userResourceManager.RemoveResourceByRequirement(playerId, requirements, count);
                if (!isReduced) throw new RequirementExecption("Requirements is not meet");

                var troopDataList = compPlayerData.Data.Troops.Where(x => x.TroopType == type).FirstOrDefault()?.TroopData;
                var response = await AddTroops(playerId, type, level, count, troopDataList, true, fromId);
                if (!response.IsSuccess || !response.HasData) throw new RequirementExecption("Requirements is not meet");

                return response;
            }
            catch (CacheDataNotExistExecption ex)
            {
                return new Response<UserTroopData>()
                {
                    Case = 203,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<UserTroopData>()
                {
                    Case = 200,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<UserTroopData>()
                {
                    Case = 201,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (RequirementExecption ex)
            {
                return new Response<UserTroopData>()
                {
                    Case = 202,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (Exception)
            {
                return new Response<UserTroopData>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError()
                };
            }
        }
        public async Task<Response<UserTroopData>> InstantTrainTroops(int playerId, TroopType type, int level, int count)
        {
            try
            {
                var troopData = CacheTroopDataManager.GetFullTroopLevelData(type, level);
                var compPlayerData = await GetFullPlayerData(playerId);
                if (!compPlayerData.IsSuccess || !compPlayerData.HasData) throw new DataNotExistExecption(compPlayerData.Message);

                var isReduced = await userResourceManager.RemoveResourceByRequirement(playerId, CacheResourceDataManager.GetGemReq(1), count);
                if (!isReduced) throw new RequirementExecption("Requirements is not meet");

                var troopDataList = compPlayerData.Data.Troops.Where(x => x.TroopType == type).FirstOrDefault()?.TroopData;
                var response = await AddTroops(playerId, type, level, count);
                if (!response.IsSuccess || !response.HasData) throw new RequirementExecption("Requirements is not meet");

                return response;
            }
            catch (CacheDataNotExistExecption ex)
            {
                return new Response<UserTroopData>()
                {
                    Case = 203,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<UserTroopData>()
                {
                    Case = 200,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<UserTroopData>()
                {
                    Case = 201,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (RequirementExecption ex)
            {
                return new Response<UserTroopData>()
                {
                    Case = 202,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (Exception)
            {
                return new Response<UserTroopData>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError()
                };
            }
        }

        public async Task<Response<UserTroopData>> AddTroops(int playerId, int id, int level, int count)
        {
            try
            {
                var troop = CacheTroopDataManager.GetFullTroopData(id);
                return await AddTroops(playerId, troop.Info.Code, level, count);
            }
            catch (CacheDataNotExistExecption ex)
            {
                return new Response<UserTroopData>()
                {
                    Case = 200,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (Exception)
            {
                return new Response<UserTroopData>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError()
                };
            }
        }
        public async Task<Response<UserTroopData>> AddTroops(int playerId, TroopType type, int level, int count) => await AddTroops(playerId, type, level, count, null, false, 0);

        public async Task<Response<UserTroopData>> UpdateTroops(int playerId, TroopType type, List<TroopDetails> troops)
        {
            try
            {
                var playerData = UserTroopDataToPlayerData(new UserTroopData() { Value = troops, ValueId = type });
                var newValueResp = await manager.AddOrUpdatePlayerData(playerId, DataType.Troop, playerData.ValueId, playerData.Value);
                return new Response<UserTroopData>(PlayerDataToUserTroopData(newValueResp.Data), newValueResp.Case, newValueResp.Message);
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<UserTroopData>()
                {
                    Case = 201,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (Exception)
            {
                return new Response<UserTroopData>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError()
                };
            }
        }


        public async Task<Response<PopulationData>> GetPopulationData(int playerId) => GetPopulationData((await GetFullPlayerData(playerId)).Data);
        public Response<PopulationData> GetPopulationData(IReadOnlyList<StructureInfos> structures, IReadOnlyList<TroopInfos> troops)
        {
            var current = GetCurrentPopulation(troops);
            var max = GetMaxPopulation(structures);

            if (current.IsSuccess && max.IsSuccess)
                return new Response<PopulationData>(new PopulationData() { Current = current.Data, Max = max.Data }, CaseType.Success, "User population data");
            else if (!current.IsSuccess) return new Response<PopulationData>(current.Case, current.Message);
            else return new Response<PopulationData>(current.Case, current.Message);
        }
        public Response<int> GetMaxPopulation(IReadOnlyList<StructureInfos> structures)
        {
            try
            {
                var maxPopulation = 0;

                foreach (var structure in structures)
                {
                    foreach (var building in structure.Buildings)
                    {
                        if (building.TimeLeft <= 0)
                        {
                            try
                            {
                                var pop = CacheStructureDataManager.GetStructureDataTable(structure.StructureType, building.Level).PopulationSupport;
                                maxPopulation += pop;
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                }

                return new Response<int>(maxPopulation, 100, "Max population");
            }
            catch (Exception ex)
            {
                return new Response<int>(CaseType.Error, ErrorManager.ShowError(ex));
            }
        }
        public Response<int> GetCurrentPopulation(IReadOnlyList<TroopInfos> troops)
        {
            var currentPopulation = 0;

            foreach (var army in troops)
            {
                foreach (var troop in army.TroopData)
                {
                    currentPopulation += troop.Count;//MODIFIED current population including training and other soldiers
                }
            }

            return new Response<int>(currentPopulation, 100, "Current population");
        }
        public Response<PopulationData> GetPopulationData(PlayerCompleteData compPlayerData)
        {
            if (compPlayerData == null) return new Response<PopulationData>(CaseType.Error, "Player data was null");
            else return GetPopulationData(compPlayerData.Structures, compPlayerData.Troops);
        }


        private async Task<Response<UserTroopData>> AddTroops(int playerId, TroopType type, int level, int count, List<TroopDetails> oldtroopInfos, bool isTraining, int fromId)
        {
            try
            {
                var troopDataLvl = CacheTroopDataManager.GetFullTroopLevelData(type, level);
                var compPlayerData = await GetFullPlayerData(playerId);

                if (oldtroopInfos == null)
                {
                    if (!compPlayerData.IsSuccess && !compPlayerData.HasData) throw new DataNotExistExecption();
                    var troopClassData = compPlayerData.Data.Troops.Where(x => x.TroopType == type).FirstOrDefault();
                    //TODO: check if this instantiation is required, the class is not used anymore. we just need the TroopData list
                    if (troopClassData == null || troopClassData.TroopData == null) troopClassData = new TroopInfos()
                    {
                        TroopType = type,
                        TroopData = new List<TroopDetails>()// { new TroopDetails() { Count = 0, Level = level } }
                    };
                    oldtroopInfos = troopClassData.TroopData;//TODO: Return null and later validate it and create new list
                }

                if (oldtroopInfos.Count == 0)//TODO: validate and create new list here
                {
                    oldtroopInfos.Add(new TroopDetails() { Count = 0, Level = level });
                }

                var toUpdateObj = oldtroopInfos.Where(x => x.Level == level).FirstOrDefault();
                if (toUpdateObj == null)
                {
                    toUpdateObj = new TroopDetails() { Level = level };
                    oldtroopInfos.Add(toUpdateObj);
                }

                toUpdateObj.Count += count;
                if (toUpdateObj.InTraning == null) toUpdateObj.InTraning = new List<UnavaliableTroopInfo>();
                toUpdateObj.InTraning = toUpdateObj.InTraning.Where(x => x != null && x.TimeLeft > 0).ToList();

                if (isTraining)
                {
                    int timeReduced = 0;
                    int? reducePercentage = compPlayerData.Data.Technologies.Where(x => x.TechnologyType == TechnologyType.TrainingSpeed)?.FirstOrDefault()?.Level;
                    if (reducePercentage.HasValue) timeReduced = reducePercentage.Value;
                    else timeReduced = 0;

                    var existingTraning = toUpdateObj.InTraning.Where(x => x.BuildingLocId == fromId).FirstOrDefault();
                    if (existingTraning == null)
                    {
                        var timestamp = DateTime.UtcNow;
                        int totalTime = troopDataLvl.Data.TraningTime * count * (1 - (timeReduced / 100));
                        toUpdateObj.InTraning.Add(new UnavaliableTroopInfo()
                        {
                            BuildingLocId = fromId,
                            Count = count,
                            StartTime = timestamp,
                            EndTime = timestamp.AddSeconds(totalTime)
                        });
                    }
                    else
                    {
                        int totalTime = troopDataLvl.Data.TraningTime * count * (1 - (timeReduced / 100));
                        existingTraning.Count += count;
                        existingTraning.EndTime = existingTraning.EndTime.AddSeconds(totalTime);
                    }
                }

                if (toUpdateObj.InTraning.Count == 0) toUpdateObj.InTraning = null;

                return await UpdateTroops(playerId, type, oldtroopInfos);
            }
            catch (CacheDataNotExistExecption ex)
            {
                return new Response<UserTroopData>()
                {
                    Case = 201,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<UserTroopData>()
                {
                    Case = 201,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (RequirementExecption ex)
            {
                return new Response<UserTroopData>()
                {
                    Case = 200,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (Exception)
            {
                return new Response<UserTroopData>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError()
                };
            }
        }

        private bool ValidateStructureInLocAndBuild(int locId, TroopType type, List<StructureInfos> structures)
        {
            try
            {
                var troopBuildingRels = CacheTroopDataManager.TroopBuildingRelation.FirstOrDefault(x => x.Troops.FirstOrDefault(y => y == type) != TroopType.Other);
                return structures.Exists(x => x.StructureType == troopBuildingRels.Structure && x.Buildings.Exists(y => y.Location == locId));
            }
            catch (Exception)
            {
                return false;
            }
        }



        //public async Task<Response> InstantTrainTroops(int playerId, int buildingLoc)
        //{
        //    var playerData = await GetPlayerData(playerId);
        //    if (playerData.IsSuccess && playerData.HasData) return await InstantTrainTroops(playerData.Data, buildingLoc);
        //    else return new Response(playerData.Case, playerData.Message);
        //}
        //public async Task<Response> InstantTrainTroops(PlayerCompleteData compPlayerData, int buildingLoc)
        //{
        //    if (compPlayerData != null && buildingLoc > 0) return await InstantTrainTroops(compPlayerData.Structures, compPlayerData.Troops, buildingLoc);
        //    else return new Response(CaseType.Invalid, "CompPlayerData was null");
        //}
        //public async Task<Response> InstantTrainTroops(IReadOnlyList<StructureInfos> structures, IReadOnlyList<TroopInfos> troops, int buildingLoc)
        //{
        //    if (structures == null || !structures.Any() || troops == null || !troops.Any()) return new Response(CaseType.Success, "Instant training troops completed");

        //    var structure = structures.Select(x => x.Buildings.FirstOrDefault(y => y.Location == buildingLoc)).FirstOrDefault();
        //    if (structure == null) return new Response(CaseType.Success, "Instant training troops completed");

        //    //var troop = troops.Select(x => x.TroopData.FirstOrDefault(y => y.InTraning != null && y.InTraning.FirstOrDefault(z => z.BuildingLocId == buildingLoc) != null)).FirstOrDefault();

        //    foreach (var troopd in troops)
        //    {
        //        if (troopd != null)
        //        {
        //            var troop = troopd.TroopData.FirstOrDefault(x => x.InTraning != null);
        //            foreach (var traning in troop.InTraning)
        //            {

        //            }
        //        }
        //    }
        //}
    }
}

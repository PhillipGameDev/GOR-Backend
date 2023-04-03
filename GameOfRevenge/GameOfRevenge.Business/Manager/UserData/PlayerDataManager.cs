﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Services;

namespace GameOfRevenge.Business.Manager.UserData
{
    public class PlayerDataManager : BaseManager, IPlayerDataManager
    {
        //TODO: STORED PROCEDURE NOT IMPLEMENTED ON SERVER
        public async Task<Response> AddOrUpdateAllPlayerData(List<PlayerDataTable> playerDatas)
        {
            try
            {
                if (playerDatas == null || playerDatas.Count <= 0) throw new InvalidModelExecption("Invalid values");

                if (playerDatas != null && playerDatas.Any())
                {
                    var players = playerDatas.GroupBy(x => x.PlayerId);

                    foreach (var item in players)
                    {
                        var playerId = item.Key;
                        var playerDataIds = string.Empty;
                        var playerDataValues = string.Empty;

                        if (item.Any())
                        {
                            foreach (var data in item)
                            {
                                if (data != null && data.PlayerId == playerId)
                                {
                                    playerDataIds += ", " + data.Id;
                                    playerDataValues += ", " + data.Value;
                                }
                            }

                            if (playerDataIds.Length >= 3)
                            {
                                playerDataIds = playerDataIds.Substring(1, playerDataIds.Length - 1);
                            }

                            if (playerDataValues.Length >= 3)
                            {
                                playerDataValues = playerDataValues.Substring(1, playerDataValues.Length - 1);
                            }

                            var spParams = new Dictionary<string, object>()
                            {
                                { "PlayerId", playerId },
                                { "PlayerDataIds", playerDataIds },
                                { "PlayerDataValues", playerDataValues },
                            };

                            await Db.ExecuteSPMultipleRow<PlayerDataTable>("AddOrUpdatePlayerDataMultiple", spParams);
                        }
                    }

                    return new Response()
                    {
                        Case = 100,
                        Message = "Updated players data"
                    };
                }
                else
                {
                    return new Response()
                    {
                        Case = 299,
                        Message = "Invalid player datas"
                    };
                }
            }
            catch (InvalidModelExecption ex)
            {
                return new Response()
                {
                    Case = 200,
                    Message = ex.Message
                };
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
 
/*        public async Task<Response<PlayerDataTable>> AddOrUpdatePlayerData(int playerId, DataType type, int valueId, string value)
        {
            try
            {
                if (playerId <= 0) throw new InvalidModelExecption("Invalid player id");
                if (valueId <= 0) throw new InvalidModelExecption("Invalid value id");

                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId },
                    { "DataCode", type.ToString() },
                    { "ValueId", valueId },
                    { "Value", value }
                };
                return await Db.ExecuteSPSingleRow<PlayerDataTable>("AddOrUpdatePlayerData", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<PlayerDataTable>()
                {
                    Case = 200,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<PlayerDataTable>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }*/

        public async Task<Response<PlayerDataTableUpdated>> AddOrUpdatePlayerData(int playerId, DataType type, int valueId, string value, bool unique = true)
        {
            try
            {
                if (playerId <= 0) throw new InvalidModelExecption("Invalid player id");
                if (valueId <= 0) throw new InvalidModelExecption("Invalid value id");

                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId },
                    { "DataCode", type.ToString() },
                    { "ValueId", valueId },
                    { "Value", value }
                };
                if (unique) spParams.Add("Unique", 1);
                //TODO: Implement a way to only optionally override data (initData true/false)

                return await Db.ExecuteSPSingleRow<PlayerDataTableUpdated>("AddOrUpdatePlayerData", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<PlayerDataTableUpdated>()
                {
                    Case = 200,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<PlayerDataTableUpdated>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

/*        public async Task<Response<PlayerDataTableUpdated>> UpdatePlayerData(int playerId, DataType type, int valueId, string value)
        {
            try
            {
                if (playerId <= 0) throw new InvalidModelExecption("Invalid player id");
                if (valueId <= 0) throw new InvalidModelExecption("Invalid value id");

                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId },
                    { "DataCode", type.ToString() },
                    { "ValueId", valueId },
                    { "Value", value }
                };
                return await Db.ExecuteSPSingleRow<PlayerDataTableUpdated>("UpdatePlayerData", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<PlayerDataTableUpdated>()
                {
                    Case = 200,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<PlayerDataTableUpdated>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }*/

        public async Task<Response<PlayerDataTableUpdated>> UpdatePlayerDataID(int playerId, long playerDataId, string value)
        {
            try
            {
                if (playerId <= 0) throw new InvalidModelExecption("Invalid player id");
                if (playerDataId <= 0) throw new InvalidModelExecption("Invalid data id");

                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId },
                    { "PlayerDataId", playerDataId },
                    { "Value", value }
                };
                return await Db.ExecuteSPSingleRow<PlayerDataTableUpdated>("UpdatePlayerDataID", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<PlayerDataTableUpdated>()
                {
                    Case = 200,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<PlayerDataTableUpdated>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<PlayerDataTableUpdated>> IncrementPlayerData(int playerId, DataType type, int valueId, int? value)
        {
            try
            {
                if (playerId <= 0) throw new InvalidModelExecption("Invalid player id");
                if (valueId <= 0) throw new InvalidModelExecption("Invalid value id");

                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId },
                    { "DataCode", type.ToString() },
                    { "ValueId", valueId }
                };
                if (value != null) spParams.Add("Value", value);

                return await Db.ExecuteSPSingleRow<PlayerDataTableUpdated>("IncrementPlayerData", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<PlayerDataTableUpdated>()
                {
                    Case = 200,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<PlayerDataTableUpdated>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<PlayerDataTableUpdated>> SumPlayerData(int playerId, long playerDataId, int value)
        {
            try
            {
                if (playerId <= 0) throw new InvalidModelExecption("Invalid player id");
                if (playerDataId <= 0) throw new InvalidModelExecption("Invalid data id");

                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId },
                    { "PlayerDataId", playerDataId },
                    { "Value", value }
                };
                return await Db.ExecuteSPSingleRow<PlayerDataTableUpdated>("IncrementPlayerData", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<PlayerDataTableUpdated>()
                {
                    Case = 200,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<PlayerDataTableUpdated>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }


        public async Task<Response> RemoveAllPlayerData(int playerId)
        {
            try
            {
                if (playerId <= 0) throw new InvalidModelExecption("Invalid player id");

                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId },
                };

                return await Db.ExecuteSPNoData("RemoveAllPlayerData", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response()
                {
                    Case = 200,
                    Message = ex.Message
                };
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

        public async Task<Response<PlayerDataTableUpdated>> RemovePlayerData(int playerId, DataType type, int valueId)
        {
            return await AddOrUpdatePlayerData(playerId, type, valueId, null);
        }

        public async Task<Response<PlayerDataTableUpdated>> RemovePlayerData(int playerId, long playerDataId)
        {
            return await UpdatePlayerDataID(playerId, playerDataId, null);
        }


        public async Task<Response<List<PlayerDataTable>>> GetAllPlayerData(int playerId, DataType type = DataType.Unknown)
        {
            try
            {
                if (playerId <= 0) throw new InvalidModelExecption("Invalid player id");

                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId }
                };
                if (type != DataType.Unknown) spParams.Add("DataCode", type.ToString());

                return await Db.ExecuteSPMultipleRow<PlayerDataTable>("GetAllPlayerData", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<PlayerDataTable>>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<PlayerDataTable>>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<List<PlayerDataTable>>> GetAllPlayerData(int playerId, DataType type, int valueId)
        {
            try
            {
                if (playerId <= 0) throw new InvalidModelExecption("Invalid player id");
                if (valueId <= 0) throw new InvalidModelExecption("Invalid value id");

                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId },
                    { "DataCode", type.ToString() }
                };
                spParams.Add("ValueId", valueId);

                return await Db.ExecuteSPMultipleRow<PlayerDataTable>("GetAllPlayerData", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<PlayerDataTable>>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<PlayerDataTable>>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }
        public async Task<Response<PlayerDataTable>> GetPlayerData(int playerId, DataType type, int valueId)
        {
            try
            {
                if (playerId < 1) throw new InvalidModelExecption("Invalid player id");
                if (valueId < 1) throw new InvalidModelExecption("Invalid value id");

                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId },
                    { "DataCode", type.ToString() },
                    { "ValueId", valueId }
                };

                return await Db.ExecuteSPSingleRow<PlayerDataTable>("GetPlayerData", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<PlayerDataTable>()
                {
                    Case = 200,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<PlayerDataTable>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }
        public async Task<Response<PlayerDataTable>> GetPlayerDataById(long playerDataId)
        {
            try
            {
                if (playerDataId <= 0) throw new InvalidModelExecption("Invalid player data id");

                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerDataId", playerDataId },
                };

                return await Db.ExecuteSPSingleRow<PlayerDataTable>("GetPlayerDataById", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<PlayerDataTable>()
                {
                    Case = 200,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<PlayerDataTable>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<List<PlayerDataTable>>> AddPlayerResourceData(int playerId, int food, int wood, int ore, int gem)
        {
            try
            {
                var spParams = ResourceUpdateSpParams(playerId, food, wood, ore, gem);
                return await Db.ExecuteSPMultipleRow<PlayerDataTable>("AddPlayerResourceData", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<PlayerDataTable>>()
                {
                    Case = 200,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<PlayerDataTable>>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }
        public async Task<Response<List<PlayerDataTable>>> RemovePlayerResourceData(int playerId, int food, int wood, int ore, int gem)
        {
            try
            {
                var spParams = ResourceUpdateSpParams(playerId, food, wood, ore, gem);
                return await Db.ExecuteSPMultipleRow<PlayerDataTable>("RemovePlayerResourceData", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<PlayerDataTable>>()
                {
                    Case = 200,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<PlayerDataTable>>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }
        public async Task<Response<List<PlayerDataTable>>> UpdatePlayerResourceData(int playerId, int food, int wood, int ore, int gem)
        {
            try
            {
                var spParams = ResourceUpdateSpParams(playerId, food, wood, ore, gem);
                return await Db.ExecuteSPMultipleRow<PlayerDataTable>("UpdatePlayerResourceData", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<PlayerDataTable>>()
                {
                    Case = 200,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<PlayerDataTable>>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<List<StoredDataTable>>> GetAllPlayerStoredData(int playerId, int structureLocationId = -1)
        {
            try
            {
                if (playerId <= 0) throw new InvalidModelExecption("Invalid player id");
                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId }
                };
                if (structureLocationId != -1) spParams.Add("StructureLocationId", structureLocationId);

                return await Db.ExecuteSPMultipleRow<StoredDataTable>("GetAllPlayerStoredData", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<StoredDataTable>>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<StoredDataTable>>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response> StoreResource(int playerId, int structureLocationId, int valueId, int value)
        {
//            var response = await manager.TransferResource(playerId, structureLocationId, DataType.Resource, resId, value);

            var spParams = new Dictionary<string, object>()
            {
                { "PlayerId", playerId },
                { "StructureLocationId", structureLocationId },
                { "DataTypeId", DataType.Resource },
                { "ValueId", valueId },
                { "Value", value }
            };

            try
            {
                return await Db.ExecuteSPNoData("AddOrUpdatePlayerStoredData", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response()
                {
                    Case = 200,
                    Message = ex.Message
                };
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

        public async Task<Response<RankingElement>> GetRanking(int playerId)
        {
            var spParams = new Dictionary<string, object>()
            {
                { "PlayerId", playerId }
            };

            try
            {
                return await Db.ExecuteSPSingleRow<RankingElement>("GetRanking", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<RankingElement>()
                {
                    Case = 200,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<RankingElement>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<List<RankingElement>>> GetRankings(long rankId = 0)
        {
            var spParams = new Dictionary<string, object>()
            {
                { "RankId", rankId }
            };

            try
            {
                return await Db.ExecuteSPMultipleRow<RankingElement>("GetRankings", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<RankingElement>>()
                {
                    Case = 200,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<RankingElement>>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        private static Dictionary<string, object> ResourceUpdateSpParams(int playerId, int food, int wood, int ore, int gem, bool showResult = true)
        {
            if (playerId <= 0) throw new InvalidModelExecption("Invalid player id");

            var spParams = new Dictionary<string, object>()
            {
                { "PlayerId", playerId },
                { "ShowResult", showResult }
            };

            if (food > 0) spParams.Add("Food", food);
            if (wood > 0) spParams.Add("Wood", wood);
            if (ore > 0) spParams.Add("Ore", ore);
            if (gem > 0) spParams.Add("Gem", gem);

            return spParams;
        }
    }
}

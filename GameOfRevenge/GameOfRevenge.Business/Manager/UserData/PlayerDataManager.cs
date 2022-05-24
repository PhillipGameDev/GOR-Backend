using System;
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
        public async Task<Response> AddOrUpdateAllPlayerData(List<PlayerDataTable> playerDatas)
        {
            try
            {
                if (playerDatas == null || playerDatas.Count <= 0) throw new InvalidModelExecption("Invalid values was provided");

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
                                    playerDataValues += ", " + data.Id;
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
        public async Task<Response<PlayerDataTable>> AddOrUpdatePlayerData(int playerId, DataType type, int valueId, string value)
        {
            try
            {
                if (playerId <= 0) throw new InvalidModelExecption("Invalid id was provided");
                if (valueId <= 0) throw new InvalidModelExecption("Invalid value id was provided");

                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId },
                    { "DataCode", type.ToString() },
                    { "ValueId", valueId },
                    { "Value", value },
                };
                return await Db.ExecuteSPSingleRow<PlayerDataTable>("AddPlayerData", spParams);
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

        public async Task<Response<List<PlayerDataTable>>> GetAllPlayerData(int playerId)
        {
            try
            {
                if (playerId <= 0) throw new InvalidModelExecption("Invalid id was provided");
                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId }
                };

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
                if (playerId <= 0) throw new InvalidModelExecption("Invalid id was provided");
                if (valueId <= 0) throw new InvalidModelExecption("Invalid value id was provided");

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
        public async Task<Response<PlayerDataTable>> GetPlayerDataById(int playerDataId)
        {
            try
            {
                if (playerDataId <= 0) throw new InvalidModelExecption("Invalid id was provided");

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

        public async Task<Response> RemoveAllPlayerData(int playerId)
        {
            try
            {
                if (playerId <= 0) throw new InvalidModelExecption("Invalid id was provided");

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
        public async Task<Response> RemovePlayerData(int playerId, DataType type, int valueId)
        {
            try
            {
                if (playerId <= 0) throw new InvalidModelExecption("Invalid id was provided");
                if (valueId <= 0) throw new InvalidModelExecption("Invalid value id was provided");

                var resp = await AddOrUpdatePlayerData(playerId, type, valueId, string.Empty);
                return new Response()
                {
                    Case = resp.Case,
                    Message = resp.Message
                };
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

        private static Dictionary<string, object> ResourceUpdateSpParams(int playerId, int food, int wood, int ore, int gem, bool showResult = true)
        {
            if (playerId <= 0) throw new InvalidModelExecption("Invalid id was provided");

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

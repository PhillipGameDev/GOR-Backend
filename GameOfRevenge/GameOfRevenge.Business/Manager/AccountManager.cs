using System;
using System.Threading.Tasks;
using GameOfRevenge.Common.Services;
using GameOfRevenge.Common;
using GameOfRevenge.Business.Manager.UserData;
using System.Collections.Generic;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Structure;
using System.Linq;
using Newtonsoft.Json;

namespace GameOfRevenge.Business.Manager
{
    public class AccountManager : BaseManager, IAccountManager
    {
        private readonly IUserResourceManager resManager = new UserResourceManager();
        private readonly IUserStructureManager strManager = new UserStructureManager();

        public async Task<Response<Player>> TryLoginOrRegister(string identifier, string name, bool accepted)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(identifier)) throw new InvalidModelExecption("Invalid identifier was provided");
                else identifier = identifier.Trim();
                if (string.IsNullOrWhiteSpace(name)) name = "Guest";
                else name = name.Trim();
                if (accepted)
                {
                    var spParams = new Dictionary<string, object>()
                    {
                        { "Identifier", identifier },
                        { "Name", name },
                        { "Accepted", accepted }
                    };

                    var response = await Db.ExecuteSPSingleRow<Player>("TryLoginOrRegister", spParams);

                    //todo add in database
                    if (response.IsSuccess && response.HasData && response.Case == 100)
                    {
                        int playerId = response.Data.PlayerId;
                        await resManager.SumMainResource(playerId, 10000, 10000, 10000, 100);
#if DEBUG
                        await resManager.SumMainResource(playerId, 100000, 100000, 100000, 10000);
#endif
                        var cityCounselLoc = CacheData.CacheStructureDataManager.GetFullStructureData(StructureType.CityCounsel).Locations.FirstOrDefault();
                        var gateLoc = CacheData.CacheStructureDataManager.GetFullStructureData(StructureType.Gate).Locations.FirstOrDefault();
                        var wtLoc = CacheData.CacheStructureDataManager.GetFullStructureData(StructureType.WatchTower).Locations.FirstOrDefault();

                        var dataManager = new PlayerDataManager();
                        var king = new UserKingDetails
                        {
                            MaxStamina = 20
                        };
                        var json = JsonConvert.SerializeObject(king);
                        await dataManager.AddOrUpdatePlayerData(playerId, DataType.Custom, 1, json);

                        var builder = new UserBuilderDetails();
                        json = JsonConvert.SerializeObject(builder);
                        await dataManager.AddOrUpdatePlayerData(playerId, DataType.Custom, 2, json);

                        await strManager.CreateBuilding(playerId, StructureType.CityCounsel, cityCounselLoc, false, false);
                        await strManager.UpgradeBuilding(playerId, StructureType.CityCounsel, cityCounselLoc, false, false);

                        await strManager.CreateBuilding(playerId, StructureType.Gate, gateLoc, false, false);
                        await strManager.UpgradeBuilding(playerId, StructureType.Gate, gateLoc, false, false);

                        await strManager.CreateBuilding(playerId, StructureType.WatchTower, wtLoc, false, false);
                        await strManager.UpgradeBuilding(playerId, StructureType.WatchTower, wtLoc, false, false);
                    }

                    return response;
                }
                else throw new InvalidModelExecption("Kindly accept terms and condition");
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<Player>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<Player>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<Player>> GetAccountInfo(string identifier)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(identifier)) throw new InvalidModelExecption("Invalid identifier was provided");

                var spParams = new Dictionary<string, object>()
                {
                    { "Identifier", identifier },
                };

                return await Db.ExecuteSPSingleRow<Player>("GetPlayerDetailsByIdentifier", spParams);

            }
            catch (InvalidModelExecption ex)
            {
                return new Response<Player>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<Player>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<Player>> GetAccountInfo(int userId)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", userId },
                };

                return await Db.ExecuteSPSingleRow<Player>("GetPlayerDetailsById", spParams);

            }
            catch (InvalidModelExecption ex)
            {
                return new Response<Player>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<Player>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }


        public async Task<Response<PlayerTutorialData>> GetTutorialInfo(string identifier)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerIdentifier", identifier },
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

        public async Task<Response<PlayerTutorialData>> UpdateTutorialInfo(string identifier, string playerData, bool isComplete)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerIdentifier", identifier },
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
    }
}

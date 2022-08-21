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
                        await resManager.AddMainResource(response.Data.PlayerId, 10000, 10000, 10000, 100);
#if DEBUG
                        await resManager.AddMainResource(response.Data.PlayerId, 100000, 100000, 100000, 10000);
#endif
                        var cityCounselLocs = CacheData.CacheStructureDataManager.GetFullStructureData(StructureType.CityCounsel).Locations;
                        var gateLocs = CacheData.CacheStructureDataManager.GetFullStructureData(StructureType.Gate).Locations;
                        var wtLocs = CacheData.CacheStructureDataManager.GetFullStructureData(StructureType.WatchTower).Locations;

                        await strManager.CreateBuilding(response.Data.PlayerId, StructureType.CityCounsel, cityCounselLocs.FirstOrDefault());
                        await strManager.CreateBuilding(response.Data.PlayerId, StructureType.Gate, gateLocs.FirstOrDefault());
                        await strManager.CreateBuilding(response.Data.PlayerId, StructureType.WatchTower, wtLocs.FirstOrDefault());
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

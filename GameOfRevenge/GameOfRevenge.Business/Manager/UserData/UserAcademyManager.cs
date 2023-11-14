using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common;
using System;
using GameOfRevenge.Common.Services;
using GameOfRevenge.Common.Models.Academy;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Common.Interface.UserData;

namespace GameOfRevenge.Business.Manager.UserData
{
    public class UserAcademyManager : BaseManager, IUserAcademyManager
    {
        private readonly IUserResourceManager userResourceManager = new UserResourceManager();
        public async Task<Response<List<AcademyUserDataTable>>> GetUserAllItems(int playerId)
        {
            try
            {
                if (playerId <= 0) throw new InvalidModelExecption("Invalid player id");

                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId }
                };

                return await Db.ExecuteSPMultipleRow<AcademyUserDataTable>("GetPlayerAcademyItems", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<AcademyUserDataTable>>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<AcademyUserDataTable>>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<AcademyUserDataTable>> UpgradeItem(int playerId, int itemId, DateTime startTime)
        {
            try
            {
                if (playerId <= 0) throw new InvalidModelExecption("Invalid player id");

                var item = CacheAcademyDataManager.AllAcademyItems.First(e => e.Id == itemId);
                var userItem = (await GetUserAllItems(playerId)).Data.Find(e => e.ItemId == itemId);

                int level = userItem == null ? 0 : userItem.Level;

                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId },
                    { "ItemId", itemId },
                    { "Level", level + 1},
                    { "StartTime", startTime },
                    { "Duration", item.Duration * (int)Math.Pow(item.Multiple, level) },
                };

                var resp = await Db.ExecuteSPSingleRow<AcademyUserDataTable>("AcademyUpgradeItem", spParams);

                if (resp.IsSuccess)
                {
                    var requirements = CacheAcademyDataManager.AllAcademyRequirements.Where(e => e.ItemId == itemId).ToList();

                    foreach (var req in requirements)
                    {
                        await userResourceManager.SumResource(playerId, req.ResourceType, -AlgorithmService.GetValue(req.AlgorithmType, req.InitValue, req.Key, level));
                    }
                }

                return new Response<AcademyUserDataTable>()
                {
                    Case = resp.Case,
                    Data = resp.Data,
                    Message = resp.Message
                };
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<AcademyUserDataTable>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<AcademyUserDataTable>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<AcademyUserDataTable>> InstantUpgradeItem(int playerId, int itemId)
        {
            try
            {
                if (playerId <= 0) throw new InvalidModelExecption("Invalid player id");

                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId },
                    { "ItemId", itemId }
                };

                return await Db.ExecuteSPSingleRow<AcademyUserDataTable>("AcademyUpgradeItemBoost", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<AcademyUserDataTable>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<AcademyUserDataTable>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<AcademyUserDataTable>> SpeedUpUpgradeItem(int playerId, int itemId, int duration)
        {
            try
            {
                if (playerId <= 0) throw new InvalidModelExecption("Invalid player id");

                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId },
                    { "ItemId", itemId },
                    { "Duration", duration }
                };

                return await Db.ExecuteSPSingleRow<AcademyUserDataTable>("AcademyUpgradeItemBoost", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<AcademyUserDataTable>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<AcademyUserDataTable>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }
    }
}

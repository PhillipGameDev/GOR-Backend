using System;
using System.Threading.Tasks;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Common.Models.Inventory;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common.Models;
using Newtonsoft.Json;
using GameOfRevenge.Common.Services;
using GameOfRevenge.Business.CacheData;

namespace GameOfRevenge.Business.Manager.UserData
{
    public class UserActiveBuffsManager : BaseUserDataManager, IUserActiveBuffsManager
    {
        public async Task<Response<UserBuffData>> AddBuff(int playerId, InventoryItemType type) => await AddBuff(playerId, type, 1);
        public async Task<Response<UserBuffData>> AddBuff(int playerId, InventoryItemType type, int count)
        {
            try
            {
                BuffType buffType = GetBuffType(type);
                if (BuffType.Other == buffType) throw new DataNotExistExecption("Buff was null");
                var timeStamp = DateTime.UtcNow;
                if (count <= 0) return new Response<UserBuffData>(CaseType.Success, "Count was zero");

                int invId = CacheInventoryDataManager.GetFullInventoryItemData(type).Info.Id;
                var buffRespData = await manager.GetPlayerData(playerId, DataType.ActiveBuffs, invId);
                UserBuffData buffData;

                if (buffRespData.IsSuccess && buffRespData.HasData)
                {
                    buffData = PlayerDataToUserBuffData(buffRespData.Data);
                    if (buffData == null) throw new DataNotExistExecption("Buff was null");
                    if (buffData.Value == null || buffData.Value.TimeLeft <= 0)
                    {
                        buffData.Value = new UserBuffDetails
                        {
                            BuffType = buffType,
                            StartTime = timeStamp,
                            EndTime = timeStamp.AddDays(count)
                        };
                    }
                    else
                    {
                        buffData.Value.EndTime = buffData.Value.EndTime.AddDays(count);
                    }
                }
                else
                {
                    buffData = new UserBuffData()
                    {
                        DataType = DataType.ActiveBuffs,
                        Value = new UserBuffDetails()
                        {
                            BuffType = buffType,
                            StartTime = timeStamp,
                            EndTime = timeStamp.AddDays(count)
                        },
                        ValueId = type
                    };
                }

                var resp = await manager.AddOrUpdatePlayerData(playerId, DataType.ActiveBuffs, invId, JsonConvert.SerializeObject(buffData.Value));
                if (resp.IsSuccess & resp.HasData) return new Response<UserBuffData>(buffData, resp.Case, resp.Message);
                else return new Response<UserBuffData>(resp.Case, resp.Message);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<UserBuffData>()
                {
                    Case = 200,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<UserBuffData>()
                {
                    Case = 201,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (RequirementExecption ex)
            {
                return new Response<UserBuffData>()
                {
                    Case = 202,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (Exception)
            {
                return new Response<UserBuffData>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError()
                };
            }

        }

        public async Task<Response<UserBuffData>> RemoveBuff(int playerId, InventoryItemType type)
        {
            try
            {
                if (BuffType.Other == GetBuffType(type)) throw new DataNotExistExecption("Buff was null");

                var timeStamp = DateTime.UtcNow;
                var resp = await manager.AddOrUpdatePlayerData(playerId, DataType.ActiveBuffs, CacheInventoryDataManager.GetFullInventoryItemData(type).Info.Id, string.Empty);
                if (resp.IsSuccess & resp.HasData)
                {
                    var buffData = PlayerDataToUserBuffData(resp.Data);
                    return new Response<UserBuffData>(buffData, resp.Case, resp.Message);
                }
                else return new Response<UserBuffData>(resp.Case, resp.Message);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<UserBuffData>()
                {
                    Case = 200,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<UserBuffData>()
                {
                    Case = 201,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (RequirementExecption ex)
            {
                return new Response<UserBuffData>()
                {
                    Case = 202,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (Exception)
            {
                return new Response<UserBuffData>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError()
                };
            }
        }
        public async Task<Response<UserBuffData>> RemoveBuff(int playerId, InventoryItemType type, int count)
        {
            try
            {
                BuffType buffType = GetBuffType(type);
                if (BuffType.Other == buffType) throw new DataNotExistExecption("Buff was null");

                var timeStamp = DateTime.UtcNow;
                if (count <= 0) return new Response<UserBuffData>(CaseType.Success, "Count was zero");

                int invId = CacheInventoryDataManager.GetFullInventoryItemData(type).Info.Id;
                var buffRespData = await manager.GetPlayerData(playerId, DataType.ActiveBuffs, invId);
                UserBuffData buffData = null;

                if (buffRespData.IsSuccess && buffRespData.HasData)
                {
                    buffData = PlayerDataToUserBuffData(buffRespData.Data);
                    if (buffData != null)
                    {
                        if (buffData.Value == null || buffData.Value.TimeLeft <= 0)
                        {
                            buffData.Value = new UserBuffDetails
                            {
                                BuffType = buffType,
                                StartTime = timeStamp,
                                EndTime = timeStamp.AddDays(-count)
                            };
                        }
                        else
                        {
                            buffData.Value.EndTime = buffData.Value.EndTime.AddDays(-count);
                        }
                    }
                }

                if (buffData.Value.TimeLeft <= 0) buffData.Value = null;
                string strValue = buffData.Value == null ? string.Empty : JsonConvert.SerializeObject(buffData.Value);
                var resp = await manager.AddOrUpdatePlayerData(playerId, DataType.ActiveBuffs, invId, strValue);
                if (resp.IsSuccess & resp.HasData) return new Response<UserBuffData>(buffData, resp.Case, resp.Message);
                else return new Response<UserBuffData>(resp.Case, resp.Message);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<UserBuffData>()
                {
                    Case = 200,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<UserBuffData>()
                {
                    Case = 201,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (RequirementExecption ex)
            {
                return new Response<UserBuffData>()
                {
                    Case = 202,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (Exception)
            {
                return new Response<UserBuffData>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError()
                };
            }
        }
    }
}

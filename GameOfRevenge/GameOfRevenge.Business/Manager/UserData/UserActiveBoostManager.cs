using System;
using System.Threading.Tasks;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Common.Models.Boost;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common.Models;
using Newtonsoft.Json;
using GameOfRevenge.Common.Services;
using GameOfRevenge.Business.CacheData;

namespace GameOfRevenge.Business.Manager.UserData
{
    public class UserActiveBoostManager : BaseUserDataManager, IUserActiveBoostsManager
    {
        public async Task<Response<UserBoostData>> AddBoost(int playerId, BoostType type) => await AddBoost(playerId, type, 1);
        public async Task<Response<UserBoostData>> AddBoost(int playerId, BoostType type, int count)
        {
            try
            {
                BoostType boostType = GetBoostType(type);
                if (BoostType.Unknown == boostType) throw new DataNotExistExecption("Boost was null");
                var timeStamp = DateTime.UtcNow;
                if (count <= 0) return new Response<UserBoostData>(CaseType.Success, "Count was zero");

//                int invId = CacheBoostDataManager.GetFullBoostDataByType(type).Info.BoostTypeId;
                int valueId = CacheBoostDataManager.GetFullBoostDataByType(type).BoostTypeId;//CacheBoostDataManager.GetFullBoostDataByType(type).Info.BoostTypeId;
                var boostRespData = await manager.GetPlayerData(playerId, DataType.ActiveBoost, valueId);
                UserBoostData boostData;

                if (boostRespData.IsSuccess && boostRespData.HasData)
                {
                    boostData = PlayerDataToUserBoostData(boostRespData.Data);
                    if (boostData == null) throw new DataNotExistExecption("Boost was null");
                    if (boostData.Value == null || boostData.Value.TimeLeft <= 0)
                    {
                        boostData.Value = new FullUserBoostDetails
                        {
                            Id = boostData.Id,
                            BoostType = boostType,
                            StartTime = timeStamp,
                            EndTime = timeStamp.AddDays(count)
                        };
                    }
                    else
                    {
                        boostData.Value.EndTime = boostData.Value.EndTime.AddDays(count);
                    }
                }
                else
                {
                    boostData = new UserBoostData()
                    {
                        DataType = DataType.ActiveBoost,
                        Value = new UserBoostDetails()
                        {
                            BoostType = boostType,
                            StartTime = timeStamp,
                            EndTime = timeStamp.AddDays(count)
                        },
                        ValueId = type
                    };
                }

                var resp = await manager.AddOrUpdatePlayerData(playerId, DataType.ActiveBoost, valueId, JsonConvert.SerializeObject(boostData.Value));
                if (resp.IsSuccess & resp.HasData) return new Response<UserBoostData>(boostData, resp.Case, resp.Message);
                else return new Response<UserBoostData>(resp.Case, resp.Message);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<UserBoostData>()
                {
                    Case = 200,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<UserBoostData>()
                {
                    Case = 201,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (RequirementExecption ex)
            {
                return new Response<UserBoostData>()
                {
                    Case = 202,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (Exception)
            {
                return new Response<UserBoostData>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError()
                };
            }

        }

        public async Task<Response<UserBoostData>> RemoveBoost(int playerId, BoostType type)
        {
            try
            {
                if (BoostType.Unknown == GetBoostType(type)) throw new DataNotExistExecption("Boost was null");

                var timeStamp = DateTime.UtcNow;
                int valueId = CacheBoostDataManager.GetFullBoostDataByType(type).BoostTypeId;
//                var valueId = CacheBoostDataManager.GetFullBoostDataByType(type).Info.BoostTypeId;
                var resp = await manager.AddOrUpdatePlayerData(playerId, DataType.ActiveBoost, valueId, string.Empty);
                if (resp.IsSuccess & resp.HasData)
                {
                    var buffData = PlayerDataToUserBoostData(resp.Data);
                    return new Response<UserBoostData>(buffData, resp.Case, resp.Message);
                }
                else return new Response<UserBoostData>(resp.Case, resp.Message);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<UserBoostData>()
                {
                    Case = 200,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<UserBoostData>()
                {
                    Case = 201,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (RequirementExecption ex)
            {
                return new Response<UserBoostData>()
                {
                    Case = 202,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (Exception)
            {
                return new Response<UserBoostData>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError()
                };
            }
        }
        public async Task<Response<UserBoostData>> RemoveBoost(int playerId, BoostType type, int count)
        {
            try
            {
                BoostType buffType = GetBoostType(type);
                if (BoostType.Unknown == buffType) throw new DataNotExistExecption("Boost was null");

                var timeStamp = DateTime.UtcNow;
                if (count <= 0) return new Response<UserBoostData>(CaseType.Success, "Count was zero");

                var invId = CacheBoostDataManager.GetFullBoostDataByType(type).BoostTypeId;
                var buffRespData = await manager.GetPlayerData(playerId, DataType.ActiveBoost, invId);
                UserBoostData buffData = null;

                if (buffRespData.IsSuccess && buffRespData.HasData)
                {
                    buffData = PlayerDataToUserBoostData(buffRespData.Data);
                    if (buffData != null)
                    {
                        if (buffData.Value == null || buffData.Value.TimeLeft <= 0)
                        {
                            buffData.Value = new FullUserBoostDetails
                            {
                                Id = buffData.Id,
                                BoostType = buffType,
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
                var resp = await manager.AddOrUpdatePlayerData(playerId, DataType.ActiveBoost, invId, strValue);
                if (resp.IsSuccess & resp.HasData) return new Response<UserBoostData>(buffData, resp.Case, resp.Message);
                else return new Response<UserBoostData>(resp.Case, resp.Message);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<UserBoostData>()
                {
                    Case = 200,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<UserBoostData>()
                {
                    Case = 201,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (RequirementExecption ex)
            {
                return new Response<UserBoostData>()
                {
                    Case = 202,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (Exception)
            {
                return new Response<UserBoostData>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError()
                };
            }
        }
    }
}

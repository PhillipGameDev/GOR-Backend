using System;
using System.Threading.Tasks;
using System.Linq;
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
using System.Collections.Generic;

namespace GameOfRevenge.Business.Manager.UserData
{
    public class UserActiveBoostManager : BaseUserDataManager, IUserActiveBoostsManager
    {
        public async Task<Response<List<UserRecordNewBoost>>> GetAllPlayerActiveBoostData(int playerId)
        {
            try
            {
                if (playerId < 1) throw new InvalidModelExecption("Invalid player id");

                var list = new List<UserRecordNewBoost>();
                var response = await manager.GetAllPlayerData(playerId, DataType.ActiveBoost);
                if (response.IsSuccess && response.HasData)
                {
                    foreach (var data in response.Data)
                    {
                        UserNewBoost boost = null;
                        try
                        {
                            boost = JsonConvert.DeserializeObject<UserNewBoost>(data.Value);
                        }
                        catch {}

                        if (boost?.TimeLeft > 0)
                        {
                            list.Add(new UserRecordNewBoost(data.Id, boost));
                        }
                    }
                }

                return new Response<List<UserRecordNewBoost>>(list);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<UserRecordNewBoost>>() { Case = 200, Message = ErrorManager.ShowError(ex) };
            }
            catch (Exception)
            {
                return new Response<List<UserRecordNewBoost>>() { Case = 0, Message = ErrorManager.ShowError() };
            }
        }

        public async Task<Response<BoostActivatedResponse>> ActivateBoost(int playerId, CityBoostType type, int seconds, int reqGems)
        {
            try
            {
                if (playerId < 1) throw new InvalidModelExecption("Invalid player id");

                if (type == CityBoostType.Unknown) throw new DataNotExistExecption("Boost not supported");

                var timeStamp = DateTime.UtcNow;

                var allData = await GetAllPlayerData(playerId);
                if (!allData.IsSuccess || !allData.HasData) throw new DataNotExistExecption("Player data not found");

                var allPlayerData = allData.Data;
                PlayerDataTable gemsData = null;
                List<PlayerDataTable> changes = null;
                if (reqGems > 0)
                {
                    var gemsAmount = 0;
                    gemsData = allPlayerData.Find(x => (x.DataType == DataType.Resource) && (x.ValueId == (int)ResourceType.Gems));
                    if (gemsData != null) int.TryParse(gemsData.Value, out gemsAmount);
                    if (gemsAmount < reqGems) throw new RequirementExecption("Not enough gems");

                    var gemresp = await manager.SumPlayerData(playerId, gemsData.Id, -reqGems);
                    if (!gemresp.IsSuccess) throw new RequirementExecption("Unable to process your request");
                    changes = new List<PlayerDataTable>() { gemresp.Data.ToPlayerDataTable };
                }

                var boostRespData = allPlayerData.Find(x => (x.DataType == DataType.ActiveBoost) && (x.ValueId == (int)type));
                UserNewBoostData boostData = null;
                if (boostRespData != null)
                {
                    boostData = PlayerData.PlayerDataToUserNewBoostData(boostRespData);
                }

                UserNewBoost boostValue = boostData?.Value;
                if (boostValue?.TimeLeft <= 0) boostValue = null;
                if (boostValue == null)
                {
                    boostValue = new UserNewBoost()
                    {
                        Type = (NewBoostType)type,
                        StartTime = timeStamp
                    };
                }

/*                byte boostLevel = boostValue.Level;
                if (Enum.IsDefined(typeof(CityBoostType), (byte)type))
                {
                    var castle = allPlayerData.Find(x => (x.DataType == DataType.Structure) && (x.ValueId == (int)Common.Models.Structure.StructureType.CityCounsel));
                    //                var castle = await manager.GetPlayerData(playerId, DataType.Structure, (int)Common.Models.Structure.StructureType.CityCounsel);
                    var castleData = PlayerData.PlayerDataToUserStructureData(castle);//.Data);
                    var castleBuilding = castleData.Value.FirstOrDefault();// Sort((x, y) => x.Level.CompareTo(y.Level));
                    var castleLevel = castleBuilding.Level;
                    if (castleBuilding.TimeLeft > 0) castleLevel--;
                    if (castleLevel < 1) castleLevel = 1;
                    boostLevel = (byte)castleLevel;
                }*/

/*                int levelVal = 0;
                var specBoostData = CacheBoostDataManager.SpecNewBoostDatas.First(x => x.Type == type);
                if (specBoostData.Table > 0)
                {
                    if (specBoostData.Levels.ContainsKey(boostLevel))
                    {
                        int.TryParse(specBoostData.Levels[boostLevel].ToString(), out levelVal);
                    }
                }*/
                boostValue.Duration += seconds;


                var json = JsonConvert.SerializeObject(boostValue);
                Response<PlayerDataTableUpdated> resp;
                if (boostData != null)
                {
                    resp = await manager.UpdatePlayerDataID(playerId, boostData.Id, json);
                }
                else
                {
                    resp = await manager.AddOrUpdatePlayerData(playerId, DataType.ActiveBoost, (int)type, json);
                }

                if (!resp.IsSuccess && (reqGems > 0))
                {
                    //return gems
                    await manager.SumPlayerData(playerId, gemsData.Id, reqGems);

                    return new Response<BoostActivatedResponse>(resp.Case, resp.Message);
                }

                var response = new BoostActivatedResponse()
                {
                    Boost = new UserRecordNewBoost(resp.Data.Id, boostValue),
                    Changes = changes
                };
                return new Response<BoostActivatedResponse>(response, resp.Case, resp.Message);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<BoostActivatedResponse>() { Case = 200, Message = ErrorManager.ShowError(ex) };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<BoostActivatedResponse>() { Case = 201, Message = ErrorManager.ShowError(ex) };
            }
            catch (RequirementExecption ex)
            {
                return new Response<BoostActivatedResponse>() { Case = 202, Message = ErrorManager.ShowError(ex) };
            }
            catch (Exception ex)
            {
                return new Response<BoostActivatedResponse>() { Case = 0, Message = ErrorManager.ShowError(ex) };
            }
        }

        public async Task<Response<UserBoostData>> RemoveBoost(int playerId, NewBoostType type)
        {
            try
            {
                if (NewBoostType.Unknown == GetBoostType(type)) throw new DataNotExistExecption("Boost was null");

                var timeStamp = DateTime.UtcNow;
                int valueId = (int)type;//CacheBoostDataManager.GetFullBoostDataByType(type).BoostTypeId;
//                var valueId = CacheBoostDataManager.GetFullBoostDataByType(type).Info.BoostTypeId;
                var resp = await manager.AddOrUpdatePlayerData(playerId, DataType.ActiveBoost, valueId, string.Empty);
                if (resp.IsSuccess & resp.HasData)
                {
                    var buffData = PlayerData.PlayerDataToUserBoostData(resp.Data);
                    return new Response<UserBoostData>(buffData, resp.Case, resp.Message);
                }
                else return new Response<UserBoostData>(resp.Case, resp.Message);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<UserBoostData>() { Case = 200, Message = ErrorManager.ShowError(ex) };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<UserBoostData>() { Case = 201, Message = ErrorManager.ShowError(ex) };
            }
            catch (RequirementExecption ex)
            {
                return new Response<UserBoostData>() { Case = 202, Message = ErrorManager.ShowError(ex) };
            }
            catch (Exception)
            {
                return new Response<UserBoostData>() { Case = 0, Message = ErrorManager.ShowError() };
            }
        }

        public async Task<Response<UserBoostData>> RemoveBoost(int playerId, NewBoostType type, int count)
        {
            try
            {
                NewBoostType buffType = GetBoostType(type);
                if (NewBoostType.Unknown == buffType) throw new DataNotExistExecption("Boost was null");

                var timeStamp = DateTime.UtcNow;
                if (count <= 0) return new Response<UserBoostData>(CaseType.Success, "Count was zero");

                var invId = (int)type;//CacheBoostDataManager.GetFullBoostDataByType(type).BoostTypeId;
                var buffRespData = await manager.GetPlayerData(playerId, DataType.ActiveBoost, invId);
                UserBoostData buffData = null;

                if (buffRespData.IsSuccess && buffRespData.HasData)
                {
                    buffData = PlayerData.PlayerDataToUserBoostData(buffRespData.Data);
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
                return new Response<UserBoostData>() { Case = 200, Message = ErrorManager.ShowError(ex) };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<UserBoostData>() { Case = 201, Message = ErrorManager.ShowError(ex) };
            }
            catch (RequirementExecption ex)
            {
                return new Response<UserBoostData>() { Case = 202, Message = ErrorManager.ShowError(ex) };
            }
            catch (Exception)
            {
                return new Response<UserBoostData>() { Case = 0, Message = ErrorManager.ShowError() };
            }
        }
    }
}

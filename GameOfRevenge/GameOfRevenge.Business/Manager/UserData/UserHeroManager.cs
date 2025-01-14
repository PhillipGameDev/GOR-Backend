﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Hero;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Services;
using Newtonsoft.Json;

namespace GameOfRevenge.Business.Manager.UserData
{
    public class UserHeroManager : BaseUserDataManager, IUserHeroManager
    {
/*        public async Task<Response<UserHeroDetails>> GetHero(int playerId, int heroId)
        {
            try
            {
                var heroRespData = await manager.GetPlayerData(playerId, DataType.Hero, heroId);
                UserHeroData heroData;
                if (heroRespData.IsSuccess && heroRespData.HasData)
                {
                    heroData = PlayerDataToUserHeroData(heroRespData.Data);
                    heroData.111
                }


                    //                var warpoints = 
                    return new Response<UserHeroDetails>(respModel.Case, respModel.Message)
                {
                    Data = new UserHeroDetails()
                    {
                        BattleCount = Convert.ToInt32(respModel.Data.Value)
                    }
                };
            }
            catch (InvalidModelExecption ex) { return new Response<UserHeroDetails>(200, ErrorManager.ShowError(ex)); }
            catch (DataNotExistExecption ex) { return new Response<UserHeroDetails>(201, ErrorManager.ShowError(ex)); }
            catch (RequirementExecption ex) { return new Response<UserHeroDetails>(202, ErrorManager.ShowError(ex)); }
            catch (Exception ex) { return new Response<UserHeroDetails>(0, ErrorManager.ShowError(ex)); }
        }*/

        public Task<Response<UserHeroDetails>> GetHeroPoint(int playerId, int heroId)
        {
            //            try
            //            {
            //                var respModel = await manager.GetPlayerData(playerId, DataType.Hero, heroId);
            //                var warpoints = 
            //                return new Response<UserHeroDetails>(respModel.Case, respModel.Message)
            //                {
            //                    Data = new UserHeroDetails()
            //                    {
            //                        WarPoints = Convert.ToInt32(respModel.Data.Value)
            //                    }
            //                };
            //            }
            //            catch (InvalidModelExecption ex) { return new Response<UserHeroDetails>(200, ErrorManager.ShowError(ex)); }
            //            catch (DataNotExistExecption ex) { return new Response<UserHeroDetails>(201, ErrorManager.ShowError(ex)); }
            //            catch (RequirementExecption ex) { return new Response<UserHeroDetails>(202, ErrorManager.ShowError(ex)); }
            //            catch (Exception ex) { return new Response<UserHeroDetails>(0, ErrorManager.ShowError(ex)); }
            throw new NotImplementedException();
        }

        public async Task<Response<UserHeroDetails>> SaveHeroPoints(int playerId, HeroType heroType, int points)
        {
            try
            {
                var hero = CacheHeroDataManager.GetFullHeroDataID((int)heroType);
                if (hero == null) throw new CacheDataNotExistExecption("Hero does not exist");

                int valueId = hero.Info.HeroId;
                var response = await manager.GetAllPlayerData(playerId, DataType.Hero);
                if (response.IsSuccess)
                {
                    UserHeroDetails heroDetails = null;
                    PlayerDataTable userHeroTable = response.Data.Find(x => (x.ValueId == valueId));
                    if (userHeroTable != null)
                    {
                        heroDetails = JsonConvert.DeserializeObject<UserHeroDetails>(userHeroTable.Value);
                    }
                    else
                    {
                        heroDetails = new UserHeroDetails() { HeroType = (HeroType)hero.Info.HeroId };
                    }
                    if (points < 0) points = 0;
                    heroDetails.Points = points;

                    var heroJson = JsonConvert.SerializeObject(heroDetails);
                    Response<PlayerDataTableUpdated> saveResponse = null;
                    if (userHeroTable != null)
                    {
                        saveResponse = await manager.UpdatePlayerDataID(playerId, userHeroTable.Id, heroJson);
                    }
                    else
                    {
                        saveResponse = await manager.AddOrUpdatePlayerData(playerId, DataType.Hero, valueId, heroJson);
                    }
                    if (!saveResponse.IsSuccess)
                    {
                        return new Response<UserHeroDetails>(saveResponse.Case, saveResponse.Message);
                    }

                    var str = "Points set";
                    return new Response<UserHeroDetails>(heroDetails, 100, str);
                }
                else
                {
                    return new Response<UserHeroDetails>(response.Case, response.Message);
                }
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<UserHeroDetails>() { Case = 200, Message = ErrorManager.ShowError(ex) };
            }
            catch (CacheDataNotExistExecption ex)
            {
                return new Response<UserHeroDetails>() { Case = 201, Message = ErrorManager.ShowError(ex) };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<UserHeroDetails>() { Case = 202, Message = ErrorManager.ShowError(ex) };
            }
            catch (RequirementExecption ex)
            {
                return new Response<UserHeroDetails>() { Case = 203, Message = ErrorManager.ShowError(ex) };
            }
            catch (Exception ex)
            {
                return new Response<UserHeroDetails>() { Case = 0, Message = ErrorManager.ShowError() };
            }
        }

/*        public async Task<Response<UserHeroDataList>> GetHeroDataList(int playerId, HeroType type)
        {
            if (type == HeroType.Unknown) throw new DataNotExistExecption("Invalid parameters");
            try
            {
                var hero = CacheHeroDataManager.GetFullHeroData(type.ToString());
                var heroDataTypes = CacheHeroDataManager.HeroDataRelation.Find(x => x[0].HeroId == hero.Info.HeroId);

                var response = await manager.GetAllPlayerData(playerId, DataType.Hero);
                if (response.IsSuccess)
                {
                    var dataList = response.Data;
                    var valId = heroDataTypes.Find(x => x.StatType == 1).Id;

                    var unlocked = dataList.Find(x => x.ValueId == valId);
                    if (unlocked != null)
                    {
                        var list = new List<UserHeroDataValue>();
                        foreach (var entry in dataList)
                        {
                            var dataRel = heroDataTypes.Find(x => x.Id == entry.ValueId);
                            if (dataRel == null)
                            {
                                System.Console.WriteLine("invalid record :" + JsonConvert.SerializeObject(entry));
                                continue;
                            }

                            int.TryParse(entry.Value, out int val);
                            list.Add(new UserHeroDataValue() { Type = dataRel.StatType, Value = val });
                        }

                        var data = new UserHeroDataList()
                        {
                            HeroType = type,
                            Data = list
                        };
                        return new Response<UserHeroDataList>(data, response.Case, "All hero data");//response.Message);
                    }
                    else
                    {
                        return new Response<UserHeroDataList>() { Case = 203, Message = "Hero locked" };
                    }
                }
                else
                {
                    return new Response<UserHeroDataList>(response.Case, response.Message);
                }
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<UserHeroDataList>() { Case = 200, Message = ex.Message };//ErrorManager.ShowError(ex) };
            }
            catch (CacheDataNotExistExecption ex)
            {
                return new Response<UserHeroDataList>() { Case = 201, Message = ex.Message };// ErrorManager.ShowError(ex) };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<UserHeroDataList>() { Case = 201, Message = ex.Message };// ErrorManager.ShowError(ex) };
            }
            catch (RequirementExecption ex)
            {
                return new Response<UserHeroDataList>() { Case = 202, Message = ex.Message };// ErrorManager.ShowError(ex) };
            }
            catch (Exception ex)
            {
                return new Response<UserHeroDataList>() { Case = 0, Message = ex.Message };//ErrorManager.ShowError() };
            }
        }*/

        public async Task<Response<UserHeroDetails>> UnlockHero(int playerId, HeroType type)
        {
            if (type == HeroType.Unknown)
            {
                return new Response<UserHeroDetails>() { Case = 202, Message = "Invalid parameters" };
            }

            return await AddHeroPoints(playerId, type.ToString(), 0, true);
        }

        public async Task<Response<UserHeroDetails>> AddHeroPoints(int playerId, string type, int pts, bool unlock = false)
        {
            try
            {
                var hero = CacheHeroDataManager.GetFullHeroData(type);
                if (hero == null) throw new CacheDataNotExistExecption("Hero does not exist");

                int valueId = hero.Info.HeroId;
                var response = await manager.GetAllPlayerData(playerId, DataType.Hero);
                if (response.IsSuccess)
                {
                    UserHeroDetails heroDetails = null;
                    PlayerDataTable userHeroTable = response.Data.Find(x => (x.ValueId == valueId));
                    if (userHeroTable != null)
                    {
                        heroDetails = JsonConvert.DeserializeObject<UserHeroDetails>(userHeroTable.Value);
                    }
                    else
                    {
                        heroDetails = new UserHeroDetails() { HeroType = (HeroType)hero.Info.HeroId };
                    }

                    if (unlock)
                    {
                        if (heroDetails.Points < UserHeroDetails.UNLOCK_POINTS)
                        {
                            heroDetails.Points = UserHeroDetails.UNLOCK_POINTS;
                        }
                        else
                        {
                            return new Response<UserHeroDetails>(heroDetails, 101, "Hero already unlocked");
                        }
                    }
                    else
                    {
                        heroDetails.Points += pts;
                    }

                    var heroJson = JsonConvert.SerializeObject(heroDetails);
                    Response<PlayerDataTableUpdated> saveResponse = null;
                    if (userHeroTable != null)
                    {
                        saveResponse = await manager.UpdatePlayerDataID(playerId, userHeroTable.Id, heroJson);
                    }
                    else
                    {
                        saveResponse = await manager.AddOrUpdatePlayerData(playerId, DataType.Hero, valueId, heroJson);
                    }
                    if (!saveResponse.IsSuccess)
                    {
                        return new Response<UserHeroDetails>(saveResponse.Case, saveResponse.Message);
                    }

                    var str = unlock ? "Hero unlocked" : "Points added";
                    return new Response<UserHeroDetails>(heroDetails, 100, str);
                }
                else
                {
                    return new Response<UserHeroDetails>(response.Case, response.Message);
                }
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<UserHeroDetails>() { Case = 200, Message = ErrorManager.ShowError(ex) };
            }
            catch (CacheDataNotExistExecption ex)
            {
                return new Response<UserHeroDetails>() { Case = 201, Message = ErrorManager.ShowError(ex) };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<UserHeroDetails>() { Case = 202, Message = ErrorManager.ShowError(ex) };
            }
            catch (RequirementExecption ex)
            {
                return new Response<UserHeroDetails>() { Case = 203, Message = ErrorManager.ShowError(ex) };
            }
            catch (Exception ex)
            {
                return new Response<UserHeroDetails>() { Case = 0, Message = ErrorManager.ShowError() };
            }
        }



/*
        public async Task<Response> UnlockHero(int playerId, HeroType type) => await AddHeroWarPoints(playerId, type, null);

        public async Task<Response<int>> AddHeroWarPoints(int playerId, HeroType type, int? value)
        {
            try
            {
                if (type == HeroType.Unknown) throw new DataNotExistExecption("Invalid parameters");
                var hero = CacheHeroDataManager.GetFullHeroData(type.ToString());// GetFullInventoryItemData(itemType);
                if (hero == null) throw new DataNotExistExecption("Hero does not exist");

                //int valueId = hero.Info.HeroId;
                int valueId = CacheHeroDataManager.GetHeroDataRelationID(type, 1);
                var response = await manager.IncrementPlayerData(playerId, DataType.Hero, valueId, value);
                if (response.IsSuccess)
                {
                    //                    var userHeroData = PlayerDataToUserHeroData(response.Data);
                    //                    var data = userHeroData.Value;//.ToUserHeroDetails();
                    int.TryParse(response.Data.Value, out int val);
                    return new Response<int>(val, response.Case, response.Message);
                }
                else
                {
                    return new Response<int>(response.Case, response.Message);
                }
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<int>() { Case = 200, Message = ex.Message };// ErrorManager.ShowError(ex) };
            }
            catch (CacheDataNotExistExecption ex)
            {
                return new Response<int>() { Case = 201, Message = ex.Message };// ErrorManager.ShowError(ex) };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<int>() { Case = 202, Message = ex.Message };// ErrorManager.ShowError(ex) };
            }
            catch (RequirementExecption ex)
            {
                return new Response<int>() { Case = 203, Message = ex.Message };// ErrorManager.ShowError(ex) };
            }
            catch (Exception ex)
            {
                return new Response<int>() { Case = 0, Message = ex.Message };// ErrorManager.ShowError() };
            }
        }
*/
    }
}

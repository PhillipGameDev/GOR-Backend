using System;
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

        public Task<Response<UserHeroDetails>> SaveHeroPoint(int playerId, int heroId, int warPoints)
        {
            throw new NotImplementedException();
        }

        public async Task<Response<UserHeroDataList>> GetHeroDataList(int playerId, HeroType type)
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
        }

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
    }
}

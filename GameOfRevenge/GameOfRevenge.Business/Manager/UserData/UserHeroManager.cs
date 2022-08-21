using System;
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

        public async Task<Response<UserHeroDetails>> UnlockHero(int playerId, HeroType type) => await AddHeroWarPoints(playerId, type, null);

        public async Task<Response<UserHeroDetails>> AddHeroWarPoints(int playerId, HeroType type, int? value)
        {
            try
            {
                if (type == HeroType.Unknown) throw new DataNotExistExecption("Invalid parameters");

                var hero = CacheHeroDataManager.GetFullHeroData((int)type);// GetFullInventoryItemData(itemType);
                if (hero == null) throw new DataNotExistExecption("Hero does not exist");

                int valueId = hero.Info.HeroId;
                var response = await manager.IncrementPlayerData(playerId, DataType.Hero, valueId, value);
                if (response.IsSuccess)
                {
                    var userHeroData = PlayerDataToUserHeroData(response.Data);
                    var data = userHeroData.ToUserHeroDetails();
                    return new Response<UserHeroDetails>(data, response.Case, response.Message);
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
            catch (DataNotExistExecption ex)
            {
                return new Response<UserHeroDetails>() { Case = 201, Message = ErrorManager.ShowError(ex) };
            }
            catch (RequirementExecption ex)
            {
                return new Response<UserHeroDetails>() { Case = 202, Message = ErrorManager.ShowError(ex) };
            }
            catch (Exception)
            {
                return new Response<UserHeroDetails>() { Case = 0, Message = ErrorManager.ShowError() };
            }
        }
    }
}

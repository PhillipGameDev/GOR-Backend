//using System;
//using System.Threading.Tasks;
//using GameOfRevenge.Business.Manager.Base;
//using GameOfRevenge.Common;
//using GameOfRevenge.Common.Interface.UserData;
//using GameOfRevenge.Common.Models;
//using GameOfRevenge.Common.Net;
//using GameOfRevenge.Common.Services;

//namespace GameOfRevenge.Business.Manager.UserData
//{
//    public class UserHeroManager : BaseUserDataManager, IUserHeroManager
//    {
//        public async Task<Response<UserHeroDetails>> GetHeroPoint(int playerId, int heroId)
//        {
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
//        }

//        public Task<Response<UserHeroDetails>> SaveHeroPoint(int playerId, int heroId, int warPoints)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}

using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameOfRevenge.Business.Manager.UserData
{
    public class UserFriendsManager : BaseManager, IUserFriendsManager
    {
        public async Task<Response<ContactElement>> SetContact(int playerId, int targetId, ContactStatus status)
        {
            var spParams = new Dictionary<string, object>()
            {
                { "PlayerId", playerId },
                { "TargetId", targetId },
                { "Status", (byte)status }
            };

            return await Db.ExecuteSPSingleRow<ContactElement>("SetPlayerContact", spParams);
        }

        public async Task<Response<List<ContactElement>>> GetContacts(int playerId, int? targetPlayerId = null)
        {
            var spParams = new Dictionary<string, object>()
            {
                { "PlayerId", playerId }
            };
            if (targetPlayerId != null) spParams.Add("TargetPlayerId", (int)targetPlayerId);

            return await Db.ExecuteSPMultipleRow<ContactElement>("GetPlayerContacts", spParams);
        }


        public async Task<Response<List<FriendRequestElement>>> GetFriendRequests(int playerId, byte filter)
        {
            var spParams = new Dictionary<string, object>()
            {
                { "PlayerId", playerId },
                { "Filter", filter }
            };

            try
            {
                return await Db.ExecuteSPMultipleRow<FriendRequestElement>("GetFriendRequests", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<FriendRequestElement>>()
                {
                    Case = 200,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<FriendRequestElement>>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<List<ContactElement>>> GetFriends(int playerId)
        {
            var spParams = new Dictionary<string, object>()
            {
                { "PlayerId", playerId }
            };

            try
            {
                return await Db.ExecuteSPMultipleRow<ContactElement>("GetPlayerFriends", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<ContactElement>>()
                {
                    Case = 200,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<ContactElement>>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<FriendRequestElement>> SendFriendRequest(int fromPlayerId, int toPlayerId)
        {
            var spParams = new Dictionary<string, object>()
            {
                { "FromPlayerId", fromPlayerId },
                { "ToPlayerId", toPlayerId }
            };

            try
            {
                return await Db.ExecuteSPSingleRow<FriendRequestElement>("SendFriendRequest", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<FriendRequestElement>()
                {
                    Case = 200,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<FriendRequestElement>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response> RespondToFriendRequest(int fromPlayerId, int toPlayerId, byte value)
        {
            var spParams = new Dictionary<string, object>()
            {
                { "FromPlayerId", fromPlayerId },
                { "ToPlayerId", toPlayerId },
                { "Value", value }/* flags 1 = accepted, 2 = rejected */
            };

            try
            {
                return await Db.ExecuteSPNoData("RespondToFriendRequest", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<ContactElement>>()
                {
                    Case = 200,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<ContactElement>>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }
    }
}

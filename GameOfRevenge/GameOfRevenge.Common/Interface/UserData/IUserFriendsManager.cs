﻿using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Common.Interface.UserData
{
    public interface IUserFriendsManager
    {
        Task<Response<List<FriendRequestElement>>> GetFriendRequests(int playerId, byte filter);
        Task<Response<List<FriendElement>>> GetFriends(int playerId);
        Task<Response<FriendRequestElement>> SendFriendRequest(int fromPlayerId, int toPlayerId);
        Task<Response> RespondToFriendRequest(int fromPlayerId, int toPlayerId, byte value);
    }
}

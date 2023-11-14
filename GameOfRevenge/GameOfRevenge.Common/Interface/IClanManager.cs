using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common.Models.Clan;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Common.Interface
{
    public interface IClanManager
    {
        Task<Response<ClanData>> GetClanData(int clanId);
        Task<Response<List<ClanMember>>> GetClanMembers(int clanId);
        //Task<Response<List<ClanInvite>>> GetClanInvites(int playerid, int clanId);
        //Task<Response<List<ClanJoinReq>>> GetClanJoinRequests(int playerid, int clanId);
        Task<Response<List<ClanJoinReq>>> GetClanJoinRequests(int clanId);
        Task<Response<List<ClanJoinReq>>> GetClanJoinRequestsByPlayerId(int playerId, bool? isNew);
        Task<Response<FullClanData>> GetFullClanData(int playerId, int clanId);

        Task<Response<List<ClanData>>> GetClans(string tag, string name);
        Task<Response<List<ClanData>>> GetClans(string tag, string name, bool andClause);
        Task<Response<List<ClanData>>> GetClans(string tag, string name, bool andClause, int page, int count);

        Task<Response<ClanData>> CreateClan(int playerId, string name, string tag, string description, bool isPublic, int level);
        Task<Response> DeleteClan(int playerId, int clanId);

        Task<Response<ClanInvite>> GetPlayerClanInvitations(int playerId);
        Task<Response<ClanData>> GetPlayerClanData(int playerId);

        //Task<Response<ClanInvite>> InviteMemberToClan(int playerId, int clanId);
        //Task<Response> ReplyInvitationToClan(int playerId, int clanId, bool accept);
        Task<Response> RequestJoiningToClan(int playerId, int clanId);
        Task<Response> ReplyToJoinRequest(int playerId, int clanId, bool accept);
        Task<Response> LeaveClan(int playerId);

        Task<Response> UpdateClanCapacity(int clanId, int level);
        Task<Response> UpdateClanRole(int clanId, int playerId, int roleId);

        Task<Response> RequestJoiningToUnion(int fromClanId, int toClanId);
        Task<Response> UpdateJoiningToUnion(int fromClanId, int toClanId, bool? accepted);

        int GetCapacityByEmbassyLevel(int level);
    }
}

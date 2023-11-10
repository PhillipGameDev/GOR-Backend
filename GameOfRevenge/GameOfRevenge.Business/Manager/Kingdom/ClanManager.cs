using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Models.Clan;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameOfRevenge.Business.Manager.Kingdom
{
    public class ClanManager : BaseManager, IClanManager
    {
        public async Task<Response<ClanData>> CreateClan(int playerId, string name, string tag, string description, bool isPublic, int level)
        {
            try
            {
                if (playerId <= 0) throw new InvalidModelExecption("Account dose not exist");
                if (string.IsNullOrWhiteSpace(tag)) throw new InvalidModelExecption("Invalid name");
                if (string.IsNullOrWhiteSpace(name)) throw new InvalidModelExecption("Invalid tag");

                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId },
                    { "Name", name },
                    { "Tag", tag },
                    { "Description", description },
                    { "IsPublic", isPublic },
                    { "Capacity", GetCapacityByEmbassyLevel(level) }
                };

                var response = await Db.ExecuteSPSingleRow<ClanData>("CreateClan", spParams);
                return response;
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<ClanData>() { Case = 200, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new Response<ClanData>() { Case = 0, Message = ErrorManager.ShowError(ex) };
            }
        }
        public async Task<Response> DeleteClan(int playerId, int clanId)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId },
                    { "ClanId", clanId }
                };

                return await Db.ExecuteSPNoData("DeleteClan", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response() { Case = 200, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new Response() { Case = 0, Message = ErrorManager.ShowError(ex) };
            }
        }

        public async Task<Response<List<ClanData>>> GetClans(string tag, string name) => await GetClans(tag, name, true, 1, 100);
        public async Task<Response<List<ClanData>>> GetClans(string tag, string name, bool andClause) => await GetClans(tag, name, andClause, 1, 100);
        public async Task<Response<List<ClanData>>> GetClans(string tag, string name, bool andClause, int page, int count)
        {
            try
            {
                if (page <= 0) page = 1;
                if (count <= 10) count = 10;
                if (string.IsNullOrWhiteSpace(tag)) tag = string.Empty;
                if (string.IsNullOrWhiteSpace(name)) name = string.Empty;

                var spParams = new Dictionary<string, object>()
                {
                    { "Tag", tag },
                    { "Name", name },
                    { "AndClause", andClause }
                };

                return await Db.ExecuteSPMultipleRow<ClanData>("GetClans", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<ClanData>>() { Case = 200, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new Response<List<ClanData>>() { Case = 0, Message = ErrorManager.ShowError(ex) };
            }
        }

        public async Task<Response<FullClanData>> GetFullClanData(int playerId, int clanId)
        {
            var clanData = await GetClanData(clanId);
            var response = new Response<FullClanData>(clanData.Case, clanData.Message);

            if (clanData.IsSuccess && clanData.HasData) response.Data = new FullClanData();
            else return response;

            response.Data.Info = clanData.Data;

            var clanMembers = await GetClanMembers(clanId);
            if (!clanMembers.IsSuccess || !clanMembers.HasData) return response;
            response.Data.Members = clanMembers.Data;

            var clanUnions = await GetClanUnions(clanId, true);
            if (!clanUnions.IsSuccess || !clanUnions.HasData) return response;
            response.Data.Unions = clanUnions.Data.Select(e => e.FromClanId == clanId ? e.ToClanId : e.FromClanId).ToList();


            //var clanMemberData = clanMembers.Data.Find(x => x.PlayerId == playerId);
            //var showInvites = clanMemberData != null && (clanMemberData.Role == ClanRole.Admin || clanMemberData.Role == ClanRole.Owner || clanMemberData.Role == ClanRole.Moderator);
            //if (showInvites)
            //{
            //    var clanInvites = await GetClanInvites(playerId, clanId);
            //    response.Data.Invites = clanInvites.Data;

            //    var calnJoinReq = await GetClanJoinRequests(playerId, clanId);
            //    response.Data.JoinRequest = calnJoinReq.Data;
            //}

            return response;
        }
        public async Task<Response<ClanData>> GetClanData(int clanId)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "ClanId", clanId }
                };

                return await Db.ExecuteSPSingleRow<ClanData>("GetClanData", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<ClanData>() { Case = 200, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new Response<ClanData>() { Case = 0, Message = ErrorManager.ShowError(ex) };
            }
        }
        public async Task<Response<List<ClanMember>>> GetClanMembers(int clanId)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "ClanId", clanId }
                };

                var response = await Db.ExecuteSPMultipleRow<ClanMember>("GetClanMembers", spParams);
                return response;
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<ClanMember>>() { Case = 200, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new Response<List<ClanMember>>() { Case = 0, Message = ErrorManager.ShowError(ex) };
            }
        }
        public async Task<Response<List<ClanUnion>>> GetClanUnions(int clanId, bool? accepted = null)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "ClanId", clanId },
                    { "Accepted", accepted }
                };

                var response = await Db.ExecuteSPMultipleRow<ClanUnion>("GetClanUnions", spParams);
                return response;
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<ClanUnion>>() { Case = 200, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new Response<List<ClanUnion>>() { Case = 0, Message = ErrorManager.ShowError(ex) };
            }

        }
        public async Task<Response<List<ClanInvite>>> GetClanInvites(int playerid, int clanId)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerid },
                    { "ClanId", clanId }
                };

                return await Db.ExecuteSPMultipleRow<ClanInvite>("GetClanInvites", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<ClanInvite>>() { Case = 200, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new Response<List<ClanInvite>>() { Case = 0, Message = ErrorManager.ShowError(ex) };
            }
        }
        public async Task<Response<List<ClanJoinReq>>> GetClanJoinRequests(int playerid, int clanId)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerid },
                    { "ClanId", clanId }
                };

                return await Db.ExecuteSPMultipleRow<ClanJoinReq>("GetClanJoinRequests", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<ClanJoinReq>>() { Case = 200, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new Response<List<ClanJoinReq>>() { Case = 0, Message = ErrorManager.ShowError(ex) };
            }
        }

        public async Task<Response<List<ClanJoinReq>>> GetClanJoinRequests(int clanId)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "ClanId", clanId }
                };

                return await Db.ExecuteSPMultipleRow<ClanJoinReq>("GetClanJoinReqs", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<ClanJoinReq>>() { Case = 200, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new Response<List<ClanJoinReq>>() { Case = 0, Message = ErrorManager.ShowError(ex) };
            }
        }

        public async Task<Response<List<ClanJoinReq>>> GetClanJoinRequestsByPlayerId(int playerId, bool? isNew)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId },
                    { "IsNew", isNew }
                };

                return await Db.ExecuteSPMultipleRow<ClanJoinReq>("GetClanJoinReqsByPlayerId", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<ClanJoinReq>>() { Case = 200, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new Response<List<ClanJoinReq>>() { Case = 0, Message = ErrorManager.ShowError(ex) };
            }
        }

        public async Task<Response<ClanData>> GetPlayerClanData(int playerId)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId },
                };

                return await Db.ExecuteSPSingleRow<ClanData>("GetPlayerClanData", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<ClanData>() { Case = 200, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new Response<ClanData>() { Case = 0, Message = ErrorManager.ShowError(ex) };
            }
        }
        public async Task<Response<ClanInvite>> GetPlayerClanInvitations(int playerId)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId },
                };

                return await Db.ExecuteSPSingleRow<ClanInvite>("GetPlayerClanInvitations", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<ClanInvite>() { Case = 200, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new Response<ClanInvite>() { Case = 0, Message = ErrorManager.ShowError(ex) };
            }
        }

        //public async Task<Response<ClanInvite>> InviteMemberToClan(int playerId, int clanId)
        //{
        //    try
        //    {
        //        var spParams = new Dictionary<string, object>()
        //        {
        //            { "PlayerId", playerId },
        //            { "ClanId", clanId },
        //        };

        //        return await Db.ExecuteSPSingleRow<ClanInvite>("InviteMemberToClan", spParams);
        //    }
        //    catch (InvalidModelExecption ex)
        //    {
        //        return new Response<ClanInvite>()
        //        {
        //            Case = 200,
        //            Message = ex.Message
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new Response<ClanInvite>()
        //        {
        //            Case = 0,
        //            Message = ErrorManager.ShowError(ex)
        //        };
        //    }
        //}
        //public async Task<Response> ReplyInvitationToClan(int playerId, int clanId, bool accpet)
        //{
        //    try
        //    {
        //        var spParams = new Dictionary<string, object>()
        //        {
        //            { "PlayerId", playerId },
        //            { "ClanId", clanId },
        //            { "Accpet", accpet },
        //        };

        //        return await Db.ExecuteSPNoData("ReplyInvitationToClan", spParams);
        //    }
        //    catch (InvalidModelExecption ex)
        //    {
        //        return new Response()
        //        {
        //            Case = 200,
        //            Message = ex.Message
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new Response()
        //        {
        //            Case = 0,
        //            Message = ErrorManager.ShowError(ex)
        //        };
        //    }
        //}
        public async Task<Response> ReplyToJoinRequest(int playerId, int clanId, bool accept)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId },
                    { "ClanId", clanId },
                    { "Accept", accept },
                };

                return await Db.ExecuteSPNoData("ReplyToJoinRequest", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response()
                {
                    Case = 200,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }
        public async Task<Response> RequestJoiningToClan(int playerId, int clanId)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId },
                    { "ClanId", clanId },
                };

                return await Db.ExecuteSPNoData("RequestJoiningToClan", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response() { Case = 200, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new Response() { Case = 0, Message = ErrorManager.ShowError(ex) };
            }
        }

        public async Task<Response> RequestJoiningToUnion(int fromClanId, int toClanId)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "FromClandId", fromClanId },
                    { "ToClanId", toClanId },
                };

                return await Db.ExecuteSPNoData("RequestJoiningToUnion", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response() { Case = 200, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new Response() { Case = 0, Message = ErrorManager.ShowError(ex) };
            }
        }

        public async Task<Response> UpdateJoiningToUnion(int fromClanId, int toClanId, bool? accepted)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "FromClandId", fromClanId },
                    { "ToClanId", toClanId },
                    { "Accepted", accepted }
                };

                return await Db.ExecuteSPNoData("UpdateJoiningToUnion", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response() { Case = 200, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new Response() { Case = 0, Message = ErrorManager.ShowError(ex) };
            }
        }

        public async Task<Response> UpdateClanCapacity(int clanId, int level)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "ClanId", clanId },
                    { "Capacity", GetCapacityByEmbassyLevel(level) },
                };

                return await Db.ExecuteSPNoData("UpdateClanCapacity", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response() { Case = 200, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new Response() { Case = 0, Message = ErrorManager.ShowError(ex) };
            }
        }

        public async Task<Response> UpdateClanRole(int clanId, int playerId, int roleId)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId },
                    { "ClanId", clanId },
                    { "RoleId", roleId },
                };

                return await Db.ExecuteSPNoData("UpdateClanRole", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response() { Case = 200, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new Response() { Case = 0, Message = ErrorManager.ShowError(ex) };
            }
        }

        public async Task<Response> LeaveClan(int playerId)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId }
                };

                return await Db.ExecuteSPNoData("LeaveClan", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response() { Case = 200, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new Response() { Case = 0, Message = ErrorManager.ShowError(ex) };
            }
        }

        private List<(int, int)> lvlMemberCount = new List<(int, int)>
        {
            (3, 10),
            (6, 15),
            (10, 20),
            (15, 25),
            (20, 30),
            (25, 40),
            (30, 50),
        };

        public int GetCapacityByEmbassyLevel(int level)
        {
            
            foreach (var lvlMember in lvlMemberCount)
            {
                if (level <= lvlMember.Item1)
                {
                    return lvlMember.Item2;
                }
            }

            return 10;
        }
    }
}

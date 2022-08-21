using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Models.Clan;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameOfRevenge.Business.Manager.Kingdom
{
    public class ClanManager : BaseManager, IClanManager
    {
        public async Task<Response<ClanData>> CreateClan(int playerId, string name, string tag, string description, bool isPublic)
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
                    { "IsPublic", isPublic }
                };

                var response = await Db.ExecuteSPSingleRow<ClanData>("CreateClan", spParams);
                return response;
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<ClanData>()
                {
                    Case = 200,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<ClanData>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
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

        public async Task<Response<List<ClanData>>> GetClans(string tag, string name) => await GetClans(tag, name, true, 1, 10);
        public async Task<Response<List<ClanData>>> GetClans(string tag, string name, bool andClause) => await GetClans(tag, name, andClause, 1, 10);
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

                return await Db.ExecuteSPMultipleRowPaged<ClanData>("GetClans", page, count, spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<ClanData>>()
                {
                    Case = 200,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<ClanData>>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
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
                return new Response<ClanData>()
                {
                    Case = 200,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<ClanData>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
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
                return new Response<List<ClanMember>>()
                {
                    Case = 200,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<ClanMember>>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
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
                return new Response<List<ClanInvite>>()
                {
                    Case = 200,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<ClanInvite>>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
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
                return new Response<List<ClanJoinReq>>()
                {
                    Case = 200,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<List<ClanJoinReq>>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
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
                return new Response<ClanData>()
                {
                    Case = 200,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<ClanData>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
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
                return new Response<ClanInvite>()
                {
                    Case = 200,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<ClanInvite>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex)
                };
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
        //public async Task<Response> ReplyToJoinRequest(int playerId, int requestId, bool accpet)
        //{
        //    try
        //    {
        //        var spParams = new Dictionary<string, object>()
        //        {
        //            { "PlayerId", playerId },
        //            { "RequestId", requestId },
        //            { "Accept", accpet },
        //        };

        //        return await Db.ExecuteSPSingleRow<ClanInvite>("ReplyToJoinRequest", spParams);
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
    }
}

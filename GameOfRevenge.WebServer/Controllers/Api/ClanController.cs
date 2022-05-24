using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using GameOfRevenge.Common.Interface;
using System.Linq;

namespace GameOfRevenge.WebServer.Controllers.Api
{
    public class ClanController : BaseApiController
    {
        private readonly IClanManager clanManager;

        public ClanController(IClanManager clanManager)
        {
            this.clanManager = clanManager;
        }

        [HttpGet]
        public async Task<IActionResult> ClanDetails(int clanId) => ReturnResponse(await clanManager.GetClanData(clanId));

        [HttpGet]
        public async Task<IActionResult> FullClanData(int clanId) => ReturnResponse(await clanManager.GetFullClanData(Token.PlayerId, clanId));

        [HttpGet]
        public async Task<IActionResult> ClanMembers(int clanId) => ReturnResponse(await clanManager.GetClanMembers(clanId));

        [HttpGet]
        public async Task<IActionResult> MyClan() => ReturnResponse(await clanManager.GetPlayerClanData(Token.PlayerId));

        //[HttpGet]
        //public async Task<IActionResult> ClanInvites(int clanId) => ReturnResponse(await clanManager.GetClanInvites(Token.PlayerId, clanId));

        //[HttpGet]
        //public async Task<IActionResult> MyClanInvitations() => ReturnResponse(await clanManager.GetPlayerClanInvitations(Token.PlayerId));

        //[HttpGet]
        //public async Task<IActionResult> JoinRequests(int clanId) => ReturnResponse(await clanManager.GetClanJoinRequests(Token.PlayerId, clanId));

        [HttpGet]
        public async Task<IActionResult> GetClansList(string tag, string name, bool clause, int page, int count)
        {
            var response = await clanManager.GetClans(tag, name, clause, page, count);
            if (response.IsSuccess && response.HasData && response.Data.Any())
            {
                var myClan = await clanManager.GetPlayerClanData(Token.PlayerId);
                if (myClan.IsSuccess && myClan.HasData)
                {
                    var clanToRemove = response.Data.Find(x => x.Id == myClan.Data.Id);
                    if (clanToRemove != null)
                        response.Data.Remove(clanToRemove);
                }
            }

            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateClan(string name, string tag, string description/*, bool isPublic*/) => ReturnResponse(await clanManager.CreateClan(Token.PlayerId, name, tag, description, true));

        [HttpPost]
        public async Task<IActionResult> DeleteClan(int clanId) => ReturnResponse(await clanManager.DeleteClan(Token.PlayerId, clanId));

        [HttpPost]
        public async Task<IActionResult> JoinClanRequest(int clanId) => ReturnResponse(await clanManager.RequestJoiningToClan(Token.PlayerId, clanId));

        //[HttpPost]
        //public async Task<IActionResult> ReplyToJoinRequest(int requestId, bool accept) => ReturnResponse(await clanManager.ReplyToJoinRequest(Token.PlayerId, requestId, accept));
    }
}

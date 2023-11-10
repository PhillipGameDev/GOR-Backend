using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using GameOfRevenge.Common.Interface;
using System.Linq;
using GameOfRevenge.Common.Models.Quest.Template;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Common.Models.Quest;
using System.Collections.Generic;
using Newtonsoft.Json;
using GameOfRevenge.Business.Manager.UserData;

namespace GameOfRevenge.WebServer.Controllers.Api
{
    public class ClanController : BaseApiController
    {
        private readonly IClanManager clanManager;

        private readonly IUserQuestManager questManager = new UserQuestManager();

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

        [HttpGet]
        public async Task<IActionResult> JoinRequests(int clanId) => ReturnResponse(await clanManager.GetClanJoinRequests(clanId));

        [HttpGet]
        public async Task<IActionResult> JoinRequestsByPlayerId(bool? isNew) => ReturnResponse(await clanManager.GetClanJoinRequestsByPlayerId(Token.PlayerId, isNew));

        [HttpGet]
        public async Task<IActionResult> GetAllClans(string tag, string name, bool clause, int page, int count) => ReturnResponse(await clanManager.GetClans(tag, name, clause, page, count));

        [HttpGet]
        public async Task<IActionResult> GetClansList(string tag, string name, bool clause, int page, int count)
        {
            var response = await clanManager.GetClans(tag, name, clause, page, count);
            if (response.IsSuccess && response.HasData)
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
        public async Task<IActionResult> CreateClan(string name, string tag, string description, int embassyLevel)
        {
            var response = await clanManager.CreateClan(Token.PlayerId, name, tag, description, true, embassyLevel);
            if (response.IsSuccess) await CompleteAllianceTaskQuest(Token.PlayerId, AllianceTaskType.JoinOrCreate);

            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteClan(int clanId) => ReturnResponse(await clanManager.DeleteClan(Token.PlayerId, clanId));

        [HttpPost]
        public async Task<IActionResult> JoinClanRequest(int clanId)
        {
            var response = await clanManager.RequestJoiningToClan(Token.PlayerId, clanId);
            if (response.IsSuccess && ((response.Case == 100) || (response.Case == 202)))
            {
                await CompleteAllianceTaskQuest(Token.PlayerId, AllianceTaskType.JoinOrCreate);
            }

            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> LeaveClan() => ReturnResponse(await clanManager.LeaveClan(Token.PlayerId));

        //[HttpPost]
        //public async Task<IActionResult> ReplyToJoinRequest(int requestId, bool accept) => ReturnResponse(await clanManager.ReplyToJoinRequest(Token.PlayerId, requestId, accept));

        private async Task<bool> CompleteAllianceTaskQuest(int playerId, AllianceTaskType taskType)
        {
            var quests = CacheQuestDataManager.AllQuestRewards
                                .Where(x => (x.Quest.QuestType == QuestType.Alliance))?.ToList();
            if (quests == null) return false;

            var questUpdated = false;
            List<PlayerQuestDataTable> allUserQuests = null;
            foreach (var quest in quests)
            {
                QuestAllianceData questData = null;
                try
                {
                    questData = JsonConvert.DeserializeObject<QuestAllianceData>(quest.Quest.DataString);
                }
                catch { }
                if ((questData == null) || (questData.AllianceTaskType != taskType)) continue;

                if (allUserQuests == null)
                {
                    var response = await questManager.GetAllQuestProgress(playerId);
                    if (response.IsSuccess && response.HasData) allUserQuests = response.Data;
                    else break;
                }
                var userQuest = allUserQuests.Find(x => (x.QuestId == quest.Quest.QuestId));
                if ((userQuest == null) || !userQuest.Completed)
                {
                    string initialString = (userQuest == null) ? quest.Quest.DataString : null;
                    var resp = await questManager.UpdateQuestData(playerId, quest.Quest.QuestId, true, initialString);
                    if (resp.IsSuccess) questUpdated = true;
                }
            }

            return questUpdated;
        }
    }
}

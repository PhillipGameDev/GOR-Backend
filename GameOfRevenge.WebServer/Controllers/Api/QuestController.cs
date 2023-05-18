using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using GameOfRevenge.Common.Interface;

namespace GameOfRevenge.WebServer.Controllers.Api
{
    public class QuestController : BaseApiController
    {
        private readonly IUserQuestManager questManager;

        public QuestController(IUserQuestManager questManager)
        {
            this.questManager = questManager;
        }

        [HttpPost]
        public async Task<IActionResult> RedeemQuestReward(int questId)
        {
            var response = await questManager.RedeemQuestReward(Token.PlayerId, questId);
            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> RedeemChapterReward(int chapterId)
        {
            var response = await questManager.RedeemChapterReward(Token.PlayerId, chapterId);
            return ReturnResponse(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllQuestProgress()
        {
            var response = await questManager.GetUserAllQuestProgress(Token.PlayerId);//GetAllQuestChapterDataWithName(Token.PlayerId);
            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> GetQuestProgress(int questId)
        {
            var response = await questManager.GetQuestProgress(Token.PlayerId, questId);
            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuestProgress(int questId, bool isCompleted, string progress)
        {
            //if (!Token.IsDeveloper || !Token.IsAdmin) return Unauthorized();
           
            var response = await questManager.UpdateQuestData(Token.PlayerId, questId, isCompleted, progress);
            return ReturnResponse(response);
        }

#region Rewards
        [HttpGet]
        public async Task<IActionResult> GetAllUserRewards()
        {
            var response = await questManager.GetUserAllRewards(Token.PlayerId);
            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> ConsumeReward(long playerDataId, string context = null)
        {
            var response = await questManager.ConsumeReward(Token.PlayerId, playerDataId, context);
            return ReturnResponse(response);
        }
        #endregion
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using GameOfRevenge.Common.Interface;
//using Microsoft.AspNetCore.Authorization;

namespace GameOfRevenge.WebServer.Controllers.Api
{
    public class ChatController : BaseApiController
    {
        private readonly IChatManager chatManager;

        public ChatController(IChatManager chatManager)
        {
            this.chatManager = chatManager;
        }

/*        [HttpPost]
        public async Task<IActionResult> CreateMessage(string content)
        {
            var response = await chatManager.CreateMessage(Token.PlayerId, content);
            return ReturnResponse(response);
        }*/

        [HttpPost]
        public async Task<IActionResult> GetMessages(long chatId = 0, int allianceId = 0)
        {
            var response = await chatManager.GetMessages(chatId, allianceId);
            return ReturnResponse(response);
        }

/*        [HttpPost]
        public async Task<IActionResult> DeleteMessage(long chatId)
        {
            var response = await chatManager.DeleteMessage(Token.PlayerId, chatId);
            return ReturnResponse(response);
        }
*/
        [HttpPost]
        public async Task<IActionResult> ReportMessage(long chatId, byte reportType, int allianceId = 0)
        {
            var response = await chatManager.ReportMessage(Token.PlayerId, chatId, reportType, allianceId);
            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> BlockPlayer(int blockPlayerId)
        {
            var response = await chatManager.BlockPlayer(Token.PlayerId, blockPlayerId);
            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> GetBlockedPlayers()
        {
            var response = await chatManager.GetBlockedPlayers(Token.PlayerId);
            return ReturnResponse(response);
        }
    }
}

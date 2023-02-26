using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using GameOfRevenge.Common.Interface;
using Microsoft.AspNetCore.Authorization;

namespace GameOfRevenge.WebServer.Controllers.Api
{
    [AllowAnonymous]
    public class ChatController : BaseApiController
    {
        private readonly IChatManager chatManager;

        public ChatController(IChatManager chatManager)
        {
            this.chatManager = chatManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int playerId, string content)
        {
            var response = await chatManager.CreateMessage(playerId, content);
            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> GetMessages(int chatId = 0)
        {
            var response = await chatManager.GetMessages(chatId);
            return ReturnResponse(response);
        }
    }
}

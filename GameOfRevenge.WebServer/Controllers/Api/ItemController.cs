using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using GameOfRevenge.Common.Interface;
using Newtonsoft.Json;

namespace GameOfRevenge.WebServer.Controllers.Api
{
    public class ItemController : BaseApiController
    {
        private readonly IUserItemManager itemManager;

        public ItemController(IUserItemManager itemManager)
        {
            this.itemManager = itemManager;
        }

#region Items
        [HttpGet]
        public async Task<IActionResult> GetAllUserItems()
        {
            var response = await itemManager.GetUserAllItems(Token.PlayerId);
            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> ConsumeItem(long playerDataId, int count, string context = null)
        {
            var response = await itemManager.ConsumeItem(Token.PlayerId, playerDataId, count, context);
            return ReturnResponse(response);
        }
        #endregion
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using GameOfRevenge.Common.Interface;
using Newtonsoft.Json;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Business.CacheData;
using Microsoft.AspNetCore.Authorization;
using GameOfRevenge.Business.Manager.GameDef;

namespace GameOfRevenge.WebServer.Controllers.Api
{
    public class InventoryController : BaseApiController
    {
        private readonly IUserInventoryManager inventoryManager;

        public InventoryController(IUserInventoryManager inventoryManager)
        {
            this.inventoryManager = inventoryManager;
        }

        #region Inventories

        [HttpGet]
        public async Task<IActionResult> GetAllUserItems()
        {
            var response = await inventoryManager.GetAllUserInventory(Token.PlayerId);
            return ReturnResponse(response);
        }
        #endregion
    }
}

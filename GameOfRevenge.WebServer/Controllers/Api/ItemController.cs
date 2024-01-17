using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using GameOfRevenge.Common.Interface;
using Newtonsoft.Json;
using GameOfRevenge.Business.Manager.GameDef;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Common.Models.Inventory;
using GameOfRevenge.Common.Net;
using System;
using Microsoft.AspNetCore.Authorization;
using GameOfRevenge.Business.CacheData;

namespace GameOfRevenge.WebServer.Controllers.Api
{
    public class ItemController : BaseApiController
    {
        private readonly IUserItemManager itemManager;
        private readonly IUserInventoryManager inventoryManager;

        public ItemController(IUserItemManager itemManager, IUserInventoryManager inventoryManager)
        {
            this.itemManager = itemManager;
            this.inventoryManager = inventoryManager;
        }

#region Items
        [HttpGet]
        public async Task<IActionResult> GetAllUserItems()
        {
            var response = await itemManager.GetUserAllItems(Token.PlayerId);
            return ReturnResponse(response);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAllItems() => ReturnResponse(CacheItemManager.AllItems);//obsolete starting from v1 moved to gamedefcontroller

        [HttpPost]
        public async Task<IActionResult> ConsumeItem(long playerDataId, int count, string context = null)
        {
            var response = await itemManager.ConsumeItem(Token.PlayerId, playerDataId, count, context);
            return ReturnResponse(response);
        }
        #endregion

        #region Inventories

        [HttpGet]
        public async Task<IActionResult> GetAllUserInventory()
        {
            var response = await inventoryManager.GetAllUserInventory(Token.PlayerId);
            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateInventoryOrder(int itemId, int order)
        {
            var response = await inventoryManager.UpdateInventoryOrder(Token.PlayerId, itemId, order);
            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> UpgradeInventory(int inventoryId, int level, DateTime startTime)
        {
            var response = await inventoryManager.UpgradeInventory(Token.PlayerId, inventoryId, level, startTime);
            return ReturnResponse(response);
        }
        #endregion
    }
}

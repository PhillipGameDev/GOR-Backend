using Microsoft.AspNetCore.Mvc;
using GameOfRevenge.Business.CacheData;
using Microsoft.AspNetCore.Authorization;
using GameOfRevenge.Business.Manager.GameDef;
using System.Threading.Tasks;
using GameOfRevenge.Common.Interface;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;

namespace GameOfRevenge.WebServer.Controllers.Api
{
    public class AcademyController : BaseApiController
    {
        private IUserAcademyManager manager;
        public AcademyController(IUserAcademyManager manager)
        {
            this.manager = manager;
        }

        #region Items

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAllAcademyItem()
        {
            var items = CacheAcademyDataManager.AllAcademyItems;
            return ReturnResponse(items);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAllAcademyRequirements()
        {
            var items = CacheAcademyDataManager.AllAcademyRequirements;
            return ReturnResponse(items);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUserItems()
        {
            var response = await manager.GetUserAllItems(Token.PlayerId);
            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> UpgradeItem(int itemId, DateTime startTime)
        {
            var response = await manager.UpgradeItem(Token.PlayerId, itemId, startTime);
            return ReturnResponse(response);
        }
        #endregion
    }
}

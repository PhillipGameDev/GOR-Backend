using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Common.Interface;

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
            var items = new List<Common.Models.Academy.AcademyItemTable>(CacheAcademyDataManager.AllAcademyItems);
            foreach (var item in items)
            {
//                item.Name = item.TechnologyType.ToString() + " " + item.Name;
            }

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

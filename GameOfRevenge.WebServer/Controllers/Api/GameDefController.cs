using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Models.Inventory;
using GameOfRevenge.Common.Models.Troop;
using GameOfRevenge.Common.Helper;
using System.Linq;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Interface.UserData;

namespace GameOfRevenge.WebServer.Controllers.Api
{
    [AllowAnonymous]
    public class GameDefController : BaseApiController
    {
        private readonly IBaseUserManager baseUserManager;

        public GameDefController(IBaseUserManager baseUserManager)
        {
            this.baseUserManager = baseUserManager;
        }

        [HttpGet]
        public IActionResult GetCityCounselBuffs()
        {
            var response = new Response<IReadOnlyList<IReadOnlyBuffItemRel>>(CacheInventoryDataManager.BuffItemRelations, 100, "City buff list");
            return ReturnResponse(response);
        }

        [HttpGet]
        public IActionResult GetAllHeros()
        {
            var response = CacheHeroDataManager.HeroInfos;
            return ReturnResponse(response);
        }

        [HttpGet]
        public IActionResult GetAllBoost()
        {
            var response = CacheBoostDataManager.BoostInfos;
            return ReturnResponse(response);
        }

        [HttpGet]
        public IActionResult GetAllInventory()
        {
            var response = CacheInventoryDataManager.ItemInfos;
            return ReturnResponse(response);
        }

        [HttpGet]
        public IActionResult GetAllTechnology()
        {
            var response = CacheTechnologyDataManager.TechnologyInfos;
            return ReturnResponse(response);
        }

        [HttpGet]
        public IActionResult GetAllResources()
        {
            var response = CacheResourceDataManager.ResourceInfos;
            return ReturnResponse(response);
        }

        [HttpGet]
        public IActionResult GetAllStructureData()
        {
            var response = CacheStructureDataManager.StructureInfos;
            return ReturnResponse(response);
        }

        [HttpGet]
        public IActionResult GetAllTroopData()
        {
            var response = CacheTroopDataManager.TroopInfos;
            return ReturnResponse(response);
        }

        [HttpGet]
        public IActionResult GetAllChapters()
        {
            var response = CacheQuestDataManager.QuestInfos;
            return ReturnResponse(response);
        }

        [HttpGet]
        public IActionResult GetAllTroopTraningBuildingRel()
        {
            var response = new Response<IReadOnlyList<IReadOnlyTroopBuildingRelation>>(CaseType.Success, "Troop and structure relation")
            {
                Data = CacheTroopDataManager.TroopBuildingRelation
            };

            return ReturnResponse(response);
        }

        [HttpGet]
        public IActionResult BuildingLevelDescription()
        {
            var response = CacheStructureDataManager.StructureInfoFactory;
            return ReturnResponse(response);
        }

        [HttpGet]
        public IActionResult GetInstantBuildCost(int timeLeft)
        {
            var response = baseUserManager.GetInstantBuildCost(timeLeft);
            return ReturnResponse(new Response<int>(response, 100, "Instant build cost"));
        }
    }
}

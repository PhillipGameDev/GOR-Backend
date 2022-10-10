using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Models.Boost;
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
        public IActionResult GetAllBoosts()
        {
//            var response = new Response<IReadOnlyList<IReadOnlyBoostTypeTable>>(CacheBoostDataManager.BoostInfos, 100, "City boost list");
//            CacheBoostDataManager.boostTypes;
            var response = CacheBoostDataManager.SpecNewBoostDataTables;
            return ReturnResponse(response);
        }

        [HttpGet]
        public IActionResult GetAllHeroes()
        {
            var response = CacheHeroDataManager.HeroInfos;
            return ReturnResponse(response);
        }

/*        [HttpGet]
        public IActionResult GetAllBoost()
        {
//            var response = CacheBoostDataManager.BoostInfos;
            return ReturnResponse();
        }*/

        [HttpGet]
        public IActionResult GetAllItems()
        {
            var response = CacheInventoryDataManager.ItemList;
//            var response = new List<InventoryDataTable> ();
//            response.Add(new InventoryDataTable {Id = 111,Code=InventoryItemType.Weapon,Name="name test",Rarity = Common.RarityType.Rare });
            if (response != null)
            {

                //config.Formatters.JsonFormatter.S‌​erializerSettings
/*                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.None,
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    PreserveReferencesHandling = PreserveReferencesHandling.None
                };*/
                //                System.Web.Http.GlobalConfiguration.Configuration.Formatters.JsonFormatter.UseDataContractJsonSerializer = true;

//                string json = JsonConvert.SerializeObject(response);//, settings);

                return ReturnResponse(response);//, "application/json");


                //                return ReturnResponse(Content(JsonConvert.SerializeObject(response), "application/json"));
                //                var json = JsonConvert.SerializeObject(response);
                //return ReturnResponse(json);
            }
            else
            {
                return ReturnResponse();
            }
        }

        [HttpGet]
        public IActionResult GetAllTechnologies()
        {
//            var response = new Response<IReadOnlyList<Common.Models.Technology.IReadOnlyTechnologyDataRequirementRel>>(CacheTechnologyDataManager.TechnologyInfos, 100, "Technology list");
            var response = CacheTechnologyDataManager.TechnologyInfos;
            return ReturnResponse(response);
        }

/*        [HttpGet]
        public IActionResult GetAllSubTechnology()
        {
            var response = CacheTechnologyDataManager.SubTechnologyInfos;
            return ReturnResponse(response);
        }*/

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
        public IActionResult GetAllTroopBuildingRelation()
        {
/*            var response = new Response<IReadOnlyList<IReadOnlyTroopBuildingRelation>>(CaseType.Success, "Troop and building relation")
            {
                Data = CacheTroopDataManager.TroopBuildingRelation
            };*/
            var response = CacheTroopDataManager.TroopBuildingRelation;

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

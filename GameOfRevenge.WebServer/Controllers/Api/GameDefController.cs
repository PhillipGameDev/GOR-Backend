using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models;

namespace GameOfRevenge.WebServer.Controllers.Api
{
    [AllowAnonymous]
    public class GameDefController : BaseApiController
    {
        [HttpGet]//group data 1
        [ResponseCache(Duration = 120, VaryByQueryKeys = new string[] { "ver" })]
        public IActionResult GetAllStructureData([FromQuery(Name = "ver")] int ver)
        {
            var response = CacheStructureDataManager.StructureInfos;
            return ReturnResponse(response);
        }

        [HttpGet]//group data 1
        [ResponseCache(Duration = 120, VaryByQueryKeys = new string[] { "ver" })]
        public IActionResult BuildingLevelDescription([FromQuery(Name = "ver")] int ver)
        {
            var response = CacheStructureDataManager.StructureInfoFactory;
            return ReturnResponse(response);
        }

        [HttpGet]//group data 1
        [ResponseCache(Duration = 120, VaryByQueryKeys = new string[] { "ver" })]
        public IActionResult GetAllTroopData([FromQuery(Name = "ver")] int ver)
        {
            var response = CacheTroopDataManager.TroopInfos;
            return ReturnResponse(response);
        }

        [HttpGet]//group data 1
        [ResponseCache(Duration = 120, VaryByQueryKeys = new string[] { "ver" })]
        public IActionResult GetAllTroopBuildingRelation([FromQuery(Name = "ver")] int ver)
        {
            var response = CacheTroopDataManager.TroopBuildingRelation;
            return ReturnResponse(response);
        }

        [HttpGet]//group data 1
        [ResponseCache(Duration = 120, VaryByQueryKeys = new string[] { "ver" })]
        public IActionResult GetAllHeroes([FromQuery(Name = "ver")] int ver)
        {
            var response = CacheHeroDataManager.HeroInfos;
            return ReturnResponse(response);
        }

        [HttpGet]//group data 1
        [ResponseCache(Duration = 120, VaryByQueryKeys = new string[] { "ver" })]
        public IActionResult GetAllItems([FromQuery(Name = "ver")] int ver)
        {
            return ReturnResponse(CacheItemManager.AllItems);
        }

        [HttpGet]//group data 1
        [ResponseCache(Duration = 120, VaryByQueryKeys = new string[] { "ver" })]
        public IActionResult GetAllInventories([FromQuery(Name = "ver")] int ver)
        {
            return ReturnResponse(CacheInventoryDataManager.AllInventoryItems);
        }

        [HttpGet]//group data 1
        [ResponseCache(Duration = 120, VaryByQueryKeys = new string[] { "ver" })]
        public IActionResult GetAllInventoryData([FromQuery(Name = "ver")] int ver)
        {
            return ReturnResponse(CacheInventoryDataManager.AllInventoryData);
        }


        [HttpGet]//group data 2
        [ResponseCache(Duration = 120, VaryByQueryKeys = new string[] { "ver" })]
        public IActionResult GetAllBoosts([FromQuery(Name = "ver")] int ver)
        {
            SpecNewBoostDataTable response;
            if (ver < 1)//old version, king type not declared
            {
                var data = CacheBoostDataManager.SpecNewBoostDataTables;
                response = new SpecNewBoostDataTable();
                response.CityBoosts = data.CityBoosts;
                response.VIPBoosts = data.VIPBoosts;
                response.Tables = data.Tables;

                var kingBoosts = data.Boosts.Find(x => x.Type == Common.Models.Boost.NewBoostType.KING);
                if (kingBoosts != null)
                {
                    response.Boosts = new List<SpecNewBoostData>(data.Boosts);
                    response.Boosts.Remove(kingBoosts);
                }
                else
                {
                    response.Boosts = data.Boosts;
                }
            }
            else
            {
                response = CacheBoostDataManager.SpecNewBoostDataTables;
            }

            return ReturnResponse(response);
        }

        [HttpGet]//group data 2
        [ResponseCache(Duration = 120, VaryByQueryKeys = new string[] { "ver" })]
        public IActionResult GetAllTechnologies([FromQuery(Name = "ver")] int ver)
        {
            var response = CacheTechnologyDataManager.TechnologyGroups;
            return ReturnResponse(response);
        }

        [HttpGet]//group data 2
        [ResponseCache(Duration = 120, VaryByQueryKeys = new string[] { "ver" })]
        public IActionResult GetAllChapters([FromQuery(Name = "ver")] int ver)
        {
            var response = CacheQuestDataManager.ChapterQuests;
            return ReturnResponse(response);
        }

        [HttpGet]//group data 2
        [ResponseCache(Duration = 120, VaryByQueryKeys = new string[] { "ver" })]
        public IActionResult GetAllDailyQuests([FromQuery(Name = "ver")] int ver)
        {
            var response = CacheQuestDataManager.DailyQuests;
            return ReturnResponse(response);
        }





        readonly string[] monsterNames = new string[] { "Cyclops", "Darkness Spider", "Ghoul", "Golem Rock", "Oak Tree", "Troll", "Vampire" };
        [HttpGet]
        public IActionResult GetAllMonsters()//obsolete
        {
            var response = new List<EntityData>();
            foreach (var name in monsterNames)
            {
                var hp = 10000;
                var atk = 5000;
                var def = 5000;
                response.Add(new EntityData(EntityType.Default, name, hp, atk, def));
            }

            return ReturnResponse(response);
        }

        [HttpGet]
        public IActionResult GetAllResources()//obsolete
        {
            var response = CacheResourceDataManager.ResourceInfos;
            return ReturnResponse(response);
        }

        [HttpGet]
        public IActionResult GetAllSideQuests()//obsolete or not used in new version
        {
            return ReturnResponse(null);
        }

/*        [HttpGet]
        public IActionResult GetAllItems()//obsolete use GetAllInventories
        {
            var response = CacheInventoryDataManager.AllInventoryItems;
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
                                };* /
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
        }*/
    }
}

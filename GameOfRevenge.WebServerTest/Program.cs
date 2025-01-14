using System;
using GameOfRevenge.Business;
using System.Configuration;
using System.Threading.Tasks;
using GameOfRevenge.Business.Manager.UserData;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Business.Manager.Kingdom;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Common.Models.Kingdom.AttackAlertReport;
using GameOfRevenge.Business.Manager.GameDef;
using GameOfRevenge.WebServer.Controllers.Api;
using GameOfRevenge.Common;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Common.Net;
using System.Collections.Generic;
using Newtonsoft.Json;
using GameOfRevenge.Business.Manager;
using Newtonsoft.Json.Converters;

namespace GameOfRevenge.WebServer
{
    public class Program
    {
        static async Task Main(string[] args)
        {

            //            Config.ConnectionString = ConfigurationManager.AppSettings["ConString"];
//            Config.ConnectionString = "Data Source=52.194.60.233,1433;Initial Catalog=GameOfRevenge;Persist Security Info=True;User ID=developer;Password=developer";
//            Config.ConnectionString = "Data Source=162.19.19.108,1433;Initial Catalog=GameOfRevenge;Persist Security Info=True;User ID=developer;Password=developer";
            Config.ConnectionString = "Data Source=141.95.53.0,1433;Initial Catalog=GameOfRevenge;Persist Security Info=True;User ID=sa;Password=*a%w5SmIys4sx2Iq";
//            Config.ConnectionString = "Data Source=162.19.19.108,1433;Initial Catalog=GameOfRevenge;Persist Security Info=True;User ID=sa;Password=*a%w5SmIys4sx2Iq";

            Config.DefaultWorldCode = ConfigurationManager.AppSettings["DefaultWorldCode"];

            Console.WriteLine(">>>>>> Main "+ Config.ConnectionString);
            await Test();
        }

        private static async Task Test()
        {
            var uqm = new QuestManager();
//            var response = uqm.GetAllChapterQuestRelData();
//            var response = uqm.GetAllQuestProgress(71);
            var sm = new StructureManager();
//                        var response = await sm.GetAllStructureBuildLimit();

            System.Console.WriteLine("----------------");
            System.Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(CacheStructureDataManager.StructureInfos));
            System.Console.WriteLine("----------------");
        }

        private static async Task TestBattle()
        {
//               await Startup.ReloadDataBaseDataAsync();
/*            val.Troops = (new List<TroopInfos>());
            val.EndTime = val.TaskTime.AddMilliseconds(delay + socketResponse.ReachedTime);
            val.Heros = request.HeroIds != null ? request.HeroIds.ToList() : new List<int>();

            var location = new MapLocation() { X = attacker.Tile.X, Y = attacker.Tile.Y };*/
            KingdomPvPManager pvpManager = new KingdomPvPManager();
//            var attackBattle = pvpManager.AttackOtherPlayer(71, 1071, val, location);


            //            Enemy = attacker.World.PlayersManager.GetPlayer(request.EnemyUserName);

/*            PlayerCompleteData attackerArmy = new PlayerCompleteData();
            attackerArmy.King = new UserKingDetails();
            attackerArmy.Resources = new ResourcesList();
            attackerArmy.Structures = new List<StructureInfos>();
            attackerArmy.Boosts = new List<UserRecordNewBoost>();
            attackerArmy.Troops = new List<TroopInfos>();
            var troop1 = new TroopInfos();
            troop1.TroopType = TroopType.Swordsman;
            troop1.TroopData = new List<TroopDetails>();
            attackerArmy.Troops.Add(troop1);
                var troopData1 = new TroopDetails();
                troopData1.Count = 1100;
                troopData1.Level = 1;
                troop1.TroopData.Add(troopData1);

                troopData1 = new TroopDetails();
                troopData1.Count = 1100;
                troopData1.Level = 2;
                troop1.TroopData.Add(troopData1);


            var marching = new MarchingArmy();
            marching.Troops = new List<TroopInfos>();
                var troop = new TroopInfos();
                troop.TroopType = TroopType.Swordsman;
                troop.TroopData = new List<TroopDetails>();
                marching.Troops.Add(troop);
                    var troopData = new TroopDetails();
                    troopData.Level = 1;
                    troopData.Count = 1000;
                    troop.TroopData.Add(troopData);

                    troopData = new TroopDetails();
                    troopData.Level = 2;
                    troopData.Count = 100;
                    troop.TroopData.Add(troopData);

/ *                troop = new TroopInfos();
                troop.TroopType = TroopType.Swordsman;
                troop.TroopData = new List<TroopDetails>();
                troop.TroopData.Add(troopData);
                marching.Troops.Add(troop);* /

            Console.WriteLine("---initial: ");
            System.Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(attackerArmy));

//            var attkResp = pvpManager.UpdatePlayerArmyToMarch(attackerArmy, marching, true);
//            Console.WriteLine("attk = " + attkResp);

            Console.WriteLine("---marching: ");
            System.Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(attackerArmy) );

            PlayerCompleteData defenderArmy = new PlayerCompleteData();
            defenderArmy.Resources = new ResourcesList();
            defenderArmy.Structures = new List<StructureInfos>();
            defenderArmy.Boosts = new List<UserRecordNewBoost>();
            defenderArmy.Troops = new List<TroopInfos>();
            var troop2 = new TroopInfos();
                troop2.TroopType = TroopType.Swordsman;
                troop2.TroopData = new List<TroopDetails>();
                    var troopData2 = new TroopDetails();
                    troopData2.Count = 200;
                    troopData2.Level = 1;
                troop2.TroopData.Add(troopData2);
            defenderArmy.Troops.Add(troop2);

//            var respbattle = await pvpManager.BattleSimulation(attackerArmy, defenderArmy);
            var response = "";//respbattle.Data;
            Console.WriteLine("---result: ");
            Console.WriteLine("---attacker: ");
            System.Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(attackerArmy.MarchingArmy));
            System.Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(attackerArmy.Troops) );
            Console.WriteLine("---defender: ");
            System.Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(defenderArmy.Troops) );*/


//            var plyManager = new PlayerDataManager();
//            var resp = await plyManager.GetPlayerData(71, Common.DataType.ActiveBuffs, 1);


//            int itemId = CacheInventoryDataManager.GetFullInventoryItemData(Common.Models.Inventory.InventoryItemType.Weapon).Id;
//            UserInventoryManager uim = new UserInventoryManager();
//            var response1 = await uim.AddUniqueItem(71, Common.Models.Inventory.InventoryItemType.Weapon);
//            var response1 = await uim.UpdateItem(71, 10818, 1);
//            var response1 = await uim.UpdateItem(71, Common.Models.Inventory.InventoryItemType.Weapon, 1);
            //            var resp = await uim.UpdateItem(71, Common.Models.Inventory.InventoryItemType.Weapon, 1);
//                        var response = await uim.GetPlayerData(71);//, DataType.Inventory);
            //            var resp = response.Data?.Items;
//            System.Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(response1));

//   await Startup.ReloadDataBaseDataAsync();

            var pdm = new PlayerDataManager();
            var acm = new AccountManager();
            var qsm = new UserQuestManager();
//            var response = await qsm.RedeemQuestReward(1103, 22);//GetAllSideQuestRelData();

            var sm = new StructureManager();
//            var response = await sm.GetAllStructureBuildLimit();

            var urm = new UserResourceManager();
//            var response = await urm.GetFullPlayerData(1103);// GetPlayerData(71);
//            var response = await new UserActiveBoostManager().AddBoost(71, Common.Models.Boost.NewBoostType.Shield);
//            var response = CacheBoostDataManager.SpecNewBoostDataTables;
//            var response = await pdm.GetAllPlayerData(1103);//, DataType.Inventory);
                                                          //            var response = await pdm.GetAllPlayerData(71, DataType.Resource, (int)ResourceType.Gems);//, (int)Common.Models.Inventory.InventoryItemType.Weapon);
                                                          //            if (response.Data == null) return;
                                                          //            var resp = response1;

            //            var manager = new BoostManager();

            //            GameDefController gdc = new GameDefController(null);
            //            var resp = gdc.GetAllInventory();
            //            var resp = CacheInventoryDataManager.ItemTypes;

            //            var response = CacheInventoryDataManager.ItemList;
            //var response = CacheHeroDataManager.HeroInfos;

            ///               var response = await acm.ChangeName(1104, "dev2");


//config.Formatters.JsonFormatter.S‌​erializerSettings
/*                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.None,
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    PreserveReferencesHandling = PreserveReferencesHandling.None
                };
                IsoDateTimeConverter dateConverter = new IsoDateTimeConverter
                {
                    DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'"
                };
                settings.Converters.Add(dateConverter);*/
//                System.Web.Http.GlobalConfiguration.Configuration.Formatters.JsonFormatter.UseDataContractJsonSerializer = true;



//            string resp = JsonConvert.SerializeObject(response, settings);
//            resp = resp.Replace("\\", "");
/*
            resp = resp.ToString()
        .Replace("\"[", "[").Replace("]\"", "]")
        .Replace("\\\"{", "{").Replace("}\\\"", "}")
        .Replace("\\\\\\\"", "\"");
*/


            //                        var resp = await uim.GetAllInventoryItems();//GetAllBoostTypes();
            System.Console.WriteLine("----------------");
//            System.Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(response) );
            System.Console.WriteLine("----------------");
//            System.Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject( resp, new Newtonsoft.Json.Converters.StringEnumConverter()));


            /*            var manager = new ClanManager();
                        //var response = await manager.GetPlayerClanData(71);
                        //            var response = await manager.GetFullClanData(71, 2);
                        var response = await manager.GetClans("", "", true, 1, 10);
            //            var response = await manager.GetPlayerClanData(71);
                        //            var manager = new PlayerDataManager();

                        //            var response = await manager.GetAllPlayerStoredData(1071);
                        string res = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                        System.Console.WriteLine(res);
            */



        }
    }
}

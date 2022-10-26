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

namespace GameOfRevenge.WebServer
{
    public class Program
    {
        static void Main(string[] args)
        {

            //            Config.ConnectionString = ConfigurationManager.AppSettings["ConString"];
//            Config.ConnectionString = "Data Source=52.194.60.233,1433;Initial Catalog=GameOfRevenge;Persist Security Info=True;User ID=developer;Password=developer";
            Config.ConnectionString = "Data Source=162.19.19.108,1433;Initial Catalog=GameOfRevenge;Persist Security Info=True;User ID=developer;Password=developer";

            Config.DefaultWorldCode = ConfigurationManager.AppSettings["DefaultWorldCode"];

            Console.WriteLine(">>>>>> Main "+ Config.ConnectionString);
            Test().Wait();
        }

        private static async Task Test()
        {
/*            val.Troops = (new List<TroopInfos>());
            val.EndTime = val.TaskTime.AddMilliseconds(delay + socketResponse.ReachedTime);
            val.Heros = request.HeroIds != null ? request.HeroIds.ToList() : new List<int>();

            var location = new MapLocation() { X = attacker.Tile.X, Y = attacker.Tile.Y };
            IKingdomPvPManager pvpManager = new KingdomPvPManager();
            var attackBattle = pvpManager.AttackOtherPlayer(71, 1071, val, location);*/


            //            Enemy = attacker.World.PlayersManager.GetPlayer(request.EnemyUserName);

/*            int attackerId = 0;
            PlayerCompleteData attackerArmy = new PlayerCompleteData();
            int defenderId = 1;
            PlayerCompleteData defenderArmy = null;*/


//            string response = null;// await pvpManager.BattleSimulation(attackerId, attackerArmy, defenderId, defenderArmy);

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

   await Startup.ReloadDataBaseDataAsync();

            var pdm = new PlayerDataManager();
            var urm = new UserResourceManager();
            var response = await urm.GetFullPlayerData(71);// GetPlayerData(71);
//            var response = await new UserActiveBoostManager().AddBoost(71, Common.Models.Boost.NewBoostType.Shield);
//            var response = CacheBoostDataManager.SpecNewBoostDataTables;
//            var response = await pdm.GetAllPlayerData(71);//, DataType.Inventory);
//            var response = await pdm.GetAllPlayerData(71, DataType.Resource, (int)ResourceType.Gems);//, (int)Common.Models.Inventory.InventoryItemType.Weapon);
//            if (response.Data == null) return;
//            var resp = response1;

            //            var manager = new BoostManager();

//            GameDefController gdc = new GameDefController(null);
//            var resp = gdc.GetAllInventory();
//            var resp = CacheInventoryDataManager.ItemTypes;

//            var response = CacheInventoryDataManager.ItemList;
//var response = CacheHeroDataManager.HeroInfos;
            string resp = JsonConvert.SerializeObject(response);//, settings);
//            resp = resp.Replace("\\", "");
/*
            resp = resp.ToString()
        .Replace("\"[", "[").Replace("]\"", "]")
        .Replace("\\\"{", "{").Replace("}\\\"", "}")
        .Replace("\\\\\\\"", "\"");
*/


            //                        var resp = await uim.GetAllInventoryItems();//GetAllBoostTypes();
            System.Console.WriteLine("----------------");
            System.Console.WriteLine(resp);// Newtonsoft.Json.JsonConvert.SerializeObject(resp) );
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

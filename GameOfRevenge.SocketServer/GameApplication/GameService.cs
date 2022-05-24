using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ExitGames.Logging;
using GameOfRevenge.Buildings.Handlers;
using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.Business;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Business.Manager;
using GameOfRevenge.Business.Manager.UserData;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Common.Models.Kingdom;
using GameOfRevenge.GameHandlers;
using GameOfRevenge.Interface;
using GameOfRevenge.Troops;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Business.Manager.GameDef;
using GameOfRevenge.Business.Manager.Kingdom;

namespace GameOfRevenge.GameApplication
{
    public static class GameService
    {
        private static object instance = null;
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public static Dictionary<StructureType, IGameBuildingManager> GameBuildingManager { get; private set; }
        public static Dictionary<TroopType, IGameTroop> Troops { get; private set; }
        public static IStructureManager BStructureManager { get; private set; }
        public static IUserTroopManager BUsertroopManager { get; private set; }
        public static IPlayerDataManager BPlayerManager { get; private set; }
        public static IUserStructureManager BPlayerStructureManager { get; private set; }
        public static IUserResourceManager BPlayerResourceManager { get; private set; }
        public static IKingdomManager BKingdomManager { get; private set; }
        public static IKingdomPvPManager BkingdomePvpManager { get; private set; }
        public static ITechnologyManager BTechnologyManager { get; private set; }
        public static IRealTimeUpdateManager BRealTimeUpdateManager { get; private set; }
        public static IUserTechnologyManager BUserTechnologyManager { get; private set; }
        public static IInstantProgressManager InstantProgressManager { get; private set; }
        public static GameLobbyHandler GameLobby { get; private set; }
        public static IWorldHandler WorldHandler { get; private set; }
        public static NewRealTimeUpdateManager NewRealTimeUpdateManager { get; private set; }

        public static void StartInstance()
        {
            if (instance != null) return;

            GameBuildingManager = new Dictionary<StructureType, IGameBuildingManager>();
            Troops = new Dictionary<TroopType, IGameTroop>();

            //Config.ConnectionString = ConfigurationManager.AppSettings["ConString"];
            //Config.DefaultWorldCode = ConfigurationManager.AppSettings["DefaultWorldCode"];

            Config.ConnectionString = "Data Source=135.125.204.124,1433;Initial Catalog=GameOfRevenge;Persist Security Info=True;User ID=developer;Password=developer";
            Config.DefaultWorldCode = "AADSPX";

            //Database Game Def
            CacheStructureDataManager.LoadCacheMemory();
            CacheResourceDataManager.LoadCacheMemory();
            CacheTroopDataManager.LoadCacheMemory();

            WorldHandler = new WorldHandler();
            GameLobby = new GameLobbyHandler();

            //Business Interfaces
            InstantProgressManager = new InstantProgressManager();
            BStructureManager = new StructureManager();
            NewRealTimeUpdateManager = new NewRealTimeUpdateManager();
            BUserTechnologyManager = new UserTechnologyManager();
            BTechnologyManager = new TechnologyManager();
            BPlayerStructureManager = new UserStructureManager();
            BPlayerManager = new PlayerDataManager();
            BPlayerResourceManager = new UserResourceManager();
            BUsertroopManager = new UserTroopManager();
            BKingdomManager = new KingdomManager();
            BkingdomePvpManager = new KingdomPvPManager();
            BRealTimeUpdateManager = new RealTimeUpdateManager();

            instance = new object();


            List<WorldDataTable> worldData = null;

            var world = BKingdomManager.GetWorld(Config.DefaultWorldCode).Result;
            //if (!world.IsSuccess) world = BKingdomManager.CreateWorld(Config.DefaultWorldCode).Result;
            //else
            //{
            //    log.InfoFormat("Get World {0} ", JsonConvert.SerializeObject(world));
            //    worldData = BKingdomManager.GetWorldTileData(world.Data.Id).Result.Data;
            //}

            //if (!world.IsSuccess || !world.HasData) throw new Exception(world.Message);
            //else
            //{
                log.InfoFormat("Get World {0} ", JsonConvert.SerializeObject(world));
                worldData = BKingdomManager.GetWorldTileData(world.Data.Id).Result.Data;
            //}

            WorldHandler.SetupPvpWorld(world.Data.Id, worldData);

            // GetAllTroopsOnSocketServer();
            var troops = CacheTroopDataManager.TroopInfos;
            //log.InfoFormat("Troops Data {0} ", JsonConvert.SerializeObject(troops));
            TroopFactory factory = new ConcreteTroopFactory();
            foreach (var troop in troops)
                Troops.Add(troop.Info.Code, factory.GetAllGameTroops(troop));


            //ConstructGameBuildingOnSocketServer();
            var structureData = CacheStructureDataManager.StructureInfos;
            //log.InfoFormat("Structure Data {0} ", JsonConvert.SerializeObject(structureData));
            foreach (var item in structureData)
            {
                Dictionary<TroopType, IGameTroop> gtroops = new Dictionary<TroopType, IGameTroop>();
                switch (item.Info.Code)
                {
                    case StructureType.Barracks:
                        gtroops = Troops.Where(d => d.Value.TroopType == TroopType.Swordsmen).ToDictionary(f => f.Key, s => s.Value);
                        break;
                    case StructureType.InfantryCamp:
                        gtroops = Troops;
                        break;
                    case StructureType.WorkShop:
                        gtroops = Troops;
                        break;
                    case StructureType.ShootingRange:
                        gtroops = Troops.Where(f => f.Value.TroopType == TroopType.Archer).ToDictionary(f => f.Key, s => s.Value);
                        break;
                    case StructureType.Stable:
                        gtroops = Troops.Where(f => f.Value.TroopType == TroopType.Knight).ToDictionary(f => f.Key, s => s.Value);
                        break;
                }

                GameBuildingManager.Add(item.Info.Code, new GameBuildingManager(item, gtroops));
            }
        }
    }
}

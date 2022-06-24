using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Inventory;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models.Structure;

namespace GameOfRevenge.WebServer.Controllers.Api
{
    public class PlayerController : BaseApiController
    {
        private readonly IBaseUserManager userManager;
        private readonly IUserResourceManager userResourceManager;
        private readonly IUserStructureManager userStructureManager;
        private readonly IUserInventoryManager userInventoryManager;
        private readonly IUserActiveBuffsManager userActiveBuffsManager;
        private readonly IUserTroopManager userTroopManager;
        private readonly IUserTechnologyManager userTechnologyManager;
        private readonly IInstantProgressManager instantProgressManager;

        public PlayerController(IUserResourceManager userResourceManager, IUserStructureManager userStructureManager, IUserInventoryManager userInventoryManager, IUserActiveBuffsManager userActiveBuffsManager, IUserTechnologyManager userTechnologyManager, IUserTroopManager userTroopManager, IInstantProgressManager instantProgressManager)
        {
            userManager = userResourceManager;
            this.userResourceManager = userResourceManager;
            this.userStructureManager = userStructureManager;
            this.userInventoryManager = userInventoryManager;
            this.userActiveBuffsManager = userActiveBuffsManager;
            this.userTechnologyManager = userTechnologyManager;
            this.userTroopManager = userTroopManager;
            this.instantProgressManager = instantProgressManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserData() => ReturnResponse(await userResourceManager.GetPlayerData(Token.PlayerId));

#region Gate
        [HttpGet]
        public async Task<IActionResult> GetGateHp() => ReturnResponse(await userStructureManager.GetGateHp(Token.PlayerId));

        [HttpGet]
        public async Task<IActionResult> RepairGateCost() => ReturnResponse(await userStructureManager.RepairGateCost(Token.PlayerId));

        [HttpPost]
        public async Task<IActionResult> RepairGate() => ReturnResponse(await userStructureManager.RepairGate(Token.PlayerId));
#endregion

#region Resource
        [HttpPost]
        public async Task<IActionResult> AddResources(int food, int wood, int ore, int gems)
        {
            food = food <= 0 ? 0 : food;
            wood = wood <= 0 ? 0 : wood;
            ore = ore <= 0 ? 0 : ore;
            gems = gems <= 0 ? 0 : gems;

            return ReturnResponse(await userResourceManager.AddMainResource(Token.PlayerId, food, wood, ore, gems));
        }

        [HttpPost]
        public async Task<IActionResult> CollectResources(int locId)
        {
            return ReturnResponse(await userStructureManager.CollectResource(Token.PlayerId, locId));
        }

        [HttpPost]
        public async Task<IActionResult> GetAllPlayerStoredData(int structureLocationId = -1)
        {
            return ReturnResponse(await userResourceManager.GetAllPlayerStoredData(Token.PlayerId, structureLocationId));
        }

        [HttpPost]
        public async Task<IActionResult> StoreResource(int structureLocationId, int valueId, int value)
        {
            return ReturnResponse(await userResourceManager.StoreResource(Token.PlayerId, structureLocationId, valueId, value));
        }
#endregion

        #region Inventory Item
        [HttpPost]
        public async Task<IActionResult> BuyAndUseItem(InventoryItemType itemId, int count)
        {
            var buyResponse = await userInventoryManager.BuyItem(Token.PlayerId, itemId, count);
            if (!buyResponse.IsSuccess) return ReturnResponse(buyResponse);
            else return ReturnResponse(await userInventoryManager.UseItem(Token.PlayerId, itemId, count));
        }

        [HttpPost]
        public async Task<IActionResult> BuyItem(InventoryItemType itemId, int count) => ReturnResponse(await userInventoryManager.BuyItem(Token.PlayerId, itemId, count));

        [HttpPost]
        public async Task<IActionResult> UseItem(InventoryItemType itemId, int count) => ReturnResponse(await userInventoryManager.UseItem(Token.PlayerId, itemId, count));

        [HttpGet]
        public async Task<IActionResult> GetItemList()
        {
            var response = await userInventoryManager.GetPlayerData(Token.PlayerId);
            return ReturnResponse(new Response<List<InventoryInfo>>(response.Data?.Items, response.Case, response.Message));
        }
#endregion

#region Buff
        [HttpPost]
        public async Task<IActionResult> AddBuff(InventoryItemType itemId, int count)
        {
            if (!Token.IsAdmin) return StatusCode(401);
            return ReturnResponse(await userActiveBuffsManager.AddBuff(Token.PlayerId, itemId, count));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveBuff(InventoryItemType itemId, int count)
        {
            if (!Token.IsAdmin) return StatusCode(401);
            return ReturnResponse(await userActiveBuffsManager.RemoveBuff(Token.PlayerId, itemId, count));
        }

        [HttpGet]
        public async Task<IActionResult> GetBuffList()
        {
            var response = await userInventoryManager.GetPlayerData(Token.PlayerId);
            return ReturnResponse(new Response<List<UserBuffDetails>>(response.Data?.Buffs, response.Case, response.Message));
        }
#endregion

#region Hero
        [HttpGet]
        public async Task<IActionResult> GetHeroList()
        {
            var response = await userManager.GetPlayerData(Token.PlayerId);
            return ReturnResponse(new Response<List<UserHeroDetails>>(response.Data?.Heros, response.Case, response.Message));
        }
#endregion

#region Technology
        [HttpGet]
        public async Task<IActionResult> GetUserTechnologyData()
        {
            var response = await userManager.GetPlayerData(Token.PlayerId);
            return ReturnResponse(new Response<List<TechnologyInfos>>(response.Data?.Technologies, response.Case, response.Message));
        }

        [HttpPost]
        public async Task<IActionResult> UpgradeTechnologyDataByType(TechnologyType type)
        {
            var response = await userTechnologyManager.UpgradeTechnology(Token.PlayerId, type);
            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> UpgradeTechnologyDataById(int id)
        {
            var response = await userTechnologyManager.UpgradeTechnology(Token.PlayerId, id);
            return ReturnResponse(response);
        }
#endregion

#region Troops
        [HttpGet]
        public async Task<IActionResult> GetUserTroopData()
        {
            var response = await userManager.GetPlayerData(Token.PlayerId);
            return ReturnResponse(new Response<List<TroopInfos>>(response.Data?.Troops, response.Case, response.Message));
        }

        [HttpPost]
        public async Task<IActionResult> TrainTroopByType(TroopType type, int level, int count, int buildingId)
        {
            var response = await userTroopManager.TrainTroops(Token.PlayerId, type, level, count, buildingId);
            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> TrainTroopById(int id, int level, int count, int buildingId)
        {
            var response = await userTroopManager.TrainTroops(Token.PlayerId, id, level, count, buildingId);
            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddTroopByType(TroopType type, int level, int count)
        {
            //if (!Token.IsAdmin) return StatusCode(401);
            var response = await userTroopManager.AddTroops(Token.PlayerId, type, level, count);
            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddTroopById(int id, int level, int count)
        {
            //if (!Token.IsAdmin) return StatusCode(401);
            var response = await userTroopManager.AddTroops(Token.PlayerId, id, level, count);
            return ReturnResponse(response);
        }

        //public async Task<IActionResult> InstantTrainTroops(int buildingLoc)
        //{
        //    var response = await userTroopManager.InstantTrainTroops(Token.PlayerId, buildingLoc);
        //    return ReturnResponse(response);
        //}

#endregion

#region Instant Build
        [HttpGet]
        public IActionResult GetInstantBuildCost(StructureType type, int level) => ReturnResponse(instantProgressManager.GetInstantBuildCost(Token.PlayerId, type, level));

        [HttpGet]
        public async Task<IActionResult> GetBuildingSpeedUpCost(int locId) => ReturnResponse(await instantProgressManager.GetBuildingSpeedUpCost(Token.PlayerId, locId));

        [HttpPost]
        public async Task<IActionResult> InstantBuildStructure(StructureType type, int level, int locId) => ReturnResponse(await instantProgressManager.InstantBuildStructure(Token.PlayerId, type, level, locId));

        [HttpPost]
        public async Task<IActionResult> SpeedUpBuildStructure(int locId) => ReturnResponse(await instantProgressManager.SpeedUpBuildStructure(Token.PlayerId, locId));
#endregion

        [HttpGet]
        public IActionResult GetInstantRecruitCost(TroopType type, int level, int count) => ReturnResponse(instantProgressManager.GetInstantRecruitCost(type, level, count));

        [HttpPost]
        public async Task<IActionResult> InstantRecruitTroops(int locId, TroopType type, int level, int count) => ReturnResponse(await instantProgressManager.InstantRecruitTroops(Token.PlayerId, locId, type, level, count));
    }
}

using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Boost;
using GameOfRevenge.Common.Models.Inventory;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Models.Hero;
using GameOfRevenge.Common.Models.PlayerData;

namespace GameOfRevenge.WebServer.Controllers.Api
{
    public class PlayerController : BaseApiController
    {
        private readonly IBaseUserManager userManager;
        private readonly IUserResourceManager userResourceManager;
        private readonly IUserStructureManager userStructureManager;
        private readonly IUserInventoryManager userInventoryManager;
        private readonly IUserActiveBoostsManager userActiveBuffsManager;
        private readonly IUserTroopManager userTroopManager;
        private readonly IUserTechnologyManager userTechnologyManager;
        private readonly IInstantProgressManager instantProgressManager;
        private readonly IUserHeroManager userHeroManager;

        public PlayerController(IUserResourceManager userResourceManager, IUserStructureManager userStructureManager,
                                IUserInventoryManager userInventoryManager, IUserActiveBoostsManager userActiveBuffsManager,
                                IUserTechnologyManager userTechnologyManager, IUserTroopManager userTroopManager,
                                IInstantProgressManager instantProgressManager, IUserHeroManager userHeroManager)
        {
            userManager = userResourceManager;
            this.userResourceManager = userResourceManager;
            this.userStructureManager = userStructureManager;
            this.userInventoryManager = userInventoryManager;
            this.userActiveBuffsManager = userActiveBuffsManager;
            this.userTechnologyManager = userTechnologyManager;
            this.userTroopManager = userTroopManager;
            this.instantProgressManager = instantProgressManager;
            this.userHeroManager = userHeroManager;
        }

        [HttpPost]
        public async Task<IActionResult> GetUserData(int playerId = 0)
        {
            int plyId = (playerId == 0)? Token.PlayerId : playerId;
            return ReturnResponse(await userResourceManager.GetFullPlayerData(plyId));
        }

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

            return ReturnResponse(await userResourceManager.SumMainResource(Token.PlayerId, food, wood, ore, gems));
        }

        [HttpPost]//temporal api
        public async Task<IActionResult> RemoveResources(int food, int wood, int ore, int gems)
        {
            food = food <= 0 ? 0 : food;
            wood = wood <= 0 ? 0 : wood;
            ore = ore <= 0 ? 0 : ore;
            gems = gems <= 0 ? 0 : gems;

            return ReturnResponse(await userResourceManager.SumMainResource(Token.PlayerId, -food, -wood, -ore, -gems));
        }

        [HttpPost]
        public async Task<IActionResult> CollectResource(int locationId, StructureType structureType = StructureType.Unknown)
        {
            var response = await userStructureManager.CollectResource(Token.PlayerId, locationId, structureType);
            return ReturnResponse(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserResources()
        {
            return ReturnResponse(await userResourceManager.GetResources(Token.PlayerId));
        }

        [HttpPost]
        public async Task<IActionResult> GetAllPlayerStoredResource(int? locationId = null)
        {
            var response = await userResourceManager.GetAllPlayerStoredResource(Token.PlayerId, locationId);
            if (response.HasData && (locationId != null))
            {
                foreach (var element in response.Data)
                {
                    element.LocationId = 0;
                }
            }

            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> SetStoredResource(int locationId, ResourceType resourceType, int value)
        {
            return ReturnResponse(await userResourceManager.SetStoredResource(Token.PlayerId, locationId, resourceType, value));
        }
#endregion

#region Inventory Item
/*        [HttpPost]
        public async Task<IActionResult> UpdateItem(int playerDataId, int value)
        {
//            if (!Token.IsAdmin) return StatusCode(401);
            return ReturnResponse(await userInventoryManager.UpdateItem(Token.PlayerId, playerDataId, value));
        }*/

        [HttpPost]//used to buy king items
        public async Task<IActionResult> UpdateItemType(InventoryItemType type, int? value)
        {
            if ((type == InventoryItemType.Unknown) || (value == null))
            {
                return ReturnResponse(new Response()
                {
                    Case = 0,
                    Message = "Invalid parameters"
                });
            }
            else
            {
                var response = await userInventoryManager.UpdateItem(Token.PlayerId, type, -1, (int)value);
                return ReturnResponse(response);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateItemID(long id, int? value)
        {
            if ((id <= 0) || (value == null))
            {
                return ReturnResponse(new Response()
                {
                    Case = 0,
                    Message = "Invalid parameters"
                });
            }
            else
            {
                var response = await userInventoryManager.UpdateItem(Token.PlayerId, InventoryItemType.Unknown, id, (int)value);
                return ReturnResponse(response);
            }
        }




        [HttpPost]
        public async Task<IActionResult> AddUniqueItem(InventoryItemType type, int value = 1)
        {
            if ((type == InventoryItemType.Unknown))
            {
                return ReturnResponse(new Response()
                {
                    Case = 0,
                    Message = "Invalid parameters"
                });
            }
            else
            {
                return ReturnResponse(await userInventoryManager.AddUniqueItem(Token.PlayerId, type, value));
            }
        }




        [HttpPost]
        public async Task<IActionResult> AddItem(InventoryItemType type, int value = 1)//TODO:implement a way to check if old value is higher
        {
            if ((type == InventoryItemType.Unknown))
            {
                return ReturnResponse(new Response()
                {
                    Case = 0,
                    Message = "Invalid parameters"
                });
            }
            else
            {
                return ReturnResponse(await userInventoryManager.AddItem(Token.PlayerId, type, value));
            }
        }

        /*        [HttpPost]
                public async Task<IActionResult> AddItem(InventoryItemType itemType, int value, bool unique = true)
                {
                    return ReturnResponse(await userInventoryManager.AddItem(Token.PlayerId, itemType, value, unique));
                }*/



        [HttpPost]
        public async Task<IActionResult> AddItemType(InventoryItemType type, int? amount)
        {
            if ((type == InventoryItemType.Unknown) || (amount == null))
            {
                return ReturnResponse(new Response()
                {
                    Case = 0,
                    Message = "Invalid parameters"
                });
            }
            else
            {
                var response = await userInventoryManager.AddItem(Token.PlayerId, type, -1, (int)amount);
                return ReturnResponse(response);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddItemID(long id, int? amount)
        {
            if ((id <= 0) || (amount == null))
            {
                return ReturnResponse(new Response()
                {
                    Case = 0,
                    Message = "Invalid parameters"
                });
            }
            else
            {
                var response = await userInventoryManager.AddItem(Token.PlayerId, InventoryItemType.Unknown, id, (int)amount);
                return ReturnResponse(response);
            }
        }



        [HttpPost]
        public async Task<IActionResult> RemoveItemType(InventoryItemType type)
        {
            if (type == InventoryItemType.Unknown)
            {
                return ReturnResponse(new Response()
                {
                    Case = 0,
                    Message = "Invalid parameters"
                });
            }
            else
            {
                var response = await userInventoryManager.RemoveItem(Token.PlayerId, type, -1);
                return ReturnResponse(response);
            }
        }


        [HttpPost]
        public async Task<IActionResult> RemoveItemID(long id)
        {
            if (id <= 0)
            {
                return ReturnResponse(new Response()
                {
                    Case = 0,
                    Message = "Invalid parameters"
                });
            }
            else
            {
                var response = await userInventoryManager.RemoveItem(Token.PlayerId, InventoryItemType.Unknown, id);
                return ReturnResponse(response);
            }
        }


        [HttpPost]
        public async Task<IActionResult> BuyAndUseItem(InventoryItemType itemId, int count)
        {
/*            var response = await userInventoryManager.BuyItem(Token.PlayerId, itemId, count);
            if (response.IsSuccess)
            {
                response = await userInventoryManager.UseItem(Token.PlayerId, itemId, count);
            }*/

            return ReturnResponse();//response);
        }

        [HttpPost]
        public async Task<IActionResult> BuyItem(InventoryItemType itemId, int count)
        {
            return ReturnResponse();// await userInventoryManager.BuyItem(Token.PlayerId, itemId, count));
        }




        [HttpPost]
        public async Task<IActionResult> UseItem(InventoryItemType itemId, int count)
        {
            return ReturnResponse(await userInventoryManager.UseItem(Token.PlayerId, itemId, count));
        }

        [HttpGet]
        public async Task<IActionResult> GetItemList(InventoryItemType? type)
        {
            if (type == InventoryItemType.Unknown)
            {
                return ReturnResponse(new Response()
                {
                    Case = 0,
                    Message = "Invalid item type"
                });
            }
            else
            {
                var response = await userInventoryManager.GetFullPlayerData(Token.PlayerId);
                if (response.IsSuccess && response.HasData)
                {
                    string msg = string.Empty;
                    List<UserItemDetails> items = response.Data.Items;
                    if (items != null)
                    {
                        if (type != null)
                        {
                            items = items.FindAll(item => item.ItemType == type);
                        }
                        else
                        {
                            msg = "Full ";
                        }
                    }
                    else
                    {
                        items = new List<UserItemDetails>();
                    }
                    return ReturnResponse(new Response<List<UserItemDetails>>(items, response.Case, msg + "Item List"));
                }

                return ReturnResponse(response);
            }
        }
#endregion

#region Boost
        [HttpPost]
        public async Task<IActionResult> AddVIPPoints(int points)
        {
            return ReturnResponse(await userManager.AddVIPPoints(Token.PlayerId, points));
        }

        [HttpPost]
        public async Task<IActionResult> ActivateVIPBoosts()
        {
            return ReturnResponse(await userManager.ActivateVIPBoosts(Token.PlayerId));
        }

        [HttpPost]
        public async Task<IActionResult> AddBoost(NewBoostType type)
        {
//            if (!Token.IsAdmin) return StatusCode(401);
            return ReturnResponse(await userActiveBuffsManager.AddBoost(Token.PlayerId, type));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveBoost(NewBoostType itemId, int count)
        {
//            if (!Token.IsAdmin) return StatusCode(401);
            return ReturnResponse(await userActiveBuffsManager.RemoveBoost(Token.PlayerId, itemId, count));
        }

        [HttpGet]
        public async Task<IActionResult> GetActiveBoostList()
        {
            var response = await userInventoryManager.GetFullPlayerData(Token.PlayerId);
            return ReturnResponse(new Response<List<UserRecordNewBoost>>(response.Data?.Boosts, response.Case, response.Message));
        }
#endregion

#region Hero
        [HttpGet]
        public async Task<IActionResult> GetHeroList()
        {
            var response = await userManager.GetFullPlayerData(Token.PlayerId);
            if (response.IsSuccess)
            {
                return ReturnResponse(new Response<List<UserHeroDetails>>(response.Data?.Heroes, response.Case, "User heroes"));
            }
            else
            {
                return ReturnResponse(new Response(response.Case, response.Message));
            }
        }

/*        [HttpPost]
        public async Task<IActionResult> GetHeroDataList(HeroType type)
        {
            var response = await userHeroManager.GetHeroDataList(Token.PlayerId, type);
            if (response.IsSuccess && response.HasData)
            {
                return ReturnResponse(new Response<UserHeroDataList>(response.Data, response.Case, response.Message));
            }
            else
            {
                return ReturnResponse(new Response(response.Case, response.Message));
            }
        }*/

        [HttpPost]
        public async Task<IActionResult> UnlockHero(HeroType type)
        {
            var response = await userHeroManager.UnlockHero(Token.PlayerId, type);
            return ReturnResponse(new Response<UserHeroDetails>(response.Data, response.Case, response.Message));

/*            if (response.IsSuccess)
            {
                return ReturnResponse(new Response(response.Case, "Hero unlocked"));
            }
            else
            {
                return ReturnResponse(new Response(response.Case, response.Message));
            }*/
        }

        [HttpPost]
        public async Task<IActionResult> AddHeroWarPoints(HeroType heroType, int value)
        {
/*            var response = await userHeroManager.AddHeroWarPoints(Token.PlayerId, heroType, value);
            if (response.IsSuccess && response.HasData)
            {
                return ReturnResponse(new Response<int>(response.Data, response.Case, response.Message));
            }
            else
            {
                return ReturnResponse(new Response(response.Case, response.Message));
            }*/
            return ReturnResponse(null);
        }
        #endregion

#region Technology
        [HttpGet]
        public async Task<IActionResult> GetUserTechnologyData()
        {
            var response = await userManager.GetFullPlayerData(Token.PlayerId);

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


/*        [HttpGet]
        public async Task<IActionResult> GetUserSubTechnologyData()
        {
            var response = await userManager.GetFullPlayerData(Token.PlayerId);

            return ReturnResponse(new Response<List<SubTechnologyInfos>>(response.Data?.SubTechnologies, response.Case, response.Message));
        }

        [HttpPost]
        public async Task<IActionResult> UpgradeSubTechnologyDataByType(SubTechnologyType type)
        {
            var response = await userTechnologyManager.UpgradeSubTechnology(Token.PlayerId, type);
            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> UpgradeSubTechnologyDataById(int id)
        {
            var response = await userTechnologyManager.UpgradeSubTechnology(Token.PlayerId, id);
            return ReturnResponse(response);
        }*/

#endregion

#region Troops
        [HttpGet]
        public async Task<IActionResult> GetUserTroopData()
        {
            var response = await userManager.GetFullPlayerData(Token.PlayerId);
            return ReturnResponse(new Response<List<TroopInfos>>(response.Data?.Troops, response.Case, response.Message));
        }

        [HttpPost]
        public async Task<IActionResult> TrainTroopByType(TroopType type, int level, int count, int location)
        {
            var response = await userTroopManager.TrainTroops(Token.PlayerId, type, level, count, location);
            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> TrainTroopById(int id, int level, int count, int location)
        {
            var response = await userTroopManager.TrainTroops(Token.PlayerId, id, level, count, location);
            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddTroopByType(TroopType type, int level, int amount)
        {
            //if (!Token.IsAdmin) return StatusCode(401);
            var response = await userTroopManager.AddTroops(Token.PlayerId, type, level, amount);
            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddTroopById(int id, int level, int amount)
        {
            //if (!Token.IsAdmin) return StatusCode(401);
            //TODO: implement error messages, when parameter amount is set as count, there are no fail message.
            var response = await userTroopManager.AddTroops(Token.PlayerId, id, level, amount);
            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> GetRanking()
        {
            var response = await userManager.GetRanking(Token.PlayerId);
            return ReturnResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> GetRankings(long rankId = 0)
        {
            var response = await userManager.GetRankings(rankId);
            return ReturnResponse(response);
        }

        //public async Task<IActionResult> InstantTrainTroops(int buildingLoc)
        //{
        //    var response = await userTroopManager.InstantTrainTroops(Token.PlayerId, buildingLoc);
        //    return ReturnResponse(response);
        //}

        #endregion
        //OBSOLETE, remove later
        [HttpPost]
        public async Task<IActionResult> HelpBuilding(int toPlayerId, StructureType type, int locId, int helpSeconds)
        {
            return ReturnResponse();
//            return ReturnResponse(await userStructureManager.HelpBuilding(Token.PlayerId, toPlayerId, type, locId, helpSeconds));
        }


#region Instant Build
        [HttpPost]
        public IActionResult GetInstantBuildCost(StructureType type, int level) => ReturnResponse(instantProgressManager.GetInstantBuildCost(Token.PlayerId, type, level));

        [HttpPost]
        public async Task<IActionResult> GetBuildingSpeedUpCost(int locId) => ReturnResponse(await instantProgressManager.GetBuildingSpeedUpCost(Token.PlayerId, locId));

        [HttpPost]
        public async Task<IActionResult> InstantBuildStructure(StructureType type, int level, int locId) => ReturnResponse(await instantProgressManager.InstantBuildStructure(Token.PlayerId, type, level, locId));

        [HttpPost]
        public async Task<IActionResult> SpeedUpBuildStructure(int locId) =>
            ReturnResponse(await instantProgressManager.SpeedUpBuildStructure(Token.PlayerId, locId));

        [HttpPost]
        public IActionResult GetInstantRecruitCost(TroopType type, int level, int count) => ReturnResponse(instantProgressManager.GetInstantRecruitCost(type, level, count));

        [HttpPost]
        public async Task<IActionResult> InstantRecruitTroops(int locId, TroopType type, int level, int count) => ReturnResponse(await instantProgressManager.InstantRecruitTroops(Token.PlayerId, locId, type, level, count));
        #endregion
    }
}

using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Business.Manager.UserData;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Helper;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Boost;
using GameOfRevenge.Common.Models.Hero;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameOfRevenge.Business.Manager.Base
{
    public class BaseUserDataManager : IBaseUserManager
    {
        protected static readonly IAccountManager accountManager = new AccountManager();
        protected static readonly IPlayerDataManager manager = new PlayerDataManager();

        protected IReadOnlyDataRequirement GetGemReq(int value)
        {
            return new DataRequirement() { DataType = DataType.Resource, ValueId = CacheResourceDataManager.Gems.Id, Value = value };
        }

        public async Task<Response<PlayerCompleteData>> GetFullPlayerData(int playerId)
        {
            string line = "-";
            try
            {
                var currentTimestamp = DateTime.UtcNow;
                var playerName = "";
                var resp = await accountManager.GetAccountInfo(playerId);
                if (resp.IsSuccess)
                {
                    playerName = resp.Data.Name;
                }

                var response = await manager.GetAllPlayerData(playerId);

                line = "1";
                if (response.IsSuccess)
                {
//                    System.Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(response.Data));

                    var finalData = new Response<PlayerCompleteData>()
                    {
                        Case = 100,
                        Message = "Complete player data",
                        Data = new PlayerCompleteData()
                        {
                            PlayerId = playerId,
                            PlayerName = playerName,
                            HelpedBuild = 0,

                            Resources = new ResourcesList(),
                            Structures = new List<StructureInfos>(),
                            Troops = new List<TroopInfos>(),
                            Items = new List<UserItemDetails>(),
                            Technologies = new List<TechnologyInfos>(),
                            SubTechnologies = new List<SubTechnologyInfos>(),
                            Boosts = new List<FullUserBoostDetails>(),
                            Heroes = new List<UserHeroDetails>()
                        }
                    };

                    line = "2";
                    var customs = response.Data.Where(x => x.DataType == DataType.Custom)?.ToList();
                    line = "3";
                    var kingData = customs?.Find(x => x.ValueId == 1);
                    line = "4";
                    if (kingData != null)
                    {
                        try
                        {
                            finalData.Data.King = JsonConvert.DeserializeObject<UserKingDetails>(kingData.Value);
                        }
                        catch {}
                    }
                    if (finalData.Data.King == null) finalData.Data.King = new UserKingDetails();

                    line = "5";

                    var userResources = response.Data.Where(x => x.DataType == DataType.Resource)?.ToList();
                    var userStructures = response.Data.Where(x => x.DataType == DataType.Structure)?.ToList();
                    var userTroops = response.Data.Where(x => x.DataType == DataType.Troop)?.ToList();
                    var userMarching = response.Data.Where(x => x.DataType == DataType.Marching)?.FirstOrDefault();
                    var userTechnologies = response.Data.Where(x => x.DataType == DataType.Technology)?.ToList();
                    var userSubTechs = response.Data.Where(x => x.DataType == DataType.SubTechnology)?.ToList();
                    var userItems = response.Data.Where(x => x.DataType == DataType.Inventory)?.ToList();
                    var userBoosts = response.Data.Where(x => x.DataType == DataType.ActiveBoost)?.ToList();
                    var userHeroData = response.Data.Where(x => x.DataType == DataType.Hero)?.ToList();
                    var userActivities = response.Data.Where(x => x.DataType == DataType.Activity)?.ToList();
                    line = "6";

                    if (userActivities != null)
                    {
                        foreach (var item in userActivities)
                        {
                            if (item == null) continue;
                            if (item.ValueId != 1) continue;

                            int.TryParse(item.Value, out int value);
                            finalData.Data.HelpedBuild = value;
                            break;
                        }
                    }

                    line = "7";
                    if (userResources != null)
                    {
                        var resourceList = new List<UserResourceData>();
                        foreach (var item in userResources)
                        {
                            if (item == null) continue;

                            resourceList.Add(PlayerDataToUserResourceData(item));
                        }
                        finalData.Data.Resources.Food = (int)resourceList.FirstOrDefault(x => x.ValueId == ResourceType.Food)?.Value;
                        finalData.Data.Resources.Wood = (int)resourceList.FirstOrDefault(x => x.ValueId == ResourceType.Wood)?.Value;
                        finalData.Data.Resources.Ore = (int)resourceList.FirstOrDefault(x => x.ValueId == ResourceType.Ore)?.Value;
                        finalData.Data.Resources.Gems = (int)resourceList.FirstOrDefault(x => x.ValueId == ResourceType.Gems)?.Value;
                    }

                    line = "8";
                    if (userStructures != null)
                    {
                        var structureList = new List<UserStructureData>();
                        foreach (var item in userStructures)
                        {
                            if (item == null) continue;

                            structureList.Add(PlayerDataToUserStructureData(item));
                        }

                        foreach (var item in structureList)
                        {
                            finalData.Data.Structures.Add(new StructureInfos()
                            {
                                StructureType = item.ValueId,
                                Buildings = item.Value
                            });
                        }
                    }

                    line = "9";
                    if (userTroops != null)
                    {
                        var troopList = new List<UserTroopData>();
                        foreach (var item in userTroops)
                        {
                            if (item == null) continue;

                            troopList.Add(PlayerDataToUserTroopData(item));
                        }
                        foreach (var userTroop in troopList)
                        {
                            //                            var troopData = CacheTroopDataManager.GetFullTroopData(userTroop.ValueId);

                            if (userTroop.Value != null)
                            {
                                foreach (TroopDetails troop in userTroop.Value)
                                {
                                    if (troop.InRecovery != null)
                                    {
                                        troop.InRecovery = troop.InRecovery.Where(x => x.TimeLeft > 0).ToList();
                                        if (troop.InRecovery.Count == 0) troop.InRecovery = null;
                                    }

                                    if (troop.InTraning != null)
                                    {
                                        troop.InTraning = troop.InTraning.Where(x => x.TimeLeft > 0).ToList();
                                        if (troop.InTraning.Count == 0) troop.InTraning = null;
                                    }
                                }

                                finalData.Data.Troops.Add(new TroopInfos()
                                {
                                    Id = userTroop.Id,
                                    TroopType = userTroop.ValueId,
                                    TroopData = userTroop.Value
                                });
                            }
                        }
                    }

                    line = "10";
                    if (userMarching != null)
                    {
                        finalData.Data.MarchingArmy = JsonConvert.DeserializeObject<MarchingArmy>(userMarching.Value);
                    }


                    //POPULATE TECHNOLOGIES
/*                    var techs = CacheTechnologyDataManager.TechnologyInfos;
                    foreach (var tech in techs)
                    {
                        finalData.Data.Technologies.Add(new TechnologyInfos()
                        {
                            TechnologyType = tech.Info.Code,
                            Level = 0
                        });
                    }*/

                    line = "11";
                    if (userTechnologies != null)
                    {
                        foreach (var tech in userTechnologies)
                        {
                            if (tech == null) continue;

                            var technology = PlayerDataToUserTechnologyData(tech);
    /*                        foreach (var item in finalData.Data.Technologies)
                            {
                                if (technology.ValueId == item.TechnologyType)
                                {
                                    item.Level = technology.Value.Level;
                                    item.StartTime = technology.Value.StartTime;
                                    item.EndTime = technology.Value.EndTime;
                                }
                            }*/
                            finalData.Data.Technologies.Add(technology.Value);
                        }
                    }

                    if (userSubTechs != null)
                    {
                        foreach (var tech in userSubTechs)
                        {
                            if (tech == null) continue;

                            var subTech = PlayerDataToUserSubTechnologyData(tech);
                            finalData.Data.SubTechnologies.Add(subTech.Value);
                        }
                    }

                    //POPULATE ITEMS
/*                    var items = CacheInventoryDataManager.ItemInfos;
                    foreach (var item in items)
                    {
                        finalData.Data.Items.Add(new InventoryInfo()
                        {
                            ItemType = item.Code,
                            Level = 0
                        });
                    }*/
                    line = "12";

                    if (userItems != null)
                    {
                        foreach (var pItemData in userItems)
                        {
                            if (pItemData == null) continue;

                            var itemData = PlayerDataToUserInventoryData(pItemData);
                            if (itemData != null)
                            {
                                finalData.Data.Items.Add(new UserItemDetails
                                {
                                    id = itemData.Id,
                                    ItemType = itemData.ValueId,
                                    Level = itemData.Value
                                });
                            }
    /*                            foreach (var item in finalData.Data.Items)
                            {
                                if (itemData.ValueId == item.ItemType)
                                    item.Level = itemData.Value;
                            }*/
                        }
                    }

                    line = "13";
                    //ADD BOOST
                    foreach (var item in userBoosts)
                    {
                        var bufdData = PlayerDataToUserBoostData(item);
                        if (bufdData != null)
                        {
                            FullUserBoostDetails fullUserBoost = new FullUserBoostDetails()
                            {
                                Id = bufdData.Id,
                                BoostType = bufdData.Value.BoostType,
                                StartTime = bufdData.Value.StartTime,
                                EndTime = bufdData.Value.EndTime
                            };
                            finalData.Data.Boosts.Add(fullUserBoost);
                        }
                    }


                    line = "14";
                    int line2 = 0;
                    ////ADD HEROES
                    if (userHeroData != null)
                    {
                        foreach (var heroInfo in CacheHeroDataManager.HeroInfos)
                        {
//                            HeroType heroType = heroInfo.Info.Code.ToEnum<HeroType>();

                            var userHero = userHeroData.Find(x => x.ValueId == heroInfo.Info.HeroId);
                            if (userHero == null) continue;

                            var userHeroDetails = JsonConvert.DeserializeObject<UserHeroDetails>(userHero.Value);
                            if (userHeroDetails == null) continue;

                            var data = new UserHeroDetails()
                            {
                                HeroCode = heroInfo.Info.Code,
                                Points = userHeroDetails.Points,
                                Power = userHeroDetails.Power,
                                AttackCount = userHeroDetails.AttackCount,
                                AttackFail = userHeroDetails.AttackFail,
                                DefenseCount = userHeroDetails.DefenseCount,
                                DefenseFail = userHeroDetails.DefenseFail
                            };


/*                            var heroDataRelations = CacheHeroDataManager.GetHeroDataRelations(heroType);

                            //Points        1
                            //Power         2
                            //AttackCount   3
                            //AttackFail    4
                            //DefenseCount  5
                            //DefenseFail   6
                            var valId = heroDataRelations.Find(x => x.StatType == 1).Id;//Points
                            var entry = userHeroData.Find(x => x.ValueId == valId);
                            if (entry == null) continue;

                            int.TryParse(entry.Value, out int points);

                            valId = heroDataRelations.Find(x => x.StatType == 2).Id;//Power
                            entry = userHeroData.Find(x => x.ValueId == valId);
                            int.TryParse(entry?.Value, out int power);

                            valId = heroDataRelations.Find(x => x.StatType == 3).Id;//AttackCount
                            entry = userHeroData.Find(x => x.ValueId == valId);
                            int.TryParse(entry?.Value, out int attkCount);

                            valId = heroDataRelations.Find(x => x.StatType == 4).Id;//AttackFail
                            entry = userHeroData.Find(x => x.ValueId == valId);
                            int.TryParse(entry?.Value, out int attkFail);

                            valId = heroDataRelations.Find(x => x.StatType == 5).Id;//DefenseCount
                            entry = userHeroData.Find(x => x.ValueId == valId);
                            int.TryParse(entry?.Value, out int defCount);

                            valId = heroDataRelations.Find(x => x.StatType == 6).Id;//DefenseFail
                            entry = userHeroData.Find(x => x.ValueId == valId);
                            int.TryParse(entry?.Value, out int defFail);

                            var data = new UserHeroDetails()
                            {
                                HeroCode = heroInfo.Info.Code,
                                Points = points,
                                Power = power,
                                AttackCount = attkCount,
                                AttackFail = attkFail,
                                DefenseCount = defCount,
                                DefenseFail = defFail
                            };*/
                            finalData.Data.Heroes.Add(data);

                            if ((finalData.Data.MarchingArmy != null) && (finalData.Data.MarchingArmy.Heroes != null))
                            {
                                data.IsMarching = finalData.Data.MarchingArmy.Heroes.Exists(x => x == heroInfo.Info.HeroId);//(int)heroType);
                            }
                        }

/*                        foreach (var item in userHeroData)
                        {
                            if (item == null) continue;

                            line2++;
                            var userHeroData = PlayerDataToUserHeroData(item);
                            if (userHeroData == null) continue;

                            HeroType heroType = userHeroData.ValueId.ToEnum<HeroType>();
                            if (heroType == HeroType.Unknown) continue;

                            var data = userHeroData.ToUserHeroDetails();
                            finalData.Data.Heroes.Add(data);

                            if ((finalData.Data.MarchingArmy != null) && (finalData.Data.MarchingArmy.Heroes != null))
                            {
                                data.IsMarching = finalData.Data.MarchingArmy.Heroes.Exists(x => x == (int)heroType);
                            }
                        }*/
                    }


/*                    var heroes = CacheHeroDataManager.HeroInfos;
                    foreach (var hero in heroes)
                    {
                        UserHeroDetails userHeroData = new UserHeroDetails()
                        {
                            HeroId = hero.Info.HeroId,
                            Code = hero.Info.Code,
                            Unlocked = false,
                            IsMarching = false,
                            WarPoints = 0
                        };
                        finalData.Data.Heroes.Add(userHeroData);


                        if (finalData.Data.MarchingArmy != null)
                        {
                            userHeroData.IsMarching = finalData.Data.MarchingArmy.Heros.Exists(x => x == userHeroData.HeroId);
                        }
                    }*/

/* OLD HERO IMPLEMENTATION, REQUIREMENT BASED ON STRUCTURE
                    var structureDataIds = heroes.Select(x => x.Requirements.Select(y => y.StructureDataId)).ToList();

                    foreach (var hero in heroes)
                    {
                        UserHeroDetails userHeroData = new UserHeroDetails()
                        {
                            HeroId = hero.Info.HeroId,
                            IsMarching = false,
                            Unlocked = false,
                            Code = hero.Info.Code
                        };

                        finalData.Data.Heroes.Add(userHeroData);

                        foreach (var req in hero.Requirements)
                        {
                            var structure = CacheStructureDataManager.GetFullStructureData(req.StructureId);
                            var structureType = structure.Info.Code;

                            foreach (var userStructure in finalData.Data.Structures)
                            {
                                if (userStructure.StructureType == structureType)
                                {
                                    var structureData = structure.Levels.FirstOrDefault(x => x.Data.DataId == req.StructureDataId);
                                    var structureLevel = structureData.Data.Level;

                                    foreach (var userBuildings in userStructure.Buildings)
                                    {
                                        if (userBuildings.Level >= structureLevel)
                                        {
                                            userHeroData.Unlocked = true;
                                            userHeroData.IsMarching = finalData.Data.MarchingArmy != null ? finalData.Data.MarchingArmy.Heros.Exists(x => x == userHeroData.HeroId) : false;
                                        }
                                    }
                                }
                            }
                        }
                    }*/

                    return finalData;
                }

                return new Response<PlayerCompleteData>()
                {
                    Case = response.Case,
                    Message = response.Message + line,
                    Data = null
                };
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<PlayerCompleteData>()
                {
                    Case = 200,
                    Message = ErrorManager.ShowError(ex),
                    Data = null
                };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<PlayerCompleteData>()
                {
                    Case = 201,
                    Message = ErrorManager.ShowError(ex),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new Response<PlayerCompleteData>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex) + line,
                    Data = null
                };
            }
        }

        public UserKingData PlayerDataToKingData(PlayerDataTable playerData)
        {
            if (playerData == null) return default;

            return new UserKingData()
            {
                Id = playerData.Id,
                DataType = DataType.Custom,
                ValueId = 1,
                Value = JsonConvert.DeserializeObject<UserKingDetails>(playerData.Value)
            };
        }

        public UserResourceData PlayerDataToUserResourceData(PlayerDataTable playerData)
        {
            float.TryParse(playerData.Value, out float value);

            return new UserResourceData()
            {
                Id = playerData.Id,
                DataType = DataType.Resource,
                ValueId = CacheResourceDataManager.GetResourceData(playerData.ValueId).Code,
                Value = value
            };
        }

        public UserResourceData PlayerDataToUserResourceData(PlayerDataTableUpdated playerDataUpdated) => PlayerDataToUserResourceData(playerDataUpdated.ToPlayerDataTable);

        public UserStructureData PlayerDataToUserStructureData(PlayerDataTable playerData)
        {
            return new UserStructureData()
            {
                Id = playerData.Id,
                DataType = DataType.Structure,
                ValueId = CacheStructureDataManager.GetFullStructureData(playerData.ValueId).Info.Code,
                Value = JsonConvert.DeserializeObject<List<StructureDetails>>(playerData.Value)
            };
        }

        public UserStructureData PlayerDataToUserStructureData(PlayerDataTableUpdated playerDataUpdated) => PlayerDataToUserStructureData(playerDataUpdated.ToPlayerDataTable);

        public UserTroopData PlayerDataToUserTroopData(PlayerDataTable playerData)
        {
            if (playerData == null) return default;

            return new UserTroopData()
            {
                Id = playerData.Id,
                DataType = DataType.Troop,
                ValueId = CacheTroopDataManager.GetFullTroopData(playerData.ValueId).Info.Code,
                Value = JsonConvert.DeserializeObject<List<TroopDetails>>(playerData.Value)
            };
        }

        public UserTroopData PlayerDataToUserTroopData(PlayerDataTableUpdated playerDataUpdated)
        {
            if (playerDataUpdated == null) return default;

            return PlayerDataToUserTroopData(playerDataUpdated.ToPlayerDataTable);
        }

        public UserTechnologyData PlayerDataToUserTechnologyData(PlayerDataTable playerData)
        {
            if (playerData == null) return null;
            var details = JsonConvert.DeserializeObject<TechnologyInfos>(playerData.Value);

            return new UserTechnologyData()
            {
                Id = playerData.Id,
                DataType = DataType.Technology,
                ValueId = CacheTechnologyDataManager.GetFullTechnologyData(playerData.ValueId).Info.Code,
                Value = details
            };
        }

        public UserTechnologyData PlayerDataToUserTechnologyData(PlayerDataTableUpdated playerDataUpdated)
        {
            if (playerDataUpdated == null) return null;
            return PlayerDataToUserTechnologyData(playerDataUpdated.ToPlayerDataTable);
        }

        public UserSubTechnologyData PlayerDataToUserSubTechnologyData(PlayerDataTable playerData)
        {
            if (playerData == null) return null;
            var details = JsonConvert.DeserializeObject<SubTechnologyInfos>(playerData.Value);

            return new UserSubTechnologyData()
            {
                Id = playerData.Id,
                DataType = DataType.SubTechnology,
                ValueId = CacheTechnologyDataManager.GetFullSubTechnologyData(playerData.ValueId).Info.Code,
                Value = details
            };
        }

        public UserSubTechnologyData PlayerDataToUserSubTechnologyData(PlayerDataTableUpdated playerDataUpdated)
        {
            if (playerDataUpdated == null) return null;
            return PlayerDataToUserSubTechnologyData(playerDataUpdated.ToPlayerDataTable);
        }

        public UserInventoryData PlayerDataToUserInventoryData(PlayerDataTable playerData)
        {
            int.TryParse(playerData.Value, out int val);

            return new UserInventoryData()
            {
                Id = playerData.Id,
                DataType = DataType.Inventory,
                ValueId = CacheInventoryDataManager.GetFullInventoryItemData(playerData.ValueId).Code,
                Value = val
            };
        }

        public UserInventoryDataUpdated PlayerDataToUserInventoryData(PlayerDataTableUpdated playerDataUpdated)
        {
            int.TryParse(playerDataUpdated.Value, out int val);

            return new UserInventoryDataUpdated()
            {
                Id = playerDataUpdated.Id,
                DataType = DataType.Inventory,
                ValueId = CacheInventoryDataManager.GetFullInventoryItemData(playerDataUpdated.ValueId).Code,
                Value = val,
                OldValue = playerDataUpdated.OldValue
            };
        }

        public UserBoostData PlayerDataToUserBoostData(PlayerDataTable playerData)
        {
            return new UserBoostData()
            {
                Id = playerData.Id,
                DataType = DataType.ActiveBoost,
                ValueId = CacheBoostDataManager.GetFullBoostDataByTypeId(playerData.ValueId).BoostType,
//                ValueId = CacheBoostDataManager.GetFullBoostDataByBoostId(playerData.ValueId).Info.BoostType,
                Value = JsonConvert.DeserializeObject<UserBoostDetails>(playerData.Value)
            };
        }

        public UserBoostData PlayerDataToUserBoostData(PlayerDataTableUpdated playerDataUpdated) => PlayerDataToUserBoostData(playerDataUpdated.ToPlayerDataTable);

        public UserHeroData PlayerDataToUserHeroData(PlayerDataTable playerData)
        {
            int.TryParse(playerData.Value, out int val);

            return new UserHeroData()
            {
                Id = playerData.Id,
                DataType = DataType.Hero,
                ValueId = CacheHeroDataManager.GetFullHeroData(playerData.ValueId.ToString()).Info.Code, //hero type
                Value = val//war points
            };
        }

        public UserHeroData PlayerDataToUserHeroData(PlayerDataTableUpdated playerDataUpdated) => PlayerDataToUserHeroData(playerDataUpdated.ToPlayerDataTable);




        public PlayerDataTable UserResourceDataToPlayerData(UserResourceData playerData)
        {
            return new PlayerDataTable()
            {
                Id = playerData.Id,
                DataType = DataType.Resource,
                ValueId = CacheResourceDataManager.GetResourceData(playerData.ValueId).Id,
                Value = playerData.Value.ToString()
            };
        }
        public PlayerDataTable UserStructureDataToPlayerData(UserStructureData playerData)
        {
            return new PlayerDataTable()
            {
                Id = playerData.Id,
                DataType = DataType.Structure,
                ValueId = CacheStructureDataManager.GetFullStructureData(playerData.ValueId).Info.Id,
                Value = JsonConvert.SerializeObject(playerData.Value)
            };
        }
        public PlayerDataTable UserTroopDataToPlayerData(UserTroopData playerData)
        {
            return new PlayerDataTable()
            {
                Id = playerData.Id,
                DataType = DataType.Troop,
                ValueId = CacheTroopDataManager.GetFullTroopData(playerData.ValueId).Info.Id,
                Value = JsonConvert.SerializeObject(playerData.Value)
            };
        }
        public PlayerDataTable UserTechnologyDataToPlayerData(UserTechnologyData playerData)
        {
            return new PlayerDataTable()
            {
                Id = playerData.Id,
                DataType = DataType.Technology,
                ValueId = CacheTechnologyDataManager.GetFullTechnologyData(playerData.ValueId).Info.Id,
                Value = JsonConvert.SerializeObject(playerData.Value),
            };
        }
        public PlayerDataTable UserInventoryDataToPlayerData(UserInventoryData data)
        {
            return new PlayerDataTable()
            {
                Id = data.Id,
                DataType = DataType.Inventory,
                ValueId = CacheInventoryDataManager.GetFullInventoryItemData(data.ValueId).Id,// Info.Id,
                Value = data.Value.ToString()
            };
        }
        public PlayerDataTable UserBoostDataToPlayerData(UserBoostData data)
        {
            return new PlayerDataTable()
            {
                Id = data.Id,
                DataType = DataType.ActiveBoost,
                ValueId = CacheBoostDataManager.GetFullBoostDataByType(data.ValueId).BoostTypeId,
                Value = JsonConvert.SerializeObject(data.Value)
            };
        }


        public BoostType GetBoostType(BoostType type) => GetBoostType(type.ToString());
        public BoostType GetBoostType(string type) => type.ToEnum<BoostType>();

        public Dictionary<int, UserStructureData> GetMultipleBuildings(UserStructureData structure)
        {
            var dict = new Dictionary<int, UserStructureData>();
            List<int> locId = structure.Value.GroupBy(d => d.Location).Select(d => d.Key).ToList();
            foreach (var loc in locId)
            {
                var u = new UserStructureData()
                {
                    Id = structure.Id,
                    Value = structure.Value.Where(d => d.Location == loc).ToList(),
                    ValueId = structure.ValueId,
                    StructureId = structure.StructureId
                };
                dict.Add(loc, u);
            }
            return dict;
        }
        public UserStructureData GetStructureDataAccLoc(UserStructureData structure, int locId)
        {
            return new UserStructureData()
            {
                Id = structure.Id,
                Value = structure.Value.Where(d => d.Location == locId).ToList(),
                ValueId = structure.ValueId,
                StructureId = structure.StructureId
            };
        }

        public bool HasRequirements(IReadOnlyList<IReadOnlyDataRequirement> requirements, List<StructureInfos> structures, ResourcesList resourcess) => HasRequirements(requirements, structures, resourcess, 1);
        public bool HasRequirements(IReadOnlyList<IReadOnlyDataRequirement> requirements, List<StructureInfos> structures, ResourcesList resourcess, int count)
        {
            var hasResReq = HasRequirements(requirements.Where(x => x.DataType == DataType.Resource).ToList(), resourcess, count);
            var hasStructReq = HasRequirements(requirements.Where(x => x.DataType == DataType.Structure).ToList(), structures);

            return hasResReq && hasStructReq;
        }
        public bool HasRequirements(IReadOnlyList<IReadOnlyDataRequirement> requirements, ResourcesList resourcess, int count)
        {
            try
            {
                if (requirements == null || requirements.Count <= 0) return true;
                if (resourcess == null) return false;

                foreach (var requirement in requirements)
                {
                    if (requirement.DataType == DataType.Resource)
                    {
                        var resData = CacheResourceDataManager.GetResourceData(requirement.ValueId);
                        var value = false;

                        switch (resData.Code)
                        {
                            case ResourceType.Food:
                                value = resourcess.Food >= requirement.Value * count;
                                break;
                            case ResourceType.Wood:
                                value = resourcess.Wood >= requirement.Value * count;
                                break;
                            case ResourceType.Ore:
                                value = resourcess.Ore >= requirement.Value * count;
                                break;
                            case ResourceType.Gems:
                                value = resourcess.Gems >= requirement.Value * count;
                                break;
                        }

                        if (!value) return false;
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool HasRequirements(IReadOnlyList<IReadOnlyDataRequirement> requirements, List<StructureInfos> structures)
        {
            try
            {
                if (requirements == null || requirements.Count <= 0) return true;
                if (structures == null) return false;

                foreach (var requirement in requirements)
                {
                    if (requirement.DataType == DataType.Structure)
                    {
                        var structData = CacheStructureDataManager.GetFullStructureData(requirement.ValueId);
                        var value = false;

                        foreach (var structure in structures)
                        {
                            if (structData.Info.Code == structure.StructureType)
                            {
                                var lvlBldg = structure.Buildings.Where(x => x.Level >= requirement.Value).FirstOrDefault();
                                if (lvlBldg != null)
                                {
                                    if (lvlBldg.TimeLeft <= 0)
                                    {
                                        value = true;
                                        break;
                                    };
                                }
                            }
                        }

                        if (!value) return false;
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public async Task<bool> ValidateStructureInLocAndBuild(int playerId, int locId)
        {
            var compPlayerData = await GetFullPlayerData(playerId);
            if (!compPlayerData.IsSuccess || !compPlayerData.HasData) return false;
            return ValidateStructureInLocAndBuild(locId, compPlayerData.Data.Structures, null, null);
        }
        public bool ValidateStructureInLocAndBuild(int locId, IReadOnlyList<StructureInfos> structures) => ValidateStructureInLocAndBuild(locId, structures, null, null);
        public bool ValidateStructureInLocAndBuild(int locId, IReadOnlyList<StructureInfos> structures, IReadOnlyList<StructureType> allowedType) => ValidateStructureInLocAndBuild(locId, structures, allowedType, null);
        public bool ValidateStructureInLocAndBuild(int locId, IReadOnlyList<StructureInfos> structures, IReadOnlyList<StructureType> allowedType, Func<bool> onExist)
        {
            try
            {
                if (structures == null || structures.Count == 0) return false;
                if (allowedType == null || allowedType.Count == 0) allowedType = CacheStructureDataManager.StructureTypes;

                foreach (var type in allowedType)
                {
                    foreach (var structure in structures)
                    {
                        if (structure != null && structure.StructureType == type && structure.Buildings.Exists(x => x.Location == locId))
                        {
                            if (onExist == null) return true;

                            var returnVal = onExist?.Invoke();
                            return returnVal.GetValueOrDefault();
                        }
                    }
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public StructureInfos GetStructureLocation(int locId, List<StructureInfos> structures)
        {
            try
            {
                if (structures == null || structures.Count == 0) return null;

                foreach (var structure in structures)
                {
                    if (structure != null && structure.Buildings.Exists(x => x.Location == locId))
                        return structure;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public int GetInstantBuildCost(int timeLeft)
        {
            if (timeLeft <= 240) return 0;
            else timeLeft -= 240;

            var baseCostMultiplierPerSec = 0.01f;
            var cost = timeLeft * baseCostMultiplierPerSec;
            return (int)cost;
        }
    }
}

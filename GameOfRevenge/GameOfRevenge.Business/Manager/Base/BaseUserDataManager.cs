using ExitGames.Logging;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Business.Manager.Kingdom;
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
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        protected static readonly IAccountManager accountManager = new AccountManager();
        protected static readonly IPlayerDataManager manager = new PlayerDataManager();
        protected static readonly IClanManager clanManager = new ClanManager();

        protected IReadOnlyDataRequirement GetGemReq(int value)
        {
            return new DataRequirement() { DataType = DataType.Resource, ValueId = CacheResourceDataManager.Gems.Id, Value = value };
        }

        public async Task<Response<List<PlayerDataTable>>> GetAllPlayerData(int playerId) => await manager.GetAllPlayerData(playerId);

        public static UserResourceData PlayerDataToUserResourceData(PlayerDataTable playerData)
        {
            if (!long.TryParse(playerData.Value, out long value))
            {
                log.Info("ERROR parsing resource "+Newtonsoft.Json.JsonConvert.SerializeObject(playerData));
            }

            return new UserResourceData()
            {
                Id = playerData.Id,
                DataType = DataType.Resource,
                ValueId = CacheResourceDataManager.GetResourceData(playerData.ValueId).Code,
                Value = value
            };
        }

        public static async Task<Response<PlayerCompleteData>> GetFullPlayerData(int playerId)
        {
            string line = "-";
            try
            {
                var resp = await accountManager.GetAccountInfo(playerId);
                if (!resp.IsSuccess) throw new DataNotExistExecption(resp.Message);

                var playerName = resp.Data.Name;
                var isDeveloper = resp.Data.IsDeveloper;
                var isAdmin = resp.Data.IsAdmin;

                var response = await manager.GetAllPlayerData(playerId);
                if (!response.IsSuccess) throw new DataNotExistExecption(response.Message + line);

                line = "1";
                var finalData = new Response<PlayerCompleteData>()
                {
                    Case = 100,
                    Message = "Complete player data",
                    Data = new PlayerCompleteData()
                    {
                        PlayerId = playerId,
                        PlayerName = playerName,
                        IsDeveloper = isDeveloper,
                        IsAdmin = isAdmin,
                        HelpedBuild = 0,
                        ClanId = 0,

                        Resources = new ResourcesList(),
                        Structures = new List<StructureInfos>(),
                        Troops = new List<TroopInfos>(),
                        Items = new List<UserItemDetails>(),
                        Technologies = new List<TechnologyInfos>(),
//                            SubTechnologies = new List<SubTechnologyInfos>(),
                        Boosts = new List<UserRecordNewBoost>(),
                        Heroes = new List<UserHeroDetails>()
                    }
                };

                var clanData = await clanManager.GetPlayerClanData(playerId);
                if (clanData.IsSuccess && clanData.HasData) finalData.Data.ClanId = clanData.Data.Id;

                var customs = response.Data.Where(x => x.DataType == DataType.Custom)?.ToList();
                var kingData = customs?.Find(x => x.ValueId == (int)CustomValueType.KingDetails);
                if (kingData != null)
                {
                    try
                    {
                        finalData.Data.King = JsonConvert.DeserializeObject<UserKingDetails>(kingData.Value);
                    }
                    catch {}
                }
                if (finalData.Data.King == null) finalData.Data.King = new UserKingDetails();

                var vipData = customs?.Find(x => x.ValueId == (int)CustomValueType.VIPPoints);
                if (vipData != null)
                {
                    try
                    {
                        finalData.Data.VIP = JsonConvert.DeserializeObject<UserVIPDetails>(vipData.Value);
                    }
                    catch { }
                }
                if (finalData.Data.VIP == null) finalData.Data.VIP = new UserVIPDetails();

                var builders = new List<UserRecordBuilderDetails>();
                if (customs != null)
                {
                    foreach (var customData in customs)
                    {
                        if (customData.ValueId != (int)CustomValueType.BuildingWorker) continue;

                        UserRecordBuilderDetails bld = null;
                        try
                        {
                            bld = JsonConvert.DeserializeObject<UserRecordBuilderDetails>(customData.Value);
                        }
                        catch { }
                        if (bld == null) continue;

                        bld.Id = customData.Id;
                        builders.Add(bld);
                    }
                }
//                    var builderData = new UserRecordBuilderDetails();
                if (builders.Count == 0)
                {
                    var json = JsonConvert.SerializeObject(new UserBuilderDetails());
                    var builderResp = await manager.AddOrUpdatePlayerData(playerId, DataType.Custom, (int)CustomValueType.BuildingWorker, json);
                    if (builderResp.IsSuccess)
                    {
                        builders.Add(new UserRecordBuilderDetails()
                        {
                            Id = builderResp.Data.Id
                        });
                    }
                }
                finalData.Data.Workers = builders;

                line = "5";

                var userResources = response.Data.Where(x => (x.DataType == DataType.Resource))?.ToList();
                var userStructures = response.Data.Where(x => (x.DataType == DataType.Structure))?.ToList();
                var userTroops = response.Data.Where(x => (x.DataType == DataType.Troop))?.ToList();
                var userMarchingArmies = response.Data.Where(x => (x.DataType == DataType.Marching))?.ToList();
                var userTechnologies = response.Data.Where(x => (x.DataType == DataType.Technology))?.ToList();
//                var userSubTechs = response.Data.Where(x => (x.DataType == DataType.SubTechnology))?.ToList();
                var userItems = response.Data.Where(x => (x.DataType == DataType.Inventory))?.ToList();
                var userBoosts = response.Data.Where(x => (x.DataType == DataType.ActiveBoost))?.ToList();
                var userHeroData = response.Data.Where(x => (x.DataType == DataType.Hero))?.ToList();
                var userActivities = response.Data.Where(x => (x.DataType == DataType.Activity))?.ToList();
                line = "6";

                if (userActivities != null)
                {
                    //TODO: find a better way to store this value, we should insert it into your main account data
                    var helpActivity = userActivities.Find(x => (x.ValueId == 1));
                    if ((helpActivity != null) && int.TryParse(helpActivity.Value, out int value))
                    {
                        finalData.Data.HelpedBuild = value;
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

                    var resVal = resourceList.Find(x => (x.ValueId == ResourceType.Food));
                    finalData.Data.Resources.Food = (resVal != null)? resVal.Value : 0;

                    resVal = resourceList.Find(x => (x.ValueId == ResourceType.Wood));
                    finalData.Data.Resources.Wood = (resVal != null) ? resVal.Value : 0;

                    resVal = resourceList.Find(x => (x.ValueId == ResourceType.Ore));
                    finalData.Data.Resources.Ore = (resVal != null) ? resVal.Value : 0;

                    resVal = resourceList.Find(x => (x.ValueId == ResourceType.Gems));
                    finalData.Data.Resources.Gems = (resVal != null) ? resVal.Value : 0;

                    resVal = resourceList.Find(x => (x.ValueId == ResourceType.Gold));
                    finalData.Data.Resources.Gold = (resVal != null) ? resVal.Value : 0;
                }

                line = "8";
                if (userStructures != null)
                {
                    foreach (var item in userStructures)
                    {
                        if (item != null)
                        {
                            var data = PlayerData.PlayerDataToUserStructureData(item);
                            finalData.Data.Structures.Add(new StructureInfos(data));
                        }
                    }
                }

                line = "9";
                if (userTroops != null)
                {
                    foreach (var item in userTroops)
                    {
                        if ((item == null) || (item.Value == null)) continue;

                        var userTroop = PlayerData.PlayerDataToUserTroopData(item);

                        finalData.Data.Troops.Add(new TroopInfos(userTroop.Id, userTroop.ValueId, userTroop.Value));
                    }
                }

                line = "10";
                if (userMarchingArmies != null)
                {
                    List<MarchingArmy> list = null;
                    foreach (var item in userMarchingArmies)
                    {
                        if ((item == null) || string.IsNullOrEmpty(item.Value)) continue;

                        MarchingArmy marching = null;
                        try
                        {
                            marching = JsonConvert.DeserializeObject<MarchingArmy>(item.Value);
                        }
                        catch { }
                        if ((marching == null) || (marching.TimeLeft == 0)) continue;

                        marching.MarchingId = item.Id;
                        marching.MarchingSlot = item.ValueId;
                        if (list == null) list = new List<MarchingArmy>();
                        list.Add(marching);
                    }
                    finalData.Data.MarchingArmies = list;
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

                        var technology = PlayerData.PlayerDataToUserTechnologyData(tech);
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

/*                    if (userSubTechs != null)
                {
                    foreach (var tech in userSubTechs)
                    {
                        if (tech == null) continue;

                        var subTech = PlayerDataToUserSubTechnologyData(tech);
                        finalData.Data.SubTechnologies.Add(subTech.Value);
                    }
                }*/

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

                        var itemData = PlayerData.PlayerDataToUserInventoryData(pItemData);
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
                    var boostData = PlayerData.PlayerDataToUserNewBoostData(item);
//                        if (boostData.ValueId == 0)
                    if ((boostData != null) && (!boostData.Value.HasDuration || (boostData.Value.TimeLeft > 0)))
                    {
                        finalData.Data.Boosts.Add(new UserRecordNewBoost(boostData.Id, boostData.Value));
                    }
                }


                line = "14";
                int line2 = 0;
                ////ADD HEROES
                if (userHeroData != null)
                {
                    List<HeroType> marchingHeroes = null;
                    if (finalData.Data.MarchingArmies != null)
                    {
                        marchingHeroes = finalData.Data.MarchingArmies
                            .Where(marchingArmy => (marchingArmy.Heroes != null))
                            .SelectMany(marchingArmy => marchingArmy.Heroes)
                            .ToList();
                    }

                    foreach (var heroInfo in CacheHeroDataManager.HeroInfos)
                    {
//                            HeroType heroType = heroInfo.Info.Code.ToEnum<HeroType>();

                        var userHero = userHeroData.Find(x => (x.ValueId == heroInfo.Info.HeroId));
                        if (userHero == null) continue;

                        var userHeroDetails = JsonConvert.DeserializeObject<UserHeroDetails>(userHero.Value);
                        if (userHeroDetails == null) continue;

                        var data = new UserHeroDetails()
                        {
//                            HeroId = userHeroDetails.HeroId,
                            HeroType = (HeroType)heroInfo.Info.HeroId,
                            Points = userHeroDetails.Points,
                            Power = userHeroDetails.Power,
                            AttackCount = userHeroDetails.AttackCount,
                            AttackFail = userHeroDetails.AttackFail,
                            DefenseCount = userHeroDetails.DefenseCount,
                            DefenseFail = userHeroDetails.DefenseFail
                        };
                        if (marchingHeroes != null)
                        {
                            data.IsMarching = marchingHeroes.Exists(x => (x == data.HeroType));
                        }
                        finalData.Data.Heroes.Add(data);
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
            catch (InvalidModelExecption ex)
            {
                return new Response<PlayerCompleteData>()
                {
                    Case = 200,
                    Message = ErrorManager.ShowError(ex) + "i"+line,
                    Data = null
                };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<PlayerCompleteData>()
                {
                    Case = 201,
                    Message = ErrorManager.ShowError(ex) + "d"+line,
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new Response<PlayerCompleteData>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex) + "e"+line,
                    Data = null
                };
            }
        }


        public NewBoostType GetBoostType(NewBoostType type) => GetBoostType(type.ToString());
        public NewBoostType GetBoostType(string type) => type.ToEnum<NewBoostType>();

        public Dictionary<int, UserStructureData> GetMultipleBuildings(UserStructureData structure)
        {
            var dict = new Dictionary<int, UserStructureData>();
            try
            {
                List<int> locId = structure.Value.GroupBy(d => d.Location).Select(d => d.Key).ToList();
                foreach (var loc in locId)
                {
                    var u = new UserStructureData()
                    {
                        Id = structure.Id,
                        DataType = structure.DataType,
                        ValueId = structure.ValueId,
                        Value = structure.Value.Where(d => d.Location == loc).ToList()
                    };
                    dict.Add(loc, u);
                }
            }
            catch (Exception ex)
            {
                log.Info("EXCEPTION3 " + ex.Message);
                log.Info(Newtonsoft.Json.JsonConvert.SerializeObject(structure));
                throw ex;
            }
            return dict;
        }

        public bool HasRequirements(IReadOnlyList<IReadOnlyDataRequirement> requirements, PlayerCompleteData playerData) => HasRequirements(requirements, playerData, 1);
        public bool HasRequirements(IReadOnlyList<IReadOnlyDataRequirement> requirements, PlayerCompleteData playerData, int count)
        {
            var hasResReq = HasResourceRequirements(requirements, playerData.Resources, count);
            var hasStructReq = HasStructureRequirements(requirements, playerData.Structures);
            var hasBoostReq = HasActiveBoostRequirements(requirements, playerData.Boosts);

            return hasResReq && hasStructReq && hasBoostReq;
        }

        public bool HasResourceRequirements(IReadOnlyList<IReadOnlyDataRequirement> requirements, ResourcesList resources, int count)
        {
            if ((requirements == null) || (requirements.Count <= 0)) return true;
            if (resources == null) return false;

            bool resp = true;
            try
            {
                foreach (var requirement in requirements)
                {
                    if (requirement.DataType != DataType.Resource) continue;

                    long val = 0;
                    var resData = CacheResourceDataManager.GetResourceData(requirement.ValueId);
                    switch (resData.Code)
                    {
                        case ResourceType.Food: val = resources.Food; break;
                        case ResourceType.Wood: val = resources.Wood; break;
                        case ResourceType.Ore: val = resources.Ore; break;
                        case ResourceType.Gems: val = resources.Gems; break;
                    }
                    if (val < requirement.Value * count)
                    {
                        resp = false;
                        break;
                    }
                }
            }
            catch (Exception)
            {
                resp = false;
            }

            return resp;
        }

        public bool HasStructureRequirements(IReadOnlyList<IReadOnlyDataRequirement> requirements, List<StructureInfos> structures)
        {
            if ((requirements == null) || (requirements.Count <= 0)) return true;
            if (structures == null) return false;

            bool resp = true;
            try
            {
                foreach (var requirement in requirements)
                {
                    if (requirement.DataType != DataType.Structure) continue;

                    var structData = CacheStructureDataManager.GetFullStructureData(requirement.ValueId);
                    StructureInfos structure = structures.First(x =>
                    {
                        StructureDetails building = null;
                        if (x.StructureType == structData.Info.Code)
                        {
                            building = x.Buildings.Find(y => (y.Level > requirement.Value) ||
                                                             ((y.Level == requirement.Value) && (y.TimeLeft <= 0)));
                        }

                        return building != null;
                    });
                    if (structure != null) continue;

                    resp = false;
                    break;
                }
            }
            catch (Exception)
            {
                resp = false;
            }

            return resp;
        }

        public bool HasActiveBoostRequirements(IReadOnlyList<IReadOnlyDataRequirement> requirements, List<UserRecordNewBoost> userBoosts)
        {
            if ((requirements == null) || (requirements.Count <= 0)) return true;
            if (userBoosts == null) return false;

            bool resp = true;
            try
            {
                foreach (var requirement in requirements)
                {
                    if (requirement.DataType != DataType.ActiveBoost) continue;

                    var boostType = (NewBoostType)requirement.ValueId;
//                    var boostData = CacheBoostDataManager.GetNewBoostDataByType((NewBoostType)requirement.ValueId);
                    UserNewBoost boost = userBoosts.Find(x =>
                    {
                        bool reqLevel = false;
                        if (x.Type == boostType)//boostData.Type)
                        {
                            reqLevel = (x.Level > requirement.Value) ||
                                        ((x.Level == requirement.Value) && (x.TimeLeft <= 0));
                        }

                        return reqLevel;
                    });
                    if (boost != null) continue;

                    resp = false;
                    break;
                }
            }
            catch (Exception)
            {
                resp = false;
            }

            return resp;
        }

        public async Task<bool> ValidateStructureInLocAndBuild(int playerId, int locId)
        {
            var compPlayerData = await GetFullPlayerData(playerId);
            if (!compPlayerData.IsSuccess || !compPlayerData.HasData) return false;
            return ValidateStructureInLocAndBuild(locId, compPlayerData.Data.Structures, null, null);
        }
        public bool ValidateStructureInLocAndBuild(int location, IReadOnlyList<StructureInfos> structures) => ValidateStructureInLocAndBuild(location, structures, null, null);
        public bool ValidateStructureInLocAndBuild(int location, IReadOnlyList<StructureInfos> structures, IReadOnlyList<StructureType> allowedType) => ValidateStructureInLocAndBuild(location, structures, allowedType, null);
        public bool ValidateStructureInLocAndBuild(int location, IReadOnlyList<StructureInfos> structures, IReadOnlyList<StructureType> allowedType, Func<bool> onExist)
        {
            try
            {
                if (structures == null || structures.Count == 0) return false;
                if (allowedType == null || allowedType.Count == 0) allowedType = CacheStructureDataManager.StructureTypes;

                foreach (var type in allowedType)
                {
                    foreach (var structure in structures)
                    {
                        if (structure != null && structure.StructureType == type && structure.Buildings.Exists(x => x.Location == location))
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

        public static StructureInfos GetStructureLocation(int location, List<StructureInfos> structures)
        {
            if ((structures == null) || (structures.Count == 0)) return null;

            return structures.FirstOrDefault(x =>
            {
                return (x != null) && (x.Buildings != null) &&
                        x.Buildings.Exists(y => (y.Location == location));
            });
        }

        [Obsolete]
        public int GetInstantBuildCost(int timeLeft)
        {
            if (timeLeft <= 240) return 0;
            else timeLeft -= 240;

            var baseCostMultiplierPerSec = 0.01f;
            var cost = timeLeft * baseCostMultiplierPerSec;
            return (int)cost;
        }

        public async Task<Response<UserVIPDetails>> AddVIPPoints(int playerId, int points)
        {
            try
            {
                var response = await manager.GetAllPlayerData(playerId);
                if (!response.IsSuccess) throw new InvalidModelExecption(response.Message);

                var gemsdata = response.Data.Find(x => (x.DataType == DataType.Resource) && (x.ValueId == (int)ResourceType.Gems));
                if (gemsdata == null) throw new InvalidModelExecption("No resource gems");

                long.TryParse(gemsdata.Value, out long plyGems);
                if (plyGems < points) throw new InvalidModelExecption("Not enough gems");

                var vipdata = response.Data.Find(x => (x.DataType == DataType.Custom) && (x.ValueId == (int)CustomValueType.VIPPoints));
                if (vipdata != null)
                {
                    var vipdetails = JsonConvert.DeserializeObject<UserVIPDetails>(vipdata.Value);
                    vipdetails.Points += points;//TODO: add vippoints to player info
                    var json = JsonConvert.SerializeObject(vipdetails);
                    var saveResp = await manager.UpdatePlayerDataID(playerId, vipdata.Id, json);
                    if (saveResp.IsSuccess)
                    {
                        await manager.RemovePlayerResourceData(playerId, 0, 0, 0, points, 0);
                    }
                    return new Response<UserVIPDetails>()
                    {
                        Case = 100,
                        Message = "Success",
                        Data = vipdetails
                    };
                }

                throw new DataNotExistExecption("VIP data missing");
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<UserVIPDetails>()
                {
                    Case = 200,
                    Message = ErrorManager.ShowError(ex),
                    Data = null
                };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<UserVIPDetails>()
                {
                    Case = 201,
                    Message = ErrorManager.ShowError(ex),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new Response<UserVIPDetails>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex),
                    Data = null
                };
            }
        }

        public async Task<Response<UserVIPDetails>> ActivateVIPBoosts(int playerId, int duration = 1800)
        {
            try
            {
                var response = await manager.GetAllPlayerData(playerId, DataType.Custom);
                if (!response.IsSuccess) throw new InvalidModelExecption(response.Message);

                var vipdata = response.Data.Find(x => (x.DataType == DataType.Custom) && (x.ValueId == (int)CustomValueType.VIPPoints));
                if (vipdata != null)
                {
                    var vipdetails = JsonConvert.DeserializeObject<UserVIPDetails>(vipdata.Value);

                    /*int levelVal = 0;
                    var specBoostData = CacheBoostDataManager.SpecNewBoostDatas.First(x => x.Type == NewBoostType.Blessing);
                    if (specBoostData.Table > 0)
                    {
                        if (specBoostData.Levels.ContainsKey((byte)vipdetails.Level))
                        {
                            int.TryParse(specBoostData.Levels[(byte)vipdetails.Level].ToString(), out levelVal);
                        }
                    }*/
                    /*                    var nvip = CacheBoostDataManager.GetNewBoostByTypeId((int)NewBoostType.VIP);
                                        foreach (var tech in nvip.Techs)
                                        {
                                            if ((tech.StartLevel > 0) && (vipdetails.Level < tech.StartLevel)) continue;
                                            if (tech.Tech != (NewBoostTech)VIPBoostTech.BuildingTimeBonus) continue;

                                            var value = tech.GetValue(vipdetails.Level);
                                            if (value > maxVal) maxVal = value;
                                        }*/
                    if (duration > 0)
                    {
                        if (vipdetails.TimeLeft == 0)
                        {
                            vipdetails.StartTime = DateTime.UtcNow;
                            vipdetails.Duration = 0;
                        }
                        vipdetails.Duration += duration;

                        var json = JsonConvert.SerializeObject(vipdetails);
                        var saveResp = await manager.UpdatePlayerDataID(playerId, vipdata.Id, json);

                        return new Response<UserVIPDetails>()
                        {
                            Case = 100,
                            Message = "Success",
                            Data = vipdetails
                        };
                    }

                    return new Response<UserVIPDetails>()
                    {
                        Case = 101,
                        Message = "Activation not required",
                        Data = null
                    };
                }

                throw new DataNotExistExecption("VIP data missing");
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<UserVIPDetails>()
                {
                    Case = 200,
                    Message = ErrorManager.ShowError(ex),
                    Data = null
                };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<UserVIPDetails>()
                {
                    Case = 201,
                    Message = ErrorManager.ShowError(ex),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new Response<UserVIPDetails>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex),
                    Data = null
                };
            }
        }

        public async Task<Response<RankingElement>> GetRanking(int playerId)
        {
            try
            {
                var response = await manager.GetRanking(playerId);
                if (!response.IsSuccess) throw new InvalidModelExecption(response.Message);

                return response;
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<RankingElement>()
                {
                    Case = 200,
                    Message = ErrorManager.ShowError(ex),
                    Data = null
                };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<RankingElement>()
                {
                    Case = 201,
                    Message = ErrorManager.ShowError(ex),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new Response<RankingElement>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex),
                    Data = null
                };
            }
        }

        public async Task<Response<List<RankingElement>>> GetRankings(long rankId = 0)
        {
            try
            {
                var response = await manager.GetRankings(rankId);
                if (!response.IsSuccess) throw new InvalidModelExecption(response.Message);

                return response;
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<List<RankingElement>>()
                {
                    Case = 200,
                    Message = ErrorManager.ShowError(ex),
                    Data = null
                };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<List<RankingElement>>()
                {
                    Case = 201,
                    Message = ErrorManager.ShowError(ex),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new Response<List<RankingElement>>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError(ex),
                    Data = null
                };
            }
        }
    }
}

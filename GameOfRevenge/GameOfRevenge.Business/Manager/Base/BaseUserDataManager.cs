using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Business.Manager.UserData;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Helper;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Inventory;
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
        protected static readonly IPlayerDataManager manager = new PlayerDataManager();

        protected IReadOnlyDataRequirement GetGemReq(int value)
        {
            return new DataRequirement() { DataType = DataType.Resource, ValueId = CacheResourceDataManager.Gems.Id, Value = value };
        }

        public async Task<Response<PlayerCompleteData>> GetPlayerData(int playerId)
        {
            try
            {
                var currentTimestamp = DateTime.UtcNow;
                var response = await manager.GetAllPlayerData(playerId);

                if (response.IsSuccess)
                {
                    var finalData = new Response<PlayerCompleteData>()
                    {
                        Case = 100,
                        Message = "Complete player data",
                        Data = new PlayerCompleteData()
                        {
                            Resources = new ResourcesList(),
                            Structures = new List<StructureInfos>(),
                            Troops = new List<TroopInfos>(),
                            Items = new List<InventoryInfo>(),
                            Technologies = new List<TechnologyInfos>(),
                            Buffs = new List<UserBuffDetails>(),
                            PlayerId = playerId,
                            Heros = new List<UserHeroDetails>()
                        }
                    };

                    var dataR = response.Data.Where(x => x.DataType == DataType.Resource).ToList();
                    var dataS = response.Data.Where(x => x.DataType == DataType.Structure).ToList();
                    var dataT = response.Data.Where(x => x.DataType == DataType.Troop).ToList();
                    var dataM = response.Data.Where(x => x.DataType == DataType.Marching)?.FirstOrDefault();
                    var dataTh = response.Data.Where(x => x.DataType == DataType.Technology).ToList();
                    var dataItems = response.Data.Where(x => x.DataType == DataType.Inventory).ToList();
                    var dataBffs = response.Data.Where(x => x.DataType == DataType.ActiveBuffs).ToList();
                    var dataheros = response.Data.Where(x => x.DataType == DataType.Hero).ToList();

                    var resourceList = new List<UserResourceData>();
                    foreach (var item in dataR) resourceList.Add(PlayerDataToUserResourceData(item));
                    finalData.Data.Resources.Food = (int)resourceList.FirstOrDefault(x => x.ValueId == ResourceType.Food)?.Value;
                    finalData.Data.Resources.Wood = (int)resourceList.FirstOrDefault(x => x.ValueId == ResourceType.Wood)?.Value;
                    finalData.Data.Resources.Ore = (int)resourceList.FirstOrDefault(x => x.ValueId == ResourceType.Ore)?.Value;
                    finalData.Data.Resources.Gems = (int)resourceList.FirstOrDefault(x => x.ValueId == ResourceType.Gems)?.Value;

                    var structureList = new List<UserStructureData>();
                    foreach (var item in dataS) structureList.Add(PlayerDataToUserStructureData(item));

                    foreach (var item in structureList)
                    {
                        finalData.Data.Structures.Add(new StructureInfos()
                        {
                            StructureType = item.ValueId,
                            Buildings = item.Value
                        });
                    }

                    var troopList = new List<UserTroopData>();
                    foreach (var item in dataT) troopList.Add(PlayerDataToUserTroopData(item));
                    foreach (var userTroop in troopList)
                    {
                        var troopData = CacheTroopDataManager.GetFullTroopData(userTroop.ValueId);

                        if (userTroop.Value != null)
                        {
                            foreach (var troop in userTroop.Value)
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
                        }

                        finalData.Data.Troops.Add(new TroopInfos()
                        {
                            TroopType = userTroop.ValueId,
                            TroopData = userTroop.Value
                        });
                    }

                    if (dataM != null)
                    {
                        finalData.Data.MarchingArmy = JsonConvert.DeserializeObject<MarchingArmy>(dataM.Value);
                    }


                    //POPULATE TECHNOLOGIES
                    var techs = CacheTechnologyDataManager.TechnologyInfos;
                    foreach (var tech in techs)
                    {
                        finalData.Data.Technologies.Add(new TechnologyInfos()
                        {
                            TechnologyType = tech.Info.Code,
                            Level = 0
                        });
                    }

                    foreach (var tech in dataTh)
                    {
                        if (tech != null)
                        {
                            var technology = PlayerDataToUserTechnologyData(tech);
                            foreach (var item in finalData.Data.Technologies)
                            {
                                if (technology.ValueId == item.TechnologyType)
                                {
                                    item.Level = technology.Value.Level;
                                    item.StartTime = technology.Value.StartTime;
                                    item.EndTime = technology.Value.EndTime;
                                }
                            }
                            //finalData.Data.Technologies.Add(new TechnologyInfos() { TechnologyType = technology.ValueId, Level = technology.Value });
                        }
                    }

                    //POPULATE ITEMS
                    var items = CacheInventoryDataManager.ItemInfos;
                    foreach (var item in items)
                    {
                        finalData.Data.Items.Add(new InventoryInfo()
                        {
                            ItemType = item.Info.Code,
                            Count = 0
                        });
                    }

                    foreach (var pItemData in dataItems)
                    {
                        if (pItemData != null)
                        {
                            var itemData = PlayerDataToUserInventoryData(pItemData);
                            foreach (var item in finalData.Data.Items)
                            {
                                if (itemData.ValueId == item.ItemType)
                                    item.Count += itemData.Value;
                            }
                        }
                    }

                    //ADD BUFFS
                    foreach (var item in dataBffs)
                    {
                        var bufdData = PlayerDataToUserBuffData(item);
                        if (bufdData != null)
                            finalData.Data.Buffs.Add(bufdData.Value);
                    }



                    ////ADD HEROS
                    var heros = CacheHeroDataManager.HeroInfos;
                    var structureDataIds = heros.Select(x => x.Requirements.Select(y => y.StructureDataId)).ToList();

                    foreach (var hero in heros)
                    {
                        UserHeroDetails userHeroData = new UserHeroDetails()
                        {
                            HeroId = hero.Info.HeroId,
                            IsMarching = false,
                            Unlocked = false,
                            Code = hero.Info.Code
                        };

                        finalData.Data.Heros.Add(userHeroData);

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
                    }

                    return finalData;
                }

                return new Response<PlayerCompleteData>()
                {
                    Case = response.Case,
                    Message = response.Message,
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
                    Message = ErrorManager.ShowError(ex),
                    Data = null
                };
            }
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
        public UserInventoryData PlayerDataToUserInventoryData(PlayerDataTable playerData)
        {
            int.TryParse(playerData.Value, out int techVal);

            return new UserInventoryData()
            {
                Id = playerData.Id,
                DataType = DataType.Inventory,
                ValueId = CacheInventoryDataManager.GetFullInventoryItemData(playerData.ValueId).Info.Code,
                Value = techVal
            };
        }
        public UserBuffData PlayerDataToUserBuffData(PlayerDataTable playerData)
        {
            return new UserBuffData()
            {
                Id = playerData.Id,
                DataType = DataType.ActiveBuffs,
                ValueId = CacheInventoryDataManager.GetFullInventoryItemData(playerData.ValueId).Info.Code,
                Value = JsonConvert.DeserializeObject<UserBuffDetails>(playerData.Value)
            };
        }

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
        public PlayerDataTable UserInventoryDataToPlayerData(UserInventoryData playerData)
        {
            return new PlayerDataTable()
            {
                Id = playerData.Id,
                DataType = DataType.Inventory,
                ValueId = CacheInventoryDataManager.GetFullInventoryItemData(playerData.ValueId).Info.Id,
                Value = playerData.Value.ToString()
            };
        }
        public PlayerDataTable UserBuffDataToPlayerData(UserBuffData playerData)
        {
            return new PlayerDataTable()
            {
                Id = playerData.Id,
                DataType = DataType.ActiveBuffs,
                ValueId = CacheInventoryDataManager.GetFullInventoryItemData(playerData.ValueId).Info.Id,
                Value = JsonConvert.SerializeObject(playerData.Value)
            };
        }


        public BuffType GetBuffType(InventoryItemType type) => GetBuffType(type.ToString());
        public BuffType GetBuffType(string type) => type.ToEnum<BuffType>();

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
            var compPlayerData = await GetPlayerData(playerId);
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

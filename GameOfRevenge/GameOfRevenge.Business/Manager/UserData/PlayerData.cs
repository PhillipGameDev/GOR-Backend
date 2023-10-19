using System.Collections.Generic;
using Newtonsoft.Json;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Boost;
using GameOfRevenge.Common.Models.PlayerData;
using System.Linq;

namespace GameOfRevenge.Business.Manager.UserData
{
    public class PlayerData
    {
        public static UserKingData PlayerDataToKingData(PlayerDataTable playerData)
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

        public static UserResourceData PlayerDataToUserResourceData(PlayerDataTable playerData)
        {
            long.TryParse(playerData.Value, out long value);

            return new UserResourceData()
            {
                Id = playerData.Id,
                DataType = DataType.Resource,
                ValueId = CacheResourceDataManager.GetResourceData(playerData.ValueId).Code,
                Value = value
            };
        }

        public static UserResourceData PlayerDataToUserResourceData(PlayerDataTableUpdated playerDataUpdated) => PlayerDataToUserResourceData(playerDataUpdated.ToPlayerDataTable);

        public static UserStructureData PlayerDataToUserStructureData(PlayerDataTable playerData)
        {
            return new UserStructureData()
            {
                Id = playerData.Id,
                DataType = DataType.Structure,
                ValueId = CacheStructureDataManager.GetFullStructureData(playerData.ValueId).Info.Code,
                Value = JsonConvert.DeserializeObject<List<StructureDetails>>(playerData.Value)
            };
        }

        public static UserStructureData PlayerDataToUserStructureData(PlayerDataTableUpdated playerDataUpdated) => PlayerDataToUserStructureData(playerDataUpdated.ToPlayerDataTable);

        public static UserTroopData PlayerDataToUserTroopData(PlayerDataTable playerData)
        {
            if (playerData == null) return default;

            var troopData = new UserTroopData()
            {
                Id = playerData.Id,
                DataType = DataType.Troop,
                ValueId = CacheTroopDataManager.GetFullTroopData(playerData.ValueId).Info.Code,
                Value = JsonConvert.DeserializeObject<List<TroopDetails>>(playerData.Value)
            };

            foreach (var troop in troopData.Value)
            {
                if (troop.InRecovery != null)
                {
                    troop.InRecovery = troop.InRecovery.Where(x => (x.TimeLeft > 0)).ToList();
                    if (troop.InRecovery.Count == 0) troop.InRecovery = null;
                }

                if (troop.InTraning != null)
                {
                    troop.InTraning = troop.InTraning.Where(x => (x.TimeLeft > 0)).ToList();
                    if (troop.InTraning.Count == 0) troop.InTraning = null;
                }
            }

            return troopData;
        }

        public static UserTroopData PlayerDataToUserTroopData(PlayerDataTableUpdated playerDataUpdated)
        {
            if (playerDataUpdated == null) return default;

            return PlayerDataToUserTroopData(playerDataUpdated.ToPlayerDataTable);
        }

        public static UserTechnologyData PlayerDataToUserTechnologyData(PlayerDataTable playerData)
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

        public static UserTechnologyData PlayerDataToUserTechnologyData(PlayerDataTableUpdated playerDataUpdated)
        {
            if (playerDataUpdated == null) return null;
            return PlayerDataToUserTechnologyData(playerDataUpdated.ToPlayerDataTable);
        }

        /*        public static UserSubTechnologyData PlayerDataToUserSubTechnologyData(PlayerDataTable playerData)
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

                public static UserSubTechnologyData PlayerDataToUserSubTechnologyData(PlayerDataTableUpdated playerDataUpdated)
                {
                    if (playerDataUpdated == null) return null;
                    return PlayerDataToUserSubTechnologyData(playerDataUpdated.ToPlayerDataTable);
                }*/

        public static UserInventoryData PlayerDataToUserInventoryData(PlayerDataTable playerData)
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

        public static UserInventoryDataUpdated PlayerDataToUserInventoryData(PlayerDataTableUpdated playerDataUpdated)
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

        public static UserNewBoostData PlayerDataToUserNewBoostData(PlayerDataTable playerData)
        {
            UserNewBoostData data;

            //            System.Console.WriteLine(playerData.Value);
            if (playerData.Value.Contains("EndTime"))
            {
                var boostDetails = JsonConvert.DeserializeObject<UserBoostDetails>(playerData.Value);
                var oldBoostType = boostDetails.BoostType;
                NewBoostType boostType = NewBoostType.Unknown;
                switch (oldBoostType)
                {
                    case NewBoostType.Shield: boostType = NewBoostType.Shield; break;
                    case NewBoostType.Blessing: boostType = NewBoostType.Blessing; break;
                    case NewBoostType.LifeSaver: boostType = NewBoostType.LifeSaver; break;
                    case NewBoostType.ProductionBoost: boostType = NewBoostType.ProductionBoost; break;
                    //                    case BoostType.SpeedGathering: boostType = NewBoostType.ResearchSpeed
                    case NewBoostType.Fog: boostType = NewBoostType.Fog; break;
                    case NewBoostType.TechBoost: boostType = NewBoostType.TechBoost; break;
                }

                data = new UserNewBoostData()
                {
                    Id = playerData.Id,
                    DataType = DataType.ActiveBoost,
                    ValueId = (NewBoostType)playerData.ValueId,
                    Value = new UserNewBoost
                    {
                        Type = boostType,
                        StartTime = boostDetails.StartTime,
                        Duration = (int)(boostDetails.EndTime - boostDetails.StartTime).TotalSeconds
                        //                        Level = 1
                    }
                };
            }
            else
            {
                var boost = JsonConvert.DeserializeObject<UserNewBoost>(playerData.Value);
                data = new UserNewBoostData()
                {
                    Id = playerData.Id,
                    DataType = DataType.ActiveBoost,
                    ValueId = (NewBoostType)playerData.ValueId,//.BoostType,
                                                               //                ValueId = CacheBoostDataManager.GetFullBoostDataByBoostId(playerData.ValueId).Info.BoostType,
                    Value = boost// JsonConvert.DeserializeObject<UserNewBoost>(playerData.Value)
                };
                data.Value.Type = data.ValueId;
            }

            return data;
        }

        public static UserBoostData PlayerDataToUserBoostData(PlayerDataTable playerData)
        {
            return new UserBoostData()
            {
                Id = playerData.Id,
                DataType = DataType.ActiveBoost,
                ValueId = CacheBoostDataManager.GetNewBoostByTypeId(playerData.ValueId).Type,//CacheBoostDataManager.GetFullBoostDataByTypeId(playerData.ValueId).BoostType,
                                                                                             //                ValueId = CacheBoostDataManager.GetFullBoostDataByBoostId(playerData.ValueId).Info.BoostType,
                Value = JsonConvert.DeserializeObject<UserBoostDetails>(playerData.Value)
            };
        }

        public static UserBoostData PlayerDataToUserBoostData(PlayerDataTableUpdated playerDataUpdated) => PlayerDataToUserBoostData(playerDataUpdated.ToPlayerDataTable);

        public static UserHeroData PlayerDataToUserHeroData(PlayerDataTable playerData)
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

        public static UserHeroData PlayerDataToUserHeroData(PlayerDataTableUpdated playerDataUpdated) => PlayerDataToUserHeroData(playerDataUpdated.ToPlayerDataTable);




        public static PlayerDataTable UserResourceDataToPlayerData(UserResourceData playerData)
        {
            return new PlayerDataTable()
            {
                Id = playerData.Id,
                DataType = DataType.Resource,
                ValueId = CacheResourceDataManager.GetResourceData(playerData.ValueId).Id,
                Value = playerData.Value.ToString()
            };
        }

        public static PlayerDataTable UserStructureDataToPlayerData(UserStructureData playerData)
        {
            return new PlayerDataTable()
            {
                Id = playerData.Id,
                DataType = DataType.Structure,
                ValueId = CacheStructureDataManager.GetFullStructureData(playerData.ValueId).Info.Id,
                Value = JsonConvert.SerializeObject(playerData.Value)
            };
        }

        public static PlayerDataTable UserTroopDataToPlayerData(UserTroopData playerData)
        {
            return new PlayerDataTable()
            {
                Id = playerData.Id,
                DataType = DataType.Troop,
                ValueId = CacheTroopDataManager.GetFullTroopData(playerData.ValueId).Info.Id,
                Value = JsonConvert.SerializeObject(playerData.Value)
            };
        }

        public static PlayerDataTable UserTechnologyDataToPlayerData(UserTechnologyData playerData)
        {
            return new PlayerDataTable()
            {
                Id = playerData.Id,
                DataType = DataType.Technology,
                ValueId = CacheTechnologyDataManager.GetFullTechnologyData(playerData.ValueId).Info.Id,
                Value = JsonConvert.SerializeObject(playerData.Value),
            };
        }

        public static PlayerDataTable UserInventoryDataToPlayerData(UserInventoryData data)
        {
            return new PlayerDataTable()
            {
                Id = data.Id,
                DataType = DataType.Inventory,
                ValueId = CacheInventoryDataManager.GetFullInventoryItemData(data.ValueId).Id,// Info.Id,
                Value = data.Value.ToString()
            };
        }

        public static PlayerDataTable UserBoostDataToPlayerData(UserBoostData data)
        {
            return new PlayerDataTable()
            {
                Id = data.Id,
                DataType = DataType.ActiveBoost,
                ValueId = (int)data.ValueId,//GetFullBoostDataByType(data.ValueId).BoostTypeId,
                Value = JsonConvert.SerializeObject(data.Value)
            };
        }
    }
}

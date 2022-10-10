using GameOfRevenge.Common.Interface.Model.Table;
using GameOfRevenge.Common.Models.Inventory;
using GameOfRevenge.Common.Models.Boost;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Models.Table;
using System.Collections.Generic;
using Newtonsoft.Json;
using GameOfRevenge.Common.Models.Hero;
using System.Runtime.Serialization;

namespace GameOfRevenge.Common.Models.PlayerData
{
    public class UserResourceData : BaseUserDataTable<ResourceType, long>, IBaseUserDataTable<ResourceType, long>, IReadOnlyBaseUserDataTable<ResourceType, long>, IBaseDataTypeTable<ResourceType, long>, IReadOnlyBaseDataTypeTable<ResourceType, long>
    {

    }

    public class UserStructureData : BaseUserDataTable<StructureType, List<StructureDetails>>, IBaseUserDataTable<StructureType, List<StructureDetails>>, IReadOnlyBaseUserDataTable<StructureType, List<StructureDetails>>, IBaseDataTypeTable<StructureType, List<StructureDetails>>, IReadOnlyBaseDataTypeTable<StructureType, List<StructureDetails>>
    {
//        public int StructureId { get; set; }
    }

    public class UserTroopData : BaseUserDataTable<TroopType, List<TroopDetails>>, IBaseUserDataTable<TroopType, List<TroopDetails>>, IReadOnlyBaseUserDataTable<TroopType, List<TroopDetails>>, IBaseDataTypeTable<TroopType, List<TroopDetails>>, IReadOnlyBaseDataTypeTable<TroopType, List<TroopDetails>>
    {

    }

    public class UserTechnologyData : BaseUserDataTable<TechnologyType, TechnologyInfos>, IBaseUserDataTable<TechnologyType, TechnologyInfos>, IReadOnlyBaseUserDataTable<TechnologyType, TechnologyInfos>, IBaseDataTypeTable<TechnologyType, TechnologyInfos>, IReadOnlyBaseDataTypeTable<TechnologyType, TechnologyInfos>
    {

    }

/*    public class UserSubTechnologyData : BaseUserDataTable<SubTechnologyType, SubTechnologyInfos>, IBaseUserDataTable<SubTechnologyType, SubTechnologyInfos>, IReadOnlyBaseUserDataTable<SubTechnologyType, SubTechnologyInfos>, IBaseDataTypeTable<SubTechnologyType, SubTechnologyInfos>, IReadOnlyBaseDataTypeTable<SubTechnologyType, SubTechnologyInfos>
    {

    }*/

    public class UserInventoryData : BaseUserDataTable<InventoryItemType, int>, IBaseUserDataTable<InventoryItemType, int>, IReadOnlyBaseUserDataTable<InventoryItemType, int>, IBaseDataTypeTable<InventoryItemType, int>, IReadOnlyBaseDataTypeTable<InventoryItemType, int>
    {

    }

    public class UserInventoryDataUpdated : UserInventoryData//, BaseUserDataTableUpdated<InventoryItemType, int>, IBaseUserDataTableUpdated<InventoryItemType, int>, IReadOnlyBaseUserDataTable<InventoryItemType, int>, IBaseDataTypeTable<InventoryItemType, int>, IReadOnlyBaseDataTypeTable<InventoryItemType, int>
    {
        [JsonProperty(Order = 1)]
        public string OldValue { get; set; }
    }

/*    public class UserRecordNewBoostData : BaseUserDataTable<NewBoostType, UserNewBoost>, IBaseUserDataTable<NewBoostType, UserNewBoost>, IReadOnlyBaseUserDataTable<NewBoostType, UserNewBoost>, IBaseDataTypeTable<NewBoostType, UserNewBoost>, IReadOnlyBaseDataTypeTable<NewBoostType, UserNewBoost>
    {

    }*/

    public class UserNewBoostData : BaseUserDataTable<NewBoostType, UserNewBoost>, IBaseUserDataTable<NewBoostType, UserNewBoost>, IReadOnlyBaseUserDataTable<NewBoostType, UserNewBoost>, IBaseDataTypeTable<NewBoostType, UserNewBoost>, IReadOnlyBaseDataTypeTable<NewBoostType, UserNewBoost>
    {

    }

    public class UserBoostData : BaseUserDataTable<NewBoostType, UserBoostDetails>, IBaseUserDataTable<NewBoostType, UserBoostDetails>, IReadOnlyBaseUserDataTable<NewBoostType, UserBoostDetails>, IBaseDataTypeTable<NewBoostType, UserBoostDetails>, IReadOnlyBaseDataTypeTable<NewBoostType, UserBoostDetails>
    {

    }

    public class UserHeroData : BaseUserDataTable<string, int>, IBaseUserDataTable<string, int>, IReadOnlyBaseUserDataTable<string, int>, IBaseDataTypeTable<string, int>, IReadOnlyBaseDataTypeTable<string, int>
    {
/*        public UserHeroDetails ToUserHeroDetails()
        {
            try
            {
                JsonConvert.DeserializeObject<UserHeroDetails>(this.Value);
            }
            catch { }

            return new UserHeroDetails()
            {
//                Id = this.Id,
                HeroCode = this.ValueId,
                
                Code = this.ValueId,
                BattleCount = this.Value,
                //                Unlocked = true,
                IsMarching = false
            };
        }*/
    }

    public class UserKingData : BaseUserDataTable<int, UserKingDetails>, IBaseUserDataTable<int, UserKingDetails>, IReadOnlyBaseUserDataTable<int, UserKingDetails>, IBaseDataTypeTable<int, UserKingDetails>, IReadOnlyBaseDataTypeTable<int, UserKingDetails>
    {
    }

    public class UserHeroDataList
    {
        public HeroType HeroType { get; set; }
        public List<UserHeroDataValue> Data { get; set; }
    }

    public class UserHeroDataValue
    {
        public int Type { get; set; }
        public int Value { get; set; }
    }

}

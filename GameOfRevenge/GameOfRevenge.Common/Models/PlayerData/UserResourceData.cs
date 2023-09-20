using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using GameOfRevenge.Common.Interface.Model.Table;
using GameOfRevenge.Common.Models.Inventory;
using GameOfRevenge.Common.Models.Boost;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Models.Table;
using GameOfRevenge.Common.Models.Hero;
using GameOfRevenge.Common.Models.Quest;

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

    [DataContract, Serializable]
    public class ResourceData
    {
        [DataMember]//, JsonProperty(Order = -2)]
        public long Id { get; set; }

        [DataMember(EmitDefaultValue = false)]//, Order = -2)]
        public int LocationId { get; set; }

        [DataMember(EmitDefaultValue = false), JsonConverter(typeof(StringEnumConverter))]//JsonProperty(Order = -2), JsonConverter(typeof(StringEnumConverter))]
        public ResourceType ResourceType { get; set; }
        [DataMember]//, JsonProperty(Order = -2)]
        public long Value { get; set; }

        public ResourceData()
        {
        }

        public ResourceData(PlayerDataTable data)
        {
            Id = data.Id;
            ResourceType = (ResourceType)data.ValueId;
            Value = long.Parse(data.Value);
        }

        public ResourceData(StoredDataTable data)
        {
            Id = data.DataId;
            LocationId = data.LocationId;
            ResourceType = (ResourceType)data.ValueId;
            Value = data.Value;
        }
    }

    [DataContract]
    public class BoostUpResourceResponse
    {
        [DataMember]
        public DateTime StartTime { get; set; }
        [DataMember]
        public int Duration { get; set; }
        [DataMember]
        public int Percentage { get; set; }
    }

    [DataContract]
    public class CollectedResourceResponse
    {
        [DataMember]
        public CollectedResourceData CollectedResource { get; set; }
//        [DataMember(EmitDefaultValue = false)]
//        public int LocationId { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<PlayerQuestDataTable> UpdatedQuests { get; set; }

        public CollectedResourceResponse()
        {
        }

        public CollectedResourceResponse(CollectedResourceData collected, List<PlayerQuestDataTable> updatedQuests)
        {
            CollectedResource = collected;
            if ((updatedQuests != null) && (updatedQuests.Count > 0)) UpdatedQuests = updatedQuests;
        }
    }

    [DataContract]
    public class CollectedResourceData : ResourceData
    {
        [DataMember(Order = 2)]
        public int Collected { get; set; }
//        [DataMember(EmitDefaultValue = false)]
//        public int LocationId { get; set; }
//        [DataMember(EmitDefaultValue = false)]
//        public List<UserQuestProgressData> UpdatedQuests { get; set; }

        public CollectedResourceData()
        {
        }

        public CollectedResourceData(int collected, UserResourceData data, int locationId = 0)
        {
            Id = data.Id;
            ResourceType = data.ValueId;
            Value = data.Value;

            Collected = collected;
            LocationId = locationId;
        }

        public CollectedResourceData(int collected, PlayerDataTable data) : base(data)
        {
            Collected = collected;
        }
    }

    public class StoredResourceData
    {
        public ResourceData Stored { get; set; }
        public ResourceData Resource { get; set; }

        public StoredResourceData()
        {
        }

        public StoredResourceData(ResourceData stored, ResourceData resource)
        {
            Stored = stored;
            Resource = resource;
        }
    }
}

using GameOfRevenge.Common.Interface.Model.Table;
using GameOfRevenge.Common.Models.Hero;
using GameOfRevenge.Common.Models.Inventory;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Models.Table;
using System.Collections.Generic;

namespace GameOfRevenge.Common.Models.PlayerData
{
    public class UserResourceData : BaseUserDataTable<ResourceType, float>, IBaseUserDataTable<ResourceType, float>, IReadOnlyBaseUserDataTable<ResourceType, float>, IBaseDataTypeTable<ResourceType, float>, IReadOnlyBaseDataTypeTable<ResourceType, float>
    {

    }

    public class UserStructureData : BaseUserDataTable<StructureType, List<StructureDetails>>, IBaseUserDataTable<StructureType, List<StructureDetails>>, IReadOnlyBaseUserDataTable<StructureType, List<StructureDetails>>, IBaseDataTypeTable<StructureType, List<StructureDetails>>, IReadOnlyBaseDataTypeTable<StructureType, List<StructureDetails>>
    {
        public int StructureId { get; set; }
    }

    public class UserTroopData : BaseUserDataTable<TroopType, List<TroopDetails>>, IBaseUserDataTable<TroopType, List<TroopDetails>>, IReadOnlyBaseUserDataTable<TroopType, List<TroopDetails>>, IBaseDataTypeTable<TroopType, List<TroopDetails>>, IReadOnlyBaseDataTypeTable<TroopType, List<TroopDetails>>
    {

    }

    public class UserTechnologyData : BaseUserDataTable<TechnologyType, TechnologyInfos>, IBaseUserDataTable<TechnologyType, TechnologyInfos>, IReadOnlyBaseUserDataTable<TechnologyType, TechnologyInfos>, IBaseDataTypeTable<TechnologyType, TechnologyInfos>, IReadOnlyBaseDataTypeTable<TechnologyType, TechnologyInfos>
    {

    }

    public class UserInventoryData : BaseUserDataTable<InventoryItemType, int>, IBaseUserDataTable<InventoryItemType, int>, IReadOnlyBaseUserDataTable<InventoryItemType, int>, IBaseDataTypeTable<InventoryItemType, int>, IReadOnlyBaseDataTypeTable<InventoryItemType, int>
    {

    }

    public class UserBuffData : BaseUserDataTable<InventoryItemType, UserBuffDetails>, IBaseUserDataTable<InventoryItemType, UserBuffDetails>, IReadOnlyBaseUserDataTable<InventoryItemType, UserBuffDetails>, IBaseDataTypeTable<InventoryItemType, UserBuffDetails>, IReadOnlyBaseDataTypeTable<InventoryItemType, UserBuffDetails>
    {

    }
}

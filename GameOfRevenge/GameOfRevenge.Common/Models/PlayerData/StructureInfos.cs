using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Common.Models.Structure;
using Newtonsoft.Json;

namespace GameOfRevenge.Common.Models
{
    [Serializable]
    public class StructureInfos
    {
        public long Id { get; set; }
        public StructureType StructureType { get; set; }
        public List<StructureDetails> Buildings { get; set; }

        public StructureInfos()
        {
        }

        public StructureInfos(PlayerDataTable userDataTable)
        {
            Id = userDataTable.Id;
            StructureType = (StructureType)userDataTable.ValueId;
            Buildings = JsonConvert.DeserializeObject<List<StructureDetails>>(userDataTable.Value);
        }

        public StructureInfos(UserStructureData userData)
        {
            Id = userData.Id;
            StructureType = userData.ValueId;
            Buildings = userData.Value;
        }
    }

    public class BuildingStructure
    {
        public long WorkerId { get; set; }
        //        public StructureType StructureType { get; set; }
        public StructureDetails Structure { get; set; }

        public BuildingStructure()
        {
        }

        public BuildingStructure(StructureDetails structure, UserRecordBuilderDetails worker)
        {
            WorkerId = worker.Id;
            Structure = structure;
        }
    }

    public class BuildingStructureData
    {
        public long WorkerId { get; set; }
        public UserStructureData StructureData { get; set; }

        public BuildingStructureData()
        {
        }

        public BuildingStructureData(UserStructureData structureData, UserRecordBuilderDetails worker = null)
        {
            if (worker != null) WorkerId = worker.Id;
            StructureData = structureData;
        }
    }

    [Serializable, DataContract]
    public class StructureDetails : TimerBase
    {
        [DataMember]
        public int Location { get; set; }
        [DataMember]
        public int Level { get; set; }
        [DataMember]
        public int HitPoints { get; set; }
        [DataMember]
        public int Helped { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime LastCollected { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public TimerBase Boost { get; set; }

        public int CurrentLevel => (TimeLeft == 0) ? Level : (Level - 1);
    }
}
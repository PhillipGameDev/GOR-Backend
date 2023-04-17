using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Common.Models.Structure;
using Newtonsoft.Json;

namespace GameOfRevenge.Common.Models
{
    public class StructureInfos
    {
        public long Id { get; set; }
        public StructureType StructureType { get; set; }
        public List<StructureDetails> Buildings { get; set; }

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

    [Serializable, DataContract]
    public class StructureDetails : TimerBase
    {
        [DataMember(Name = "Location")]
        public int LocationId { get; set; }
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
    }
}
using GameOfRevenge.Common.Models.Kingdom;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GameOfRevenge.Common.Models
{
    public class EnlistedTroop
    {
        public TroopType TroopType { get; set; }
        public List<EnlistedTroopDetails> TroopData { get; set; }
    }

    public class EnlistedTroopDetails
    {
        public int Level { get; set; }
        public int Count { get; set; }
    }

    [Serializable]
    public class TroopInfos
    {
        public long Id { get; set; }
        public BattleTroopType TroopMainType { get; set; }
        public TroopType TroopType { get; set; }
        public MonsterType MonsterType { get; set; }
        public List<TroopDetails> TroopData { get; set; }

        public TroopInfos()
        {
        }

        public TroopInfos(long id, TroopType troopType, List<TroopDetails> troopData)
        {
            Id = id;
            TroopType = troopType;
            TroopData = troopData;
        }
    }

    [DataContract, Serializable]
    public class TroopDetails
    {
        [DataMember]
        public int Level { get; set; }
        [DataMember]
        public int Count { get; set; } // total count of troop in training and ready for war// training + trained

        [DataMember(EmitDefaultValue = false)]
        public int Wounded { get; set; }

        //        [JsonIgnore]
        public int FinalCount//AVAILABLE
        {
            get
            {
                var count = Count - (InRecoveryCount + InTrainingCount + Wounded);

                return (count > 0) ? count : 0;
            }
        }

        public int InRecoveryCount
        {
            get
            {
                var count = 0;
                if (InRecovery != null)
                {
                    foreach (var inRecovery in InRecovery)
                    {
                        if (inRecovery.TimeLeft > 0) count += inRecovery.Count;
                    }
                }

                return count;
            }
        }

        public int InTrainingCount
        {
            get
            {
                var count = 0;
                if (InTraning != null)
                {
                    foreach (var inTraining in InTraning)
                    {
                        if (inTraining.TimeLeft > 0) count += inTraining.Count;
                    }
                }

                return count;
            }
        }

        [DataMember(EmitDefaultValue = false)]
        public List<UnavaliableTroopInfo> InTraning { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<UnavaliableTroopInfo> InRecovery { get; set; }
    }

    [DataContract, Serializable]
    public class UnavaliableTroopInfo : TimerBase
    {
        [DataMember]
        public int Count { get; set; }
        [DataMember]
        public int BuildingLocId { get; set; }
    }
}

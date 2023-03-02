using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GameOfRevenge.Common.Models.Hero;
//using Newtonsoft.Json;

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
        public TroopType TroopType { get; set; }
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
                var unavailable = 0;
                if (InTraning != null)
                {
                    foreach (var trainingData in InTraning)
                    {
                        if (trainingData?.TimeLeft > 0) unavailable += trainingData.Count;
                    }
                }
                if (InRecovery != null)
                {
                    foreach (var recoveryData in InRecovery)
                    {
                        if (recoveryData?.TimeLeft > 0) unavailable += recoveryData.Count;
                    }
                }
                var count = Count - (unavailable + Wounded);

                return (count > 0)? count: 0;
            }
        }

        [DataMember(EmitDefaultValue = false)]
        public List<UnavaliableTroopInfo> InTraning { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<UnavaliableTroopInfo> InRecovery { get; set; }
    }

    [DataContract]
    public class UnavaliableTroopInfo : TimerBase
    {
        [DataMember]
        public int Count { get; set; }
        [DataMember]
        public int BuildingLocId { get; set; }
    }

    [DataContract]
    public class MarchingArmy
    {
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public List<TroopInfos> Troops { get; set; }
        [DataMember]
        public int TargetPlayer { get; set; }
        [DataMember]
        public DateTime StartTime { get; set; }
        [DataMember]
        public int ReachedTime { get; set; }  //reach to destination time from start point
        [DataMember]
        public int BattleDuration { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<HeroType> Heroes { get; set; }

        public double TimeLeftForTask
        {
            get
            {
                DateTime taskTime = StartTime.ToUniversalTime().AddSeconds(ReachedTime);
                double totalSeconds = (taskTime - DateTime.UtcNow).TotalSeconds;
                return (totalSeconds > 0) ? totalSeconds : 0;
            }
        }

        public bool IsTimeForReturn
        {
            get
            {
                DateTime returnTime = StartTime.ToUniversalTime().AddSeconds(ReachedTime + BattleDuration);
                return DateTime.UtcNow > returnTime;
            }
        }

        public double TimeLeft
        {
            get
            {
                DateTime endTime = StartTime.ToUniversalTime().AddSeconds((ReachedTime * 2) + BattleDuration);
                double totalSecs = (endTime - DateTime.UtcNow).TotalSeconds;
                return (totalSecs > 0)? totalSecs : 0;
            }
        }
    }
}

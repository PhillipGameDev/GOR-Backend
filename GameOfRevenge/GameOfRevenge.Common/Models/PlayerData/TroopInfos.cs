﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GameOfRevenge.Common.Models.Hero;
using GameOfRevenge.Common.Models.Kingdom;
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
                var count = Count - (InRecoveryCount + InTrainingCount + Wounded);

                return (count > 0)? count: 0;
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

    [DataContract]
    public class MarchingArmy
    {
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public int TargetPlayer { get; set; }
        [DataMember]
        public DateTime StartTime { get; set; }
        [DataMember]
        public int ReachedTime { get; set; }
        [DataMember]
        public int BattleDuration { get; set; }
        [DataMember]
        public List<TroopInfos> Troops { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<HeroType> Heroes { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public BattleReport Report { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<TroopDetailsPvP> TroopChanges { get; set; }

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

        public int[] TroopsToArray()
        {
            var list = new List<int>();
            foreach (var troopClass in Troops)
            {
                foreach (var troop in troopClass.TroopData)
                {
                    list.Add((int)troopClass.TroopType);
                    list.Add(troop.Level);
                    list.Add(troop.Count);
                }
            }
            return list.ToArray();
        }

        public int[] HeroesToArray(List<UserHeroDetails> userHeroes)
        {
            int[] heroes = null;
            if ((Heroes != null) && (Heroes.Count > 0))
            {
                var len = Heroes.Count;
                var idx = 0;
                heroes = new int[len * 2];
                for (int num = 0; num < len; num++)
                {
                    var heroType = Heroes[num];
                    heroes[idx] = (int)heroType;
                    var userHero = userHeroes.Find(x => (x.HeroType == heroType));
                    if (userHero != null) heroes[idx + 1] = userHero.Level;

                    idx += 2;
                }
            }

            return heroes;
        }
    }
}

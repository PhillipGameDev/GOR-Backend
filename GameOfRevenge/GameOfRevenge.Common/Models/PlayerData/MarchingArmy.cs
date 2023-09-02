using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using GameOfRevenge.Common.Models.Hero;
using GameOfRevenge.Common.Models.Kingdom;

namespace GameOfRevenge.Common.Models.PlayerData
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MarchingType : byte
    {
        Unknown = 0,
        AttackPlayer = 1,
        ReinforcementPlayer = 2,
        AttackMonster = 3
    }

    [DataContract]
    public class MarchingArmy : MarchingArmyBase
    {
        [DataMember]
        public long MarchingId { get; set; }
        public int MarchingSlot { get; set; }

        public MarchingArmyBase Base()
        {
            return new MarchingArmyBase()
            {
                MarchingType = MarchingType,
                TargetId = TargetId,
                StartTime = StartTime,
                Recall = Recall,
                Distance = Distance,
                AdvanceReduction = AdvanceReduction,
                ReturnReduction = ReturnReduction,
                Duration = Duration,
                Troops = Troops,
                Heroes = Heroes,
                Report = Report,
                TroopChanges = TroopChanges
            };
        }
    }

    [DataContract]
    public class MarchingArmyBase
    {
        [DataMember]
        public MarchingType MarchingType { get; set; }
        [DataMember]
        public int TargetId { get; set; }
        [DataMember]
        public DateTime StartTime { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int Recall { get; set; }

        [DataMember]
        public int Distance { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int AdvanceReduction { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int ReturnReduction { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int Duration { get; set; }

        [DataMember]
        public List<TroopInfos> Troops { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<HeroType> Heroes { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public BattleReport Report { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<TroopDetailsPvP> TroopChanges { get; set; }

        public bool IsRecall => (Recall > 0);
        public bool IsTimeForReturn => (TimeLeftForReturn == 0);

        public double TimeLeftForTask => TimeLeftFor(Distance - AdvanceReduction);
        public double TimeLeftForReturn => TimeLeftFor(Distance - AdvanceReduction + Duration);
        public double TimeLeft
        {
            get
            {
                int value = Recall + Distance - ReturnReduction;
                if (Recall == 0)
                {
                    var returnDist = ((MarchingType != MarchingType.ReinforcementPlayer)? Distance : 0);
                    value += returnDist - AdvanceReduction + Duration;
                }

                return TimeLeftFor(value);
            }
        }

        double TimeLeftFor(int duration)
        {
            var taskTime = StartTime.ToUniversalTime().AddSeconds(duration);
            var totalSeconds = (taskTime - DateTime.UtcNow).TotalSeconds;

            return (totalSeconds > 0) ? totalSeconds : 0;
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

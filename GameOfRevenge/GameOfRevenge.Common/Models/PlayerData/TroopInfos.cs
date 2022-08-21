using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
//using Newtonsoft.Json;

namespace GameOfRevenge.Common.Models
{
    public class TroopInfos
    {
        public long Id { get; set; }
        public TroopType TroopType { get; set; }
        public List<TroopDetails> TroopData { get; set; }
    }

    [DataContract]
    public class TroopDetails
    {
        [DataMember]
        public int Level { get; set; }
        [DataMember]
        public int Count { get; set; } // total count of troop in training and ready for war// training + trained
        [DataMember]
        public int Wounded { get; set; }

//        [JsonIgnore]
        public int FinalCount
        {
            get
            {
                var minusCount = 0;

                if (InTraning != null)
                {
                    foreach (var trainingData in InTraning)
                    {
                        if (trainingData != null && trainingData.TimeLeft > 0)
                        {
                            minusCount += trainingData.Count;
                        }
                    }
                }

                if (InRecovery != null)
                {
                    foreach (var recoveryData in InRecovery)
                    {
                        if (recoveryData != null && recoveryData.TimeLeft > 0)
                        {
                            minusCount += recoveryData.Count;
                        }
                    }
                }

                return Count - minusCount - Wounded;
            }
        }
        [DataMember]
        public List<UnavaliableTroopInfo> InTraning { get; set; }
        [DataMember]
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

    public class MarchingArmy
    {
        public List<TroopInfos> Troops { get; set; }
        public int TargetPlayer { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime TaskTime { get; set; }  //reach to destination time from start point
        public DateTime EndTime { get; set; }
        public double TimeLeftForTask
        {
            get
            {
                double totalSeconds = (TaskTime - DateTime.UtcNow).TotalSeconds;
                return totalSeconds <= 0 ? 0 : totalSeconds;
            }
        }

        public List<int> Heroes { get; set; }

        public bool IsTimeForReturn
        {
            get
            {
                double marchingTime = (TaskTime - StartTime).TotalMilliseconds;
                DateTime returnTime = EndTime.AddMilliseconds(-marchingTime);
                return returnTime < DateTime.UtcNow;
            }
        }

/*        public double TimeLeftForReturn
        {
            get
            {
                double marchingTime = (TaskTime - StartTime).TotalMilliseconds;
                double totalSeconds = (EndTime.AddMilliseconds(-marchingTime) - DateTime.UtcNow).TotalSeconds;
                return totalSeconds <= 0 ? 0 : totalSeconds;
            }
        }*/

        public double TimeLeft
        {
            get
            {
                double totalSeconds = (EndTime - DateTime.UtcNow).TotalSeconds;
                return totalSeconds <= 0 ? 0 : totalSeconds;
            }
        }
    }
}

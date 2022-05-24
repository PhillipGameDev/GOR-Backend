using System;
using System.Collections.Generic;

namespace GameOfRevenge.Common.Models
{
    public class TroopInfos
    {
        public TroopType TroopType { get; set; }
        public List<TroopDetails> TroopData { get; set; }
    }

    public class TroopDetails
    {
        public int Level { get; set; }
        public int Count { get; set; } // total count of troop in training and ready for war// training + trained
        public int Wounded { get; set; }
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
        public List<UnavaliableTroopInfo> InTraning { get; set; }
        public List<UnavaliableTroopInfo> InRecovery { get; set; }
    }

    public class UnavaliableTroopInfo : TimerBase
    {
        public int Count { get; set; }
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

        public List<int> Heros { get; set; }

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

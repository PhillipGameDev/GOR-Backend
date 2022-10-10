using GameOfRevenge.Common.Models.Boost;
using GameOfRevenge.Common.Models.PlayerData;
using System;
using System.Runtime.Serialization;

namespace GameOfRevenge.Common.Models
{
    [Serializable]
    [DataContract]
    public class UserBoostDetails
    {
        [DataMember]
        public NewBoostType BoostType { get; set; }
/*#if UNITY_2019_1_OR_NEWER
        [DataMember]
        public string StartTime { get; set; }
        [DataMember]
        public string EndTime { get; set; }
#else*/
        [DataMember]
        public DateTime StartTime { get; set; }
        [DataMember]
        public DateTime EndTime { get; set; }
//#endif
        private DateTime EndTimeDate
        {
            get
            {
/*#if UNITY_2019_1_OR_NEWER
                DateTime.TryParse(EndTime, out DateTime endTime);
                return endTime;
#else*/
                return EndTime;
//#endif
            }
        }

        public double TimeLeft
        {
            get
            {
                double totalSeconds = (EndTimeDate - DateTime.UtcNow).TotalSeconds;
                return (totalSeconds > 0) ? totalSeconds : 0;
            }
        }
    }

    [Serializable]
    [DataContract]
    public class FullUserBoostDetails : UserBoostDetails
    {
        [DataMember]
        public long Id { get; set; }

    }
}

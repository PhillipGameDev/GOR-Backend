using System;
using System.Runtime.Serialization;

namespace GameOfRevenge.Common.Models
{
    [DataContract]
    public class UserBuilderDetails : TimerBase
    {
        [DataMember]
        public int Location { get; set; }
    }

    [Serializable]
    [DataContract]
    public class UserRecordBuilderDetails : UserBuilderDetails
    {
        [DataMember]
        public long Id { get; set; }
    }
}

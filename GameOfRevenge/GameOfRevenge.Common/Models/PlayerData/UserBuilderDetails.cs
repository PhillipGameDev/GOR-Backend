using System;
using System.Runtime.Serialization;
//using GameOfRevenge.Common.Models.Hero;
//using Newtonsoft.Json;

namespace GameOfRevenge.Common.Models
{
    [DataContract]
    public class UserBuilderDetails : TimerBase
    {
        [DataMember]
        public int Location { get; set; }
    }

    [DataContract]
    public class UserRecordBuilderDetails : UserBuilderDetails
    {
        [DataMember]
        public long Id { get; set; }
    }
}

using System.Runtime.Serialization;

namespace GameOfRevenge.Common.Models
{
    [DataContract]
    public class BoostUpData : TimerBase
    {
        [DataMember(EmitDefaultValue = false)]
        public int Value { get; set; }
    }
}

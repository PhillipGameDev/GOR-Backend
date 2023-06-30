using System.Runtime.Serialization;

namespace GameOfRevenge.Common.Models.Kingdom
{
    [DataContract]
    public class BattleReport
    {
        [DataMember]
        public ClientBattleReport Attacker { get; set; }
        [DataMember]
        public ClientBattleReport Defender { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public bool AttackerWon { get; set; }

        public int WinnerId => AttackerWon? Attacker.PlayerId : Defender.PlayerId;
    }
}

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
        public int WinnerId { get; set; }

        public bool AttackerWon => (WinnerId == Attacker.PlayerId);
    }
}

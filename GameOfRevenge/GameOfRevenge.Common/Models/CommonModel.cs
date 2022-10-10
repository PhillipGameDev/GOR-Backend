using GameOfRevenge.Common.Models.Kingdom.AttackAlertReport;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace GameOfRevenge.Common.Models
{
    public class AttackStatusData
    {
        public AttackSocketResponse AttackData { get; set; }
        public int WinnerPlayerId { get; set; }
        public PlayerCompleteData Attacker { get; set; }
        public PlayerCompleteData Defender { get; set; }
        public UnderAttackReport Report { get; set; }
        public int State { get; set; }
    }

/*    public class PlayerPvpData
    {
        public int PlayerId { get; set; }
        public PlayerCompleteData Data { get; set; }
    }*/
    /*
        public string AttckerUserName { get; set; }
        public string EnemyUserName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int ReachedTime { get; set; }
        public int[] TroopCount { get; set; }
        public int[] TroopType { get; set; }
        public int[] Heros { get; set; }
        public double CurrentTime
        {
            get
            {
                var reachedTime = StartTime.AddMilliseconds(this.ReachedTime);
//                var returnTime = reachedTime.AddMilliseconds(this.BattleTime + this.ReachedTime);
                if (reachedTime > DateTime.UtcNow)
                    return (reachedTime - DateTime.UtcNow).TotalMilliseconds;
                else if (this.EndTime.AddMilliseconds(-this.ReachedTime) < DateTime.UtcNow)
                    return 0;
                else
                    return (this.EndTime - DateTime.UtcNow).TotalMilliseconds;
            }
        }
*/
    [DataContract]
    public class AttackSocketResponse
    {
        [DataMember]
        public int AttackerId { get; set; }
        [DataMember]
        public string AttackerUsername { get; set; }

        [DataMember]
        public int EnemyId { get; set; }
        [DataMember]
        public string EnemyUsername { get; set; }

        [DataMember]
        public int LocationX { get; set; } = -1;
        [DataMember]
        public int LocationY { get; set; } = -1;

        [DataMember]
        public DateTime StartTime { get; set; }
        [DataMember]
        public int ReachedTime { get; set; }
        [DataMember]
        public int BattleDuration { get; set; }

        [DataMember]
        public int KingLevel { get; set; } = -1;
        [DataMember]
        public int[] TroopCount { get; set; }
        [DataMember]
        public int[] TroopType { get; set; }
        [DataMember]
        public int[] HeroIds { get; set; }

/*        public double CurrentTime
        {
            get
            {
                var reachedTime = StartTime.AddSeconds(ReachedTime);
                var returnTime = reachedTime.AddSeconds(ReachedTime + BattleDuration);
                if (reachedTime > DateTime.UtcNow)
                    return (reachedTime - DateTime.UtcNow).TotalMilliseconds;
                else
                    return (returnTime - DateTime.UtcNow).TotalMilliseconds;
            }
        }*/

/*        public bool IsReturnFromAttack
        {
            get
            {
                var reachedTime = StartTime.AddSeconds(ReachedTime + BattleDuration);
                return reachedTime <= DateTime.UtcNow;
            }
        }*/
    }
}

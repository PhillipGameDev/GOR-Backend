using GameOfRevenge.Common.Models.Kingdom.AttackAlertReport;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameOfRevenge.Common.Models
{
    public class AttackStatusData
    {
        public AttackSocketResponse AttackData { get; set; }
        public int WinnerPlayerId { get; set; }
        public PlayerPvpData Attacker { get; set; }
        public PlayerPvpData Defender { get; set; }
        public UnderAttackReport Report { get; set; }
        public int State { get; set; }
    }

    public class PlayerPvpData
    {
        public int PlayerId { get; set; }
        public PlayerCompleteData Data { get; set; }
    }
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
    public class AttackSocketResponse
    {
        public DateTime StartTime { get; set; }

        public int AttackerUserId { get; set; }
        public string AttckerUserName { get; set; }

        public int EnemyUserId { get; set; }
        public string EnemyUserName { get; set; }

        public int ReachedTime { get; set; }
        public int[] TroopCount { get; set; }
        public int[] TroopType { get; set; }
        public int[] Heros { get; set; }//TODO:fix typo

        public double CurrentTime
        {
            get
            {
                var reachedTime = StartTime.AddMilliseconds(this.ReachedTime);
                var returnTime = reachedTime.AddMilliseconds(this.ReachedTime);
                if (reachedTime > DateTime.UtcNow)
                    return (reachedTime - DateTime.UtcNow).TotalMilliseconds;
                else
                    return (returnTime - DateTime.UtcNow).TotalMilliseconds;
            }
        }

        public bool IsReturnFromAttack
        {
            get
            {
                var reachedTime = StartTime.AddMilliseconds(this.ReachedTime);
                return reachedTime <= DateTime.UtcNow;
            }
        }
    }
}

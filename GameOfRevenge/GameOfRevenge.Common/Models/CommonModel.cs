using System;
using System.Collections.Generic;
using GameOfRevenge.Common.Models.Kingdom;
using GameOfRevenge.Common.Models.Kingdom.AttackAlertReport;

namespace GameOfRevenge.Common.Models
{
    public class AttackStatusData
    {
        public AttackResponseData AttackData { get; set; }
        public MarchingArmy MarchingArmy { get; set; }
//        public int WinnerPlayerId => BattleReport.AttackerWon ? AttackerPower.PlayerId : DefenderPower.PlayerId;

        public PlayerCompleteData Attacker { get; set; }
        public PlayerCompleteData Defender { get; set; }
        public BattlePower AttackerPower { get; set; }
        public BattlePower DefenderPower { get; set; }
//        public BattleReport BattleReport { get; set; }

        public int State { get; set; }
    }

    public class AttackResponseData
    {
        public int AttackerId { get; set; }
        public string AttackerUsername { get; set; }

        public int EnemyId { get; set; }
        public string EnemyUsername { get; set; }

//        public int LocationX { get; set; }
//        public int LocationY { get; set; }

        public string StartTime { get; set; }
        public int ReachedTime { get; set; }
        public int BattleDuration { get; set; }

        public byte KingLevel { get; set; }
        public byte WatchLevel { get; set; }

        public int[] Troops { get; set; }
        public int[] Heroes { get; set; }

        public AttackResponseData()
        {
        }

        public AttackResponseData(PlayerCompleteData attackerCompleteData, MarchingArmy marchingArmy, int enemyId, string enemyName, byte watchLevel)
        {
            AttackerId = attackerCompleteData.PlayerId;
            AttackerUsername = attackerCompleteData.PlayerName;

            EnemyId = enemyId;
            EnemyUsername = enemyName;

//            LocationX = location.X;
//            LocationY = location.Y;

            KingLevel = attackerCompleteData.King.Level;
            WatchLevel = watchLevel;

            StartTime = marchingArmy.StartTime.ToUniversalTime().ToString("s") + "Z";
            ReachedTime = marchingArmy.ReachedTime;
            BattleDuration = marchingArmy.BattleDuration;

            Troops = marchingArmy.TroopsToArray();
            Heroes = marchingArmy.HeroesToArray(attackerCompleteData.Heroes);
        }
    }
}

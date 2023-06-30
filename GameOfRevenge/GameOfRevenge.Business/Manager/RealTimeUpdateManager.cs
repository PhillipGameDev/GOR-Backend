using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExitGames.Logging;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Business.Manager.UserData;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Boost;
using GameOfRevenge.Common.Models.Kingdom;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Services;

namespace GameOfRevenge.Business.Manager
{
    public class RealTimeUpdateManager : IRealTimeUpdateManager
    {
        public static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private static readonly List<AttackStatusData> attackInProgress = new List<AttackStatusData>();

        private readonly KingdomPvPManager pvpManager = new KingdomPvPManager();
        protected readonly object SyncRoot = new object(); // that is for world user access

        public DelayedAction Disposable;

        public AttackStatusData GetAttackerData(int attackerId)
        {
            AttackStatusData data = null;
            lock (SyncRoot)
            {
                data = attackInProgress.Find(x => (x.AttackData.AttackerId == attackerId));
            }
            return data;
        }

        public AttackStatusData GetAttackDataForDefender(int defenderId)
        {
            AttackStatusData data = null;
            lock (SyncRoot)
            {
                data = attackInProgress.Find(x => (x.AttackData.EnemyId == defenderId));
            }
            return data;
        }

        public void AddNewAttackOnWorld(AttackStatusData data)
        {
            if (data.MarchingArmy.Report != null) data.State = 5;
            lock (SyncRoot)
            {
                attackInProgress.Add(data);
            }
        }

        public async Task Update(Action<AttackStatusData, bool> attackResultCallback)
        {
            if (attackInProgress.Count > 0)
            {
                List<AttackStatusData> list = null;
                lock (SyncRoot)
                {
                    list = attackInProgress.ToList();
                }
                if (list != null)
                {
                    double timeleft = double.MaxValue;
                    log.Debug("****** UPDATE BATTLE START ****** x " + list.Count);
                    foreach (AttackStatusData item in list)
                    {
                        try
                        {
                            switch (item.State)
                            {
                                case 0://marching to target
                                    if (item.MarchingArmy.TimeLeftForTask <= 0)
                                    {
                                        var atkResp = await BaseUserDataManager.GetFullPlayerData(item.AttackData.AttackerId);
                                        if (!atkResp.IsSuccess) throw new Exception("attacker data not found");

                                        item.Attacker = atkResp.Data;
                                        item.State++;
                                    }
                                    break;

                                case 1://pulling data
                                    if (item.AttackData.MonsterId == 0)
                                    {
                                        var defResp = await BaseUserDataManager.GetFullPlayerData(item.AttackData.EnemyId);
                                        if (!defResp.IsSuccess) throw new Exception("defender data not found");

                                        item.Defender = defResp.Data;
                                    }
                                    item.State++;
                                    break;

                                case 2://prepare data
                                    log.Debug("------------ PREPARE BATTLE SIMULATION " + item.Attacker.PlayerId + " vs " + item.AttackData.EnemyId);

                                    var attackerPower = new BattlePower(item.Attacker, item.MarchingArmy, CacheTroopDataManager.GetFullTroopData, GetAtkDefMultiplier);
                                    item.AttackerPower = attackerPower;

                                    BattlePower defenderPower;
                                    if (item.AttackData.MonsterId > 0)
                                    {
                                        var hitPoints = (int)(attackerPower.HitPoints * (new Random().Next(6, 9) / 10f));
                                        var attack = (int)(attackerPower.Attack * (new Random().Next(8, 11) / 10f));
                                        var defense = (int)(attackerPower.Defense * 0.7f);
                                        defenderPower = new BattlePower(item.AttackData.EnemyId, item.AttackData.MonsterId, hitPoints, attack, defense);
                                    }
                                    else
                                    {
                                        defenderPower = new BattlePower(item.Defender, null, CacheTroopDataManager.GetFullTroopData, GetAtkDefMultiplier);
                                    }
                                    item.DefenderPower = defenderPower;

                                    log.Debug("atk pwr= " + attackerPower.HitPoints + " vs def pwr=" + defenderPower.HitPoints);
                                    item.State++;
                                    break;

                                case 3://battle simulation
                                    if ((item.DefenderPower.HitPoints > 0) && (item.AttackerPower.HitPoints > 0))
                                    {
                                        if (item.AttackData.MonsterId > 0)
                                        {
                                            item.AttackerPower.AttackMonster(item.DefenderPower);
                                        }
                                        else
                                        {
                                            item.AttackerPower.AttackPlayer(item.DefenderPower);
                                        }
                                        log.Debug("atk pwr= " + item.AttackerPower.HitPoints + " xx def pwr=" + item.DefenderPower.HitPoints);
                                    }
                                    else
                                    {
                                        await pvpManager.FinishBattleData(item.Attacker, item.AttackerPower, item.Defender, item.DefenderPower, item.MarchingArmy);
//                                        item.BattleReport = reportResp.Result;
                                        item.State++;
                                    }
                                    break;

                                case 4://waiting for return
                                    if (item.MarchingArmy.IsTimeForReturn)
                                    {
                                        if (item.AttackData.MonsterId == 0)
                                        {
                                            log.Debug("send defender under attack end event");
                                            attackResultCallback(item, false);
                                        }
                                        item.State++;
                                    }
                                    break;

                                case 5://marching to castle
                                    if (item.MarchingArmy.TimeLeft <= 0)
                                    {
                                        //SAVE PLAYER REPORT
                                        log.Debug("ApplyAttackerChangesAndSendReport!!!");
                                        await pvpManager.ApplyAttackerChangesAndSendReport(item.MarchingArmy);

                                        log.Debug("send attacker attack end event");
                                        attackResultCallback(item, true);
                                        log.Debug("** END ** " + item.AttackData.AttackerId + " vs " + item.AttackData.EnemyId);
                                        lock (SyncRoot) attackInProgress.Remove(item);
                                    }
                                    break;
                            }
                            if (item.MarchingArmy.TimeLeft < timeleft) timeleft = item.MarchingArmy.TimeLeft;
                        }
                        catch (Exception ex)
                        {
                            log.Debug("EXCEPTION " + item.State+"  "+ item.AttackData.AttackerId+" vs "+item.AttackData.EnemyId+"  " +ex.Message);
                            lock (SyncRoot) attackInProgress.Remove(item);
                        }
                    }
                    log.Debug("****** UPDATE BATTLE END ****** " + timeleft);
                    list.Clear();
                    list = null;
                }
            }

            if (Disposable != null) Disposable.Dispose();
            Disposable = new DelayedAction();
            Disposable.WaitForCallBack(async () => { await Update(attackResultCallback); }, 1000);
        }

        private List<AttackDefenseMultiplier> GetAtkDefMultiplier(PlayerCompleteData playerData, MarchingArmy marchingArmy)
        {
            var list = new List<AttackDefenseMultiplier>();

            float attack = 1;
//            TODO: NotImplementedException 
//            var techInfo = data.Boosts.Where(x => (byte)x.Type == TechnologyType.a)?.FirstOrDefault();
//            if (techInfo != null) attack = techInfo.Level;

            float defense = 1;
//            techInfo = data.Technologies.Where(x => x.TechnologyType == TechnologyType.ArmyDefense)?.FirstOrDefault();
//            if (techInfo != null) defense = techInfo.Level;

            StructureDetails castleBuilding = null;
            foreach (var boost in playerData.Boosts)
            {
//                var boost = playerData.Boosts.Find(x => (byte)x.Type == (byte)CityBoostType.Blessing);
                if ((boost == null) || (boost.TimeLeft <= 0)) continue;

                var specBoostData = CacheBoostDataManager.SpecNewBoostDatas.FirstOrDefault(x => (x.Type == boost.Type));
                if (specBoostData == null) continue;//.Table == 0)

                var lvl = boost.Level;
                if (lvl == 0)//get level from castle
                {
                    if (castleBuilding == null)
                    {
                        var castleData = playerData.Structures.Find(x => (x.StructureType == StructureType.CityCounsel));
                        castleBuilding = castleData?.Buildings.FirstOrDefault();
                    }
                    if (castleBuilding != null) lvl = (byte)castleBuilding.CurrentLevel;
                }

                float boostAtkValue = 0;
                float boostDefValue = 0;
                foreach (var tech in specBoostData.Techs)
                {
                    if (!tech.Levels.ContainsKey(lvl)) continue;
                    if (!float.TryParse(tech.Levels[lvl].ToString(), out float levelVal)) continue;

                    if (marchingArmy != null)
                    {
                        switch (tech.Tech)
                        {
                            case NewBoostTech.TroopAttackMultiplier: boostAtkValue += levelVal; break;
                            case NewBoostTech.TroopDefenseMultiplier: boostDefValue += levelVal; break;
                        }
                    }
                    else
                    {
                        switch (tech.Tech)
                        {
                            case NewBoostTech.CityTroopAttackMultiplier: boostAtkValue += levelVal; break;
                            case NewBoostTech.CityTroopDefenseMultiplier: boostDefValue += levelVal; break;
                        }
                    }
                }

                attack += boostAtkValue / 100f;
                defense += boostDefValue / 100f;
            }
            float atkPercentage = 0;
            float defPercentage = 0;
            var vip = playerData.VIP;
            if (vip != null)
            {
                log.Debug("------- VIP LEVEL = " + vip.Level + " timeLeft = " + vip.TimeLeft);
            }
            if ((vip != null) && (vip.TimeLeft > 0))
            {
                var vipBoostData = CacheBoostDataManager.SpecNewBoostDatas.FirstOrDefault(x => (x.Type == (NewBoostType)VIPBoostType.VIP));
                if (vipBoostData != null)
                {
                    log.Debug("------- VIP BOOST DATA FOUND");
                    var vipTech = vipBoostData.Techs.FirstOrDefault(x => (x.Tech == (NewBoostTech)VIPBoostTech.TroopAttackMultiplier));
                    if (vipTech != null)
                    {
                        atkPercentage += vipTech.GetValue(vip.Level);
                        log.Debug("------- ATK TECH FOUND val =" + atkPercentage);
                    }
                    vipTech = vipBoostData.Techs.FirstOrDefault(x => (x.Tech == (NewBoostTech)VIPBoostTech.TroopDefenseMultiplier));
                    if (vipTech != null)
                    {
                        defPercentage += vipTech.GetValue(vip.Level);
                        log.Debug("------- DEF TECH FOUND val =" + defPercentage);
                    }
                }
            }
            var multiplier = new AttackDefenseMultiplier()
            {
                AttackMultiplier = attack * (1 + (atkPercentage / 100f)),
                DefenseMultiplier = defense * (1 + (defPercentage / 100f))
            };
            list.Add(multiplier);

            if ((marchingArmy != null) && (marchingArmy.Troops != null))
            {
                var troops = marchingArmy.Troops;
                foreach (var troop in troops)
                {
                    var atkTechType = NewBoostType.Unknown;
                    var defTechType = NewBoostType.Unknown;
                    switch (troop.TroopType)
                    {
                        case TroopType.Swordsman:
                            atkTechType = (NewBoostType)TechnologyType.BarracksAttackTechnology;
                            defTechType = (NewBoostType)TechnologyType.BarracksDefenseTechnology;
                            break;
                        case TroopType.Archer:
                            atkTechType = (NewBoostType)TechnologyType.ShootingRangeAttackTechnology;
                            defTechType = (NewBoostType)TechnologyType.ShootingRangeDefenseTechnology;
                            break;
                        case TroopType.Knight:
                            atkTechType = (NewBoostType)TechnologyType.StableAttackTechnology;
                            defTechType = (NewBoostType)TechnologyType.StableDefenseTechnology;
                            break;
                        case TroopType.Slingshot:
                            atkTechType = (NewBoostType)TechnologyType.WorkshopAttackTechnology;
                            defTechType = (NewBoostType)TechnologyType.WorkshopDefenseTechnology;
                            break;
                        default: continue;
                    }

                    atkPercentage = 0;
                    if (atkTechType != NewBoostType.Unknown)
                    {
                        var tech = playerData.Boosts.Find(x => (x.Type == atkTechType));
                        if ((tech != null) && (tech.TimeLeft > 0))
                        {
                            var specBoostData = CacheBoostDataManager.SpecNewBoostDatas.FirstOrDefault(x => (x.Type == atkTechType));
                            if ((specBoostData != null) && (specBoostData.Table > 0))
                            {
                                float.TryParse(specBoostData.Levels[tech.Level].ToString(), out float levelVal);
                                atkPercentage += levelVal;
                            }
                        }
                    }
                    defPercentage = 0;
                    if (defTechType != NewBoostType.Unknown)
                    {
                        var tech = playerData.Boosts.Find(x => (x.Type == defTechType));
                        if ((tech != null) && (tech.TimeLeft > 0))
                        {
                            var specBoostData = CacheBoostDataManager.SpecNewBoostDatas.FirstOrDefault(x => (x.Type == defTechType));
                            if ((specBoostData != null) && (specBoostData.Table > 0))
                            {
                                float.TryParse(specBoostData.Levels[tech.Level].ToString(), out float levelVal);
                                defPercentage += levelVal;
                            }
                        }
                    }

                    if ((atkPercentage > 0) || (defPercentage > 0))
                    {
                        multiplier = new AttackDefenseMultiplier
                        {
                            Troop = troop,
                            AttackMultiplier = 1 + (atkPercentage / 100f),
                            DefenseMultiplier = 1 + (defPercentage / 100f)
                        };
                        list.Add(multiplier);
                    }
                }
            }

/*            if ((data.MarchingArmy != null) && (data.MarchingArmy.Heroes != null))
            {
                var heroes = data.MarchingArmy.Heroes.Select(x => CacheHeroDataManager.GetFullHeroDataID(x));
                if (heroes != null)
                {
                    foreach (var hero in heroes)
                    {
                        foreach (var boost in hero.Boosts)
                        {
                            try
                            {
                                var boostData = CacheBoostDataManager.GetFullBoostDataByBoostId(boost.BoostId);//throw exceptions
                                if ((boostData.Info.BoostType == BoostType.SpeedGathering_armyAttack) ||
                                    (boostData.Info.BoostType == BoostType.Fog_armyDefence))
                                {
                                    var boostDataValue = boostData.Values.First(x => x.BoostId == boost.BoostId);
                                    if (boostDataValue != null)
                                    {
                                        if (boostData.Info.BoostType == BoostType.SpeedGathering_armyAttack)
                                            attack += boostDataValue.Percentage;
                                        else
                                            defence += boostDataValue.Percentage;
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                }
            }*/

            return list;
        }
    }
}

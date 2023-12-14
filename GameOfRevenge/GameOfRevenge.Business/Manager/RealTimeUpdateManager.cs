using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExitGames.Logging;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Business.Manager.GameDef;
using GameOfRevenge.Business.Manager.UserData;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Email;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Boost;
using GameOfRevenge.Common.Models.Kingdom;
using GameOfRevenge.Common.Models.PlayerData;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Services;
using Newtonsoft.Json;

namespace GameOfRevenge.Business.Manager
{
    public class RealTimeUpdateManager : IRealTimeUpdateManager
    {
        public static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private static readonly List<AttackStatusData> attackInProgress = new List<AttackStatusData>();

        private readonly KingdomPvPManager pvpManager = new KingdomPvPManager();
        private readonly MonsterManager monsterManager = new MonsterManager();
        private readonly UserMailManager mailManager = new UserMailManager();

        protected readonly object SyncRoot = new object(); // that is for world user access

        public DelayedAction Disposable;

        public const int NOTIFY_TARGET = 0;
        public const int NOTIFY_ATTACKER = 1;
        public const int NOTIFY_ALL = 2;

        public List<AttackStatusData> GetAllAttackerData(int attackerId)
        {
            List<AttackStatusData> list;
            lock (SyncRoot)
            {
                list = attackInProgress.FindAll(x => (x.AttackData.AttackerId == attackerId));
            }

            return list;
        }

        public List<AttackStatusData> GetAllAttackDataForDefender(int defenderId)
        {
            List<AttackStatusData> list;
            lock (SyncRoot)
            {
                list = attackInProgress.FindAll(x => (x.AttackData.TargetId == defenderId));
            }

            return list;
        }

        public void AddNewMarchingArmy(AttackStatusData data)
        {
            if ((data.MarchingArmy.Report != null) || data.MarchingArmy.IsRecalling) data.State = 5;

            lock (SyncRoot) attackInProgress.Add(data);
        }

        public bool UpdateMarchingArmy(MarchingArmy marchingArmy)
        {
            if (marchingArmy == null) return false;

            var marchingId = marchingArmy.MarchingId;
            var updated = false;
            lock (SyncRoot)
            {
                var data = attackInProgress.Find(x => (x.MarchingArmy.MarchingId == marchingId));
                if (data != null)
                {
                    if ((marchingArmy.Report != null) || marchingArmy.IsRecalling) data.State = 5;
                    data.MarchingArmy = marchingArmy;

                    var attackData = data.AttackData;
                    if ((attackData.Recall != marchingArmy.Recall) ||
                        (attackData.AdvanceReduction != marchingArmy.AdvanceReduction) ||
                        (attackData.ReturnReduction != marchingArmy.ReturnReduction))
                    {
                        attackData.Recall = marchingArmy.Recall;
                        attackData.AdvanceReduction = marchingArmy.AdvanceReduction;
                        attackData.ReturnReduction = marchingArmy.ReturnReduction;
                        updated = true;
                    }
                }
            }

            return updated;
        }

        public MarchingArmy GetMarchingArmy(long marchingId)
        {
            MarchingArmy marchingArmy = null;
            lock (SyncRoot)
            {
                var data = attackInProgress.Find(x => (x.MarchingArmy.MarchingId == marchingId));
                if (data != null) marchingArmy = data.MarchingArmy;
            }

            return marchingArmy;
        }

        public async Task Update(Action<AttackStatusData, int> attackResultCallback)
        {
            if (attackInProgress.Count > 0)
            {
                List<AttackStatusData> list = null;
                lock (SyncRoot) list = attackInProgress.ToList();

                await UpdateProgress(list, attackResultCallback);
            }

            if (Disposable != null) Disposable.Dispose();
            Disposable = new DelayedAction();
            Disposable.WaitForCallBack(async () => { await Update(attackResultCallback); }, 1000);
        }

        async Task UpdateProgress(List<AttackStatusData> list, Action<AttackStatusData, int> attackResultCallback)
        {
            var timeleftTask = double.MaxValue;
            var timeleftTotal = double.MaxValue;
            log.Debug("****** UPDATE MARCHING START ****** x " + list.Count);
            foreach (var item in list)
            {
                log.Debug(item.State);
                try
                {
                    var marchingArmy = item.MarchingArmy;
                    switch (item.State)
                    {
                        case 0://marching to target
                            if (marchingArmy.TimeLeftForTask == 0)
                            {
                                var atkResp = await BaseUserDataManager.GetFullPlayerData(item.AttackData.AttackerId);
                                if (!atkResp.IsSuccess) throw new Exception("attacker data not found");

                                item.Attacker = atkResp.Data;
                                item.AttackData.AttackerName = item.Attacker.PlayerName;
                                item.State++;
                                if (marchingArmy.MarchingType == MarchingType.AttackGloryKingdom)
                                {
                                    await ProcessGloryKingdomBattle(item, attackResultCallback);
                                }
                            }
                            break;

                        case 1://pulling data
                            item.State++;

                            if ((marchingArmy.MarchingType == MarchingType.AttackPlayer) ||
                                (marchingArmy.MarchingType == MarchingType.ReinforcementPlayer))
                            {
                                var defResp = await BaseUserDataManager.GetFullPlayerData(item.AttackData.TargetId);
                                if (!defResp.IsSuccess)
                                {
                                    if (marchingArmy.MarchingType == MarchingType.AttackPlayer)
                                    {
                                        throw new Exception("defender data not found");
                                    }
                                    else
                                    {
                                        throw new Exception("destination data not found");
                                    }
                                }

                                item.Defender = defResp.Data;
                                item.AttackData.TargetName = item.Defender.PlayerName;
                                if (marchingArmy.MarchingType == MarchingType.ReinforcementPlayer) item.State = 10;
                            }
                            break;

                        case 2://prepare data
                            log.Debug("------------ PREPARE BATTLE SIMULATION " + item.Attacker.PlayerId + " vs " + item.AttackData.TargetId);

                            var attackerPower = new BattlePower(item.Attacker, marchingArmy, CacheTroopDataManager.GetFullTroopData, GetAtkDefMultiplier, (msg) => log.Debug(msg));
                            item.AttackerPower = attackerPower;
                            BattlePower defenderPower;
                            if (marchingArmy.MarchingType == MarchingType.AttackPlayer)
                            {
                                defenderPower = new BattlePower(item.Defender, null, CacheTroopDataManager.GetFullTroopData, GetAtkDefMultiplier, (msg) => log.Debug(msg));
                            }
                            else if (marchingArmy.MarchingType == MarchingType.AttackMonster)
                            {
                                var resp = await monsterManager.GetFullMonsterData(item.AttackData.TargetId);
                                if (!resp.IsSuccess) throw new Exception("Monster data not found");

                                defenderPower = new BattlePower(resp.Data);
                            }
                            else//attack glory kingdom
                            {
                                defenderPower = new BattlePower(item.Defender, null, CacheTroopDataManager.GetFullTroopData, null);
                            }
                            item.DefenderPower = defenderPower;

                            item.Replay = new BattleReplay();

                            log.Debug("atk hp= " + attackerPower.HitPoints + " vs def hp=" + defenderPower.HitPoints);
                            item.State++;
                            break;

                        case 3://battle simulation
                            {
                                if (marchingArmy.MarchingType == MarchingType.AttackPlayer)
                                {
                                    item.AttackerPower.AttackPlayer(item.DefenderPower, item.Replay, (str) => log.Info(str));
                                }
                                else
                                {
                                    item.AttackerPower.AttackMonster(item.DefenderPower, item.Replay);
                                }
                                log.Debug("atk pwr= " + item.AttackerPower.HitPoints + " xx def pwr=" + item.DefenderPower.HitPoints);
                            }
                            {
                                await pvpManager.FinishBattleData(item.Attacker, item.AttackerPower,
                                                                item.Defender, item.DefenderPower, marchingArmy, item.Replay);

                                log.Debug("send attacker attack end event");
                                attackResultCallback(item, NOTIFY_ATTACKER);

                                // Send Mail to Attacker
                                /*try
                                {
                                    var respMail = await mailManager.SendMail(item.Attacker.PlayerId, MailType.BattleReport, JsonConvert.SerializeObject(marchingArmy.Report));
                                    if (!respMail.IsSuccess)
                                    {
                                        log.Debug(respMail.Message);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    log.Debug(ex.Message);
                                }*/

                                item.State++;
                            }
                            break;

                        case 4://waiting for return
                            if (marchingArmy.IsTimeForReturn)
                            {
                                if (marchingArmy.MarchingType == MarchingType.AttackPlayer)
                                {
                                    log.Debug("send defender under attack end event");
                                    attackResultCallback(item, NOTIFY_TARGET);
                                }

                                item.State++;
                            }
                            break;

                        case 5://marching to castle
                            if (marchingArmy.TimeLeft == 0)
                            {
                                if (!marchingArmy.IsRecalling)
                                {
                                    //SAVE PLAYER REPORT
                                    log.Debug("ApplyAttackerChangesAndSendReport!!!");
                                    await pvpManager.ApplyAttackerChangesAndSendReport(marchingArmy);
                                }

                                attackResultCallback(item, NOTIFY_ATTACKER);

                                log.Debug("** END ** " + item.AttackData.AttackerId + " vs " + item.AttackData.TargetId);
                                lock (SyncRoot) attackInProgress.Remove(item);
                            }
                            break;

                        case 10://process reinforcement
                            var success = await pvpManager.ApplyReinforcementChanges(marchingArmy, item.Attacker, item.Defender);

                            log.Debug("send reinforcement end event");
                            attackResultCallback(item, NOTIFY_ALL);

                            log.Debug("** END ** " + item.AttackData.AttackerId + " reinforcement " + item.AttackData.TargetId);
                            lock (SyncRoot) attackInProgress.Remove(item);
                            break;
                    }
                    if (marchingArmy.TimeLeftForTask < timeleftTask) timeleftTask = item.MarchingArmy.TimeLeftForTask;
                    if (marchingArmy.TimeLeft < timeleftTotal) timeleftTotal = item.MarchingArmy.TimeLeft;
                }
                catch (Exception ex)
                {
                    log.Debug("EXCEPTION " + item.State+"  "+ item.AttackData.AttackerId+" vs "+ item.AttackData.TargetId+"  " +ex.Message);
                    lock (SyncRoot) attackInProgress.Remove(item);
                }
            }
            var timeleftTxt = "";
            var timeleftTotalTxt = "";
            if (timeleftTask != double.MaxValue)
            {
                timeleftTxt = (int)timeleftTask + "(" + Common.Helper.TimeHelper.ChangeSecondsToTimeFormat(timeleftTask) + ")";
            }
            if (timeleftTotal != double.MaxValue)
            {
                timeleftTotalTxt = (int)timeleftTotal + "(" + Common.Helper.TimeHelper.ChangeSecondsToTimeFormat(timeleftTotal) + ")";
            }
            log.Debug("****** UPDATE BATTLE END ****** " + timeleftTxt + " / " + timeleftTotalTxt);
            list.Clear();
            list = null;
        }

        async Task ProcessGloryKingdomBattle(AttackStatusData data, Action<AttackStatusData, int> attackResultCallback)
        {
            var fortressResp = await pvpManager.kingdomManager.GetZoneFortressById(data.AttackData.TargetId);
            if (!fortressResp.IsSuccess || !fortressResp.HasData) throw new Exception("target data not found");

            data.AttackerPower = new BattlePower(data.Attacker, data.MarchingArmy, CacheTroopDataManager.GetFullTroopData, GetAtkDefMultiplier, (ss) => { log.Debug(ss); });

            var fortress = fortressResp.Data;
            if (fortress.ClanId == data.Attacker.ClanId)
            {
                var newPlayerTroop = new PlayerTroops(data.Attacker.PlayerId, data.MarchingArmy.Troops);

                var existingPlayerTroop = fortress.PlayerTroops.Find(x => x.PlayerId == newPlayerTroop.PlayerId);

                if (existingPlayerTroop != null)
                {
                    foreach (var newTroopInfo in newPlayerTroop.Troops)
                    {
                        TroopInfos existingTroopInfo = existingPlayerTroop.Troops.Find(t => t.TroopType == newTroopInfo.TroopType);

                        if (existingTroopInfo != null)
                        {
                            foreach (var newTroopDetail in newTroopInfo.TroopData)
                            {
                                TroopDetails existingTroopDetail = existingTroopInfo.TroopData.Find(d => d.Level == newTroopDetail.Level);

                                if (existingTroopDetail != null)
                                {
                                    existingTroopDetail.Count += newTroopDetail.Count;
                                }
                                else
                                {
                                    existingTroopInfo.TroopData.Add(newTroopDetail);
                                }
                            }
                        }
                        else
                        {
                            existingPlayerTroop.Troops.Add(newTroopInfo);
                        }
                    }
                }
                else
                {
                    fortress.PlayerTroops.Add(newPlayerTroop);
                }

                var defData = new PlayerCompleteData()
                {
                    PlayerId = fortress.ZoneFortressId,
                    Troops = fortress.GetAllTroops()
                };
                data.DefenderPower = new BattlePower(defData, null, CacheTroopDataManager.GetFullTroopData, null, (ss) => { log.Debug(ss); });
                data.DefenderPower.EntityType = EntityType.Fortress;

                var fortressData = new ZoneFortressData()
                {
                    FirstCapturedTime = fortress.FirstCapturedTime,
                    StartTime = fortress.StartTime,
                    Duration = fortress.Duration,
                    PlayerTroops = fortress.PlayerTroops
                };
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(fortressData);
                var updateResp = await pvpManager.kingdomManager.UpdateZoneFortress(fortress.ZoneFortressId,
                        hitPoints: data.DefenderPower.HitPoints, attack: data.DefenderPower.AttackCalc,
                        defense: data.DefenderPower.DefenseCalc, data: json);



                var troopChanges = data.AttackerPower.TroopChanges;
                data.MarchingArmy.TroopChanges = troopChanges;

                foreach (var troopChange in troopChanges)
                {
                    troopChange.Dead = troopChange.InitialCount;
                    troopChange.Wounded = 0;
                }
                var playerTroops = new List<PlayerTroops>()
                {
                    new PlayerTroops(data.Attacker.PlayerId, data.Attacker.Troops)
                };
                await pvpManager.ApplyTroopChanges(playerTroops, troopChanges, true);
            }
            else
            {
                data.Defender = new PlayerCompleteData()
                {
                    PlayerId = fortress.ZoneFortressId,
                    Troops = fortress.GetAllTroops()
                };
                data.DefenderPower = new BattlePower(data.Defender, null, CacheTroopDataManager.GetFullTroopData, null,(ss)=> { log.Debug(ss); });
                data.DefenderPower.EntityType = EntityType.Fortress;

                log.Debug("battle: atk pwr= " + data.AttackerPower.HitPoints + " xx def pwr=" + data.DefenderPower.HitPoints);
                log.Debug("attacker troops: " + Newtonsoft.Json.JsonConvert.SerializeObject(data.AttackerPower.TroopChanges));
                log.Debug("defender troops: " + Newtonsoft.Json.JsonConvert.SerializeObject(data.DefenderPower.TroopChanges));
                while ((data.DefenderPower.HitPoints > 0) && (data.AttackerPower.HitPoints > 0))
                {
                    data.AttackerPower.AttackPlayer(data.DefenderPower, data.Replay, (ss)=> { log.Debug(ss); });
                    log.Debug("atk pwr= " + data.AttackerPower.HitPoints + "  "+data.AttackerPower.AttackCalc+"/"+data.AttackerPower.DefenseCalc +
                                "   xx def pwr=" + data.DefenderPower.HitPoints+" "+data.DefenderPower.AttackCalc+"/"+data.DefenderPower.DefenseCalc);
                }
                log.Debug("result: atk pwr= " + data.AttackerPower.HitPoints + " xx def pwr=" + data.DefenderPower.HitPoints);

                await pvpManager.FinishBattleData(data.Attacker, data.AttackerPower,
                                                data.Defender, data.DefenderPower, data.MarchingArmy, data.Replay);

                //SAVE PLAYER REPORT
                log.Debug("ApplyAttackerChangesAndSendReport!!!");

                foreach (var troopChange in data.MarchingArmy.TroopChanges)
                {
                    troopChange.Dead = troopChange.InitialCount;
                    troopChange.Wounded = 0;
                }
                await pvpManager.ApplyAttackerChangesAndSendReport(data.MarchingArmy);
            }

            log.Debug("attack glory kingdom end event");
            attackResultCallback(data, NOTIFY_ATTACKER);

            log.Debug("** END ** " + data.AttackData.AttackerId + " glory kingdom " + data.AttackData.TargetId);
            lock (SyncRoot) attackInProgress.Remove(data);
        }

        public static List<AttackDefenseMultiplier> GetAtkDefMultiplier(PlayerCompleteData playerData, MarchingArmy marchingArmy)
        {
            var list = new List<AttackDefenseMultiplier>();

            float attack = 1;
//            TODO: NotImplementedException 
//            var techInfo = data.Boosts.Where(x => (byte)x.Type == TechnologyType.a)?.FirstOrDefault();
//            if (techInfo != null) attack = techInfo.Level;

            float defense = 1;
//            techInfo = data.Technologies.Where(x => x.TechnologyType == TechnologyType.ArmyDefense)?.FirstOrDefault();
//            if (techInfo != null) defense = techInfo.Level;
            if (playerData.Boosts != null)
            {
                foreach (var boost in playerData.Boosts)
                {
                    //                var boost = playerData.Boosts.Find(x => (byte)x.Type == (byte)CityBoostType.Blessing);
                    if ((boost == null) || (boost.TimeLeft <= 0)) continue;

                    var specBoostData = CacheBoostDataManager.SpecNewBoostDatas.FirstOrDefault(x => (x.Type == boost.Type));
                    if (specBoostData == null) continue;//.Table == 0)

                    var lvl = boost.Level;
                    if (lvl == 0)//get level from castle
                    {
                        var castleData = playerData.Structures.Find(x => (x.StructureType == StructureType.CityCounsel));
                        var castleBuilding = castleData?.Buildings.FirstOrDefault();
                        if (castleBuilding != null) lvl = (byte)castleBuilding.CurrentLevel;
                    }

                    float boostAtkValue = 0;
                    float boostDefValue = 0;
                    foreach (var tech in specBoostData.Techs)
                    {
                        if (marchingArmy != null)
                        {
                            switch (tech.Tech)
                            {
                                case NewBoostTech.TroopAttackMultiplier: boostAtkValue += tech.GetValue(lvl); break;
                                case NewBoostTech.TroopDefenseMultiplier: boostDefValue += tech.GetValue(lvl); break;
                            }
                        }
                        else
                        {
                            switch (tech.Tech)
                            {
                                case NewBoostTech.CityTroopAttackMultiplier: boostAtkValue += tech.GetValue(lvl); break;
                                case NewBoostTech.CityTroopDefenseMultiplier: boostDefValue += tech.GetValue(lvl); break;
                            }
                        }
                    }

                    attack += boostAtkValue / 100f;
                    defense += boostDefValue / 100f;
                }
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
                Troop = null, //<--- global multiplier
                AttackMultiplier = attack * (1 + (atkPercentage / 100f)),
                DefenseMultiplier = defense * (1 + (defPercentage / 100f))
            };
            list.Add(multiplier);

            if ((playerData.Boosts != null) && (marchingArmy != null) && (marchingArmy.Troops != null))
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

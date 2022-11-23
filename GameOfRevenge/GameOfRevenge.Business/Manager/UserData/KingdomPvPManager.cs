﻿using ExitGames.Logging;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Email;
using GameOfRevenge.Common.Helper;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Boost;
using GameOfRevenge.Common.Models.Inventory;
using GameOfRevenge.Common.Models.Kingdom;
using GameOfRevenge.Common.Models.Kingdom.AttackAlertReport;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static GameOfRevenge.Common.Models.Kingdom.AttackAlertReport.UnderAttackReport;

namespace GameOfRevenge.Business.Manager.UserData
{
    public class BattlePower : ClientBattleReport
    {
        public int HitPoint { get; set; }
//        public float Attack { get; set; }
//        public float Defense { get; set; }

        public float AttackMultiplier { get; set; }
        public float DefenseMultiplier { get; set; }
        public int GateHp { get; set; }

        public new List<int> Heroes { get; set; }
        public List<TroopInfos> Army { get; set; }
        public List<TroopDetailsPvP> TroopsAlive { get; set; }

        public int TotalLoad { get; set; }

        public void Recalculate()
        {
            if ((TroopsAlive == null) || (TroopsAlive.Count == 0)) return;

            float points = 0;
            float attack = 0;
            float defense = 0;

            foreach (var troops in TroopsAlive)
            {
                if (troops.Hp <= 0) continue;

                points += troops.Data.Health * troops.RemainUnits;
                attack += troops.Data.AttackDamage * troops.RemainUnits;
                defense += troops.Data.Defense * troops.RemainUnits;
            }

            HitPoint = (int)points;
            Attack = (int)(attack * (1 + AttackMultiplier));
            Defense = (int)(defense * (1 + DefenseMultiplier));
        }
    }

    public class KingdomPvPManager : BaseUserDataManager, IKingdomPvPManager
    {
        public static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly IUserTroopManager userTroopManager = new UserTroopManager();
        private readonly IUserMailManager mailManager = new UserMailManager();
        private readonly IAccountManager accManager = new AccountManager();
        private readonly IUserResourceManager resManager = new UserResourceManager();
        private readonly IUserStructureManager structManager = new UserStructureManager();

        public Task<Response<Player>> GetAccountInfo(int playerId) => accManager.GetAccountInfo(playerId);

/*        public async Task<Response<AttackStatusData>> AttackOtherPlayer(int attackerId, int defenderId, int watchLevel, MarchingArmy army, MapLocation location)
        {
            try
            {
                ValidationHelper.KeyId(defenderId);

                if ((army == null) || (army.Troops == null) || (army.Troops.Count == 0))
                {
                    throw new RequirementExecption("No army was sended to attack");
                }

                var defender = await GetFullPlayerData(defenderId);
                if (!defender.IsSuccess || !defender.HasData) throw new DataNotExistExecption(defender.Message);

                return await AttackOtherPlayer(attackerId, defender, watchLevel, army, location);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<AttackStatusData>()
                {
                    Case = 200,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<AttackStatusData>()
                {
                    Case = 201,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (RequirementExecption ex)
            {
                return new Response<AttackStatusData>()
                {
                    Case = 202,
                    Message = ErrorManager.ShowError(ex)
                };
            }
            catch (Exception)
            {
                return new Response<AttackStatusData>()
                {
                    Case = 0,
                    Message = ErrorManager.ShowError()
                };
            }
        }*/

/*        public async Task<Response<AttackStatusData>> AttackOtherPlayer(int attackerId, Response<PlayerCompleteData> defender, int watchLevel, MarchingArmy army, MapLocation location)
        {
            try
            {
                ValidationHelper.KeyId(attackerId);

                if ((army == null) || (army.Troops == null) || (army.Troops.Count == 0))
                {
                    throw new RequirementExecption("Zero marching army was sended to attack");
                }

                var attacker = await GetFullPlayerData(attackerId);
                if (!attacker.IsSuccess || !attacker.HasData) throw new DataNotExistExecption(attacker.Message);

                var boostShieldData = defender.Data.Boosts.Where(x => x.Type == NewBoostType.Fog).FirstOrDefault();
                if ((boostShieldData != null) && (boostShieldData.TimeLeft > 0))
                {
                    throw new RequirementExecption("Defender has shield activated");
                }

                if ((attacker.Data.Troops == null) || (attacker.Data.Troops.Count == 0))
                {
                    throw new RequirementExecption("User does not have any army");
                }

                if (attacker.Data.MarchingArmy != null)
                {
                    if (attacker.Data.MarchingArmy.TimeLeft > 0)
                    {
                        throw new RequirementExecption("Maximum one army marching is allowed");
                    }
                    //else throw new RequirementExecption("Socket server loop can only execute battle simulator");
                }

                if (!ArmyToMarching(attacker.Data, army, true))
                {
                    throw new RequirementExecption("User does not have required army");
                }
                //foreach (var item in attacker.Data.Troops)
                //await userTroopManager.UpdateTroops(attackerId, item.TroopType, item.TroopData);


                var response = await manager.AddOrUpdatePlayerData(attackerId, DataType.Marching, 1, JsonConvert.SerializeObject(army));
                if (response.IsSuccess)
                {
                    int defenderId = defender.Data.PlayerId;
                    var report = await SaveAlertMail(watchLevel, attacker, defenderId, location);
                    var armyResponse = new AttackStatusData()
                    {
                        Attacker = attacker.Data,
                        Defender = defender.Data,
                        Report = report.Data
                    };

                    return new Response<AttackStatusData>(armyResponse, response.Case, "Marching towards enemy city");
                }
                else
                {
                    throw new Exception(response.Message);
                }
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<AttackStatusData>() { Case = 200, Message = ErrorManager.ShowError(ex) };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<AttackStatusData>() { Case = 201, Message = ErrorManager.ShowError(ex) };
            }
            catch (RequirementExecption ex)
            {
                return new Response<AttackStatusData>() { Case = 202, Message = ErrorManager.ShowError(ex) };
            }
            catch (Exception)
            {
                return new Response<AttackStatusData>() { Case = 0, Message = ErrorManager.ShowError() };
            }
        }*/

        public async Task<Response<AttackStatusData>> AttackOtherPlayer(int attackerId, PlayerCompleteData attackerData, int defenderId, List<UserNewBoost> defenderBoosts, int watchLevel, MarchingArmy army, MapLocation location)
        {
            try
            {
                ValidationHelper.KeyId(attackerId);

                if ((army == null) || (army.Troops == null) || (army.Troops.Count == 0))
                {
                    throw new RequirementExecption("Zero marching army was sended to attack");
                }

//                var attacker = await GetFullPlayerData(attackerId);
//                if (!attacker.IsSuccess || !attacker.HasData) throw new DataNotExistExecption(attacker.Message);

                var boostFogData = defenderBoosts.Find(x => x.Type == NewBoostType.Shield);
                if (boostFogData?.TimeLeft > 0)
                {
                    throw new RequirementExecption("Defender has shield activated");
                }

                if ((attackerData.Troops == null) || (attackerData.Troops.Count == 0))
                {
                    throw new RequirementExecption("User does not have any army");
                }

                if (attackerData.MarchingArmy != null)
                {
                    if (attackerData.MarchingArmy.TimeLeft > 0)
                    {
                        throw new RequirementExecption("Maximum one army marching is allowed");
                    }
                    //else throw new RequirementExecption("Socket server loop can only execute battle simulator");
                }

                if (!UpdatePlayerArmyToMarch(attackerData, army, true))
                {
                    throw new RequirementExecption("User does not have required army");
                }
                //foreach (var item in attacker.Data.Troops)
                //await userTroopManager.UpdateTroops(attackerId, item.TroopType, item.TroopData);


                var response = await manager.AddOrUpdatePlayerData(attackerId, DataType.Marching, 1, JsonConvert.SerializeObject(army));
                if (response.IsSuccess)
                {
//                    int defenderId = defender.Data.PlayerId;
                    var report = await SaveAlertMail(watchLevel, attackerData, defenderId, location);

                    report.Data.StartTime = attackerData.MarchingArmy.StartTime;
                    report.Data.ReachedTime = attackerData.MarchingArmy.ReachedTime;

                    var armyResponse = new AttackStatusData()
                    {
                        Attacker = attackerData,
//                        Defender = defender.Data,
                        Report = report.Data
                    };

                    return new Response<AttackStatusData>(armyResponse, response.Case, "Marching towards enemy city");
                }
                else
                {
                    throw new Exception(response.Message);
                }
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<AttackStatusData>() { Case = 200, Message = ErrorManager.ShowError(ex) };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<AttackStatusData>() { Case = 201, Message = ErrorManager.ShowError(ex) };
            }
            catch (RequirementExecption ex)
            {
                return new Response<AttackStatusData>() { Case = 202, Message = ErrorManager.ShowError(ex) };
            }
            catch (Exception)
            {
                return new Response<AttackStatusData>() { Case = 0, Message = ErrorManager.ShowError() };
            }
        }

        private async Task<Response<UnderAttackReport>> SaveAlertMail(int watchLevel, PlayerCompleteData attackerData, int defenderId, MapLocation loc)
        {
            var attackerMailInfo = new UnderAttackReport()
            {
                AttackerId = attackerData.PlayerId,
                DefenderId = defenderId
            };

//            try
            {
                GenerateAlertMailFields(attackerData, loc, watchLevel, attackerMailInfo);
            }
//            catch { }

            try
            {
                await mailManager.SaveMail(defenderId, MailType.UnderAttack, JsonConvert.SerializeObject(attackerMailInfo));
            }
            catch (Exception ex)
            {
                return new Response<UnderAttackReport>(CaseType.Error, ErrorManager.ShowError(ex));
            }

            return new Response<UnderAttackReport>(attackerMailInfo, CaseType.Success, "Success");
        }

        private static void GenerateAlertMailFields(PlayerCompleteData attackerData, MapLocation location, int watchTowerLevel, UnderAttackReport attackerMailInfo)
        {
            //Reveals the incoming troops Player name.
            if (watchTowerLevel > 0)
            {
//                var attackerInfo = await new AccountManager().GetAccountInfo(attackerId);
//                if ((attackerInfo != null) && attackerInfo.IsSuccess && attackerInfo.HasData)
//                {
                    attackerMailInfo.AttackerUsername = attackerData.PlayerName;
//                }
            }

            //Reveals the exact location of the incoming troops origin.
            if (watchTowerLevel >= 3) attackerMailInfo.Location = location;

            //Reveals the incoming troops estimated time of arrival.
            if (watchTowerLevel >= 7)
            {
                attackerMailInfo.StartTime = attackerData.MarchingArmy.StartTime;
                attackerMailInfo.ReachedTime = attackerData.MarchingArmy.ReachedTime;
            }
//            if (watchTowerLevel >= 7) attackerMailInfo.TimeTaken = attacker.Data.MarchingArmy.TimeLeft;

            var troops = new List<TroopData>();
            //Reveals the total size of incoming troops.
            if (watchTowerLevel >= 11)
            {
                var armySize = 0;
                foreach (var troop in attackerData.MarchingArmy.Troops)
                {
                    if (troop == null) continue;

                    foreach (var item in troop.TroopData)
                    {
                        if (item == null) continue;

                        armySize += item.FinalCount;

                        troops.Add(new TroopData()
                        {
                            Type = troop.TroopType.ToString(),
                            Level = item.Level,
                            Name = $"{troop.TroopType} Lvl.{item.Level}",
                            Count = item.FinalCount
                        }); ;
                    }
                }

                attackerMailInfo.TotalTroopSize = armySize;
            }

            //Reveals the exact king level of the incoming troops.
            if (watchTowerLevel >= 17) attackerMailInfo.KingLevel = attackerData.King.Level;

            //Reveals the soldier types of the incoming troops.
            if (watchTowerLevel >= 19) attackerMailInfo.Troops = troops;

            //"Reveals the number of each soldier type from the incoming troops."
            if (watchTowerLevel < 23) foreach (var item in troops) item.Count = 0; //hide amounts

            attackerMailInfo.TotalHeroSize = 0;
            attackerMailInfo.Heroes = null;
            if ((attackerData.MarchingArmy.Heroes != null) && (attackerData.MarchingArmy.Heroes.Count > 0) &&
                (watchTowerLevel >= 25))
            {
                //Displays the amount of heroes in the dispatch.
                attackerMailInfo.TotalHeroSize = attackerData.MarchingArmy.Heroes.Count;

                //Displays the type of Heroes in the dispatch.
                if (watchTowerLevel >= 30)
                {
                    var heroNames = new List<TroopData>();
                    foreach (var item in attackerData.MarchingArmy.Heroes)
                    {
                        var heroData = CacheHeroDataManager.GetFullHeroDataID(item);
                        if (heroData != null)
                        {
                            heroNames.Add(new TroopData() { Name = heroData.Info.Name });
                        }
                    }
                    attackerMailInfo.Heroes = heroNames;
                }
            }
        }

        public async Task<Response<BattleReport>> BattleSimulation(PlayerCompleteData attackerArmy, PlayerCompleteData defenderArmy)
        {
            log.Debug("------------BATTLE SIMULATION "+attackerArmy.PlayerId+" vs "+ defenderArmy.PlayerId);
//            DateTime timestart = DateTime.UtcNow;
            try
            {
/*                ValidationHelper.KeyId(attackerId);
                ValidationHelper.KeyId(defenderId);

                //TODO: Improve request, use only one request instead of two requests
                var p2 = await accManager.GetAccountInfo(defenderId);
                if (!p2.IsSuccess || !p2.HasData) throw new DataNotExistExecption(p2.Message);

                var p1 = await accManager.GetAccountInfo(attackerId);
                if (!p1.IsSuccess || !p1.HasData) throw new DataNotExistExecption(p1.Message);*/

/*                if (attackerArmy == null || attackerArmy.MarchingArmy == null || attackerArmy.MarchingArmy.Troops == null)
                {
                    var user1Resp = await GetFullPlayerData(attackerId);//TODO: check this request, maybe all player data is not necessary
                    if (!user1Resp.IsSuccess || !user1Resp.HasData) throw new DataNotExistExecption(user1Resp.Message);
                    attackerArmy = user1Resp.Data;
                }
                var (attackerMultiplier_attack, attackerMultiplier_defense) = GetAtkDefMultiplier(attackerArmy);

                if ((defenderArmy == null) || (defenderArmy.Troops == null))
                {
                    var user2Resp = await GetFullPlayerData(defenderId);
                    if (!user2Resp.IsSuccess || !user2Resp.HasData) throw new DataNotExistExecption(user2Resp.Message);
                    defenderArmy = user2Resp.Data;
                }*/
//PULL current defender status
                var (attackerMultiplier_attack, attackerMultiplier_defense) = GetAtkDefMultiplier(attackerArmy);
                var attackerPower = new BattlePower();
                attackerPower.PlayerId = attackerArmy.PlayerId;
                attackerPower.Username = attackerArmy.PlayerName;
                attackerPower.Army = GetAvailableTroops(attackerArmy.MarchingArmy.Troops);
                attackerPower.AttackMultiplier = attackerMultiplier_attack;
                attackerPower.DefenseMultiplier = attackerMultiplier_defense;
                if ((attackerArmy.MarchingArmy.Heroes != null) && (attackerArmy.MarchingArmy.Heroes.Count > 0))
                {
                    attackerPower.Heroes = attackerArmy.MarchingArmy.Heroes;
                }

                var gateHitPoints = 0;
                var defenderGate = defenderArmy.Structures.Where(x => x.StructureType == StructureType.Gate)?.FirstOrDefault()?.Buildings.OrderBy(x => x.Level).FirstOrDefault();
                if (defenderGate != null) gateHitPoints = defenderGate.HitPoints;
//                var gateLevelData = CacheStructureDataManager.GetFullStructureData(StructureType.Gate).Levels.Where(x => x.Data.Level == defenderGate.Level).FirstOrDefault().Data;
//                gateHitPoints = gateLevelData.HitPoint;

                var (defenderMultiplier_attack, defenderMultiplier_defense) = GetAtkDefMultiplier(defenderArmy);
                var defenderPower = new BattlePower()
                {
                    PlayerId = defenderArmy.PlayerId,
                    Username = defenderArmy.PlayerName,
                    Army = GetAvailableTroops(defenderArmy.Troops),
                    AttackMultiplier = defenderMultiplier_attack,
                    DefenseMultiplier = defenderMultiplier_defense,
                    GateHp = gateHitPoints
                };

                SetTroopsAlive(attackerPower);
                SetTroopsAlive(defenderPower);
                attackerPower.Recalculate();
                defenderPower.Recalculate();

                while ((defenderPower.HitPoint > 0) && (attackerPower.HitPoint > 0))
                {
                    Attack(attackerPower, defenderPower);
                    log.Debug("atk pwr= " + attackerPower.HitPoint + "  def pwr=" + defenderPower.HitPoint);
                }

                //TODO: implement percentage based on level (maybe we need to add level to item)
                var atkHealingBoost = attackerArmy.Boosts.Exists(x => (x.Type == NewBoostType.LifeSaver) && (x.TimeLeft > 0));
                var defHealingBoost = defenderArmy.Boosts.Exists(x => (x.Type == NewBoostType.LifeSaver) && (x.TimeLeft > 0));

                CalculateTroopLoss(attackerPower, atkHealingBoost);
                CalculateTroopLoss(defenderPower, defHealingBoost);

                bool attackerWin = attackerPower.HitPoint > defenderPower.HitPoint;

                if (attackerArmy.MarchingArmy.Heroes != null)
                {
                    foreach (var item in attackerArmy.MarchingArmy.Heroes)
                    {
                        var heroData = CacheHeroDataManager.GetFullHeroDataID(item);
                        if (heroData == null) continue;

                        var hero = attackerArmy.Heroes.Find(x => x.HeroCode == heroData.Info.Code);
                        hero.Points++;
                        hero.AttackCount++;
                        if (!attackerWin) hero.AttackFail++;

                        int valueId = CacheHeroDataManager.GetFullHeroData(hero.HeroCode).Info.HeroId;
                        var data = JsonConvert.SerializeObject(hero);
                        var response = await manager.AddOrUpdatePlayerData(attackerArmy.PlayerId, DataType.Hero, valueId, data);
                    }
                }
                if (defenderArmy.Heroes != null)
                {
                    foreach (var hero in defenderArmy.Heroes)
                    {
                        if (hero.IsMarching) continue;

                        hero.DefenseCount++;
                        if (attackerWin) hero.DefenseFail++;

                        int valueId = CacheHeroDataManager.GetFullHeroData(hero.HeroCode).Info.HeroId;
                        var data = JsonConvert.SerializeObject(hero);
                        var response = await manager.AddOrUpdatePlayerData(defenderArmy.PlayerId, DataType.Hero, valueId, data);
                    }
                }

                var king = attackerArmy.King;
                king.Experience += 5;
                if (king.TimeLeft <= 0) king.StartTime = DateTime.UtcNow;
                king.Duration += 10 * 60;
/*                if (king.TimeLeft > 0)
                {
                    king.EndTime = king.EndTime.AddMinutes(10);
                }
                else
                {
                    king.StartTime = DateTime.UtcNow;
                    king.EndTime = king.StartTime.AddMinutes(10);
                }*/
                var kingdata = JsonConvert.SerializeObject(king);
                await manager.AddOrUpdatePlayerData(attackerArmy.PlayerId, DataType.Custom, 1, kingdata);


                var finalReport = new BattleReport
                {
                    Attacker = SetClientReport(attackerPower),
                    Defender = SetClientReport(defenderPower),
                    AttackerWon = attackerWin
                };

                await RemoveTroops(attackerArmy, attackerPower);
                await RemoveTroops(defenderArmy, defenderPower);

                if (attackerWin) await GiveLoot(defenderArmy, attackerPower, finalReport);

                await manager.AddOrUpdatePlayerData(attackerArmy.PlayerId, DataType.Marching, 1, string.Empty);
                await structManager.UpdateGate(defenderArmy.PlayerId, defenderPower.GateHp);

                string json = JsonConvert.SerializeObject(finalReport);
                _ = Task.Run(async () =>
                {
                    if (attackerWin)
                    {
                        try
                        {
                            await mailManager.SaveMail(attackerArmy.PlayerId, MailType.BattleReport,
                                                        json.Replace("INSERTMESSAGEHERE", "You succesfully raided enemy city"));
                        }
                        catch { }

                        try
                        {
                            await mailManager.SaveMail(defenderArmy.PlayerId, MailType.BattleReport,
                                                        json.Replace("INSERTMESSAGEHERE", "You failed to defend against enemy raid"));
                        }
                        catch { }
                    }
                    else
                    {
                        try
                        {
                            await mailManager.SaveMail(attackerArmy.PlayerId, MailType.BattleReport,
                                                        json.Replace("INSERTMESSAGEHERE", "You failed to raid the enemy city"));
                        }
                        catch { }

                        try
                        {
                            await mailManager.SaveMail(defenderArmy.PlayerId, MailType.BattleReport,
                                                        json.Replace("INSERTMESSAGEHERE", "You succesfully defended against enemy seige"));
                        }
                        catch { }
                    }
                });

                return new Response<BattleReport>(finalReport, 100, json.Replace("INSERTMESSAGEHERE", "Attacked succesfully"));
            }
            catch (InvalidModelExecption ex)
            {
                log.Debug("------------EXCEPTION4 " + ex.Message);
                return new Response<BattleReport>()
                {
                    Case = 200,
                    Message = ex.Message + ex.StackTrace// ErrorManager.ShowError(ex)
                };
            }
            catch (DataNotExistExecption ex)
            {
                log.Debug("------------EXCEPTION3 " + ex.Message);
                return new Response<BattleReport>()
                {
                    Case = 201,
                    Message = ex.Message + ex.StackTrace//rrorManager.ShowError(ex)
                };
            }
            catch (RequirementExecption ex)
            {
                log.Debug("------------EXCEPTION2 " + ex.Message);
                return new Response<BattleReport>()
                {
                    Case = 202,
                    Message = ex.Message + ex.StackTrace//ErrorManager.ShowError(ex)
                };
            }
            catch (Exception ex)
            {
                log.Debug("------------EXCEPTION " + ex.Message);
                return new Response<BattleReport>()
                {
                    Case = 0,
                    Message = ex.Message + ex.StackTrace//ErrorManager.ShowError(ex)
                };
            }
        }

        private static (float, float) GetAtkDefMultiplier(PlayerCompleteData data)
        {
            float attack = 0;
//            TODO: NotImplementedException 
//            var techInfo = data.Boosts.Where(x => (byte)x.Type == TechnologyType.a)?.FirstOrDefault();
//            if (techInfo != null) attack = techInfo.Level;

            float defense = 0;
//            techInfo = data.Technologies.Where(x => x.TechnologyType == TechnologyType.ArmyDefense)?.FirstOrDefault();
//            if (techInfo != null) defense = techInfo.Level;



            float boostValue = 0;
            var boost = data.Boosts.Find(x => (byte)x.Type == (byte)CityBoostType.Blessing);
            if ((boost != null) && (boost.TimeLeft > 0))
            {
                var specBoostData = CacheBoostDataManager.SpecNewBoostDatas.First(x => x.Type == boost.Type);
                if (specBoostData.Table > 0)
                {
                    float.TryParse(specBoostData.Levels[boost.Level].ToString(), out float levelVal);
                    boostValue = levelVal;
                }
            }
            attack += boostValue / 100f;
            defense += boostValue / 100f;

            if ((data.MarchingArmy != null) && (data.MarchingArmy.Troops != null))
            {
                var troops = data.MarchingArmy.Troops;
                foreach (var troop in troops)
                {
                    switch (troop.TroopType)
                    {
                        case TroopType.Swordman:
                            var tech = data.Boosts.Find(x => (byte)x.Type == (byte)TechnologyType.BarracksAttackTechnology);
                            if ((tech != null) && (!tech.HasDuration || (tech.TimeLeft > 0)))
                            {
//                                tech.Level
                            }
                            break;
                        case TroopType.Archer:
                            break;
                        case TroopType.Knight:
                            break;
                        case TroopType.Slingshot:
                            break;
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

            return (attack, defense);
        }

        private async Task RemoveTroops(PlayerCompleteData army, BattlePower power)
        {
            if (power.TroopsAlive == null) return;

            foreach (var data in army.Troops)
            {
                if (data == null) continue;

                bool hasChange = false;
                foreach (var aliveTroop in power.TroopsAlive)
                {
                    if ((aliveTroop == null) || (aliveTroop.Type != data.TroopType)) continue;

                    int wounded = aliveTroop.InitialCount - (int)Math.Ceiling(aliveTroop.RemainUnits);
                    if (wounded <= 0) continue;

                    var troopData = data.TroopData.Find(x => x.Level == aliveTroop.Level);
                    if (troopData != null)
                    {
                        troopData.Count -= aliveTroop.Dead;
                        troopData.Wounded = wounded;
                        hasChange = true;
                    }
                }

                if (hasChange)
                {
                    try
                    {
                        await userTroopManager.UpdateTroops(army.PlayerId, data.TroopType, data.TroopData);
                    }
                    catch { }
                }
            }
        }

        private static ClientBattleReport SetClientReport(BattlePower data)
        {
            List<string> heroes = null;
            if ((data.Heroes != null) && (data.Heroes.Count > 0))
            {
                heroes = data.Heroes.Select(x => CacheHeroDataManager.GetFullHeroDataID(x).Info.Name)?.ToList();
            }
            return new ClientBattleReport()
            {
                PlayerId = data.PlayerId,
                Username = data.Username,
                Attack = data.Attack,
                Defense = data.Defense,
                TotalArmy = data.TotalArmy,
                Survived = data.Survived,
                Wounded = data.Wounded,
                Dead = data.Dead,
                Food = data.Food,
                Wood = data.Wood,
                Ore = data.Ore,
                Heroes = heroes
            };
        }

        private async Task GiveLoot(PlayerCompleteData defenderArmy, BattlePower attackerPower, BattleReport finalReport)
        {
            long loadAmount = (long)(attackerPower.TotalLoad / 3);

            //                finalReport.Attacker.Food = 0;
            //                finalReport.Attacker.Wood = 0;
            //                finalReport.Attacker.Ore = 0;

            //var dfood = defenderArmy.Resources.Food;
            //var dwood = defenderArmy.Resources.Wood;
            //var dore = defenderArmy.Resources.Ore;
            //var minRes = 0;

            //var wareHouse = defenderArmy.Structures.FirstOrDefault(x => x.StructureType == StructureType.Warehouse).Buildings.FirstOrDefault();
            //if (wareHouse != null)
            //{
            //    var wareHouseData = CacheStructureDataManager.GetFullStructureLevelData(StructureType.Warehouse, wareHouse.Level);
            //    minRes = wareHouseData.Data.SafeDeposit;
            //}


            //TODO: Improve logic to remove more resources
            long food = (defenderArmy.Resources.Food > 0)? defenderArmy.Resources.Food : 0;
            long remainFood = food - loadAmount;
            if (remainFood < 0) remainFood = 0;
            food -= remainFood;

            long wood = (defenderArmy.Resources.Wood > 0)? defenderArmy.Resources.Wood : 0;
            long remainWood = wood - loadAmount;
            if (remainWood < 0) remainWood = 0;
            wood -= remainWood;

            long ore = (defenderArmy.Resources.Ore > 0) ? defenderArmy.Resources.Ore : 0;
            long remainOre = ore - loadAmount;
            if (remainOre < 0) remainOre = 0;
            ore -= remainOre;


//            try
            {
                //TODO: improve this process, we should implement a single call on database to handle this type of request
                var resp = await resManager.SumMainResource(finalReport.Attacker.PlayerId, (int)food, (int)wood, (int)ore, 0);
                if (resp.IsSuccess && resp.HasData)
                {
                    finalReport.Attacker.Food = (int)food;// resp.Data[0].Value;
                    finalReport.Attacker.Wood = (int)wood;// resp.Data[1].Value;
                    finalReport.Attacker.Ore = (int)ore;// resp.Data[2].Value;

                    resp = await resManager.SumMainResource(finalReport.Defender.PlayerId, -(int)food, -(int)wood, -(int)ore, 0);
                    if (resp.IsSuccess && resp.HasData)
                    {
                        finalReport.Defender.Food = (int)-food;// resp.Data[0].Value;
                        finalReport.Defender.Wood = (int)-wood;// resp.Data[1].Value;
                        finalReport.Defender.Ore = (int)-ore;// resp.Data[2].Value;
                    }
                }
            }
//            catch { }

/*            try
            {
                await resManager.UpdateFoodResource(finalReport.Defender.PlayerId, remainFood);
                finalReport.Defender.Food = -finalReport.Attacker.Food;
            }
            catch { }

            try
            {
                await resManager.UpdateWoodResource(finalReport.Defender.PlayerId, remainWood);
                finalReport.Defender.Wood = -finalReport.Attacker.Wood;
            }
            catch { }

            try
            {
                await resManager.UpdateOreResource(finalReport.Defender.PlayerId, remainOre);
                finalReport.Defender.Ore = -finalReport.Attacker.Ore;
            }
            catch { }*/
        }

        private void CalculateTroopLoss(BattlePower troopBattle, bool healingBuff)
        {
            if (troopBattle.TroopsAlive == null) return;

            foreach (var alive in troopBattle.TroopsAlive)
            {
                int survived = (int)Math.Ceiling(alive.RemainUnits);
                troopBattle.TotalLoad += survived * alive.LoadPerUnit;
                troopBattle.TotalArmy += alive.InitialCount;
                troopBattle.Survived += survived;

                try
                {
                    var fallen = alive.InitialCount - survived;
                    var groups = Split2(fallen, 10);

                    var randm = new Random();
                    foreach (var count in groups)
                    {
                        //random validate for choose between wounded/dead soldiers
                        var wounded = randm.Next(0, 100) <= 30;
                        if (healingBuff) wounded = randm.Next(0, 100) <= 55;

                        if (wounded)
                            troopBattle.Wounded += count;
                        else
                        {
                            alive.Dead += count;
                            troopBattle.Dead += count;
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.DebugFormat("Exception in CaclulateTroopLoss {0} {1}", ex.StackTrace, ex.Message);
                    //Config.PrintLog(String.Format("Exception in CaclulateTroopLoss {0} {1} ", ex.StackTrace, ex.Message));
                }
            }
        }

        private static void Attack(BattlePower attackerPower, BattlePower defenderPower)
        {
            var random = new Random();
            var damageDifference = attackerPower.Attack - defenderPower.Defense;
            damageDifference = damageDifference <= 1 ? random.Next(100, 150) : damageDifference;
            var damageToDefender = damageDifference * random.Next(1, 4) / 10;

            damageDifference = defenderPower.Attack - attackerPower.Defense;
            damageDifference = damageDifference <= 1 ? random.Next(100, 150) : damageDifference;
            var damageToAttacker = damageDifference * random.Next(1, 4) / 10;

            foreach (var troop in attackerPower.TroopsAlive)
            {
                var temp = troop.Hp;
                troop.Hp -= (int)damageToAttacker;
                if (troop.Hp <= 0)
                {
                    troop.Hp = 0;
                    damageToAttacker -= temp;
                    continue;
                }

                break;
            }

            if (defenderPower.GateHp > 0)
            {
                var temp = defenderPower.GateHp;
                defenderPower.GateHp -= (int)damageToDefender;
                if (defenderPower.GateHp < 0) defenderPower.GateHp = 0;
                damageToDefender -= temp;
            }
            if (damageToDefender > 0)
            {
                foreach (var troop in defenderPower.TroopsAlive)
                {
                    var temp = troop.Hp;
                    troop.Hp -= (int)damageToDefender;
                    if (troop.Hp <= 0)
                    {
                        troop.Hp = 0;
                        damageToDefender -= temp;
                        continue;
                    }

                    break;
                }
            }
            attackerPower.Recalculate();
            defenderPower.Recalculate();
        }

        private void SetTroopsAlive(BattlePower battlePower)
        {
            if (battlePower.Army == null || battlePower.Army.Count <= 0) return;

            battlePower.TroopsAlive = new List<TroopDetailsPvP>();
            foreach (var troop in battlePower.Army)
            {
                if (troop == null) continue;

                var getTroopData = CacheTroopDataManager.GetFullTroopData(troop.TroopType);
                if (getTroopData == null) continue;

                foreach (var troopData in troop.TroopData)
                {
                    if (troopData == null) continue;

                    var troopLvlDefData = getTroopData.Levels.Where(x => x.Data.Level == troopData.Level).FirstOrDefault();
                    if ((troopLvlDefData == null) || (troopLvlDefData.Data == null)) continue;

                    battlePower.TroopsAlive.Add(new TroopDetailsPvP(troop.TroopType, troopData.Count, troopLvlDefData.Data));
                }
            }
        }

        private List<TroopInfos> GetAvailableTroops(List<TroopInfos> troopCollection)
        {
            var validTroops = new List<TroopInfos>();
            if ((troopCollection == null) || (troopCollection.Count == 0)) return validTroops;

            foreach (var troops in troopCollection)
            {
                if ((troops == null) || (troops.TroopData == null) || (troops.TroopData.Count == 0)) continue;

                List<TroopDetails> troopDatas = null;
                foreach (var troopData in troops.TroopData)
                {
                    if ((troopData == null) || (troopData.Count < 1)) continue;

                    if (troopDatas == null)
                    {
                        troopDatas = new List<TroopDetails>();
                        validTroops.Add(new TroopInfos(troops.Id, troops.TroopType, troopDatas));
                    }
                    troopDatas.Add(new TroopDetails()
                    {
                        Level = troopData.Level,
                        Count = troopData.FinalCount//AVAILABLE SOLDIERS
                    });
                }
            }

            return validTroops;
        }

        private bool UpdatePlayerArmyToMarch(PlayerCompleteData data, MarchingArmy army, bool removeArmy)
        {
            try
            {
                var playerTroops = data.Troops;
                var playerMarchingTroops = army.Troops;
                foreach (var troopClass in playerMarchingTroops)
                {
                    var userTroop = playerTroops?.Where(x => x.TroopType == troopClass.TroopType).FirstOrDefault();
                    var troopData = userTroop?.TroopData;
                    if (troopData == null || troopData.Count <= 0) continue;

                    troopClass.Id = userTroop.Id;
                    foreach (var item in troopClass.TroopData)
                    {
                        var lvlData = troopData.Where(x => x.Level == item.Level).FirstOrDefault();
                        if (lvlData == null) return false;

                        var reqCount = item.Count;
                        if ((lvlData.Count - reqCount) < 0) return false;

                        if (removeArmy) lvlData.Count -= reqCount;
                    }
                }

                data.MarchingArmy = army;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static List<int> Split2(int amount, int maxPerGroup)
        {
            int amountGroups = amount / maxPerGroup;

            if (amountGroups * maxPerGroup < amount)
            {
                amountGroups++;
            }

            int groupsLeft = amountGroups;
            List<int> result = new List<int>();
            while (amount > 0)
            {
                int nextGroupValue = amount / groupsLeft;
                if (nextGroupValue * groupsLeft < amount)
                {
                    nextGroupValue++;
                }
                result.Add(nextGroupValue);
                groupsLeft--;
                amount -= nextGroupValue;
            }
            return result;
        }
    }
}

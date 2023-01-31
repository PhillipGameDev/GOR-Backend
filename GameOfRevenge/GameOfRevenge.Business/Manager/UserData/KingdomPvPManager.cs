using ExitGames.Logging;
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

        public List<AttackDefenseMultiplier> Multipliers { get; set; }
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

                float attackMultiplier = 1;
                float defenseMultiplier = 1;
                var multi = Multipliers.Find(x => (x.Troop == troops.Troop));
                if (multi != null)
                {
                    attackMultiplier = multi.AttackMultiplier;
                    defenseMultiplier = multi.DefenseMultiplier;
                    try
                    {
                        KingdomPvPManager.log.Debug("------- TROOP MULTI = " + multi.AttackMultiplier);
                    }
                    catch { }
                }
                points += troops.Data.Health * troops.RemainUnits;
                attack += (troops.Data.AttackDamage * troops.RemainUnits) * attackMultiplier;
                defense += (troops.Data.Defense * troops.RemainUnits) * defenseMultiplier;
            }
            var globalMulti = Multipliers.Find(x => (x.Troop == null));
            if (globalMulti != null)
            {
                attack *= globalMulti.AttackMultiplier;
                defense *= globalMulti.DefenseMultiplier;
                try
                {
                    KingdomPvPManager.log.Debug("------- GLOBAL MULTI = " + globalMulti.AttackMultiplier);
                }
                catch { }
            }

            HitPoint = (int)points;
            Attack = (int)attack;
            Defense = (int)defense;
        }
    }

    public class AttackDefenseMultiplier
    {
        public TroopInfos Troop { get; set; }
        public float AttackMultiplier { get; set; } = 1;
        public float DefenseMultiplier { get; set; } = 1;
    }

    public class KingdomPvPManager : BaseUserDataManager, IKingdomPvPManager
    {
        public static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly IUserTroopManager userTroopManager = new UserTroopManager();
        private readonly IUserMailManager mailManager = new UserMailManager();
        private readonly IAccountManager accManager = new AccountManager();
        private readonly IUserResourceManager resManager = new UserResourceManager();
        private readonly IUserStructureManager structManager = new UserStructureManager();

        public Task<Response<PlayerInfo>> GetAccountInfo(int playerId) => accManager.GetAccountInfo(playerId);

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
            var debugMsg = "";
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
                var SAVE = true;

                var attackerMultipliers = GetAtkDefMultiplier(true, attackerArmy);
                var attackerPower = new BattlePower();
                attackerPower.PlayerId = attackerArmy.PlayerId;
                attackerPower.Username = attackerArmy.PlayerName;
                attackerPower.Army = GetAvailableTroops(attackerArmy.MarchingArmy.Troops);
                attackerPower.Multipliers = attackerMultipliers;
                if ((attackerArmy.MarchingArmy.Heroes != null) && (attackerArmy.MarchingArmy.Heroes.Count > 0))
                {
                    attackerPower.Heroes = attackerArmy.MarchingArmy.Heroes;
                }
                debugMsg = "1";

                StructureDetails defenderGate = null;
                var structures = defenderArmy.Structures.Find(x => (x.StructureType == StructureType.Gate));
                if (structures != null) defenderGate = structures.Buildings.OrderBy(x => x.Level).FirstOrDefault();
                var gateHitPoints = (defenderGate != null)? defenderGate.HitPoints : 0;
//                var gateLevelData = CacheStructureDataManager.GetFullStructureData(StructureType.Gate).Levels.Where(x => x.Data.Level == defenderGate.Level).FirstOrDefault().Data;
//                gateHitPoints = gateLevelData.HitPoint;
                debugMsg = "2";

                var defenderMultipliers = GetAtkDefMultiplier(false, defenderArmy);
                var defenderPower = new BattlePower()
                {
                    PlayerId = defenderArmy.PlayerId,
                    Username = defenderArmy.PlayerName,
                    Army = GetAvailableTroops(defenderArmy.Troops),
                    Multipliers = defenderMultipliers,
                    GateHp = gateHitPoints
                };
                debugMsg = "3";

                SetTroopsAlive(attackerPower);
                SetTroopsAlive(defenderPower);
                debugMsg = "4";
                attackerPower.Recalculate();
                defenderPower.Recalculate();

                var initialAttackerAtkPower = attackerPower.Attack;
                var initialAttackerDefPower = attackerPower.Defense;
                var initialDefenderAtkPower = defenderPower.Attack;
                var initialDefenderDefPower = defenderPower.Defense;

                debugMsg = "5";

                log.Debug("atk pwr= " + attackerPower.HitPoint + " vs def pwr=" + defenderPower.HitPoint);
                while ((defenderPower.HitPoint > 0) && (attackerPower.HitPoint > 0))
                {
                    Attack(attackerPower, defenderPower);
                    log.Debug("atk pwr= " + attackerPower.HitPoint + " xx def pwr=" + defenderPower.HitPoint);
                }

                attackerPower.Attack = initialAttackerAtkPower;
                attackerPower.Defense = initialAttackerDefPower;
                defenderPower.Attack = initialDefenderAtkPower;
                defenderPower.Defense = initialDefenderDefPower;


//                Console.WriteLine( JsonConvert.SerializeObject(attackerPower.TroopsAlive[0]));
//                Console.WriteLine("   ");
//                Console.WriteLine( JsonConvert.SerializeObject(attackerPower.TroopsAlive[1]));
//                Console.WriteLine(JsonConvert.SerializeObject(defenderPower.TroopsAlive));


                //TODO: implement percentage based on level (maybe we need to add level to item)
                var atkHealingBoost = attackerArmy.Boosts.Exists(x => (x.Type == NewBoostType.LifeSaver) && (x.TimeLeft > 0));
                var defHealingBoost = defenderArmy.Boosts.Exists(x => (x.Type == NewBoostType.LifeSaver) && (x.TimeLeft > 0));

                CalculateTroopLoss(attackerPower, atkHealingBoost);
                CalculateTroopLoss(defenderPower, defHealingBoost);
                debugMsg = "6";

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
                        if (SAVE)
                        {
                            var response = await manager.AddOrUpdatePlayerData(attackerArmy.PlayerId, DataType.Hero, valueId, data);
                        }
                    }
                }
                debugMsg = "7";
                if (defenderArmy.Heroes != null)
                {
                    foreach (var hero in defenderArmy.Heroes)
                    {
                        if (hero.IsMarching) continue;

                        hero.DefenseCount++;
                        if (attackerWin) hero.DefenseFail++;

                        int valueId = CacheHeroDataManager.GetFullHeroData(hero.HeroCode).Info.HeroId;
                        var data = JsonConvert.SerializeObject(hero);
                        if (SAVE)
                        {
                            var response = await manager.AddOrUpdatePlayerData(defenderArmy.PlayerId, DataType.Hero, valueId, data);
                        }
                    }
                }
                debugMsg = "8";

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
                if (SAVE)
                {
                    await manager.AddOrUpdatePlayerData(attackerArmy.PlayerId, DataType.Custom, 1, kingdata);
                }
                debugMsg = "9";

                var finalReport = new BattleReport
                {
                    Attacker = SetClientReport(attackerPower),
                    Defender = SetClientReport(defenderPower),
                    AttackerWon = attackerWin
                };
                debugMsg = "10";

                //TODO: move this operation to the event when the attacker return to the castle
                await ApplyTroopChanges(true, attackerArmy, attackerPower, SAVE);

                await ApplyTroopChanges(false, defenderArmy, defenderPower, SAVE);
                debugMsg = "11";

                if (attackerWin) await GiveLoot(defenderArmy, attackerPower, finalReport, SAVE);

                if (SAVE)
                {
                    await manager.AddOrUpdatePlayerData(attackerArmy.PlayerId, DataType.Marching, 1, string.Empty);
                    await structManager.UpdateGate(defenderArmy.PlayerId, defenderPower.GateHp);
                }
                debugMsg = "12";

                string json = JsonConvert.SerializeObject(finalReport);
                log.Debug("------------REPORT " + json);
//                Console.WriteLine("------------REPORT " + json);
                _ = Task.Run(async () =>
                {
                    if (!SAVE) return;

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
                Console.WriteLine("------------EXCEPTION4 " + debugMsg + "  " + ex.Message);
                log.Debug("------------EXCEPTION4 " + debugMsg + "  " + ex.Message);
                return new Response<BattleReport>()
                {
                    Case = 200,
                    Message = ex.Message + ex.StackTrace// ErrorManager.ShowError(ex)
                };
            }
            catch (DataNotExistExecption ex)
            {
                Console.WriteLine("------------EXCEPTION3 " + debugMsg + "  " + ex.Message);
                log.Debug("------------EXCEPTION3 " + debugMsg + "  " + ex.Message);
                return new Response<BattleReport>()
                {
                    Case = 201,
                    Message = ex.Message + ex.StackTrace//rrorManager.ShowError(ex)
                };
            }
            catch (RequirementExecption ex)
            {
                Console.WriteLine("------------EXCEPTION2 " + debugMsg + "  " + ex.Message);
                log.Debug("------------EXCEPTION2 " + debugMsg + "  " + ex.Message);
                return new Response<BattleReport>()
                {
                    Case = 202,
                    Message = ex.Message + ex.StackTrace//ErrorManager.ShowError(ex)
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("------------EXCEPTION " + debugMsg + "  " + ex.Message);
                log.Debug("------------EXCEPTION " + debugMsg + "  " + ex.Message);
                return new Response<BattleReport>()
                {
                    Case = 0,
                    Message = ex.Message + ex.StackTrace//ErrorManager.ShowError(ex)
                };
            }
        }

        private static List<AttackDefenseMultiplier> GetAtkDefMultiplier(bool attacker, PlayerCompleteData playerData)
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
                    if (castleBuilding != null)
                    {
                        var castleLevel = castleBuilding.Level;
                        if (castleBuilding.TimeLeft > 0) castleLevel--;
                        lvl = (byte)castleLevel;
                    }
                }

                float boostAtkValue = 0;
                float boostDefValue = 0;
                foreach (var tech in specBoostData.Techs)
                {
                    if (!tech.Levels.ContainsKey(lvl)) continue;
                    if (!float.TryParse(tech.Levels[lvl].ToString(), out float levelVal)) continue;

                    switch (tech.Tech)
                    {
                        case NewBoostTech.TroopAttackMultiplier: if (attacker) boostAtkValue += levelVal; break;
                        case NewBoostTech.TroopDefenseMultiplier: if (attacker) boostDefValue += levelVal; break;
                        case NewBoostTech.CityTroopAttackMultiplier: if (!attacker) boostAtkValue += levelVal; break;
                        case NewBoostTech.CityTroopDefenseMultiplier: if (!attacker) boostDefValue += levelVal; break;
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
                        log.Debug("------- ATK TECH FOUND val ="+atkPercentage);
                    }
                    vipTech = vipBoostData.Techs.FirstOrDefault(x => (x.Tech == (NewBoostTech)VIPBoostTech.TroopDefenseMultiplier));
                    if (vipTech != null)
                    {
                        defPercentage += vipTech.GetValue(vip.Level);
                        log.Debug("------- DEF TECH FOUND val ="+defPercentage);
                    }
                }
            }
            var multiplier = new AttackDefenseMultiplier();
            multiplier.AttackMultiplier = attack * (1 + (atkPercentage / 100f));
            multiplier.DefenseMultiplier = defense * (1 + (defPercentage / 100f));
            list.Add(multiplier);

            if ((playerData.MarchingArmy != null) && (playerData.MarchingArmy.Troops != null))
            {
                var troops = playerData.MarchingArmy.Troops;
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
                            var specBoostData = CacheBoostDataManager.SpecNewBoostDatas.First(x => (x.Type == atkTechType));
                            if (specBoostData.Table > 0)
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
                            var specBoostData = CacheBoostDataManager.SpecNewBoostDatas.First(x => (x.Type == defTechType));
                            if (specBoostData.Table > 0)
                            {
                                float.TryParse(specBoostData.Levels[tech.Level].ToString(), out float levelVal);
                                defPercentage += levelVal;
                            }
                        }
                    }

                    if ((atkPercentage > 0) || (defPercentage > 0))
                    {
                        multiplier = new AttackDefenseMultiplier();
                        multiplier.Troop = troop;
                        multiplier.AttackMultiplier = 1 + (atkPercentage / 100f);
                        multiplier.DefenseMultiplier = 1 + (defPercentage / 100f);
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

        private async Task ApplyTroopChanges(bool attacker, PlayerCompleteData army, BattlePower power, bool SAVE)
        {
            if (power.TroopsAlive == null) return;

            foreach (var data in army.Troops)
            {
                if (data == null) continue;

                bool hasChange = false;
                foreach (var aliveTroop in power.TroopsAlive)
                {
                    if ((aliveTroop == null) || (aliveTroop.Troop.TroopType != data.TroopType)) continue;

                    if (attacker)
                    {
                        var survived = aliveTroop.InitialCount - aliveTroop.Dead;
                        if (survived < 1) continue;

                        var troopData = data.TroopData.Find(x => (x.Level == aliveTroop.Level));
                        if (troopData != null)
                        {
                            troopData.Count += survived;
                            troopData.Wounded += aliveTroop.Wounded;
                            hasChange = true;
                        }
                    }
                    else
                    {
                        if ((aliveTroop.Dead == 0) && (aliveTroop.Wounded == 0)) continue;

                        var troopData = data.TroopData.Find(x => (x.Level == aliveTroop.Level));
                        if (troopData != null)
                        {
                            troopData.Count -= aliveTroop.Dead;
                            troopData.Wounded += aliveTroop.Wounded;
                            hasChange = true;
                        }
                    }


                    /*                    int wounded = aliveTroop.InitialCount - (int)Math.Ceiling(aliveTroop.RemainUnits);
                                        if (wounded <= 0) continue;

                                        var troopData = data.TroopData.Find(x => x.Level == aliveTroop.Level);
                                        if (troopData != null)
                                        {
                                            troopData.Count -= aliveTroop.Dead;
                                            troopData.Wounded = wounded;
                                            hasChange = true;
                                        }*/
                }

                if (hasChange && SAVE)
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
//                Survived = data.Survived,
                Wounded = data.Wounded,
                Dead = data.Dead,
                Food = data.Food,
                Wood = data.Wood,
                Ore = data.Ore,
                Heroes = heroes
            };
        }

        private async Task GiveLoot(PlayerCompleteData defenderArmy, BattlePower attackerPower, BattleReport finalReport, bool SAVE)
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
            if (SAVE)
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
            else
            {
                finalReport.Attacker.Food = (int)food;// resp.Data[0].Value;
                finalReport.Attacker.Wood = (int)wood;// resp.Data[1].Value;
                finalReport.Attacker.Ore = (int)ore;// resp.Data[2].Value;

                finalReport.Defender.Food = (int)-food;// resp.Data[0].Value;
                finalReport.Defender.Wood = (int)-wood;// resp.Data[1].Value;
                finalReport.Defender.Ore = (int)-ore;// resp.Data[2].Value;
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

            var randm = new Random();
            foreach (var alive in troopBattle.TroopsAlive)
            {
                int survived = (int)Math.Ceiling(alive.RemainUnits);
                troopBattle.TotalLoad += survived * alive.LoadPerUnit;
                troopBattle.TotalArmy += alive.InitialCount;
//                troopBattle.Survived += survived;

                var newWounded = 0;
                var newDeads = 0;
                var fallen = alive.InitialCount - survived;
                var groups = Split2(fallen, 10);
                foreach (var count in groups)
                {
                    //random validate for choose between wounded/dead soldiers
                    var wounded = randm.Next(0, 100) <= (healingBuff? 55 : 30);
                    if (wounded)
                    {
                        newWounded += count;
//                        troopBattle.Wounded += count;
                    }
                    else
                    {
                        newDeads += count;
//                        alive.Dead += count;
//                        troopBattle.Dead += count;
                    }
                }
                troopBattle.Wounded += newWounded;
                troopBattle.Dead += newDeads;
                alive.Wounded = newWounded;
                alive.Dead = newDeads;
//                alive.RemainUnits += newWounded;
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
                    if (damageToAttacker < 0) damageToAttacker = 0;
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
                        if (damageToDefender < 0) damageToDefender = 0;
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

                    var troopLvlDefData = getTroopData.Levels.FirstOrDefault(x => (x.Data.Level == troopData.Level));
                    if ((troopLvlDefData == null) || (troopLvlDefData.Data == null)) continue;

                    var troopDetails = new TroopDetailsPvP(troop, troopData.Count, troopLvlDefData.Data);
                    battlePower.TroopsAlive.Add(troopDetails);
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

        public bool UpdatePlayerArmyToMarch(PlayerCompleteData data, MarchingArmy army, bool removeArmy)
        {
            try
            {
                List<KeyValuePair<TroopDetails, int>> toRemove = null;
                if (removeArmy) toRemove = new List<KeyValuePair<TroopDetails, int>>();

                var playerTroops = data.Troops;
                if (playerTroops != null)
                {
                    var playerMarchingTroops = army.Troops;
                    foreach (var troopClass in playerMarchingTroops)
                    {
                        var userTroop = playerTroops.Find(x => (x.TroopType == troopClass.TroopType));
                        if (userTroop == null) return false;

                        var troopData = userTroop.TroopData;
                        if ((troopData == null) || (troopData.Count == 0)) return false;

                        troopClass.Id = userTroop.Id;
                        foreach (var item in troopClass.TroopData)
                        {
                            var lvlData = troopData.Find(x => (x.Level == item.Level));
                            if ((lvlData == null) || (lvlData.Count < item.Count)) return false;

                            if (removeArmy) toRemove.Add(new KeyValuePair<TroopDetails, int>(lvlData, item.Count));
                        }
                    }
                }
                if (removeArmy)
                {
                    foreach (var remove in toRemove)
                    {
                        remove.Key.Count -= remove.Value;
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

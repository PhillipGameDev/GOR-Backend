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
using GameOfRevenge.Common.Models.Hero;
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

namespace GameOfRevenge.Business.Manager.UserData
{
    public class KingdomPvPManager : BaseUserDataManager, IKingdomPvPManager
    {
        public static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly IUserHeroManager userHeroManager = new UserHeroManager();
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

        private async Task<(bool, byte)> GetShieldActiveAndWatchTowerLevel(int playerId)
        {
            var task = await GetAllPlayerData(playerId);
            if (!task.IsSuccess || !task.HasData) throw new DataNotExistExecption(task.Message);

            bool shieldActive = false;
            foreach (var item in task.Data)
            {
                if (item.DataType != DataType.ActiveBoost) continue;

                var boostTable = CacheBoostDataManager.GetNewBoostByTypeId(item.ValueId);
                if (boostTable.Type != Common.Models.Boost.NewBoostType.Shield) continue;

                try
                {
                    var activeBoost = JsonConvert.DeserializeObject<UserNewBoost>(item.Value);
                    if (activeBoost?.TimeLeft > 0) shieldActive = true;
                }
                catch { }
                break;
            }

            byte watchLevel = 0;
            foreach (var item in task.Data)
            {
                if (item.DataType != DataType.Structure) continue;

                var structureTable = CacheStructureDataManager.GetStructureTable(item.ValueId);
                if (structureTable.Code != StructureType.WatchTower) continue;

                try
                {
                    var watchTowers = JsonConvert.DeserializeObject<List<StructureDetails>>(item.Value);
                    if (watchTowers?.Count > 0)
                    {
                        watchLevel = (byte)watchTowers.Max(x => (x.TimeLeft > 0) ? (x.Level - 1) : x.Level);
                    }
                }
                catch { }
                break;
            }

            return (shieldActive, watchLevel);
        }

        public async Task<Response<AttackStatusData>> AttackOtherPlayer(int attackerId, MarchingArmy army, MapLocation location, int defenderId)
        {
            try
            {
                ValidationHelper.KeyId(attackerId);

                if ((army == null) || (army.Troops == null) || (army.Troops.Count == 0))
                {
                    throw new RequirementExecption("Zero marching army was sended to attack");
                }

                var resp = await GetFullPlayerData(attackerId);
                if (!resp.IsSuccess || !resp.HasData) throw new DataNotExistExecption(resp.Message);
                var attackerData = resp.Data;

                if ((attackerData.Troops == null) || (attackerData.Troops.Count == 0))
                {
                    throw new RequirementExecption("User does not have any army");
                }

                if ((attackerData.MarchingArmy != null) && (attackerData.MarchingArmy.TimeLeft > 0))
                {
                    throw new RequirementExecption("Maximum one army marching is allowed");
                }

                (bool shieldActive, byte watchLevel) = await GetShieldActiveAndWatchTowerLevel(defenderId);

                if (shieldActive) throw new RequirementExecption("Defender has shield activated");

                try
                {
                    ApplyPlayerArmyToMarch(attackerData, army);
                }
                catch (DataNotExistExecption ex)
                {
                    throw new RequirementExecption(ex.Message);
                }
                catch (Exception)
                {
                    throw new RequirementExecption("User does not have required army");
                }

                var armyJson = JsonConvert.SerializeObject(army);
                var response = await manager.AddOrUpdatePlayerData(attackerId, DataType.Marching, 1, armyJson);
                if (response.IsSuccess)
                {
                    var report = GenerateAlertMail(attackerData, location, watchLevel);

                    try
                    {
                        var json = JsonConvert.SerializeObject(report);
                        await mailManager.SaveMail(defenderId, MailType.UnderAttack, json);
                    }
                    catch { }

                    report.StartTime = attackerData.MarchingArmy.StartTime;
                    report.ReachedTime = attackerData.MarchingArmy.ReachedTime;
                    report.DefenderId = defenderId;

                    var attackStatus = new AttackStatusData()
                    {
                        Attacker = attackerData,
                        Report = report
                    };

                    return new Response<AttackStatusData>(attackStatus, response.Case, "Marching towards enemy city");
                }

                throw new Exception(response.Message);
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

        private UnderAttackReport GenerateAlertMail(PlayerCompleteData attackerData, MapLocation location, byte watchLevel)
        {
            var attackerMailInfo = new UnderAttackReport()
            {
                AttackerId = attackerData.PlayerId,
                AttackerUsername = attackerData.PlayerName,
                Location = location,
                StartTime = attackerData.MarchingArmy.StartTime.AddSeconds(attackerData.MarchingArmy.ReachedTime),
                KingLevel = attackerData.King.Level,
                WatchLevel = watchLevel
            };

            var troops = new List<TroopData>();
            foreach (var troop in attackerData.MarchingArmy.Troops)
            {
                foreach (var troopData in troop.TroopData)
                {
                    troops.Add(new TroopData()
                    {
                        Type = troop.TroopType,
                        Level = troopData.Level,
                        Count = troopData.Count
                    });
                }
            }
            attackerMailInfo.Troops = troops;

            attackerMailInfo.Heroes = null;
            if ((attackerData.MarchingArmy.Heroes != null) && (attackerData.MarchingArmy.Heroes.Count > 0))
            {
                var heroDatas = new List<HeroData>();
                foreach (var item in attackerData.MarchingArmy.Heroes)
                {
                    var userHero = attackerData.Heroes.Find(x => (x.HeroType == item));
                    heroDatas.Add(new HeroData()
                    {
                        Type = userHero.HeroType,
                        Level = userHero.Level
                    });
                }
                attackerMailInfo.Heroes = heroDatas;
            }

            return attackerMailInfo;
        }

        private const bool SAVE = true;

        public (BattlePower, BattlePower) PrepareBattleData(PlayerCompleteData attackerArmy, PlayerCompleteData defenderArmy)
        {
            log.Debug("------------ PREPARE BATTLE SIMULATION "+attackerArmy.PlayerId+" vs "+ defenderArmy.PlayerId);

            BattlePower attackerPower = null;
            BattlePower defenderPower = null;
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

            attackerPower = new BattlePower()
            {
                PlayerId = attackerArmy.PlayerId,
                Username = attackerArmy.PlayerName,
                Army = GetAvailableTroops(attackerArmy.MarchingArmy.Troops),
                Multipliers = GetAtkDefMultiplier(true, attackerArmy),
                Heroes = GetAvailableHeroes(attackerArmy.MarchingArmy.Heroes, attackerArmy.Heroes)
            };

            StructureDetails defenderGate = null;
            var structures = defenderArmy.Structures.Find(x => (x.StructureType == StructureType.Gate));
            if (structures != null) defenderGate = structures.Buildings.OrderBy(x => x.Level).FirstOrDefault();
            var gateHitPoints = (defenderGate != null) ? defenderGate.HitPoints : 0;
            //                var gateLevelData = CacheStructureDataManager.GetFullStructureData(StructureType.Gate).Levels.Where(x => x.Data.Level == defenderGate.Level).FirstOrDefault().Data;
            //                gateHitPoints = gateLevelData.HitPoint;

            defenderPower = new BattlePower()
            {
                PlayerId = defenderArmy.PlayerId,
                Username = defenderArmy.PlayerName,
                Army = GetAvailableTroops(defenderArmy.Troops),
                Multipliers = GetAtkDefMultiplier(false, defenderArmy),
                GateHP = gateHitPoints
            };

            SetTroopsAlive(attackerPower);
            SetTroopsAlive(defenderPower);

            attackerPower.Recalculate();
            defenderPower.Recalculate();

            return (attackerPower, defenderPower);
        }

        public BattleReport FinishBattleData(PlayerCompleteData attackerArmy, BattlePower attackerPower, PlayerCompleteData defenderArmy, BattlePower defenderPower)
        {
            log.Debug("------------FINISH BATTLE SIMULATION " + attackerPower.PlayerId + " vs " + defenderPower.PlayerId);
            //TODO: implement percentage based on level (maybe we need to add level to item)
            var atkHealingBoost = attackerArmy.Boosts.Exists(x => (x.Type == NewBoostType.LifeSaver) && (x.TimeLeft > 0));
            var defHealingBoost = defenderArmy.Boosts.Exists(x => (x.Type == NewBoostType.LifeSaver) && (x.TimeLeft > 0));

            var attackerInfirmaryCapacity = 0;
            var attackerInfirmaryExist = attackerArmy.Structures.Exists(x => (x.StructureType == StructureType.Infirmary) && (x.Buildings.Count > 0));
            if (attackerInfirmaryExist)
            {
                var wounded = userTroopManager.GetCurrentPopulationWounded(attackerArmy.Troops);
                var capacity = structManager.GetMaxInfirmaryCapacity(attackerArmy.Structures);
                var remain = capacity - wounded;
                if (remain > 0) attackerInfirmaryCapacity = remain;
            }
            CalculateTroopLoss(attackerPower, attackerInfirmaryCapacity, atkHealingBoost);

            var defenderInfirmaryCapacity = 0;
            var defenderInfirmaryExist = defenderArmy.Structures.Exists(x => (x.StructureType == StructureType.Infirmary) && (x.Buildings.Count > 0));
            if (defenderInfirmaryExist)
            {
                var wounded = userTroopManager.GetCurrentPopulationWounded(defenderArmy.Troops);
                var capacity = structManager.GetMaxInfirmaryCapacity(defenderArmy.Structures);
                var remain = capacity - wounded;
                if (remain > 0) defenderInfirmaryCapacity = remain;
            }
            CalculateTroopLoss(defenderPower, defenderInfirmaryCapacity, defHealingBoost);

            bool attackerWin = attackerPower.HitPoints > defenderPower.HitPoints;

            var report = new BattleReport()
            {
                Attacker = attackerPower,
                Defender = defenderPower,
                AttackerWon = attackerWin
            };
            if (attackerWin) GiveLoot(defenderArmy, attackerPower, report);

            return report;
        }

        public async Task ApplyDefenderChangesAndSendReport(BattleReport report, PlayerCompleteData defenderArmy)
        {
            var attackerWin = report.AttackerWon;
            var defenderPower = (BattlePower)report.Defender;

            await ApplyTroopChanges(false, defenderArmy, defenderPower, SAVE);
            if (defenderArmy.Heroes != null)
            {
                foreach (var hero in defenderArmy.Heroes)
                {
                    if (hero.IsMarching) continue;

                    hero.DefenseCount++;
                    if (attackerWin) hero.DefenseFail++;

                    int valueId = CacheHeroDataManager.GetFullHeroData(hero.HeroType.ToString()).Info.HeroId;
                    var data = JsonConvert.SerializeObject(hero);
                    if (SAVE)
                    {
                        var response = await manager.AddOrUpdatePlayerData(defenderArmy.PlayerId, DataType.Hero, valueId, data);
                        if (!response.IsSuccess)
                        {
                            Console.WriteLine(response.Message);
                            log.Debug(response.Message);
                        }
                    }
                }
            }


            if (SAVE)
            {
                var response = await structManager.UpdateGate(defenderArmy.PlayerId, defenderPower.GateHP);
                if (!response.IsSuccess)
                {
                    Console.WriteLine(response.Message);
                    log.Debug(response.Message);
                }

                var respResources = await resManager.SumMainResource(report.Defender.PlayerId,
                                                                    report.Defender.Food,
                                                                    report.Defender.Wood,
                                                                    report.Defender.Ore,
                                                                    0);
                if (!respResources.IsSuccess)
                {
                    Console.WriteLine(respResources.Message);
                    log.Debug(respResources.Message);
                }


                string json = JsonConvert.SerializeObject(report);
                log.Debug("------------REPORT " + json);

                var msg = attackerWin ? "You failed to defend against enemy raid" : "You succesfully defended against enemy seige";
                msg = json.Replace("INSERTMESSAGEHERE", msg);
                try
                {
                    var respMail = await mailManager.SaveMail(defenderArmy.PlayerId, MailType.BattleReport, msg);
                    if (!respMail.IsSuccess)
                    {
                        Console.WriteLine(respMail.Message);
                        log.Debug(respMail.Message);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    log.Debug(ex.Message);
                }
            };
        }

        public async Task ApplyAttackerChangesAndSendReport(BattleReport report, PlayerCompleteData attackerArmy)
        {
            var attackerPower = (BattlePower)report.Attacker;
            var defenderPower = (BattlePower)report.Defender;

            //TODO: move this operation to the event when the attacker return to the castle
            bool attackerWin = report.AttackerWon;

            var king = attackerArmy.King;
            king.Experience += 5;
            if (king.TimeLeft <= 0) king.StartTime = DateTime.UtcNow;
            king.Duration += 10 * 60;
            king.BattleCount++;

            string heroToAward = null;
            if ((king.BattleCount % 3) == 0)
            {
                var idx = new Random().Next(0, CacheHeroDataManager.HeroInfos.Count);
                var heroTable = CacheHeroDataManager.HeroInfos[idx].Info;

                heroToAward = heroTable.Code;
            }
            /*                if (king.TimeLeft > 0)
                            {
                                king.EndTime = king.EndTime.AddMinutes(10);
                            }
                            else
                            {
                                king.StartTime = DateTime.UtcNow;
                                king.EndTime = king.StartTime.AddMinutes(10);
                            }*/
            if (SAVE)
            {
                var kingJson = JsonConvert.SerializeObject(king);
                var response = await manager.AddOrUpdatePlayerData(attackerArmy.PlayerId, DataType.Custom, 1, kingJson);
                if (!response.IsSuccess)
                {
                    Console.WriteLine(response.Message);
                    log.Debug(response.Message);
                }
            }


            await ApplyTroopChanges(true, attackerArmy, attackerPower, SAVE);
            if (attackerArmy.MarchingArmy.Heroes != null)
            {
                foreach (var item in attackerArmy.MarchingArmy.Heroes)
                {
                    var hero = attackerArmy.Heroes.Find(x => (x.HeroType == item));
                    hero.AttackCount++;
                    if (!attackerWin) hero.AttackFail++;
                    if (heroToAward != null)
                    {
                        heroToAward = null;
                        hero.Points++;
                    }

                    int valueId = CacheHeroDataManager.GetFullHeroData(hero.HeroType.ToString()).Info.HeroId;
                    var data = JsonConvert.SerializeObject(hero);
                    if (SAVE)
                    {
                        var response = await manager.AddOrUpdatePlayerData(attackerArmy.PlayerId, DataType.Hero, valueId, data);
                        if (!response.IsSuccess)
                        {
                            Console.WriteLine(response.Message);
                            log.Debug(response.Message);
                        }
                    }
                }
            }
            if ((heroToAward != null) && SAVE)
            {
                var response = await userHeroManager.AddHeroPoints(attackerArmy.PlayerId, heroToAward, 1);
                if (!response.IsSuccess)
                {
                    Console.WriteLine(response.Message);
                    log.Debug(response.Message);
                }
            }

            if (SAVE)
            {
                var respResources = await resManager.SumMainResource(report.Attacker.PlayerId,
                                                                report.Attacker.Food,
                                                                report.Attacker.Wood,
                                                                report.Attacker.Ore,
                                                                0);
                if (!respResources.IsSuccess)
                {
                    Console.WriteLine(respResources.Message);
                    log.Debug(respResources.Message);
                }

                var response = await manager.AddOrUpdatePlayerData(attackerArmy.PlayerId, DataType.Marching, 1, string.Empty);
                if (!response.IsSuccess)
                {
                    Console.WriteLine(response.Message);
                    log.Debug(response.Message);
                }

                string json = JsonConvert.SerializeObject(report);
                var msg = attackerWin? "You succesfully raided enemy city" : "You failed to raid the enemy city";
                msg = json.Replace("INSERTMESSAGEHERE", msg);
                try
                {
                    var respMail = await mailManager.SaveMail(attackerArmy.PlayerId, MailType.BattleReport, msg);
                    if (!respMail.IsSuccess)
                    {
                        Console.WriteLine(respMail.Message);
                        log.Debug(respMail.Message);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    log.Debug(ex.Message);
                }
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
            var multiplier = new AttackDefenseMultiplier()
            {
                AttackMultiplier = attack * (1 + (atkPercentage / 100f)),
                DefenseMultiplier = defense * (1 + (defPercentage / 100f))
            };
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
                        var response = await userTroopManager.UpdateTroops(army.PlayerId, data.TroopType, data.TroopData);
                        if (!response.IsSuccess)
                        {
                            Console.WriteLine(response.Message);
                            log.Debug(response.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        log.Debug(ex.Message);
                    }
                }
            }
        }

/*        private static ClientBattleReport GetClientReport(BattlePower data, List<UserHeroDetails> userHeroes)
        {
            List<HeroData> heroes = null;
            if ((data.Heroes != null) && (data.Heroes.Count > 0))
            {
//                heroes = data.Heroes.Select(x => CacheHeroDataManager.GetFullHeroDataID(x).Info.Name)?.ToList();
            
                heroes = data.Heroes.ConvertAll(x =>
                {
                    var userHero = userHeroes.Find(y => (y.HeroType == x));
                    var hero = new HeroData()
                    {
                        Type = x,
                        Level = userHero.Level
                    };

                    return hero;
                });
            }
            return new ClientBattleReport()
            {
                PlayerId = data.PlayerId,
                Username = data.Username,
                Attack = data.Attack,
                Defense = data.Defense,
                TotalArmy = data.TotalArmy,
                Wounded = data.Wounded,
                Dead = data.Dead,
                Food = data.Food,
                Wood = data.Wood,
                Ore = data.Ore,
                Heroes = heroes
            };
        }*/

        private void GiveLoot(PlayerCompleteData defenderArmy, BattlePower attackerPower, BattleReport finalReport)
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

        private void CalculateTroopLoss(BattlePower troopBattle, int infirmaryCapacity, bool healingBuff)
        {
            if (troopBattle.TroopsAlive == null) return;

            var randm = new Random();
            foreach (var alive in troopBattle.TroopsAlive)
            {
                int survived = (int)Math.Ceiling(alive.RemainUnits);
                troopBattle.TotalLoad += survived * alive.LoadPerUnit;
                troopBattle.TotalArmy += alive.InitialCount;

                var newWounded = 0;
                var newDeads = 0;
                var fallen = alive.InitialCount - survived;
                var groups = Split2(fallen, 10);
                var len = groups.Count;
                for (int num = 0; num < len; num++)
                {
                    var count = groups[num];
                    //random validate for choose between wounded/dead soldiers
                    if ((infirmaryCapacity > 0) && (randm.Next(0, 100) <= (healingBuff ? 55 : 30)))
                    {
                        var wounded = count;
                        infirmaryCapacity -= wounded;
                        if (infirmaryCapacity < 0)
                        {
                            count = -infirmaryCapacity;
                            wounded -= count;
                        }
                        else
                        {
                            count = 0;
                        }
                        newWounded += wounded;
                    }
                    newDeads += count;
                }
                troopBattle.Wounded += newWounded;
                troopBattle.Dead += newDeads;
                alive.Wounded = newWounded;
                alive.Dead = newDeads;
            }
        }

        public void Attack(BattlePower attackerPower, BattlePower defenderPower)
        {
            var random = new Random();
            var atkSoldierHealth = attackerPower.TroopsAlive.Average(x => x.Data.Health);
            var defSoldierHealth = defenderPower.TroopsAlive.Average(x => x.Data.Health);
            var atkSoldiersToSacrifice = Math.Max(10, (attackerPower.TroopsAlive.Sum(x => x.InitialCount) * 0.1f));
            var defSoldiersToSacrifice = Math.Max(20, (defenderPower.TroopsAlive.Sum(x => x.InitialCount) * 0.2f));

            var atkDamage = attackerPower.Attack - defenderPower.Defense;
            if (atkDamage < (defSoldierHealth * defSoldiersToSacrifice))
            {
                atkDamage = (int)(defSoldierHealth * defSoldiersToSacrifice * (random.Next(5, 10) / 10f));
            }
            var multiplier = (random.Next(3, 8) / 10f);
            var damageToDefender = atkDamage * multiplier;

            var defDamage = defenderPower.Attack - attackerPower.Defense;
            if (defDamage < (atkSoldierHealth * atkSoldiersToSacrifice))
            {
                defDamage = (int)(atkSoldierHealth * atkSoldiersToSacrifice * (random.Next(5, 10) / 10f));
            }
            multiplier = (random.Next(3, 8) / 10f);
            var damageToAttacker = defDamage * multiplier;

            foreach (var troop in attackerPower.TroopsAlive)
            {
                var temp = troop.TotalHP;
                troop.TotalHP -= (int)damageToAttacker;
                if (troop.TotalHP <= 0)
                {
                    troop.TotalHP = 0;
                    damageToAttacker -= temp;
                    if (damageToAttacker < 0) damageToAttacker = 0;
                    continue;
                }

                break;
            }

            if (defenderPower.GateHP > 0)
            {
                var temp = defenderPower.GateHP;
                defenderPower.GateHP -= (int)damageToDefender;
                if (defenderPower.GateHP < 0) defenderPower.GateHP = 0;
                damageToDefender -= temp;
            }
            if (damageToDefender > 0)
            {
                foreach (var troop in defenderPower.TroopsAlive)
                {
                    var temp = troop.TotalHP;
                    troop.TotalHP -= (int)damageToDefender;
                    if (troop.TotalHP <= 0)
                    {
                        troop.TotalHP = 0;
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
            if ((troopCollection != null) && (troopCollection.Count > 0))
            {
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
            }

            return validTroops;
        }

        private List<HeroData> GetAvailableHeroes(List<HeroType> marchingHeroes, List<UserHeroDetails> userHeroes)
        {
            List<HeroData> heroes = null;
            if ((marchingHeroes != null) && (marchingHeroes.Count > 0))
            {
                heroes = marchingHeroes.ConvertAll(x =>
                {
                    var userHero = userHeroes.Find(y => (y.HeroType == x));
                    var hero = new HeroData()
                    {
                        Type = x,
                        Level = userHero.Level
                    };

                    return hero;
                });
            }

            return heroes;
        }

        public void ApplyPlayerArmyToMarch(PlayerCompleteData data, MarchingArmy army, bool applyChanges = true)
        {
            var playerTroops = data.Troops;
            if (playerTroops == null) throw new DataNotExistExecption("User does not have troops");

//            var heroesToUpdate = new List<UserHeroDetails>();
            foreach (var heroId in army.Heroes)
            {
                var heroClass = CacheHeroDataManager.GetFullHeroDataID((int)heroId);
                var userHero = data.Heroes.Find(x => (x.HeroType.ToString() == heroClass.Info.Code));
                if ((userHero == null) || userHero.IsMarching)
                {
                    throw new DataNotExistExecption("User hero " + heroClass.Info.Code + " is not available");
                }

//                if (applyChanges) heroesToUpdate.Add(userHero);
            }

            var troopsToUpdate = new List<KeyValuePair<TroopDetails, int>>();
            foreach (var troopClass in army.Troops)
            {
                var troopType = troopClass.TroopType;
                var userTroop = playerTroops.Find(x => (x.TroopType == troopType));
                if ((userTroop == null) || (userTroop.TroopData == null) || (userTroop.TroopData.Count == 0))
                {
                    throw new DataNotExistExecption("User does not have troop " + troopType.ToString());
                }

                troopClass.Id = userTroop.Id;

                foreach (var item in troopClass.TroopData)
                {
                    var troopData = userTroop.TroopData.Find(x => (x.Level == item.Level));
                    if ((troopData == null) || (troopData.Count < item.Count))
                    {
                        throw new DataNotExistExecption("User does not have enough soldiers for troop " + troopType.ToString());
                    }

                    if (applyChanges) troopsToUpdate.Add(new KeyValuePair<TroopDetails, int>(troopData, item.Count));
                }
            }
            if (applyChanges)
            {
/*                foreach (var hero in heroesToUpdate)
                {
                    hero.IsMarching = true;
                }*/
                foreach (var troop in troopsToUpdate)
                {
                    troop.Key.Count -= troop.Value;
                }
            }

            data.MarchingArmy = army;
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

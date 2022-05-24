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
        public float HitPoint { get; set; }
        public float Attack { get; set; }
        public float Defence { get; set; }
        public int AttackModifier { get; set; }
        public int DefenceModifier { get; set; }
        public int GateHp { get; set; }

        public new List<int> Heros { get; set; }
        public List<TroopInfos> Army { get; set; }
        public List<TroopDetailsPvP> TroopsAlive { get; set; }

        public int TotalLoad { get; set; }
    }

    public class KingdomPvPManager : BaseUserDataManager, IKingdomPvPManager
    {
        private readonly IUserTroopManager userTroopManager = new UserTroopManager();
        private readonly IUserMailManager mailManager = new UserMailManager();
        private readonly IAccountManager accManager = new AccountManager();
        private readonly IUserResourceManager resManager = new UserResourceManager();
        private readonly IUserStructureManager structManager = new UserStructureManager();

        public async Task<Response<AttackStatusData>> AttackOtherPlayer(int attackerId, int defenderId, MarchingArmy army, MapLocation location)
        {
            try
            {
                ValidationHelper.KeyId(attackerId);
                ValidationHelper.KeyId(defenderId);

                if (army == null || army.Troops == null || army.Troops.Count == 0) throw new RequirementExecption("Zero marching army was sended to attack");

                var attacker = await GetPlayerData(attackerId);
                if (!attacker.IsSuccess || !attacker.HasData) throw new DataNotExistExecption(attacker.Message);
                var defender = await GetPlayerData(defenderId);
                if (!defender.IsSuccess || !defender.HasData) throw new DataNotExistExecption(defender.Message);

                var buffSheildData = defender.Data.Buffs.Where(x => x.BuffType == BuffType.Shield).FirstOrDefault();
                if (buffSheildData != null && buffSheildData.TimeLeft > 0) throw new RequirementExecption("Defender has sheild activated");

                if (attacker.Data.Troops == null || attacker.Data.Troops.Count == 0) throw new RequirementExecption("User does not have any army");
                if (attacker.Data.MarchingArmy != null)
                {
                    if (attacker.Data.MarchingArmy.TimeLeft > 0) throw new RequirementExecption("Maximum one army marching is allowed");
                    //else throw new RequirementExecption("Socket server loop can only execute battle simulator");
                }

                if (!ArmyToMarching(attacker.Data, army, true)) throw new RequirementExecption("User does not have required army");
                //foreach (var item in attacker.Data.Troops)
                //await userTroopManager.UpdateTroops(attackerId, item.TroopType, item.TroopData);
                var respone = await manager.AddOrUpdatePlayerData(attackerId, DataType.Marching, 1, JsonConvert.SerializeObject(army));

                var report = await SendAlertMail(attackerId, defenderId, attacker, defender, location);
                var armyResponse = new AttackStatusData()
                {
                    Attacker = new PlayerPvpData() { Data = attacker.Data, PlayerId = attackerId },
                    Defender = new PlayerPvpData() { Data = defender.Data, PlayerId = defenderId },
                    Report = report.Data
                };

                return new Response<AttackStatusData>(armyResponse, respone.Case, "Marching towards enemy city");
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
        }

        private async Task<Response<UnderAttackReport>> SendAlertMail(int attackerId, int defenderId, Response<PlayerCompleteData> attacker, Response<PlayerCompleteData> defender, MapLocation loc)
        {
            try
            {
                var watchTower = defender.Data.Structures.Where(x => x.StructureType == StructureType.WatchTower)?.FirstOrDefault()?.Buildings;
                int maxLevel = 0;
                foreach (var bldg in watchTower)
                {
                    if (bldg.TimeLeft <= 0 && bldg.Level > maxLevel)
                        maxLevel = bldg.Level;
                }

                var attackerMailInfo = new UnderAttackReport()
                {
                    Id = attackerId,
                    Defenderid = defenderId
                };

                await GenerateAlertMailFieldsFields(attackerId, attacker, loc, maxLevel, attackerMailInfo);
                await mailManager.SendMail(defenderId, MailType.UnderAttack, JsonConvert.SerializeObject(attackerMailInfo));

                return new Response<UnderAttackReport>(attackerMailInfo, CaseType.Success, "Scucess");
            }
            catch (Exception ex)
            {
                return new Response<UnderAttackReport>(CaseType.Error, ErrorManager.ShowError(ex));
            }
        }

        private static async Task GenerateAlertMailFieldsFields(int attackerId, Response<PlayerCompleteData> attacker, MapLocation location, int maxLevel, UnderAttackReport attackerMailInfo)
        {
            var troopNames = new List<TroopData>();
            var heroNames = new List<TroopData>();

            try
            {
                //Reveals the incoming troops Player name.
                if (maxLevel >= 1)
                {
                    var attackerName = string.Empty;
                    var attackerInfo = await new AccountManager().GetAccountInfo(attackerId);
                    if (attackerInfo.IsSuccess && attackerInfo.HasData) attackerName = attackerInfo.Data.Name;
                    attackerMailInfo.Name = attackerName;
                }

                //Reveals the exact loaction of the incoming troops origin.
                if (maxLevel >= 3) attackerMailInfo.Location = location;

                //Reveals the incoming troops estimated time of arrival.
                if (maxLevel >= 7) attackerMailInfo.TimeTaken = attacker.Data.MarchingArmy.TimeLeft;

                //Reveals the total size of incoming troops.
                if (maxLevel >= 11)
                {
                    var armySize = 0;

                    foreach (var troop in attacker.Data.MarchingArmy.Troops)
                    {
                        if (troop != null)
                        {
                            foreach (var item in troop.TroopData)
                            {
                                if (item != null)
                                {
                                    armySize += item.FinalCount;

                                    troopNames.Add(new TroopData()
                                    {
                                        Type = troop.TroopType.ToString(),
                                        Level = item.Level,
                                        Name = $"{troop.TroopType}  {item.Level}",
                                        Count = item.FinalCount
                                    }); ;
                                }
                            }
                        }
                    }

                    attackerMailInfo.TotalTroopSize = armySize;
                }

                //Reveals the exact king level of the incoming troops.
                if (maxLevel >= 17) attackerMailInfo.KingLevel = 0;

                //"Reveals the number of each soldier type frome the incoming troops."
                if (maxLevel < 23) foreach (var item in troopNames) item.Count = 0;

                //Reveals the soldier types of the incoming troops.
                if (maxLevel >= 19) attackerMailInfo.Troops = troopNames;

                //Displays the amount of heroes in the dispatch.
                if (maxLevel >= 25) attackerMailInfo.TotalHeroSize = attacker.Data.MarchingArmy.Heros.Count;


                //Displays the type of Heroes in the dispatch.
                if (maxLevel >= 30)
                {
                    foreach (var item in attacker.Data.MarchingArmy.Heros)
                    {
                        var heroData = CacheHeroDataManager.GetFullHeroData(item);
                        heroNames.Add(new TroopData()
                        {
                            Name = heroData.Info.Name
                        });
                    }

                    attackerMailInfo.Heros = heroNames;
                }
            }
            catch (Exception)
            {

            }
        }

        public async Task<Response<BattleReport>> BattleSimulation(int attackerId, PlayerCompleteData attackerArmy, int defenderId, PlayerCompleteData defenderArmy)
        {
            try
            {
                ValidationHelper.KeyId(attackerId);
                ValidationHelper.KeyId(defenderId);

                var p1 = await accManager.GetAccountInfo(attackerId);
                var p2 = await accManager.GetAccountInfo(defenderId);

                if (!p1.IsSuccess || !p1.HasData) throw new DataNotExistExecption(p1.Message);
                if (!p2.IsSuccess || !p2.HasData) throw new DataNotExistExecption(p1.Message);

                if (attackerArmy == null || attackerArmy.MarchingArmy == null || attackerArmy.MarchingArmy.Troops == null)
                {
                    var user1Resp = await GetPlayerData(attackerId);
                    if (!user1Resp.IsSuccess || !user1Resp.HasData) throw new DataNotExistExecption(user1Resp.Message);
                    attackerArmy = user1Resp.Data;
                }
                var attackerModifier = GetAtkDefModifier(attackerArmy);

                if (defenderArmy == null || defenderArmy.Troops == null)
                {
                    var user2Resp = await GetPlayerData(defenderId);
                    if (!user2Resp.IsSuccess || !user2Resp.HasData) throw new DataNotExistExecption(user2Resp.Message);
                    defenderArmy = user2Resp.Data;
                }
                var defenderModifier = GetAtkDefModifier(defenderArmy);

                var attackerPower = new BattlePower()
                {
                    PlayerId = p1.Data.PlayerId,
                    PlayerName = p1.Data.Name,
                    Army = RemoveUnwantedData(attackerArmy.MarchingArmy.Troops),
                    AttackModifier = attackerModifier.Item1,
                    DefenceModifier = attackerModifier.Item2,
                    Heros = defenderArmy.MarchingArmy.Heros,
                };

                var defenderGate = defenderArmy.Structures.Where(x => x.StructureType == StructureType.Gate)?.FirstOrDefault()?.Buildings.OrderBy(x => x.Level).FirstOrDefault();
                var gateLevelData = CacheStructureDataManager.GetFullStructureData(StructureType.Gate).Levels.Where(x => x.Data.Level == defenderGate.Level).FirstOrDefault().Data;

                var defenderPower = new BattlePower()
                {
                    PlayerId = p2.Data.PlayerId,
                    PlayerName = p2.Data.Name,
                    Army = RemoveUnwantedData(defenderArmy.Troops),
                    AttackModifier = defenderModifier.Item1,
                    DefenceModifier = defenderModifier.Item2,
                    GateHp = gateLevelData.HitPoint
                };

                SetTroopsAlive(attackerPower);
                SetTroopsAlive(defenderPower);
                RecalculatePower(attackerPower);
                RecalculatePower(defenderPower);

                while (attackerPower.HitPoint > 0 && defenderPower.HitPoint > 0)
                {
                    Attack(attackerPower, defenderPower);
                    RecalculatePower(attackerPower);
                    RecalculatePower(defenderPower);
                }

                var atkHealingbuff = attackerArmy.Buffs.Exists(x => x.BuffType == BuffType.LifeSaver && x.TimeLeft > 0);
                var defHealingbuff = defenderArmy.Buffs.Exists(x => x.BuffType == BuffType.LifeSaver && x.TimeLeft > 0);

                CaclulateTroopLoss(attackerPower, atkHealingbuff);
                CaclulateTroopLoss(defenderPower, defHealingbuff);

                bool attackerWin = attackerPower.HitPoint > defenderPower.HitPoint;

                var finalReport = new BattleReport
                {
                    Attacker = SetClientReport(attackerPower),
                    Defender = SetClientReport(defenderPower),
                    AttackerWon = attackerWin
                };

                await RemoveTroops(attackerArmy, attackerPower);
                await RemoveTroops(defenderArmy, defenderPower);

                if (attackerWin)
                {
                    await GiveLoot(defenderArmy, attackerPower, finalReport);

                    finalReport.Message = "You succesfully raided enemy city";
                    await mailManager.SendMail(attackerId, MailType.BattleReport, JsonConvert.SerializeObject(finalReport));
                    finalReport.Message = "You failed to defend against enemy raid";
                    await mailManager.SendMail(defenderId, MailType.BattleReport, JsonConvert.SerializeObject(finalReport));
                }
                else
                {
                    finalReport.Message = "You failed to raid the enemy city";
                    await mailManager.SendMail(attackerId, MailType.BattleReport, JsonConvert.SerializeObject(finalReport));
                    finalReport.Message = "You succesfully defended the against enemy seige";
                    await mailManager.SendMail(defenderId, MailType.BattleReport, JsonConvert.SerializeObject(finalReport));
                }

                finalReport.Message = "Attacked succesfully";
                await manager.AddOrUpdatePlayerData(attackerId, DataType.Marching, 1, string.Empty);
                await structManager.UpdateGate(defenderId, defenderPower.GateHp);

                return new Response<BattleReport>(finalReport, 100, JsonConvert.SerializeObject(finalReport));
            }

            catch (InvalidModelExecption ex)
            {
                return new Response<BattleReport>()
                {
                    Case = 200,
                    Message = ex.Message + ex.StackTrace// ErrorManager.ShowError(ex)
                };
            }
            catch (DataNotExistExecption ex)
            {
                return new Response<BattleReport>()
                {
                    Case = 201,
                    Message = ex.Message + ex.StackTrace//rrorManager.ShowError(ex)
                };
            }
            catch (RequirementExecption ex)
            {
                return new Response<BattleReport>()
                {
                    Case = 202,
                    Message = ex.Message + ex.StackTrace//ErrorManager.ShowError(ex)
                };
            }
            catch (Exception ex)
            {
                return new Response<BattleReport>()
                {
                    Case = 0,
                    Message = ex.Message + ex.StackTrace//ErrorManager.ShowError(ex)
                };
            }
        }

        private static (int, int) GetAtkDefModifier(PlayerCompleteData completeData)
        {



            int attack = 0;
            int? valueAttack = completeData.Technologies.Where(x => x.TechnologyType == TechnologyType.ArmyAttack)?.FirstOrDefault()?.Level;
            if (valueAttack.HasValue) attack = valueAttack.Value;

            int defence = 0;
            int? valueDefence = completeData.Technologies.Where(x => x.TechnologyType == TechnologyType.ArmyDefence)?.FirstOrDefault()?.Level;
            if (valueDefence.HasValue) defence = valueDefence.Value;

            if (completeData.Buffs.Exists(x => x.BuffType == BuffType.Blessing && x.TimeLeft > 0))
            {
                attack += 5;
                defence += 5;
            }

            try
            {
                var heros = completeData.MarchingArmy.Heros.Select(x => CacheHeroDataManager.GetFullHeroData(x));
                foreach (var hero in heros)
                {
                    foreach (var boost in hero.Boosts)
                    {
                        var boostData = CacheBoostDataManager.GetFullBoostDataByBoostId(boost.BoostId);
                        var boostDataValue = boostData.Values.FirstOrDefault(x => x.BoostId == boost.BoostId);
                        if (boostData.Info.BoostType == BoostType.ArmyAttack) attack += boostDataValue.Percentage;
                        if (boostData.Info.BoostType == BoostType.ArmyDefence) defence += boostDataValue.Percentage;
                    }
                }
            }
            catch (Exception)
            {

            }

            return (attack, defence);
        }

        private async Task RemoveTroops(PlayerCompleteData defenderArmy, BattlePower power)
        {

            try
            {
                foreach (var data in defenderArmy.Troops)
                {
                    bool hasChange = false;
                    if (data != null)
                    {
                        foreach (var aliveTroop in power.TroopsAlive)
                        {
                            if (aliveTroop != null && aliveTroop.Type == data.TroopType)
                            {
                                var wounded = aliveTroop.TotalCount - aliveTroop.Count;
                                if (wounded <= 0) continue;

                                var troopData = data.TroopData.Where(x => x.Level == aliveTroop.Level).FirstOrDefault();
                                if (troopData != null)
                                {
                                    troopData.Count -= aliveTroop.Dead;
                                    troopData.Wounded = aliveTroop.TotalCount - aliveTroop.Count;
                                    hasChange = true;
                                }
                            }
                        }
                    }

                    if (hasChange) await userTroopManager.UpdateTroops(power.PlayerId, data.TroopType, data.TroopData);
                }
            }
            catch (Exception)
            {

            }
        }

        private static ClientBattleReport SetClientReport(BattlePower attackerPower)
        {
            return new ClientBattleReport()
            {
                PlayerId = attackerPower.PlayerId,
                PlayerName = attackerPower.PlayerName,
                TotalArmy = attackerPower.TotalArmy,
                Survived = attackerPower.Survived,
                Wounded = attackerPower.Wounded,
                Dead = attackerPower.Dead,
                Heros = attackerPower.Heros?.Select(x => CacheHeroDataManager.GetFullHeroData(x).Info.Name)?.ToList(),
            };
        }

        private async Task GiveLoot(PlayerCompleteData defenderArmy, BattlePower attackerPower, BattleReport finalReport)
        {
            try
            {
                int perCount = attackerPower.TotalLoad / 3;

                finalReport.Attacker.Food = 0;
                finalReport.Attacker.Wood = 0;
                finalReport.Attacker.Ore = 0;

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

                var food = defenderArmy.Resources.Food - perCount;
                if (food <= 0) finalReport.Attacker.Food = defenderArmy.Resources.Food;
                else finalReport.Attacker.Food = perCount;

                var wood = defenderArmy.Resources.Wood - perCount;
                if (wood <= 0) finalReport.Attacker.Wood = defenderArmy.Resources.Food;
                else finalReport.Attacker.Wood = perCount;

                var ore = defenderArmy.Resources.Ore - perCount;
                if (ore <= 0) finalReport.Attacker.Ore = defenderArmy.Resources.Food;
                else finalReport.Attacker.Ore = perCount;

                await resManager.AddMainResource(finalReport.Attacker.PlayerId, finalReport.Attacker.Food, finalReport.Attacker.Wood, finalReport.Attacker.Ore, 0);

                await resManager.UpdateFoodResource(finalReport.Defender.PlayerId, food <= 0 ? 0 : food);
                await resManager.UpdateWoodResource(finalReport.Defender.PlayerId, wood <= 0 ? 0 : wood);
                await resManager.UpdateOreResource(finalReport.Defender.PlayerId, ore <= 0 ? 0 : ore);

                finalReport.Defender.Food = -finalReport.Attacker.Food;
                finalReport.Defender.Wood = -finalReport.Attacker.Wood;
                finalReport.Defender.Ore = -finalReport.Attacker.Ore;
            }
            catch (Exception)
            {

            }
        }

        private void CaclulateTroopLoss(BattlePower troopBattle, bool healingBuff)
        {
            try
            {
                foreach (var alive in troopBattle.TroopsAlive)
                {
                    troopBattle.TotalLoad += alive.Count * alive.LoadPerUnit;
                    troopBattle.TotalArmy += alive.TotalCount;
                    troopBattle.Survived += alive.Count;

                    var deadWounded = alive.TotalCount - alive.Count;
                    var part = Split2(deadWounded, 10);

                    var randm = new Random();
                    foreach (var item in part)
                    {
                        var wounded = randm.Next(0, 100) <= 30;
                        if (healingBuff) wounded = randm.Next(0, 100) <= 55;
                        if (wounded) troopBattle.Wounded += item;
                        else
                        {
                            alive.Dead += item;
                            troopBattle.Dead += item;
                        }
                    }
                }
            }
            catch (Exception /*ex*/)
            {
                //Config.PrintLog(String.Format("Exception in CaclulateTroopLoss {0} {1} ", ex.StackTrace, ex.Message));
            }
        }

        private static void Attack(BattlePower attackerPower, BattlePower defenderPower)
        {
            var random = new Random();
            var damageDifference = attackerPower.Attack - defenderPower.Defence;
            damageDifference = damageDifference <= 1 ? random.Next(100, 150) : damageDifference;
            var damageToDefender = damageDifference * random.Next(1, 4) / 10;

            damageDifference = defenderPower.Attack - attackerPower.Defence;
            damageDifference = damageDifference <= 1 ? random.Next(100, 150) : damageDifference;
            var damageToAttacker = damageDifference * random.Next(1, 4) / 10;

            foreach (var troop in attackerPower.TroopsAlive)
            {
                var hepToRemove = troop.Hp;
                troop.Hp -= (int)damageToAttacker;
                damageToAttacker -= hepToRemove;
                if (damageToAttacker <= 0) break;
            }

            foreach (var troop in defenderPower.TroopsAlive)
            {
                var hepToRemove = troop.Hp;
                if (defenderPower.GateHp >= 0) defenderPower.GateHp -= (int)damageToDefender;
                else troop.Hp -= (int)damageToDefender;
                damageToDefender -= hepToRemove;
                if (damageToDefender <= 0) break;
            }
        }

        private void SetTroopsAlive(BattlePower battlePower)
        {
            if (battlePower.Army == null || battlePower.Army.Count <= 0) return;

            battlePower.TroopsAlive = new List<TroopDetailsPvP>();
            foreach (var troop in battlePower.Army)
            {
                if (troop == null) continue;
                foreach (var troopData in troop.TroopData)
                {
                    if (troopData == null) continue;

                    var troopHp = CacheTroopDataManager.GetFullTroopData(troop.TroopType)?.Levels.Where(x => x.Data.Level == troopData.Level).FirstOrDefault()?.Data?.Health;
                    var troopLoad = CacheTroopDataManager.GetFullTroopData(troop.TroopType)?.Levels.Where(x => x.Data.Level == troopData.Level).FirstOrDefault()?.Data?.WeightLoad;

                    var finalTroopHp = 0;
                    var finalTroopLoad = 0;

                    if (troopHp.HasValue) finalTroopHp = (int)troopHp.Value;
                    if (troopLoad.HasValue) finalTroopLoad = (int)troopLoad.Value;
                    if (finalTroopHp <= 0) continue;

                    battlePower.TroopsAlive.Add(new TroopDetailsPvP()
                    {
                        Level = troopData.Level,
                        Type = troop.TroopType,
                        Hp = troopData.Count * finalTroopHp,
                        TotalCount = troopData.Count,
                        LoadPerUnit = finalTroopLoad
                    });
                }
            }
        }

        private List<TroopInfos> RemoveUnwantedData(IReadOnlyCollection<TroopInfos> info)
        {
            var finalTroops = new List<TroopInfos>();

            if (info != null && info.Count > 0)
            {
                foreach (var troops in info)
                {
                    if (troops == null || troops.TroopData == null || troops.TroopData.Count <= 0) continue;
                    var troopDatas = new List<TroopDetails>();

                    foreach (var troopData in troops.TroopData)
                    {
                        if (troopData == null || troopData.Count <= 0) continue;
                        troopDatas.Add(new TroopDetails() { Level = troopData.Level, Count = troopData.FinalCount });
                    }

                    if (troopDatas.Count > 0)
                        finalTroops.Add(new TroopInfos() { TroopType = troops.TroopType, TroopData = troopDatas });
                }
            }

            return finalTroops;
        }

        private void RecalculatePower(BattlePower bPower)
        {
            if (bPower == null || bPower.TroopsAlive == null || bPower.TroopsAlive.Count <= 0) return;

            bPower.HitPoint = 0;
            bPower.Attack = 0;
            bPower.Defence = 0;

            foreach (var troops in bPower.TroopsAlive)
            {
                if (troops.Hp <= 0)
                {
                    troops.Hp = 0;
                    continue;
                }

                var getTroopData = CacheTroopDataManager.GetFullTroopData(troops.Type);
                if (getTroopData == null) continue;

                var troopLvlDefData = getTroopData.Levels.Where(x => x.Data.Level == troops.Level).FirstOrDefault();
                if (troopLvlDefData == null) continue;

                troops.UnitHp = (int)troopLvlDefData.Data.Health;

                bPower.HitPoint += troopLvlDefData.Data.Health * troops.Count;
                bPower.Attack += troopLvlDefData.Data.AttackDamage * troops.Count;
                bPower.Defence += troopLvlDefData.Data.Defence * troops.Count;
            }

            bPower.Attack *= (1 + (bPower.AttackModifier / 100));
            bPower.Defence *= (1 + (bPower.DefenceModifier / 100));
        }

        private bool ArmyToMarching(PlayerCompleteData data, MarchingArmy army, bool removeArmy)
        {
            try
            {
                var playerTroops = data.Troops;
                var playerMarchingTroops = army.Troops;
                foreach (var troopClass in playerMarchingTroops)
                {
                    var troopData = playerTroops?.Where(x => x.TroopType == troopClass.TroopType).FirstOrDefault()?.TroopData;
                    if (troopData == null || troopData.Count <= 0) continue;

                    foreach (var item in troopClass.TroopData)
                    {
                        var lvlData = troopData.Where(x => x.Level == item.Level).FirstOrDefault();
                        if (lvlData == null) return false;
                        var lvlCount = lvlData.Count;
                        var reqCount = item.Count;

                        if (lvlCount - reqCount < 0) return false;
                        else if (removeArmy) lvlData.Count -= reqCount;
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

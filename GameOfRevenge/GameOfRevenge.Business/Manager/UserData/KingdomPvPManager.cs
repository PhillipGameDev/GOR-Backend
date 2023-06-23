using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExitGames.Logging;
using Newtonsoft.Json;
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

namespace GameOfRevenge.Business.Manager.UserData
{
    public class KingdomPvPManager : BaseUserDataManager//, IKingdomPvPManager
    {
        public static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly IUserHeroManager userHeroManager = new UserHeroManager();
        private readonly IUserTroopManager userTroopManager = new UserTroopManager();
        private readonly IUserMailManager mailManager = new UserMailManager();
        private readonly IAccountManager accManager = new AccountManager();
        private readonly IUserResourceManager resManager = new UserResourceManager();
        private readonly IUserStructureManager structManager = new UserStructureManager();

        private async Task<(bool, byte)> GetShieldActiveAndWatchTowerLevel(int playerId)
        {
            var task = await GetAllPlayerData(playerId);
            if (!task.IsSuccess || !task.HasData) throw new DataNotExistExecption(task.Message);

            var playerData = task.Data;
            bool shieldActive = false;
            foreach (var item in playerData)
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

            return (shieldActive, GetWatchLevel(playerData));
        }

        public static byte GetWatchLevel(List<PlayerDataTable> playerData)
        {
            byte watchLevel = 0;
            foreach (var item in playerData)
            {
                if (item.DataType != DataType.Structure) continue;

                var structureTable = CacheStructureDataManager.GetStructureTable(item.ValueId);
                if (structureTable.Code != StructureType.WatchTower) continue;

                try
                {
                    var watchTowers = JsonConvert.DeserializeObject<List<StructureDetails>>(item.Value);
                    if (watchTowers?.Count > 0)
                    {
                        watchLevel = (byte)watchTowers.Max(x => x.CurrentLevel);
                    }
                }
                catch { }
                break;
            }

            return watchLevel;
        }

        public async Task<byte> AttackOtherPlayer(PlayerCompleteData attackerData, MarchingArmy marchingArmy, MapLocation location, int defenderId)
        {
            if ((marchingArmy == null) || (marchingArmy.Troops == null) || (marchingArmy.Troops.Count == 0))
            {
                throw new RequirementExecption("Zero marching army was sended to attack");
            }

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
                ValidateArmyRequired(marchingArmy, attackerData, false);
//                    attackerData.MarchingArmy = army;
            }
            catch (DataNotExistExecption ex)
            {
                throw new RequirementExecption(ex.Message);
            }
            catch (Exception)
            {
                throw new RequirementExecption("User does not have required army");
            }

            var armyJson = JsonConvert.SerializeObject(marchingArmy);
            var response = await manager.AddOrUpdatePlayerData(attackerData.PlayerId, DataType.Marching, 1, armyJson);
            if (!response.IsSuccess) throw new Exception(response.Message);

            var report = GenerateAlertMail(attackerData, marchingArmy, location, watchLevel);
            var json = JsonConvert.SerializeObject(report);
            await mailManager.SendMail(defenderId, MailType.UnderAttack, json);

            report.StartTime = marchingArmy.StartTime;
            report.ReachedTime = marchingArmy.ReachedTime;
            report.DefenderId = defenderId;

            return watchLevel;
        }

        private UnderAttackReport GenerateAlertMail(PlayerCompleteData attackerData, MarchingArmy marchingArmy, MapLocation location, byte watchLevel)
        {
            var attackerMailInfo = new UnderAttackReport()
            {
                AttackerId = attackerData.PlayerId,
                AttackerUsername = attackerData.PlayerName,
                Location = location,
                StartTime = marchingArmy.StartTime.AddSeconds(marchingArmy.ReachedTime),
                KingLevel = attackerData.King.Level,
                WatchLevel = watchLevel
            };

            var troops = new List<TroopData>();
            foreach (var troop in marchingArmy.Troops)
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

            if ((marchingArmy.Heroes != null) && (marchingArmy.Heroes.Count > 0))
            {
                var heroDatas = new List<HeroData>();
                foreach (var item in marchingArmy.Heroes)
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

        public (BattlePower, BattlePower) PrepareBattleData(PlayerCompleteData attackerCompleteData, MarchingArmy marchingArmy, PlayerCompleteData defenderCompleteData)
        {
            log.Debug("------------ PREPARE BATTLE SIMULATION "+attackerCompleteData.PlayerId+" vs "+ defenderCompleteData.PlayerId);

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

            var attackerPower = new BattlePower(attackerCompleteData, marchingArmy, CacheTroopDataManager.GetFullTroopData, GetAtkDefMultiplier);
//            attackerPower.Multipliers = GetAtkDefMultiplier(attackerCompleteData, marchingArmy);

            var defenderPower = new BattlePower(defenderCompleteData, null, CacheTroopDataManager.GetFullTroopData, GetAtkDefMultiplier);
//            defenderPower.Multipliers = GetAtkDefMultiplier(defenderCompleteData, null);

            return (attackerPower, defenderPower);
        }

        public async Task<BattleReport> FinishBattleData(PlayerCompleteData attackerArmy, BattlePower attackerPower, PlayerCompleteData defenderArmy, BattlePower defenderPower, MarchingArmy marchingArmy)
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

            var attackerWon = attackerPower.HitPoints > defenderPower.HitPoints;
            var report = new BattleReport()
            {
                Attacker = attackerPower,
                Defender = defenderPower,
                AttackerWon = attackerWon
            };
            if (attackerWon) GiveLoot(defenderArmy, attackerPower, report);

            marchingArmy.Report = report;
            marchingArmy.TroopChanges = attackerPower.TroopChanges;
            var json = JsonConvert.SerializeObject(marchingArmy);
            var response = await manager.AddOrUpdatePlayerData(attackerPower.PlayerId, DataType.Marching, 1, json);
            if (!response.IsSuccess)
            {
                Console.WriteLine(response.Message);
//                    log.Debug(response.Message);
            }

            //SAVE DEFENDER REPORT
            log.Debug("ApplyDefenderChangesAndSendReport!!!");
            await ApplyDefenderChangesAndSendReport(report);

            log.Debug("------------FINISH BATTLE SIMULATION END " + attackerPower.PlayerId + " vs " + defenderPower.PlayerId);
            return report;
        }

        private void GiveLoot(PlayerCompleteData defenderArmy, BattlePower attackerPower, BattleReport finalReport)
        {
            long loadAmount = (long)(attackerPower.TotalLoad / 6);
            long oreLoad = loadAmount;
            long woodLoad = loadAmount * 2;
            long foodLoad = loadAmount * 3;

            long remain = 0;
            int ore;
            var defOre = defenderArmy.Resources.Ore;
            if (defOre > oreLoad)
            {
                ore = (int)oreLoad;
            }
            else
            {
                ore = (int)defOre;
                remain = oreLoad - defOre;
            }

            woodLoad += remain;
            remain = 0;
            int wood;
            var defWood = defenderArmy.Resources.Wood;
            if (defWood > woodLoad)
            {
                wood = (int)woodLoad;
            }
            else
            {
                wood = (int)defWood;
                remain = woodLoad - defWood;
            }

            foodLoad += remain;
            var defFood = defenderArmy.Resources.Food;
            var food = (defFood > foodLoad)? (int)foodLoad : (int)defFood;


            //            try
            {
                finalReport.Attacker.Food = food;// resp.Data[0].Value;
                finalReport.Attacker.Wood = wood;// resp.Data[1].Value;
                finalReport.Attacker.Ore = ore;// resp.Data[2].Value;

                finalReport.Defender.Food = -food;// resp.Data[0].Value;
                finalReport.Defender.Wood = -wood;// resp.Data[1].Value;
                finalReport.Defender.Ore = -ore;// resp.Data[2].Value;
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

        private async Task<bool> ApplyDefenderChangesAndSendReport(BattleReport report)
        {
            var defenderPower = (BattlePower)report.Defender;

            var defenderDataResp = await manager.GetAllPlayerData(defenderPower.PlayerId);
            log.Debug(">>> "+defenderDataResp.IsSuccess+"   "+defenderDataResp.HasData);
            if (!defenderDataResp.IsSuccess || !defenderDataResp.HasData) return false;

            string msg = report.AttackerWon ? "You failed to defend against enemy raid" : "You successfully defended against enemy siege";
            msg = JsonConvert.SerializeObject(report).Replace("INSERTMESSAGEHERE", msg);

            //SAVE PROCESS
            await ApplyTroopChanges(defenderPower.PlayerId, defenderDataResp.Data, defenderPower.TroopChanges, SAVE);
            if (SAVE)
            {
                log.Debug("--xxx2");
                var response = await structManager.UpdateGate(defenderPower.PlayerId, defenderPower.GateHP);
                if (!response.IsSuccess)
                {
                    Console.WriteLine(response.Message);
//                    log.Debug(response.Message);
                }

                log.Debug("--xxx3");
                var respResources = await resManager.SumMainResource(defenderPower.PlayerId,
                                                                    defenderPower.Food,
                                                                    defenderPower.Wood,
                                                                    defenderPower.Ore,
                                                                    0);
                log.Debug("--xxx4");
                if (!respResources.IsSuccess)
                {
                    Console.WriteLine(respResources.Message);
//                    log.Debug(respResources.Message);
                }

                try
                {
                    var respMail = await mailManager.SendMail(defenderPower.PlayerId, MailType.BattleReport, msg);
                    if (!respMail.IsSuccess)
                    {
                        Console.WriteLine(respMail.Message);
//                        log.Debug(respMail.Message);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
//                    log.Debug(ex.Message);
                }
            };

            return true;
        }

        List<TroopInfos> GetUserTroops(List<PlayerDataTable> data)
        {
            var troops = data.Where(x => (x.DataType == DataType.Troop));
            if (troops == null) return null;

            List<TroopInfos> userTroops = null;
            foreach (var item in troops)
            {
                if ((item == null) || (item.Value == null)) continue;

                var userTroop = PlayerData.PlayerDataToUserTroopData(item);
                foreach (TroopDetails troop in userTroop.Value)
                {
                    if (troop.InRecovery != null)
                    {
                        troop.InRecovery = troop.InRecovery.Where(x => (x.TimeLeft > 0)).ToList();
                        if (troop.InRecovery.Count == 0) troop.InRecovery = null;
                    }

                    if (troop.InTraning != null)
                    {
                        troop.InTraning = troop.InTraning.Where(x => (x.TimeLeft > 0)).ToList();
                        if (troop.InTraning.Count == 0) troop.InTraning = null;
                    }
                }

                if (userTroops == null) userTroops = new List<TroopInfos>();
                userTroops.Add(new TroopInfos(userTroop.Id, userTroop.ValueId, userTroop.Value));
            }

            return userTroops;
        }

        List<UserHeroDetails> GetUserHeroes(List<PlayerDataTable> data)
        {
            var heroes = data.Where(x => (x.DataType == DataType.Hero));
            if (heroes == null) return null;

            List<UserHeroDetails> userHeroes = null;
            foreach (var heroInfo in CacheHeroDataManager.HeroInfos)
            {
                //                            HeroType heroType = heroInfo.Info.Code.ToEnum<HeroType>();

                UserHeroDetails userHeroDetails = null;
                var userHero = heroes.FirstOrDefault(x => (x.ValueId == heroInfo.Info.HeroId));
                if (userHero != null)
                {
                    try
                    {
                        userHeroDetails = JsonConvert.DeserializeObject<UserHeroDetails>(userHero.Value);
                    }
                    catch { }
                }
                if (userHeroDetails == null) continue;

                var heroData = new UserHeroDetails()
                {
                    HeroType = (HeroType)heroInfo.Info.HeroId,
                    Points = userHeroDetails.Points,
                    Power = userHeroDetails.Power,
                    AttackCount = userHeroDetails.AttackCount,
                    AttackFail = userHeroDetails.AttackFail,
                    DefenseCount = userHeroDetails.DefenseCount,
                    DefenseFail = userHeroDetails.DefenseFail
                };
//                if (marchingHeroes != null)
//                {
//                    heroData.IsMarching = marchingHeroes.Exists(x => (x.ToString() == heroInfo.Info.Code));//(int)heroType);
//                }
                if (userHeroes == null) userHeroes = new List<UserHeroDetails>();
                userHeroes.Add(heroData);
            }

            return userHeroes;
        }

        public async Task<bool> ApplyAttackerChangesAndSendReport(Action<string> log, MarchingArmy marchingArmy)
        {
            var report = marchingArmy.Report;
            var attackerId = report.Attacker.PlayerId;

            log("1111111");
            bool attackerWon = report.AttackerWon;

            var attackerDataResp = await manager.GetAllPlayerData(attackerId);
            if (!attackerDataResp.IsSuccess || !attackerDataResp.HasData) return false;

            UserKingDetails king = null;
            string heroToAward = null;
            var customs = attackerDataResp.Data.Where(x => (x.DataType == DataType.Custom));
            if (customs != null)
            {
                var kingData = customs.FirstOrDefault(x => (x.ValueId == 1));
                if (kingData != null)
                {
                    try
                    {
                        king = JsonConvert.DeserializeObject<UserKingDetails>(kingData.Value);
                    }
                    catch { }

                    if (king != null)
                    {
                        log("7777");
                        king.Experience += 5;
                        if (king.TimeLeft <= 0) king.StartTime = DateTime.UtcNow;
                        king.Duration += 10 * 60;
                        king.BattleCount++;

                        if ((king.BattleCount % 3) == 0)
                        {
                            var idx = new Random().Next(0, CacheHeroDataManager.HeroInfos.Count);
                            var heroTable = CacheHeroDataManager.HeroInfos[idx].Info;

                            heroToAward = heroTable.Code;
                        }

                        log("888");
                    }
                }
            }

            var msg = attackerWon? "You successfully raided enemy city" : "You failed to raid the enemy city";
            msg = JsonConvert.SerializeObject(report).Replace("INSERTMESSAGEHERE", msg);

            log("9999");
            await ApplyTroopChanges(attackerId, attackerDataResp.Data, marchingArmy.TroopChanges, SAVE);

            log("6666");

            if ((marchingArmy.Heroes != null) && (marchingArmy.Heroes.Count > 0))
            {
                var userHeroes = GetUserHeroes(attackerDataResp.Data);
                if (userHeroes != null)
                {
                    foreach (var heroType in marchingArmy.Heroes)
                    {
                        var hero = userHeroes.Find(x => (x.HeroType == heroType));
                        hero.AttackCount++;
                        if (!attackerWon) hero.AttackFail++;
                        if (heroToAward != null)
                        {
                            heroToAward = null;
                            hero.Points++;
                        }

                        if (SAVE)
                        {
    //                        int valueId = CacheHeroDataManager.GetFullHeroData(hero.HeroType.ToString()).Info.HeroId;
                            var data = JsonConvert.SerializeObject(hero);
                            var response = await manager.AddOrUpdatePlayerData(attackerId, DataType.Hero, (int)heroType, data);
                            if (!response.IsSuccess)
                            {
                                Console.WriteLine(response.Message);
    //                            log.Debug(response.Message);
                            }
                        }
                    }
                }
            }

            if (SAVE)
            {
                if (heroToAward != null)
                {
                    log("bbbb");
                    var responseHero = await userHeroManager.AddHeroPoints(attackerId, heroToAward, 1);
                    if (!responseHero.IsSuccess)
                    {
                        Console.WriteLine(responseHero.Message);
                        //                    log.Debug(response.Message);
                    }
                }

                if (king != null)
                {
                    var kingJson = JsonConvert.SerializeObject(king);
                    var responseKing = await manager.AddOrUpdatePlayerData(attackerId, DataType.Custom, 1, kingJson);
                    if (!responseKing.IsSuccess)
                    {
                        Console.WriteLine(responseKing.Message);
                        //                        log.Debug(response.Message);
                    }
                }

                log("cccc");
                var respResources = await resManager.SumMainResource(attackerId,
                                                                report.Attacker.Food,
                                                                report.Attacker.Wood,
                                                                report.Attacker.Ore,
                                                                0);
                if (!respResources.IsSuccess)
                {
                    Console.WriteLine(respResources.Message);
//                    log.Debug(respResources.Message);
                }

                log("dddd");
                var response = await manager.AddOrUpdatePlayerData(attackerId, DataType.Marching, 1, string.Empty);
                if (!response.IsSuccess)
                {
                    Console.WriteLine(response.Message);
//                    log.Debug(response.Message);
                }

                try
                {
                    var respMail = await mailManager.SendMail(attackerId, MailType.BattleReport, msg);
                    if (!respMail.IsSuccess)
                    {
                        Console.WriteLine(respMail.Message);
//                        log.Debug(respMail.Message);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
//                    log.Debug(ex.Message);
                }
            }
            return true;
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

        private async Task ApplyTroopChanges(int playerId, List<PlayerDataTable> allPlayerData, List<TroopDetailsPvP> troopChanges, bool SAVE)
        {
            if (troopChanges == null) return;

            var troops = GetUserTroops(allPlayerData);
            foreach (var data in troops)
            {
                if (data == null) continue;

                bool reqUpdate = false;
                foreach (var troopChange in troopChanges)
                {
                    if ((troopChange == null) || (troopChange.TroopType != data.TroopType) ||
                        (troopChange.Dead == 0) && (troopChange.Wounded == 0)) continue;

                    var troopData = data.TroopData.Find(x => (x.Level == troopChange.Level));
                    if (troopData != null)
                    {
                        troopData.Count -= troopChange.Dead;
                        if (troopData.Count < 0) troopData.Count = 0;
                        troopData.Wounded += troopChange.Wounded;
                        reqUpdate = true;
                    }
                }

                if (reqUpdate && SAVE)
                {
                    try
                    {
                        var response = await userTroopManager.UpdateTroops(playerId, data.TroopType, data.TroopData);
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

        private void CalculateTroopLoss(BattlePower troopBattle, int infirmaryCapacity, bool healingBuff)
        {
            if (troopBattle.TroopChanges == null) return;

            var randm = new Random();
            foreach (var troop in troopBattle.TroopChanges)
            {
                int survived = (int)Math.Ceiling(troop.RemainUnits);
                troopBattle.TotalLoad += survived * troop.LoadPerUnit;
                troopBattle.TotalArmy += troop.InitialCount;

                var newWounded = 0;
                var newDeads = 0;
                var fallen = troop.InitialCount - survived;
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
                troop.Wounded = newWounded;
                troop.Dead = newDeads;
            }
        }

/*        private List<TroopInfos> GetAvailableTroops(List<TroopInfos> troops)
        {
            var validTroops = new List<TroopInfos>();
            if (troops != null)
            {
                foreach (var troop in troops)
                {
                    if ((troop == null) || (troop.TroopData == null) || (troop.TroopData.Count == 0)) continue;

                    List<TroopDetails> troopDatas = null;
                    foreach (var troopData in troop.TroopData)
                    {
                        if ((troopData == null) || (troopData.Count < 1)) continue;

                        if (troopDatas == null)
                        {
                            troopDatas = new List<TroopDetails>();
                            validTroops.Add(new TroopInfos(troop.Id, troop.TroopType, troopDatas));
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
        }*/

        public static void ValidateArmyRequired(MarchingArmy army, PlayerCompleteData data, bool applyChanges = true)
        {
            var playerTroops = data.Troops;
            if (playerTroops == null) throw new DataNotExistExecption("User does not have troops");

            if (army.Heroes != null)
            {
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
            }

            List<KeyValuePair<TroopDetails, int>> troopsToUpdate = null;
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

                    if (applyChanges)
                    {
                        if (troopsToUpdate == null) troopsToUpdate = new List<KeyValuePair<TroopDetails, int>>();
                        troopsToUpdate.Add(new KeyValuePair<TroopDetails, int>(troopData, item.Count));
                    }
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

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
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Boost;
using GameOfRevenge.Common.Models.Hero;
using GameOfRevenge.Common.Models.Kingdom;
using GameOfRevenge.Common.Models.Kingdom.AttackAlertReport;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Models.PlayerData;

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

            var marchingArmies = attackerData.MarchingArmies;
            if (marchingArmies != null)
            {
                var count = marchingArmies.Sum(x => (x.TimeLeft > 0) ? 1 : 0);
                if (count >= 10)
                {
                    throw new RequirementExecption("Maximum ten army marching are allowed");
                }
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

            marchingArmy.MarchingType = MarchingType.AttackPlayer;
            var armyJson = JsonConvert.SerializeObject(marchingArmy.Base());
            var slot = 1;
            if (marchingArmies != null)
            {
                var found = false;
                for (int idx = 1; idx <= 10; idx++)
                {
                    if (marchingArmies.Exists(x => (x.MarchingSlot == idx))) continue;

                    slot = idx;
                    found = true;
                    break;
                }
                if (!found) throw new RequirementExecption("Maximum ten army marching are allowed.");
            }
            var response = await manager.AddOrUpdatePlayerData(attackerData.PlayerId, DataType.Marching, slot, armyJson);
            if (!response.IsSuccess) throw new Exception(response.Message);

            marchingArmy.MarchingId = response.Data.Id;
            var report = GenerateAlertMail(attackerData, marchingArmy, location, watchLevel);
            var json = JsonConvert.SerializeObject(report);
            await mailManager.SendMail(defenderId, MailType.UnderAttack, json);

//            report.StartTime = marchingArmy.StartTime;
//            report.ReachedTime = marchingArmy.Distance;
//            report.DefenderId = defenderId;

            return watchLevel;
        }

        public async Task AttackMonster(PlayerCompleteData attackerData, MarchingArmy marchingArmy)
        {
            if ((marchingArmy == null) || (marchingArmy.Troops == null) || (marchingArmy.Troops.Count == 0))
            {
                throw new RequirementExecption("Zero marching army was sended to attack");
            }

            if ((attackerData.Troops == null) || (attackerData.Troops.Count == 0))
            {
                throw new RequirementExecption("User does not have any army");
            }

            var marchingArmies = attackerData.MarchingArmies;
            if (marchingArmies != null)
            {
                var count = marchingArmies.Sum(x => (x.TimeLeft > 0) ? 1 : 0);
                if (count >= 10)
                {
                    throw new RequirementExecption("Maximum ten army marching are allowed");
                }
            }

//            (bool shieldActive, byte watchLevel) = await GetShieldActiveAndWatchTowerLevel(defenderId);
//            if (shieldActive) throw new RequirementExecption("Defender has shield activated");

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

            marchingArmy.MarchingType = MarchingType.AttackMonster;
            var armyJson = JsonConvert.SerializeObject(marchingArmy.Base());
            var slot = 1;
            if (marchingArmies != null)
            {
                var found = false;
                for (int idx = 1; idx <= 10; idx++)
                {
                    if (marchingArmies.Exists(x => (x.MarchingSlot == idx))) continue;

                    slot = idx;
                    found = true;
                    break;
                }
                if (!found) throw new RequirementExecption("Maximum ten army marching are allowed.");
            }
            var response = await manager.AddOrUpdatePlayerData(attackerData.PlayerId, DataType.Marching, slot, armyJson);
            if (!response.IsSuccess) throw new Exception(response.Message);

            marchingArmy.MarchingId = response.Data.Id;
//            var report = GenerateAlertMail(attackerData, marchingArmy, location, watchLevel);
//            var json = JsonConvert.SerializeObject(report);
//            await mailManager.SendMail(defenderId, MailType.UnderAttack, json);

//            report.StartTime = marchingArmy.StartTime;
//            report.ReachedTime = marchingArmy.ReachedTime;
//            report.DefenderId = defenderId;

//            return watchLevel;
        }

        public async Task SendReinforcement(PlayerCompleteData attackerData, MarchingArmy marchingArmy)
        {
            if ((marchingArmy == null) || (marchingArmy.Troops == null) || (marchingArmy.Troops.Count == 0))
            {
                throw new RequirementExecption("Zero marching army was sended");
            }

            if ((attackerData.Troops == null) || (attackerData.Troops.Count == 0))
            {
                throw new RequirementExecption("User does not have any army");
            }

            var marchingArmies = attackerData.MarchingArmies;
            if (marchingArmies != null)
            {
                var count = marchingArmies.Sum(x => (x.TimeLeft > 0) ? 1 : 0);
                if (count >= 10)
                {
                    throw new RequirementExecption("Maximum ten army marching are allowed");
                }
            }

            try
            {
                ValidateArmyRequired(marchingArmy, attackerData, false);
            }
            catch (DataNotExistExecption ex)
            {
                throw new RequirementExecption(ex.Message);
            }
            catch (Exception)
            {
                throw new RequirementExecption("User does not have required army");
            }

            marchingArmy.MarchingType = MarchingType.ReinforcementPlayer;
            var armyJson = JsonConvert.SerializeObject(marchingArmy.Base());
            var slot = 1;
            if (marchingArmies != null)
            {
                var found = false;
                for (int idx = 1; idx <= 10; idx++)
                {
                    if (marchingArmies.Exists(x => (x.MarchingSlot == idx))) continue;

                    slot = idx;
                    found = true;
                    break;
                }
                if (!found) throw new RequirementExecption("Maximum ten army marching are allowed.");
            }
            var response = await manager.AddOrUpdatePlayerData(attackerData.PlayerId, DataType.Marching, slot, armyJson);
            if (!response.IsSuccess) throw new Exception(response.Message);

            marchingArmy.MarchingId = response.Data.Id;
        }

        private UnderAttackMail GenerateAlertMail(PlayerCompleteData attackerData, MarchingArmy marchingArmy, MapLocation location, byte watchLevel)
        {
            var attackerMailInfo = new UnderAttackMail()
            {
                AttackerId = attackerData.PlayerId,
                AttackerUsername = attackerData.PlayerName,
                Location = location,
                StartTime = marchingArmy.StartTime.AddSeconds(marchingArmy.Distance),
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

        public async Task<BattleReport> FinishBattleData(PlayerCompleteData attackerArmy, BattlePower attackerPower, PlayerCompleteData defenderArmy, BattlePower defenderPower, MarchingArmy marchingArmy)
        {
            log.Debug("------------FINISH BATTLE SIMULATION " + attackerPower.PlayerId + " vs " + defenderPower.PlayerId);
            //TODO: implement atkHealingBoost, defHealingBoost percentage based on level (maybe we need to add level to item)
            var atkHealingBoost = attackerArmy.Boosts.Exists(x => (x.Type == NewBoostType.LifeSaver) && (x.TimeLeft > 0));
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

            if (defenderArmy != null)
            {
                var defHealingBoost = defenderArmy.Boosts.Exists(x => (x.Type == NewBoostType.LifeSaver) && (x.TimeLeft > 0));
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
            }

            var attackerWon = attackerPower.HitPoints > defenderPower.HitPoints;
            var report = new BattleReport()
            {
                Attacker = attackerPower,
                Defender = defenderPower,
                WinnerId = attackerWon? attackerPower.PlayerId : defenderPower.PlayerId
            };
            if (attackerWon)
            {
                var heroReward = ((attackerArmy.King.BattleCount + 1) % 3) == 0;
                GiveLoot(defenderArmy, attackerPower, report, heroReward);
            }

            marchingArmy.Report = report;
            marchingArmy.TroopChanges = attackerPower.TroopChanges;
            var json = JsonConvert.SerializeObject(marchingArmy.Base());
            var response = await manager.UpdatePlayerDataID(attackerPower.PlayerId, marchingArmy.MarchingId, json);
//                AddOrUpdatePlayerData(attackerPower.PlayerId, DataType.Marching, 1, json);
            if (!response.IsSuccess)
            {
                Console.WriteLine(response.Message);
                //                    log.Debug(response.Message);
            }

            //SAVE DEFENDER REPORT
            if (defenderArmy != null)
            {
                log.Debug("ApplyDefenderChangesAndSendReport!!!");
                await ApplyDefenderChangesAndSendReport(report);
            }

            log.Debug("------------FINISH BATTLE SIMULATION END " + attackerPower.PlayerId + " vs " + defenderPower.PlayerId);
            return report;
        }

        public struct BattleItem
        {
            public int Id { get; set; }
            public DataType DataType { get; set; }
            public int ItemType { get; set; }
            public int Value { get; set; }
            public int Count { get; set; }

/*            public BattleItem(BattleItem other)
            {
                Id = other.Id;
                DataType = other.DataType;
                ItemType = other.ItemType;
                Value = other.Value;
                Count = other.Count;
            }*/

            public BattleItem(int id, DataType dataType, int itemType, int value, int count)
            {
                Id = id;
                DataType = dataType;
                ItemType = itemType;
                Value = value;
                Count = count;
            }

            public static BattleItem NewItemResource(int id, ResourceType itemType, int value, int count = 1)
            {
                return new BattleItem(id, DataType.Resource, (int)itemType, value, count);
            }

            public static BattleItem NewItemCustom(int id, int itemType, int value, int count = 1)
            {
                return new BattleItem(id, DataType.Custom, itemType, value, count);
            }
        }

        private void GiveLoot(PlayerCompleteData defenderArmy, BattlePower attackerPower, BattleReport report, bool heroReward)
        {
            long loadAmount = (long)(attackerPower.TotalLoad / 6);
            long oreLoad = loadAmount;
            long woodLoad = loadAmount * 2;
            long foodLoad = loadAmount * 3;

            long remain = 0;
            int ore;
            var defOre = (defenderArmy != null)? defenderArmy.Resources.Ore : 500000;
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
            var defWood = (defenderArmy != null)? defenderArmy.Resources.Wood : 1000000;
            if (defWood > woodLoad)
            {
                wood = (int)woodLoad;
                defWood -= wood;
            }
            else
            {
                wood = (int)defWood;
                remain = woodLoad - defWood;
            }

            foodLoad += remain;
            int food;
            var defFood = (defenderArmy != null)? defenderArmy.Resources.Food : 2000000;
            if (defFood > foodLoad)
            {
                food = (int)foodLoad;
            }
            else
            {
                food = (int)defFood;
                remain = foodLoad - defFood;
                if (remain > 0)
                {
                    wood += (defWood > remain) ? (int)remain : (int)defWood;
                }
            }
            if (food < 300) food = 300;
            if (wood < 300) wood = 300;


            var items = new BattleItem[]{BattleItem.NewItemResource(347, ResourceType.Food, 1000000),
                                            BattleItem.NewItemResource(348, ResourceType.Food, 100000),
                                            BattleItem.NewItemResource(349, ResourceType.Food, 10000),
                                            BattleItem.NewItemResource(350, ResourceType.Food, 1000),
                                            BattleItem.NewItemResource(351, ResourceType.Food, 300)};
            var rewards = new List<DataReward>();
            var foodItems = Split(items, food, 2);
            food = 0;
            foreach (var item in foodItems)
            {
                rewards.Add(new DataReward(item.Id, 0, item.DataType, item.ItemType, item.Value, item.Count));
                food += (item.Value * item.Count);
            }

            items = new BattleItem[]{BattleItem.NewItemResource(367, ResourceType.Wood, 1000000),
                                            BattleItem.NewItemResource(352, ResourceType.Wood, 100000),
                                            BattleItem.NewItemResource(353, ResourceType.Wood, 10000),
                                            BattleItem.NewItemResource(354, ResourceType.Wood, 1000),
                                            BattleItem.NewItemResource(355, ResourceType.Wood, 300)};
            var woodItems = Split(items, wood, 2);
            wood = 0;
            foreach (var item in woodItems)
            {
                rewards.Add(new DataReward(item.Id, 0, item.DataType, item.ItemType, item.Value, item.Count));
                wood += (item.Value * item.Count);
            }

            items = new BattleItem[]{BattleItem.NewItemResource(356, ResourceType.Ore, 10000),
                                            BattleItem.NewItemResource(357, ResourceType.Ore, 1000),
                                            BattleItem.NewItemResource(358, ResourceType.Ore, 300),
                                            BattleItem.NewItemResource(359, ResourceType.Ore, 100)};
            var oreItems = Split(items, ore, 2);
            ore = 0;
            foreach (var item in oreItems)
            {
                rewards.Add(new DataReward(item.Id, 0, item.DataType, item.ItemType, item.Value, item.Count));
                ore += (item.Value * item.Count);
            }

            items = new BattleItem[]{BattleItem.NewItemCustom(360, 1, 10000),
                                            BattleItem.NewItemCustom(361, 1, 1000),
                                            BattleItem.NewItemCustom(362, 1, 100),
                                            BattleItem.NewItemCustom(363, 1, 10)};
            var maxRewardValue = 10;
            var kingExpItems = Split(items, maxRewardValue, 2);
            foreach (var item in kingExpItems)//king experience 10
            {
                rewards.Add(new DataReward(item.Id, 0, item.DataType, item.ItemType, item.Value, item.Count));
            }

            if (heroReward)
            {
                rewards.Add(new DataReward(366, 0, DataType.Custom, 4, 1, 1));//hero points 1
            }
            report.Attacker.Items = rewards;

            if (defenderArmy != null)
            {
                rewards = new List<DataReward>();
                if (food != 0) rewards.Add(new DataReward(0, 0, DataType.Resource, (int)ResourceType.Food, -food, 1));
                if (wood != 0) rewards.Add(new DataReward(0, 0, DataType.Resource, (int)ResourceType.Wood, -wood, 1));
                if (ore != 0) rewards.Add(new DataReward(0, 0, DataType.Resource, (int)ResourceType.Ore, -ore, 1));
                report.Defender.Items = rewards;
            }
        }

        private List<BattleItem> Split(BattleItem[] list, long amount, int maxSlots)
        {
            Array.Sort(list, (a, b) => b.Value.CompareTo(a.Value));

            var slots = new List<BattleItem>();
            long remain = amount;

            foreach (var entry in list)
            {
                int quantity = (int)(remain / entry.Value);
                if (quantity > 0)
                {
                    var newEntry = entry;
                    newEntry.Count = quantity;
                    slots.Add(newEntry);
                }

                remain -= quantity * entry.Value;
            }

            while ((slots.Count > 1) && (slots.Count > maxSlots))
            {
                int total = (slots[0].Count * slots[0].Value) + (slots[1].Count * slots[1].Value);
                var slot = slots[1];
                slot.Count = total / slots[1].Value;
                slots[1] = slot;
                remain += total % slots[1].Value;
                slots.RemoveAt(0);
            }
//            if (remain > 0) slots.Add(new int[] { 1, remain });

            return slots;
        }

        public async Task<bool> ApplyReinforcementChanges(MarchingArmy marchingRequest, PlayerCompleteData playerData, PlayerCompleteData targetPlayerData)
        {
            var success = true;
            var message = playerData.PlayerName + " sent you reinforcements\n";
            foreach (var marchingTroop in marchingRequest.Troops)
            {
                var troopType = marchingTroop.TroopType;
                try
                {
                    //Remove troops
                    var troops = playerData.Troops.Find(x => (x.TroopType == troopType));
                    var troopDataList = troops.TroopData;
                    foreach (var troopGroup in marchingTroop.TroopData)
                    {
                        var troopData = troopDataList.Find(x => (x.Level == troopGroup.Level));
                        troopData.Count -= troopGroup.Count;
                    }
                    await userTroopManager.UpdateTroops(playerData.PlayerId, troopType, troopDataList);

                    //Add troops
                    var total = 0;
                    troops = targetPlayerData.Troops.Find(x => (x.TroopType == troopType));
                    troopDataList = ((troops != null) && (troops.TroopData != null)) ? troops.TroopData : new List<TroopDetails>();
                    foreach (var troopGroup in marchingTroop.TroopData)
                    {
                        var troopLevel = troopGroup.Level;
                        var troopData = troopDataList.Find(x => (x.Level == troopLevel));
                        if (troopData == null)
                        {
                            troopData = new TroopDetails() { Level = troopLevel };
                            troopDataList.Add(troopData);
                        }
                        troopData.Count += troopGroup.Count;
                        total += troopGroup.Count;
                    }
                    await userTroopManager.UpdateTroops(targetPlayerData.PlayerId, troopType, troopDataList);
                    message += "\n" + troopType.ToString() + " x " + total;
                }
                catch (Exception ex)
                {
                    log.InfoFormat("Exception transfering troops {0} {1} ", troopType, ex.Message);
                    success = false;
                }
            }
            await SendSimpleMail(targetPlayerData.PlayerId, "Reinforcements Arrived", message);

            return success;
        }

        public async Task SendSimpleMail(int targetPlayerId, string subject, string message)
        {
            try
            {
                var mail = new MailMessage()
                {
                    Subject = subject,
                    Message = message
                };
                var json = JsonConvert.SerializeObject(mail);
                await mailManager.SendMail(targetPlayerId, MailType.Message, json);
            }
            catch { }
        }

        private async Task<bool> ApplyDefenderChangesAndSendReport(BattleReport report)
        {
            var defenderPower = (BattlePower)report.Defender;

            var defenderDataResp = await manager.GetAllPlayerData(defenderPower.PlayerId);
            log.Debug(">>> "+defenderDataResp.IsSuccess+"   "+defenderDataResp.HasData);
            if (!defenderDataResp.IsSuccess || !defenderDataResp.HasData) return false;

            var msg = JsonConvert.SerializeObject(report);

            //SAVE PROCESS
            await ApplyTroopChanges(defenderPower.PlayerId, defenderDataResp.Data, defenderPower.TroopChanges, SAVE);
            if (SAVE)
            {
                var response = await structManager.UpdateGate(defenderPower.PlayerId, defenderPower.GateHP);
                if (!response.IsSuccess)
                {
                    log.Debug(response.Message);
                }

                var food = 0;
                var wood = 0;
                var ore = 0;
                foreach (var item in defenderPower.Items)
                {
                    if (item.DataType != DataType.Resource) continue;

                    switch ((ResourceType)item.ValueId)
                    {
                        case ResourceType.Food: food += item.Value * item.Count; break;
                        case ResourceType.Wood: wood += item.Value * item.Count; break;
                        case ResourceType.Ore: ore += item.Value * item.Count; break;
                    }
                }
                var respResources = await resManager.SumMainResource(defenderPower.PlayerId, food, wood, ore, 0, 0);
                if (!respResources.IsSuccess)
                {
                    log.Debug(respResources.Message);
                }

                try
                {
                    var respMail = await mailManager.SendMail(defenderPower.PlayerId, MailType.BattleReport, msg);
                    if (!respMail.IsSuccess)
                    {
                        log.Debug(respMail.Message);
                    }
                }
                catch (Exception ex)
                {
                    log.Debug(ex.Message);
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
//                    HeroId = userHeroDetails.HeroId,
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

        public async Task<bool> ApplyAttackerChangesAndSendReport(MarchingArmy marchingArmy)
        {
            var report = marchingArmy.Report;
            var attackerId = report.Attacker.PlayerId;

            var attackerDataResp = await manager.GetAllPlayerData(attackerId);
            if (!attackerDataResp.IsSuccess || !attackerDataResp.HasData) return false;

            UserKingDetails king = null;
//            string heroToAward = null;
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
//                        king.Experience += 5;
                        if (king.TimeLeft <= 0) king.StartTime = DateTime.UtcNow;
                        king.Duration += 10 * 60;
                        king.BattleCount++;

/*                        if ((king.BattleCount % 3) == 0)
                        {
                            var idx = new Random().Next(0, CacheHeroDataManager.HeroInfos.Count);
                            var heroTable = CacheHeroDataManager.HeroInfos[idx].Info;

                            heroToAward = heroTable.Code;
                        }*/
                    }
                }
            }

            var msg = JsonConvert.SerializeObject(report);

            await ApplyTroopChanges(attackerId, attackerDataResp.Data, marchingArmy.TroopChanges, SAVE);

            if ((marchingArmy.Heroes != null) && (marchingArmy.Heroes.Count > 0))
            {
                var userHeroes = GetUserHeroes(attackerDataResp.Data);
                if (userHeroes != null)
                {
                    bool attackerWon = report.AttackerWon;
                    foreach (var heroType in marchingArmy.Heroes)
                    {
                        var hero = userHeroes.Find(x => (x.HeroType == heroType));
                        hero.AttackCount++;
                        if (!attackerWon) hero.AttackFail++;
/*                        if (heroToAward != null)
                        {
                            heroToAward = null;
                            hero.Points++;
                        }*/

                        if (SAVE)
                        {
    //                        int valueId = CacheHeroDataManager.GetFullHeroData(hero.HeroType.ToString()).Info.HeroId;
                            var data = JsonConvert.SerializeObject(hero);
                            var response = await manager.AddOrUpdatePlayerData(attackerId, DataType.Hero, (int)heroType, data);
                            if (!response.IsSuccess)
                            {
//                                log.Debug(response.Message);
                            }
                        }
                    }
                }
            }

            if (SAVE)
            {
/*                if (heroToAward != null)
                {
                    var responseHero = await userHeroManager.AddHeroPoints(attackerId, heroToAward, 1);
                    if (!responseHero.IsSuccess)
                    {
                        log.Debug(response.Message);
                    }
                }*/

                if (king != null)
                {
                    var kingJson = JsonConvert.SerializeObject(king);
                    var responseKing = await manager.AddOrUpdatePlayerData(attackerId, DataType.Custom, 1, kingJson);
                    if (!responseKing.IsSuccess)
                    {
                        log.Debug(responseKing.Message);
                    }
                }

/*                if ((report.Attacker.Food + report.Attacker.Wood + report.Attacker.Ore) > 0)
                {
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
                }*/
                if (report.Attacker.Items != null)
                {
                    foreach (var item in report.Attacker.Items)
                    {
                        log.Debug("reward = " + item.RewardId + "  " + item.DataType+" "+ item.ValueId +"  "+ item.Value + "  " + item.Count);
                    }
                    await UserQuestManager.CollectRewards(report.Attacker.PlayerId, report.Attacker.Items);
                }

/*                var response = await manager.AddOrUpdatePlayerData(attackerId, DataType.Marching, 1, string.Empty);
                if (!response.IsSuccess)
                {
                    log.Debug(response.Message);
                }*/

                try
                {
                    var respMail = await mailManager.SendMail(attackerId, MailType.BattleReport, msg);
                    if (!respMail.IsSuccess)
                    {
                        log.Debug(respMail.Message);
                    }
                }
                catch (Exception ex)
                {
                    log.Debug(ex.Message);
                }
            }
            return true;
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
                    if ((troopData == null) || (troopData.FinalCount < item.Count))
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

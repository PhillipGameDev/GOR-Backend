using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExitGames.Logging;
using Newtonsoft.Json;
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
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Business.Manager.Kingdom;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Business.Manager.GameDef;

namespace GameOfRevenge.Business.Manager.UserData
{
    public class KingdomPvPManager : BaseUserDataManager//, IKingdomPvPManager
    {
        public static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly IUserTroopManager userTroopManager = new UserTroopManager();
        private readonly IUserMailManager mailManager = new UserMailManager();
        private readonly IUserResourceManager resManager = new UserResourceManager();
        private readonly IUserStructureManager structManager = new UserStructureManager();
        public readonly IKingdomManager kingdomManager = new KingdomManager();
        public readonly IMonsterManager monsterManager = new MonsterManager();

        private Random randm = new Random();

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
        
        public async Task AttackGloryKingdom(PlayerCompleteData attackerData, MarchingArmy marchingArmy)
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

            marchingArmy.MarchingType = MarchingType.AttackGloryKingdom;
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
                AttackerName = attackerData.PlayerName,
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

        public async Task<BattleReport> FinishBattleData(PlayerCompleteData attackerData, BattlePower attackerPower,
                                                        PlayerCompleteData defenderData, BattlePower defenderPower,
                                                        MarchingArmy marchingArmy, BattleReplay replay)
        {
            log.Debug("------------FINISH BATTLE SIMULATION " + attackerPower.EntityId + " vs " + defenderPower.EntityId);
            //TODO: implement atkHealingBoost, defHealingBoost percentage based on level (maybe we need to add level to item)
            var atkHealingBoost = false;
            var attackerInfirmaryCapacity = 0;
            if (marchingArmy.MarchingType != MarchingType.AttackGloryKingdom)
            {
                atkHealingBoost = attackerData.Boosts.Exists(x => (x.Type == NewBoostType.LifeSaver) && (x.TimeLeft > 0));
                var attackerInfirmaryExist = attackerData.Structures.Exists(x => (x.StructureType == StructureType.Infirmary) && (x.Buildings.Count > 0));
                if (attackerInfirmaryExist)
                {
                    var wounded = userTroopManager.GetCurrentPopulationWounded(attackerData.Troops);
                    var capacity = structManager.GetMaxInfirmaryCapacity(attackerData.Structures);
                    attackerInfirmaryCapacity = capacity - wounded;
                }
            }
            CalculateTroopLoss(attackerPower, attackerInfirmaryCapacity, atkHealingBoost);

            if ((marchingArmy.MarchingType == MarchingType.AttackPlayer) || 
                (marchingArmy.MarchingType == MarchingType.AttackGloryKingdom))
            {
                var defHealingBoost = false;
                var defenderInfirmaryCapacity = 0;
                if (defenderData?.Boosts != null)
                {
                    defHealingBoost = defenderData.Boosts.Exists(x => (x.Type == NewBoostType.LifeSaver) && (x.TimeLeft > 0));
                }
                if (defenderData?.Structures != null)
                {
                    var defenderInfirmaryExist = defenderData.Structures.Exists(x => (x.StructureType == StructureType.Infirmary) && (x.Buildings.Count > 0));
                    if (defenderInfirmaryExist)
                    {
                        var wounded = userTroopManager.GetCurrentPopulationWounded(defenderData.Troops);
                        var capacity = structManager.GetMaxInfirmaryCapacity(defenderData.Structures);
                        defenderInfirmaryCapacity = capacity - wounded;
                    }
                }
                CalculateTroopLoss(defenderPower, defenderInfirmaryCapacity, defHealingBoost);
            }

            var attackerWon = attackerPower.HitPoints > defenderPower.HitPoints;
            var report = new BattleReport()
            {
                Attacker = attackerPower,
                Defender = defenderPower,
                WinnerId = attackerWon? attackerPower.EntityId : defenderPower.EntityId
            };
            if (attackerWon)
            {
                var heroReward = ((attackerData.King.BattleCount + 1) % 3) == 0;
                if ((marchingArmy.MarchingType == MarchingType.AttackMonster) ||
                    (marchingArmy.MarchingType == MarchingType.AttackGloryKingdom))
                {
                    GiveLoot(null, attackerPower, report, heroReward);
                }
                else
                {
                    GiveLoot(defenderData, attackerPower, report, heroReward);
                }
            }
            else if (attackerPower.Wounded == 0)
            {
                marchingArmy.Duration = 0;
            }

            marchingArmy.Report = report;
            marchingArmy.TroopChanges = attackerPower.TroopChanges;
            var json = JsonConvert.SerializeObject(marchingArmy.Base());
            var response = await manager.UpdatePlayerDataID(attackerPower.EntityId, marchingArmy.MarchingId, json);
            if (!response.IsSuccess) log.Debug("Error updating marching data "+response.Message);

            replay.Attacker = report.Attacker;
            replay.Defender = report.Defender;
            replay.WinnerId = report.WinnerId;

            json = JsonConvert.SerializeObject(replay);
            var resp = await manager.AddBattleHistory(attackerPower.EntityId, true, json);
            if (!resp.IsSuccess) log.Debug("Error inserting attacker battle history " + response.Message);

            report.Attacker.ReplayDataId = resp.Data.Id;

            if (marchingArmy.MarchingType == MarchingType.AttackPlayer)
            {
                json = JsonConvert.SerializeObject(replay);
                resp = await manager.AddBattleHistory(defenderPower.EntityId, false, json);
                if (!resp.IsSuccess) log.Debug("Error inserting defender battle history " + resp.Message);

                report.Defender.ReplayDataId = resp.Data.Id;
            }

            //SAVE DEFENDER REPORT
            if (marchingArmy.MarchingType == MarchingType.AttackPlayer)
            {
                log.Debug("ApplyDefenderChangesAndSendReport!!!");
                await ApplyDefenderChangesAndSendReport(report);
            }
            else if (marchingArmy.MarchingType == MarchingType.AttackMonster)
            {
                log.Debug("ApplyMonsterChanges!!");
                await ApplyMonsterChanges(report);
            }
            else if (marchingArmy.MarchingType == MarchingType.AttackGloryKingdom)
            {
                log.Debug("ApplyGloryKingdomChanges!!");
                await ApplyGloryKingdomChanges(report);
            }

            log.Debug("------------FINISH BATTLE SIMULATION END " + attackerPower.EntityId + " vs " + defenderPower.EntityId);
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

        private void GiveLoot(PlayerCompleteData defenderData, BattlePower attackerPower, BattleReport report, bool heroReward)
        {
            const int minAmount = 1000;
            long loadAmount = (long)(attackerPower.TotalLoad / 6);
            long oreLoad = loadAmount;
            long woodLoad = loadAmount * 2;
            long foodLoad = loadAmount * 3;

            long remain = 0;
            int ore;
            var defOre = (defenderData != null)? defenderData.Resources.Ore : 500000;
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
            var defWood = (defenderData != null)? defenderData.Resources.Wood : 1000000;
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
            var defFood = (defenderData != null)? defenderData.Resources.Food : 2000000;
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
            if (food < minAmount) food = minAmount;
            if (wood < minAmount) wood = minAmount;


            var items = new BattleItem[]{   BattleItem.NewItemResource(5, ResourceType.Food, 10000000),
                                            BattleItem.NewItemResource(4, ResourceType.Food, 1000000),
                                            BattleItem.NewItemResource(3, ResourceType.Food, 100000),
                                            BattleItem.NewItemResource(2, ResourceType.Food, 10000),
                                            BattleItem.NewItemResource(1, ResourceType.Food, 1000)};
            var rewards = new List<ItemData>();
            var foodItems = Split(items, food, 2);
            foreach (var item in foodItems)
            {
                rewards.Add(new ItemData(item.Id, item.Count));
            }

            items = new BattleItem[]{       BattleItem.NewItemResource(10, ResourceType.Wood, 10000000),
                                            BattleItem.NewItemResource(9, ResourceType.Wood, 1000000),
                                            BattleItem.NewItemResource(8, ResourceType.Wood, 100000),
                                            BattleItem.NewItemResource(7, ResourceType.Wood, 10000),
                                            BattleItem.NewItemResource(6, ResourceType.Wood, 1000)};
            var woodItems = Split(items, wood, 2);
            foreach (var item in woodItems)
            {
                rewards.Add(new ItemData(item.Id, item.Count));
            }

            items = new BattleItem[]{BattleItem.NewItemResource(15, ResourceType.Ore, 1000000),
                                            BattleItem.NewItemResource(14, ResourceType.Ore, 100000),
                                            BattleItem.NewItemResource(13, ResourceType.Ore, 10000),
                                            BattleItem.NewItemResource(12, ResourceType.Ore, 1000),
                                            BattleItem.NewItemResource(11, ResourceType.Ore, 100)};
            var oreItems = Split(items, ore, 2);
            foreach (var item in oreItems)
            {
                rewards.Add(new ItemData(item.Id, item.Count));
            }

            var resourceCount = rewards.Count;

            items = new BattleItem[]{       BattleItem.NewItemCustom(103, 1, 100000),
                                            BattleItem.NewItemCustom(102, 1, 10000),
                                            BattleItem.NewItemCustom(101, 1, 1000),
                                            BattleItem.NewItemCustom(100, 1, 100)};
            var maxRewardValue = 100;
            var kingExpItems = Split(items, maxRewardValue, 2);
            foreach (var item in kingExpItems)//king experience 10
            {
                rewards.Add(new ItemData(item.Id, item.Count));
            }

            if (heroReward)
            {
                rewards.Add(new ItemData(115, 1));
            }
            report.Attacker.Items = rewards;

            if (defenderData != null)
            {
                report.Defender.Items = rewards.Take(resourceCount).Select(e => new ItemData(e.ItemId, -e.Count)).ToList();
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
            await SendSimpleMail(playerData, targetPlayerData.PlayerId, "Reinforcements Arrived", message);

            return success;
        }

        public async Task SendSimpleMail(PlayerCompleteData fromPlayer, int targetPlayerId, string subject, string message)
        {
            try
            {
                var mail = new MailMessage()
                {
                    Subject = subject,
                    Message = message,
                    SenderId = fromPlayer.PlayerId,
                    SenderName = fromPlayer.PlayerName
                };
                var json = JsonConvert.SerializeObject(mail);
                await mailManager.SendMail(targetPlayerId, MailType.Message, json);
            }
            catch { }
        }

        private async Task<bool> ApplyDefenderChangesAndSendReport(BattleReport report)
        {
            var defenderPower = (BattlePower)report.Defender;

            var defenderId = defenderPower.EntityId;
            var defenderDataResp = await manager.GetAllPlayerData(defenderId);
            if (!defenderDataResp.IsSuccess || !defenderDataResp.HasData) return false;

            var msg = JsonConvert.SerializeObject(report);

            //SAVE PROCESS
            var playerTroops = GetUserPlayerTroops(defenderId, defenderDataResp.Data);
            try
            {
                await ApplyTroopChanges(playerTroops, defenderPower.TroopChanges, SAVE);
            } catch (Exception exp)
            {
                log.Error(exp.Message);
            }
            if (SAVE)
            {
                var response = await structManager.UpdateGate(defenderId, defenderPower.GateHP);
                if (!response.IsSuccess)
                {
                    log.Debug(response.Message);
                }

                if (defenderPower.Items != null)
                {
                    var AllItems = CacheItemManager.AllItems;

                    var food = 0;
                    var wood = 0;
                    var ore = 0;
                    foreach (var item in defenderPower.Items)
                    {
                        var itemInfo = AllItems.FirstOrDefault(e => e.Id == item.ItemId);

                        if (itemInfo == null || itemInfo.DataType != DataType.Resource) continue;

                        switch ((ResourceType)itemInfo.ValueId)
                        {
                            case ResourceType.Food: food += itemInfo.Value * item.Count; break;
                            case ResourceType.Wood: wood += itemInfo.Value * item.Count; break;
                            case ResourceType.Ore: ore += itemInfo.Value * item.Count; break;
                        }
                    }

                    var respResources = await resManager.SumMainResource(defenderId, food, wood, ore, 0, 0);
                    if (!respResources.IsSuccess)
                    {
                        log.Debug(respResources.Message);
                    }
                }

                try
                {
                    var respMail = await mailManager.SendMail(defenderId, MailType.BattleReport, msg);
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

        private async Task<bool> ApplyMonsterChanges(BattleReport report)
        {
            var defenderPower = (BattlePower)report.Defender;

            var resp = await monsterManager.UpdateMonsterHealth(defenderPower.EntityId, defenderPower.TroopChanges.First().MonsterHP);

            return resp.IsSuccess;
        }

        private async Task<bool> ApplyGloryKingdomChanges(BattleReport report)
        {
            var defenderPower = (BattlePower)report.Defender;

            var fortressResp = await kingdomManager.GetZoneFortressById(defenderPower.EntityId);
            if (!fortressResp.IsSuccess || !fortressResp.HasData) return false;

            //SAVE TROOP CHANGES
            var zoneFortress = fortressResp.Data;
            await ApplyTroopChanges(zoneFortress.PlayerTroops, defenderPower.TroopChanges, false);

            var data = new ZoneFortressData()
            {
                FirstCapturedTime = zoneFortress.FirstCapturedTime,
                StartTime = DateTime.UtcNow,
                PlayerTroops = zoneFortress.PlayerTroops
            };

            int? playerId = null;
            var troopChanges = ((BattlePower)report.Attacker).TroopChanges;
            if (report.AttackerWon)
            {
//                log.Debug("current troop inside = " + JsonConvert.SerializeObject(data.PlayerTroops));
                playerId = report.Attacker.EntityId;
                data.PlayerTroops = new List<PlayerTroops>()
                {
                    new PlayerTroops((int)playerId, troopChanges.Select(x => x.Troop).ToList())
                };
//                log.Debug("new troop data = " + JsonConvert.SerializeObject(data.PlayerTroops));


                if ((data.FirstCapturedTime == null) || (data.FirstCapturedTime == DateTime.MinValue))
                {
                    data.FirstCapturedTime = data.StartTime;
                }

                var elapsed = DateTime.UtcNow - ((DateTime)data.FirstCapturedTime).ToUniversalTime();
                var totalDays = (int)elapsed.TotalDays;
                var duration = 3600 * 4;
                for (var num = 0; num < totalDays; num++)
                {
                    duration /= 2;
                }
                data.Duration = duration;
            }
            else
            {
                var duration = 24;
                var DECREASE_PER_DAY = 6;

                var totalDays = (DateTime.UtcNow - zoneFortress.StartTime.ToUniversalTime()).TotalDays / 2;
                duration -= (int)Math.Floor(totalDays) * DECREASE_PER_DAY;
                if (duration < 12) duration = 12;

                data.Duration = 3600 * duration;
            }
            var json = JsonConvert.SerializeObject(data);
            var updateResp = await kingdomManager.UpdateZoneFortress(defenderPower.EntityId,
                    hitPoints: defenderPower.HitPoints, attack: defenderPower.AttackCalc,
                    defense: defenderPower.DefenseCalc, playerId: playerId, data: json);

            return updateResp.IsSuccess;
        }

        List<PlayerTroops> GetUserPlayerTroops(int playerId, List<PlayerDataTable> data)
        {
            var troops = data.Where(x => (x.DataType == DataType.Troop));
            if (troops == null) return null;

            List<TroopInfos> userTroops = null;
            foreach (var item in troops)
            {
                if ((item == null) || (item.Value == null)) continue;

                var userTroop = PlayerData.PlayerDataToUserTroopData(item);
                if (userTroop == null) continue;

                if (userTroops == null) userTroops = new List<TroopInfos>();
                userTroops.Add(new TroopInfos(userTroop.Id, userTroop.ValueId, userTroop.Value));
            }
            var list = new List<PlayerTroops>()
            {
                new PlayerTroops(playerId, userTroops)
            };

            return list;
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
            var attackerId = report.Attacker.EntityId;

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
                            var idx = randm.Next(0, CacheHeroDataManager.HeroInfos.Count);
                            var heroTable = CacheHeroDataManager.HeroInfos[idx].Info;

                            heroToAward = heroTable.Code;
                        }*/
                    }
                }
            }

            var msg = JsonConvert.SerializeObject(report);

            var playerTroops = GetUserPlayerTroops(attackerId, attackerDataResp.Data);
            await ApplyTroopChanges(playerTroops, marchingArmy.TroopChanges, SAVE);

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
                    var responseKing = await manager.AddOrUpdatePlayerData(attackerId, DataType.Custom, (int)CustomValueType.KingDetails, kingJson);
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
                    var rewards = new List<DataReward>();

                    foreach (var item in report.Attacker.Items)
                    {
                        rewards.Add(new DataReward()
                        {
                            ItemId = item.ItemId,
                            Count = item.Count
                        });
                        log.Debug("attacker item = " + item.ItemId + ", " + item.Count);
                    }
                    var resp = await UserQuestManager.CollectRewards(report.Attacker.EntityId, rewards);
                }

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

        public async Task ApplyTroopChanges(List<PlayerTroops> playerTroops, List<TroopDetailsPvP> troopChanges, bool SAVE)
        {
            if (playerTroops == null || troopChanges == null) return;

            var allTroops = playerTroops.SelectMany(x => x.Troops)
                                        .Select(troop => new EditedTroopInfo(troop)).ToList();
            //TODO: Verify that in the case of multiple players, there is no possibility of erroneously assigning the deceased to another player who does not have the required number of soldiers. 
            //This is because the data used for validation are the maximums of the player, not the portion of soldiers that were in transit. 
            //Possible solution: use the soldiers in transit as limits and apply the subtractions based on that, then subtract those changes from the player's global data.
            foreach (var troopChange in troopChanges)
            {
                if (troopChange == null) continue;

                if (troopChange.Dead > 0)
                {
                    var idx = -1;
                    var groups = Split2(troopChange.Dead, playerTroops.Count);
                    do
                    {
                        var value = groups[0];
                        groups.RemoveAt(0);

                        /*var troopInfos = allTroops
                        .Where(x => (x.TroopInfo != null) && (x.TroopInfo.TroopType == troopChange.TroopType) &&
                                    x.TroopInfo.TroopData.Any(data => (data.Level == troopChange.Level) && (data.Count > 0)))
                                    .FirstOrDefault();*/

                        EditedTroopInfo troopInfos = null;
                        var len = allTroops.Count;
                        for (int num = 0; num < len; num++)
                        {
                            idx = (idx + 1) % len;
                            var info = allTroops[idx].TroopInfo;
                            if ((info != null) && (info.TroopType == troopChange.TroopType) &&
                                info.TroopData.Any(data => (data.Level == troopChange.Level) && (data.FinalCount > 0)))
                            {
                                troopInfos = allTroops[idx];
                                break;
                            }
                        }

                        var troopData = troopInfos?.TroopInfo.TroopData.Find(x => (x.Level == troopChange.Level) && (x.FinalCount > 0));
                        if (troopData != null)
                        {
                            var finalCount = troopData.FinalCount;
                            if (finalCount < value)
                            {
                                var excess = value - finalCount;
                                groups.Add(excess);
                                value -= excess;
                            }
                            troopData.Count -= value;
                            troopInfos.IsEdited = true;
                        }
                        else
                        {
                            break;
                        }
                    } while (groups.Count > 0);
                }

                if (troopChange.Wounded > 0)
                {
                    var idx = -1;
                    var groups = Split2(troopChange.Wounded, playerTroops.Count);
                    do
                    {
                        var value = groups[0];
                        groups.RemoveAt(0);

                        EditedTroopInfo troopInfos = null;
                        var len = allTroops.Count;
                        for (int num = 0; num < len; num++)
                        {
                            idx = (idx + 1) % len;
                            var info = allTroops[idx].TroopInfo;
                            if ((info != null) && (info.TroopType == troopChange.TroopType) &&
                                info.TroopData.Any(data => (data.Level == troopChange.Level) && (data.FinalCount > 0)))
                            {
                                troopInfos = allTroops[idx];
                                break;
                            }
                        }

                        var troopData = troopInfos?.TroopInfo.TroopData.Find(x => (x.Level == troopChange.Level) && (x.FinalCount > 0));
                        if (troopData != null)
                        {
                            var finalCount = troopData.FinalCount;
                            if (finalCount < value)
                            {
                                var excess = value - finalCount;
                                groups.Add(excess);
                                value -= excess;
                            }
                            troopData.Wounded += value;
                            troopInfos.IsEdited = true;
                        }
                        else
                        {
                            break;
                        }
                    } while (groups.Count > 0);
                }
            }

            if (SAVE)
            {
                foreach (var group in allTroops)
                {
                    if (!group.IsEdited) continue;

                    var playerTroop = playerTroops.Find(x => x.Troops.Contains(group.TroopInfo));
                    log.Debug("ApplyTroopChanges " + playerTroop.PlayerId);
                    var troopInfo = group.TroopInfo;
                    try
                    {
                        var response = await userTroopManager.UpdateTroops(playerTroop.PlayerId, troopInfo.TroopType, troopInfo.TroopData);
                        if (!response.IsSuccess)
                        {
                            log.Debug(response.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Debug(ex.Message);
                    }
                }
            }




/*            foreach (var data in troops)
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
            }*/
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

            if (infirmaryCapacity < 0) infirmaryCapacity = 0;
            foreach (var troop in troopBattle.TroopChanges)
            {
                int survived = troop.Count;
                troopBattle.TotalLoad += survived * troop.LoadPerUnit;
                troopBattle.TotalArmy += troop.InitialCount;

                var newWounded = 0;
                var newDeads = 0;
                var fallen = troop.InitialCount - survived;
                //TODO: move validation slingshot and infirmary here to avoid loop
                var groups = Split2(fallen, 10);
                var len = groups.Count;
                for (int num = 0; num < len; num++)
                {
                    var count = groups[num];
                    //random validate for choose between wounded/dead soldiers
                    if ((troop.TroopType != TroopType.Slingshot) && (infirmaryCapacity > 0) &&
                        (randm.Next(0, 100) <= (healingBuff ? 55 : 30)))
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

        public class EditedTroopInfo
        {
            public bool IsEdited { get; set; }
            public TroopInfos TroopInfo { get; set; }

            public EditedTroopInfo(TroopInfos troop)
            {
                TroopInfo = troop;
            }
        }
    }
}

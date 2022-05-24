using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Business.Manager.UserData;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Quest;
using GameOfRevenge.Common.Models.Quest.Template;
using GameOfRevenge.Common.Services;
using Newtonsoft.Json;

namespace GameOfRevenge.Business.Manager
{
    public class NewRealTimeUpdateManager
    {
        private readonly object SyncRoot = new object();

        private readonly UserQuestManager questManager = new UserQuestManager();
        private readonly BaseUserDataManager userManager = new BaseUserDataManager();

        private readonly Dictionary<int, PlayerData> questDatas = new Dictionary<int, PlayerData>();

        public NewRealTimeUpdateManager()
        {
            Update();
        }

        public bool Stoped { get; set; }

        public async Task UpdatePlayerData(int playerId)
        {
            questDatas.TryGetValue(playerId, out var playerData);
            if (playerData != null)
            {
                var response = await userManager.GetPlayerData(playerId);
                if (response.IsSuccess)
                {
                    lock (SyncRoot)
                    {
                        playerData.UserData = response.Data;
                    }
                }
            }
        }

        public void CollectResourceUpdate(int playerId, string resCode, int count)
        {
            if (count <= 0) return;
            if (string.IsNullOrWhiteSpace(resCode)) return;

            questDatas.TryGetValue(playerId, out var data);
            if (data != null)
            {
                var currentChapterQuest = data.QuestData.FirstOrDefault(x => !x.Completed);
                if (currentChapterQuest != null)
                {
                    var currentQuest = currentChapterQuest.Quests.FirstOrDefault(x => !x.Completed);
                    if (currentQuest != null)
                    {
                        if (currentQuest.QuestType == QuestType.ResourceCollection)
                        {
                            var initialData = JsonConvert.DeserializeObject<QuestCollectXResourceData>(currentQuest.InitialData);
                            if (initialData.ResourceType.Equals(resCode))
                            {
                                var progressData = JsonConvert.DeserializeObject<QuestCollectXResourceData>(currentQuest.ProgressData);
                                progressData.Count += count;
                                currentQuest.ProgressData = JsonConvert.SerializeObject(progressData);

                                UpdateQuestData(data, currentQuest, currentQuest.InitialData);
                            }
                        }
                    }
                }
            }
        }

        public void TrainTroopsUpdate(int playerId, string troopCode, int level, int count)
        {
            if (count <= 0) return;
            if (string.IsNullOrWhiteSpace(troopCode)) return;

            questDatas.TryGetValue(playerId, out var data);
            if (data != null)
            {
                var currentChapterQuest = data.QuestData.FirstOrDefault(x => !x.Completed);
                if (currentChapterQuest != null)
                {
                    var currentQuest = currentChapterQuest.Quests.FirstOrDefault(x => !x.Completed);
                    if (currentQuest != null)
                    {
                        if (currentQuest.QuestType == QuestType.TrainTroops)
                        {
                            var initialData = JsonConvert.DeserializeObject<QuestTrainXTroopData>(currentQuest.InitialData);
                            if (initialData.TroopType.Equals(troopCode) && level == initialData.Level)
                            {
                                var progressData = JsonConvert.DeserializeObject<QuestTrainXTroopData>(currentQuest.ProgressData);
                                progressData.Count += count;
                                currentQuest.ProgressData = JsonConvert.SerializeObject(progressData);

                                UpdateQuestData(data, currentQuest, currentQuest.InitialData);
                            }
                        }
                    }
                }
            }
        }

        public async void TryPushPlayerData(int playerId, Action<UserQuestProgressData> questAction)
        {
            try
            {
                var userData = await userManager.GetPlayerData(playerId);
                var userQuestData = await questManager.GetAllQuestChapterDataWithName(playerId);

                var playerData = new PlayerData()
                {
                    PlayerId = playerId,
                    QuestData = userQuestData.Data,
                    UserData = userData.Data,
                    QuestEventAction = questAction
                };

                lock (SyncRoot)
                    questDatas.Add(playerId, playerData);
            }
            catch (Exception)
            {

            }
        }

        public bool DeletePlayerData(int playerId)
        {
            lock (SyncRoot)
                return questDatas.Remove(playerId);
        }

        public void Update()
        {
            var currentIterationList = questDatas.Select(x => x.Value).ToList();
            foreach (var data in currentIterationList) UpdatePlayerData(data);

            if (Stoped) return;

            var action = new DelayedAction();
            action.WaitForCallBack(Update, 1000);
        }

        public void UpdatePlayerData(PlayerData data)
        {
            var currentChapterQuest = data.QuestData.FirstOrDefault(x => !x.Completed);
            if (currentChapterQuest != null)
            {
                var currentQuest = currentChapterQuest.Quests.FirstOrDefault(x => !x.Completed);
                if (currentQuest != null)
                {
                    if (currentQuest.QuestType == QuestType.BuildingUpgrade)
                    {
                        var initialData = JsonConvert.DeserializeObject<QuestBuildingData>(currentQuest.InitialData);
                        var progressData = JsonConvert.DeserializeObject<QuestBuildingData>(currentQuest.ProgressData);

                        if (progressData.Equals(initialData))
                        {
                            currentQuest.ProgressData = currentQuest.InitialData;
                            currentQuest.Completed = true;

                            UpdateQuestData(data, currentQuest, currentQuest.InitialData);
                        }
                        else
                        {
                            var structureDetails = data.UserData.Structures.FirstOrDefault(x => x.StructureType.ToString() == initialData.StructureType)?.Buildings.FirstOrDefault(x => x.TimeLeft <= 0 && x.Level >= initialData.Level);
                            if (structureDetails != null)
                            {
                                currentQuest.ProgressData = currentQuest.InitialData;
                                currentQuest.Completed = true;

                                UpdateQuestData(data, currentQuest, currentQuest.InitialData);
                            }
                        }
                    }
                    else if (currentQuest.QuestType == QuestType.XBuildingCount)
                    {
                        var initialData = JsonConvert.DeserializeObject<QuestHaveXBuildingData>(currentQuest.InitialData);
                        var progressData = JsonConvert.DeserializeObject<QuestHaveXBuildingData>(currentQuest.ProgressData);

                        if (progressData.Equals(initialData))
                        {
                            currentQuest.ProgressData = currentQuest.InitialData;
                            currentQuest.Completed = true;

                            UpdateQuestData(data, currentQuest, currentQuest.InitialData);
                        }
                        else
                        {
                            var structureDetails = data.UserData.Structures.FirstOrDefault(x => x.StructureType.ToString() == initialData.StructureType)?.Buildings.Where(x => x.TimeLeft <= 0)?.ToList();
                            if (structureDetails != null && structureDetails.Count >= initialData.Count)
                            {
                                currentQuest.ProgressData = currentQuest.InitialData;
                                currentQuest.Completed = true;

                                UpdateQuestData(data, currentQuest, currentQuest.InitialData);
                            }
                        }
                    }
                    else if (currentQuest.QuestType == QuestType.ResourceCollection)
                    {
                        var initialData = JsonConvert.DeserializeObject<QuestCollectXResourceData>(currentQuest.InitialData);
                        var progressData = JsonConvert.DeserializeObject<QuestCollectXResourceData>(currentQuest.ProgressData);

                        if (progressData.Equals(initialData))
                        {
                            currentQuest.ProgressData = currentQuest.InitialData;
                            currentQuest.Completed = true;

                            UpdateQuestData(data, currentQuest, currentQuest.InitialData);
                        }
                    }
                    else if (currentQuest.QuestType == QuestType.XTroopCount)
                    {
                        var initialData = JsonConvert.DeserializeObject<QuestHaveXTroopData>(currentQuest.InitialData);
                        var progressData = JsonConvert.DeserializeObject<QuestHaveXTroopData>(currentQuest.ProgressData);

                        if (progressData.Equals(initialData))
                        {
                            currentQuest.ProgressData = currentQuest.InitialData;
                            currentQuest.Completed = true;

                            UpdateQuestData(data, currentQuest, currentQuest.InitialData);
                        }
                        else
                        {
                            var count = 0;
                            data.UserData.Troops.FirstOrDefault(x => x.TroopType.ToString() == initialData.TroopType)?.TroopData.ForEach(x => count += x.FinalCount);
                            if (count >= initialData.Count)
                            {
                                currentQuest.ProgressData = currentQuest.InitialData;
                                currentQuest.Completed = true;

                                UpdateQuestData(data, currentQuest, currentQuest.InitialData);
                            }
                        }
                    }
                    else if (currentQuest.QuestType == QuestType.TrainTroops)
                    {
                        var initialData = JsonConvert.DeserializeObject<QuestTrainXTroopData>(currentQuest.InitialData);
                        var progressData = JsonConvert.DeserializeObject<QuestTrainXTroopData>(currentQuest.ProgressData);

                        if (progressData.Equals(initialData))
                        {
                            currentQuest.ProgressData = currentQuest.InitialData;
                            currentQuest.Completed = true;

                            UpdateQuestData(data, currentQuest, currentQuest.InitialData);
                        }
                    }
                }
            }
        }

        private void UpdateQuestData(PlayerData data, UserQuestProgressData currentQuest, string initialData)
        {
            new DelayedAction().WaitForCallBack(async () =>
            {
                await questManager.UpdateQuestData(data.PlayerId, currentQuest.QuestId, true, initialData);
                data.QuestEventAction?.Invoke(currentQuest);
            }, 100);
        }
    }

    public class PlayerData
    {
        public int PlayerId { get; set; }
        public PlayerCompleteData UserData { get; set; }
        public List<UserChapterQuestData> QuestData { get; set; }
        public Action<UserQuestProgressData> QuestEventAction { get; set; }
    }
}

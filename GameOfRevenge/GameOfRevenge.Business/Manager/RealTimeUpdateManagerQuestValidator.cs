using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExitGames.Logging;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Business.Manager.UserData;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Quest;
using GameOfRevenge.Common.Models.Quest.Template;
using GameOfRevenge.Common.Services;
using Newtonsoft.Json;

namespace GameOfRevenge.Business.Manager
{
    public class RealTimeUpdateManagerQuestValidator
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private readonly object SyncRoot = new object();

        private readonly UserQuestManager questManager = new UserQuestManager();
        private readonly BaseUserDataManager userManager = new BaseUserDataManager();

        private readonly Dictionary<int, PlayerUserQuestData> allPlayerDatas = new Dictionary<int, PlayerUserQuestData>();

        private bool running;
        private int reseting;
        private DateTime tomorrow;

        public RealTimeUpdateManagerQuestValidator()
        {
            Running = true;
            tomorrow = DateTime.UtcNow.AddDays(1).Date;
        }

        public bool Running
        {
            get
            {
                return running;
            }
            set
            {
                running = value;
                if (running) Update();
            }
        }

        public async Task PlayerDataChanged(int playerId)
        {
            log.Info("player " + playerId + " data changed");
            allPlayerDatas.TryGetValue(playerId, out var data);
            if (data == null) return;

            //TODO: use internal data reference instead of pulling data from server
            //peer.Actor.InternalPlayerDataManager
            //TODO2: method used to pull fresh data from server after a change was done in server
            //we should improve the update request to reduce the pull data

            var userData = await userManager.GetFullPlayerData(playerId);
            var userQuestData = await questManager.GetUserAllQuestProgress(playerId, true);

            lock (SyncRoot)
            {
                if (userData.IsSuccess) data.UserData = userData.Data;
                if (userQuestData.IsSuccess) data.QuestData = userQuestData.Data;
            }
        }


        public void CheckAllQuestProgress(PlayerUserQuestData data, QuestType questType, System.Action<UserQuestProgressData> callback)
        {
            //check current milestone quest
            var currChapterQuest = data.QuestData.ChapterQuests.Find(x => !x.Completed());
            if (currChapterQuest != null)
            {
                var currQuest = currChapterQuest.Quests.Find(x => !x.Completed);
                if (currQuest?.QuestType == questType) callback(currQuest);
            }

            //check side quest
            foreach (var userQuest in data.QuestData.SideQuests)
            {
                if (!userQuest.Completed && (userQuest.QuestType == questType)) callback(userQuest);
            }

            //check daily quest
            foreach (var userQuest in data.QuestData.DailyQuests)
            {
                if (!userQuest.Completed && (userQuest.QuestType == questType)) callback(userQuest);
            }
        }

        public void CollectResourceCheckQuestProgress(int playerId, ResourceType resourceType, int count)
        {
            if ((count <= 0) || (resourceType == ResourceType.Other)) return;

            allPlayerDatas.TryGetValue(playerId, out var data);
            if (data == null) return;

            CheckAllQuestProgress(data, QuestType.ResourceCollection, (currentQuest) =>
            {
                var initialData = JsonConvert.DeserializeObject<QuestResourceData>(currentQuest.InitialData);
                if ((initialData == null) || (initialData.ResourceType != resourceType)) return;

                QuestResourceData progressData = null;
                if (!string.IsNullOrEmpty(currentQuest.ProgressData))
                {
                    progressData = JsonConvert.DeserializeObject<QuestResourceData>(currentQuest.ProgressData);
                }
                else
                {
                    progressData = initialData;
                    progressData.Count = 0;
                }
                progressData.Count += count;
                if (progressData.Count >= initialData.Count)
                {
                    progressData.Count = initialData.Count;
//                    currentQuest.Completed = true;
                }

                currentQuest.ProgressData = JsonConvert.SerializeObject(progressData);
//                    CheckQuestProgress(data , currentQuest);
                UpdateQuestData(data, currentQuest);
            });
        }


        public void TrainTroopsCheckQuestProgress(int playerId, TroopType troopType, int level, int count)
        {
            if ((count <= 0) || (troopType == TroopType.Other)) return;

            allPlayerDatas.TryGetValue(playerId, out var data);
            if (data == null) return;

            CheckAllQuestProgress(data, QuestType.TrainTroops, (currentQuest) =>
            {
                var initialData = JsonConvert.DeserializeObject<QuestTroopData>(currentQuest.InitialData);
                if ((initialData == null) || (initialData.TroopType != troopType)) return;

                if ((initialData.Level == 0) || (level == initialData.Level))
                {
                    QuestTroopData progressData = null;
                    if (!string.IsNullOrEmpty(currentQuest.ProgressData))
                    {
                        progressData = JsonConvert.DeserializeObject<QuestTroopData>(currentQuest.ProgressData);
                    }
                    else
                    {
                        progressData = initialData;
                        progressData.Count = 0;
                    }
                    progressData.Count += count;
                    if (progressData.Count >= initialData.Count)
                    {
                        progressData.Count = initialData.Count;
//                        currentQuest.Completed = true;
                    }

                    currentQuest.ProgressData = JsonConvert.SerializeObject(progressData);
//                    CheckQuestProgress(data , currentQuest);
                    UpdateQuestData(data, currentQuest);
                }
            });
        }


        public async void TryAddPlayerQuestData(int playerId, Action<UserQuestProgressData> questAction)
        {
            try
            {
                var userData = await userManager.GetFullPlayerData(playerId);
                var userQuestData = await questManager.GetUserAllQuestProgress(playerId, true);

                var playerQuestData = new PlayerUserQuestData()
                {
                    PlayerId = playerId,
                    QuestData = userQuestData.Data,
                    UserData = userData.Data,
                    QuestEventAction = questAction
                };

                lock (SyncRoot)
                    allPlayerDatas.Add(playerId, playerQuestData);
            }
            catch { }
        }

        public bool DeletePlayerQuestData(int playerId)
        {
            lock (SyncRoot)
                return allPlayerDatas.Remove(playerId);
        }


        public static string ChangeSecondToFormat(float seconds)
        {
            return ChangeSecondToFormat(TimeSpan.FromSeconds(seconds));
        }

        public static string ChangeSecondToFormat(TimeSpan time)
        {
            string str = string.Empty;
            if (time.Days > 0)
            {
                str = string.Format((time.Days == 1) ? "{0} day" : "{0} days", time.Days);
            }
            else if (time.Hours > 0)
            {
                str = string.Format("{0:D2}:{1:D2}:{2:D2}",
                                time.Hours,
                                time.Minutes,
                                time.Seconds);
            }
            else
            {
                str = string.Format("{0:D2}:{1:D2}",
                          time.Minutes,
                          time.Seconds);
            }
            return str;
        }


        public void Update()
        {
            if ((reseting == 0) && (DateTime.UtcNow >= tomorrow))
            {
//                var seconds = (tomorrow - DateTime.UtcNow).TotalSeconds;
//                if (seconds < 1) 
                TryToResetQuests(2);
            }

            var currentIterationList = allPlayerDatas.Select(x => x.Value).ToList();
            foreach (var data in currentIterationList) CheckPlayerQuestData(data);

            if (Running) new DelayedAction().WaitForCallBack(Update, 5000);
        }

        private void TryToResetQuests(int count)
        {
            log.Info("TryToResetQuests: " + count);
            reseting = count;
            if (reseting == 0)
            {
                var currentIterationList = allPlayerDatas.Select(x => x.Value).ToList();
                foreach (var data in currentIterationList)
                {
                    data.QuestEventAction?.Invoke(null);
                }
                return;
            }

            new DelayedAction().WaitForCallBack(async () =>
            {
                var response = await questManager.ResetAllDailyQuests();
                if (response.IsSuccess)
                {
                    tomorrow = DateTime.UtcNow.AddDays(1).Date;
                    log.Info("TryToResetQuests: " + response.Message);
                    count = 0;
                }
                else
                {
                    log.Error("TryToResetQuests: " + response.Message);
                    count--;
                }
                TryToResetQuests(count);
            }, 1000);
        }

        public void CheckPlayerQuestData(PlayerUserQuestData data)
        {
            //check daily quest
            log.Info("--check quests");
            var showLog = data.UserData.IsAdmin;
            int idx = 0;
            foreach (var userQuest in data.QuestData.DailyQuests)
            {
                idx++;
                if (showLog) log.Info(idx+"  "+userQuest.QuestId+"   c:"+userQuest.Completed+"   "+userQuest.ProgressData);
                if (!userQuest.Completed) CheckQuestProgress(data, userQuest);
            }

            //check side quest
            if (showLog) log.Info("--side quests");
            foreach (var userQuest in data.QuestData.SideQuests)
            {
                if (showLog) log.Info(userQuest.QuestId + "   c:" + userQuest.Completed + "   " + userQuest.ProgressData);
                if (!userQuest.Completed) CheckQuestProgress(data, userQuest);
            }

            //check current milestone quest
            if (showLog) log.Info("--chapter quests");
            var currChapterQuest = data.QuestData.ChapterQuests.Find(x => !x.Completed());
            if (currChapterQuest != null)
            {
                var currQuest = currChapterQuest.Quests.Find(x => !x.Completed);
                if (currQuest != null) CheckQuestProgress(data, currQuest);
            }
            if (showLog) log.Info("----");
        }

        public void CheckQuestProgress(PlayerUserQuestData data, UserQuestProgressData currentQuest)
        {
            var showLog = data.UserData.IsAdmin;

            switch (currentQuest.QuestType)
            {
                case QuestType.BuildingUpgrade:
                    if (showLog) log.Info("check building upgrade");
                    var initialData1 = JsonConvert.DeserializeObject<QuestBuildingData>(currentQuest.InitialData);
                    bool completed1 = false;
                    if (!string.IsNullOrEmpty(currentQuest.ProgressData))
                    {
                        var progressData1 = JsonConvert.DeserializeObject<QuestBuildingData>(currentQuest.ProgressData);
                        completed1 = (progressData1 != null) && (progressData1.Level >= initialData1.Level);
                    }

                    if (!completed1)
                    {
                        var bld = data.UserData.Structures.Find(x => (x.StructureType == initialData1.StructureType))?
                                        .Buildings.Find(x =>
                                        {
                                            var lvl = x.Level - ((x.TimeLeft > 0)? 1 : 0);
                                            return lvl >= initialData1.Level;
                                        });
                        completed1 = (bld != null);
                    }

                    if (completed1 && !currentQuest.Completed)
                    {
                        currentQuest.ProgressData = currentQuest.InitialData;
                        currentQuest.Completed = true;

                        UpdateQuestData(data, currentQuest);
                    }
                    break;
                case QuestType.XBuildingCount:
                    if (showLog) log.Info("check building count");
                    var initialData2 = JsonConvert.DeserializeObject<QuestBuildingData>(currentQuest.InitialData);
                    QuestBuildingData progressData2 = null;
                    bool completed2 = false;
                    if (!string.IsNullOrEmpty(currentQuest.ProgressData))
                    {
                        progressData2 = JsonConvert.DeserializeObject<QuestBuildingData>(currentQuest.ProgressData);
                        completed2 = (progressData2 != null) && (progressData2.Count >= initialData2.Count);
                    }

                    if (!completed2)
                    {
                        var blds = data.UserData.Structures.Find(x => (x.StructureType == initialData2.StructureType));
                        if (blds != null)
                        {
                            var count = blds.Buildings.Sum(x =>
                            {
                                var lvl = x.Level - ((x.TimeLeft > 0) ? 1 : 0);
                                return (lvl >= 1) ? 1 : 0;
                            });

                            if (count >= initialData2.Count)
                            {
                                completed2 = true;
                            }
                            else if ((progressData2 == null) || (count > progressData2.Count))
                            {
                                progressData2 = initialData2;
                                progressData2.Count = count;
                                currentQuest.ProgressData = JsonConvert.SerializeObject(progressData2);

                                UpdateQuestData(data, currentQuest);
                            }
                        }
                    }
                    if (completed2 && !currentQuest.Completed)
                    {
                        progressData2 = initialData2;
                        progressData2.Count = 0;
                        currentQuest.ProgressData = JsonConvert.SerializeObject(progressData2);
                        currentQuest.Completed = true;

                        UpdateQuestData(data, currentQuest);
                    }
                    break;
                case QuestType.ResourceCollection:
                    if (showLog) log.Info("check resource collection");
                    if (!string.IsNullOrEmpty(currentQuest.ProgressData))
                    {
                        var progressData3 = JsonConvert.DeserializeObject<QuestResourceData>(currentQuest.ProgressData);
                        if (progressData3 != null)
                        {
                            var initialData3 = JsonConvert.DeserializeObject<QuestResourceData>(currentQuest.InitialData);
                            if (progressData3.Count >= initialData3.Count)
                            {
                                if (progressData3.Iteration > 0) progressData3.Iteration--;
                                progressData3.Count = 0;//initialData3.Count;
                                currentQuest.ProgressData = JsonConvert.SerializeObject(progressData3);
                                currentQuest.Completed = true;

                                UpdateQuestData(data, currentQuest);
                            }
                        }
                    }
                    break;
                case QuestType.XTroopCount:
                    if (showLog) log.Info("check troop count");
                    var initialData4 = JsonConvert.DeserializeObject<QuestTroopData>(currentQuest.InitialData);
                    QuestTroopData progressData4 = null;
                    bool completed4 = false;
                    if (!string.IsNullOrEmpty(currentQuest.ProgressData))
                    {
                        progressData4 = JsonConvert.DeserializeObject<QuestTroopData>(currentQuest.ProgressData);
                        completed4 = (progressData4 != null) && (progressData4.Count >= initialData4.Count);
                    }

                    if (!completed4)
                    {
                        var troops = data.UserData.Troops.Find(x => (x.TroopType == initialData4.TroopType));
                        if (troops != null)
                        {
                            var count = troops.TroopData.Sum(x => x.FinalCount);

                            if (count >= initialData4.Count)
                            {
                                completed4 = true;
                            }
                            else if ((progressData4 == null) || (count > progressData4.Count))
                            {
                                progressData4 = initialData4;
                                progressData4.Count = count;
                                currentQuest.ProgressData = JsonConvert.SerializeObject(progressData4);

                                UpdateQuestData(data, currentQuest);
                            }
                        }
                    }
                    if (completed4 && !currentQuest.Completed)
                    {
                        if (progressData4.Iteration > 0) progressData4.Iteration--;
                        progressData4.Count = 0;
                        currentQuest.ProgressData = JsonConvert.SerializeObject(progressData4);
                        currentQuest.Completed = true;

                        UpdateQuestData(data, currentQuest);
                    }
                    break;
                case QuestType.TrainTroops:
                    if (showLog) log.Info("check train troops");
                    if (!string.IsNullOrEmpty(currentQuest.ProgressData))
                    {
                        var progressData5 = JsonConvert.DeserializeObject<QuestTroopData>(currentQuest.ProgressData);
                        if (progressData5 != null)
                        {
                            var initialData5 = JsonConvert.DeserializeObject<QuestTroopData>(currentQuest.InitialData);
                            if ((progressData5.Count >= initialData5.Count) &&
                                ((initialData5.Level == 0) || (progressData5.Level == initialData5.Level)))
                            {
        //                        currentQuest.ProgressData = currentQuest.InitialData;
                                if (progressData5.Iteration > 0) progressData5.Iteration--;
                                progressData5.Count = 0;
                                currentQuest.ProgressData = JsonConvert.SerializeObject(progressData5);
                                currentQuest.Completed = true;

                                UpdateQuestData(data, currentQuest);
                            }
                        }
                    }
                    break;
            }
            if (showLog) log.Info("end chk");
        }

        private void UpdateQuestData(PlayerUserQuestData data, UserQuestProgressData currentQuest)
        {
            new DelayedAction().WaitForCallBack(async () =>
            {
                await questManager.UpdateQuestData(data.PlayerId, currentQuest.QuestId, currentQuest.Completed, currentQuest.ProgressData);
                data.QuestEventAction?.Invoke(currentQuest);
            }, 100);
        }
    }

    public class PlayerUserQuestData
    {
        public int PlayerId { get; set; }
        public PlayerCompleteData UserData { get; set; }
        public UserChapterAllQuestProgress QuestData { get; set; }
        public Action<UserQuestProgressData> QuestEventAction { get; set; }
    }
}

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Quest;
using GameOfRevenge.Common.Services;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Business.Manager.Base;
using ExitGames.Logging;

namespace GameOfRevenge.Business.Manager
{
    public class RealTimeUpdateManagerQuestValidator
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private readonly object SyncRoot = new object();

        private readonly IAdminDataManager adminManager;
        private readonly IUserQuestManager questManager;

        private readonly Dictionary<int, PlayerUserQuestData> allPlayerDatas = new Dictionary<int, PlayerUserQuestData>();

        private bool running;
        private int reseting;
        private DateTime tomorrow;
        private DateTime lastUpdate;
        private int ticks;

        private const int UPDATE_INTERVAL = 5000;

        public RealTimeUpdateManagerQuestValidator(IAdminDataManager adminDataManager, IUserQuestManager userQuestManager)
        {
            adminManager = adminDataManager;
            questManager = userQuestManager;
            Running = true;
            tomorrow = DateTime.UtcNow.AddDays(1).Date;
            lastUpdate = DateTime.UtcNow;
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
                if (running) _ = UpdateAsync();
            }
        }

        public async Task<PlayerCompleteData> UpdatePlayerData(PlayerUserQuestData data)
        {
            var playerId = data.PlayerId;
            var userData = await BaseUserDataManager.GetFullPlayerData(playerId);
            var userQuestData = await questManager.GetUserAllQuestProgress(playerId, true);

            lock (SyncRoot)
            {
                if (userData.IsSuccess) data.UserData = userData.Data;
                if (userQuestData.IsSuccess) data.QuestData = userQuestData.Data;
            }
            log.Info("player " + playerId + " data changed EXIT");

            return userData.Data;
        }


/*        public static List<UserQuestProgressData> GetQuestInProgressList(PlayerUserQuestData data, QuestType questType)
        {
            var list = new List<UserQuestProgressData>();
            //check current milestone quest
            var currChapterQuest = data.QuestData.ChapterQuests.Find(x => !x.AllQuestsCompleted);
            if (currChapterQuest != null)
            {
                var currQuest = currChapterQuest.Quests.Find(x => !x.Completed);
                if ((currQuest != null) && (currQuest.QuestType == questType)) list.Add(currQuest);
            }

            //check side quest
            foreach (var userQuest in data.QuestData.SideQuests)
            {
                if (!userQuest.Completed && (userQuest.QuestType == questType)) list.Add(userQuest);
            }

            //check daily quest
            foreach (var userQuest in data.QuestData.DailyQuests)
            {
                if (!userQuest.Completed && (userQuest.QuestType == questType)) list.Add(userQuest);
            }

            return list;
        }*/

        public PlayerUserQuestData GetCachedPlayerData(int playerId)
        {
            allPlayerDatas.TryGetValue(playerId, out var data);

            return data;
        }



        public async void TryAddPlayerQuestData(int playerId, Action<PlayerQuestDataTable> questAction)
        {
            try
            {
                var userData = await BaseUserDataManager.GetFullPlayerData(playerId);
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

                if (running) _ = UpdateAsync();
            }
            catch { }
        }

        public bool DeletePlayerQuestData(int playerId)
        {
            lock (SyncRoot)
                return allPlayerDatas.Remove(playerId);
        }

        public async Task UpdateAsync()
        {
            if ((reseting == 0) && (DateTime.UtcNow >= tomorrow))
            {
                //                var seconds = (tomorrow - DateTime.UtcNow).TotalSeconds;
                //                if (seconds < 1) 
                FinishDailyProcess(2);
            }
            if ((DateTime.UtcNow - lastUpdate).TotalMilliseconds < UPDATE_INTERVAL) return;

            lastUpdate = DateTime.UtcNow;
            if (allPlayerDatas.Count > 0)
            {
                log.Info("player datas = " + allPlayerDatas.Count);
                foreach (var data in allPlayerDatas)
                {
                    await questManager.CheckPlayerQuestDataAsync(data.Value);
                }
                log.Info("quest check end: " + (DateTime.UtcNow - lastUpdate).TotalSeconds);
            }
            else
            {
                ticks++;
                if (ticks > 100)
                {
                    log.Info("quest thread alive");
                    ticks = 0;
                }
            }

            if (running) new DelayedAction().WaitForCallBack(() =>
            {
                _ = UpdateAsync();
            }, UPDATE_INTERVAL + 1);
        }

        private void FinishDailyProcess(int count)
        {
            log.Info("FinishDailyProcess: " + count);
            reseting = count;
            if (reseting == 0)
            {
/*                var currentIterationList = allPlayerDatas.Select(x => x.Value).ToList();
                foreach (var data in currentIterationList)
                {
//                    data.QuestEventAction?.Invoke(null);
                }*/
                return;
            }

            new DelayedAction().WaitForCallBack(async () =>
            {
                var save = false;
                var reset = false;

                var respSave = await adminManager.SaveDailyVisits();
                log.Info("TryToSaveDailyVisits: " + respSave.Message);
                save = respSave.IsSuccess;

                var respReset = await adminManager.ResetAllDailyQuests();
                log.Info("TryToResetQuests: " + respReset.Message);
                reset = respReset.IsSuccess;

                if (save && reset)
                {
                    tomorrow = DateTime.UtcNow.AddDays(1).Date;
                    count = 0;
                }
                else
                {
                    count--;
                }

                FinishDailyProcess(count);
            }, 1000);
        }



/*        public static string ChangeSecondToFormat(float seconds)
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
        }*/
    }
}

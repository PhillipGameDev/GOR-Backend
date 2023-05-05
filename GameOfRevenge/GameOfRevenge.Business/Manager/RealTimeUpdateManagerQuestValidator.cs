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
        private DateTime lastUpdate;
        private int ticks;

        private const int UPDATE_INTERVAL = 5000;

        public RealTimeUpdateManagerQuestValidator()
        {
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

        public async Task<PlayerCompleteData> PlayerDataChanged(int playerId)
        {
            log.Info("player " + playerId + " data changed ENTER");
            allPlayerDatas.TryGetValue(playerId, out var data);
            if (data == null)
            {
                log.Info("player " + playerId + " data null RETURN");
                return null;
            }

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

        public PlayerUserQuestData GetPlayerData(int playerId)
        {
            allPlayerDatas.TryGetValue(playerId, out var data);

            return data;
        }



        public async void TryAddPlayerQuestData(int playerId, Action<PlayerQuestDataTable> questAction)
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
                TryToResetQuests(2);
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

        private void TryToResetQuests(int count)
        {
            log.Info("TryToResetQuests: " + count);
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

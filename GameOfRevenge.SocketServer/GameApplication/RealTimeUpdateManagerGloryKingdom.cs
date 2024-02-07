using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using ExitGames.Logging;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Services;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Models.Kingdom;
using GameOfRevenge.Business.CacheData;

namespace GameOfRevenge.GameApplication
{
    public class RealTimeUpdateManagerGloryKingdom
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly IKingdomManager kingdomManager;

        private readonly List<ZoneFortressTable> allZoneFortress = new List<ZoneFortressTable>();

        private bool running;
        private int reseting;
        private DateTime currentEventEndTimeUTC;

        private DateTime lastCheck;

        private const int UPDATE_INTERVAL = 10000;
        private const DayOfWeek EVENT_DAY_OF_WEEK = DayOfWeek.Saturday;
        private const int EVENT_HOUR_OF_DAY = 8;
        private const int FINISH_PERIOD = 3600 * 48;

        public RealTimeUpdateManagerGloryKingdom(IKingdomManager kingdomManager)
        {
            this.kingdomManager = kingdomManager;

            var task1 = kingdomManager.GetAllZoneFortress();
            task1.Wait();
            if (!task1.Result.IsSuccess || !task1.Result.HasData)
            {
                throw new Exception("Error Retrieving All Zone Fortress");
            }
            allZoneFortress = task1.Result.Data;
            log.Info("Total zone fortress = " + allZoneFortress.Count);

            var task2 = kingdomManager.GetGloryKingdomDetails();
            task2.Wait();
            if (!task2.Result.IsSuccess || !task2.Result.HasData)
            {
                throw new Exception("Error Retrieving Glory Kingdom Details");
            }
            var gloryKingdomData = task2.Result.Data;
            log.Info("Glory Kingdom event end time = " + gloryKingdomData.EndTime.ToUniversalTime());

            currentEventEndTimeUTC = gloryKingdomData.EndTime.ToUniversalTime();
            Running = true;
        }

        public bool Running
        {
            get => running;
            set
            {
                running = value;
                if (running) _ = UpdateAsync();
            }
        }

        public void UpdateFortressData(ZoneFortress data)
        {
            var fortress = allZoneFortress.Find(x => x.ZoneFortressId == data.ZoneFortressId);
            if (fortress != null)
            {
                fortress.Finished = data.Finished;
            }
        }

        public async Task UpdateAsync()
        {
            if ((reseting == 0) && (DateTime.UtcNow >= currentEventEndTimeUTC))
            {
                FinishEventProcess(2);
            }
            if ((DateTime.UtcNow - lastCheck).TotalMilliseconds > UPDATE_INTERVAL)
            {
                lastCheck = DateTime.UtcNow;
                foreach (var fortressData in allZoneFortress)
                {
                    if (!fortressData.Finished) await TryToCloseFortress(fortressData.ZoneFortressId);
                }
                log.Info("zone fortress check end: " + (DateTime.UtcNow - lastCheck).TotalSeconds);
            }

            if (running) new DelayedAction().WaitForCallBack(() =>
            {
                _ = UpdateAsync();
            }, UPDATE_INTERVAL + 1);
        }

        private async Task TryToCloseFortress(int fortressId, bool reset = false)
        {
            var task = await kingdomManager.GetZoneFortressById(fortressId);
            if (!task.IsSuccess || !task.HasData) return;

            var fortress = task.Data;
            if (fortress.TimeSinceEnd < FINISH_PERIOD) return;

            if (fortress.PlayerId > 0)
            {
                var recallResp = await GameService.GameLobby.RecallAllTroopsFromGloryKingdom(fortressId);
                if (recallResp != ReturnCode.OK)
                {
                    log.Info("Failed to recall troops from zone fortress " + fortressId);
                }
                if ((recallResp == ReturnCode.OK) || reset)
                {
                    var collectResp = await GameService.GameLobby.DistributeGloryKingdomRewards(fortressId, 0.5f);
                    if (collectResp != ReturnCode.OK)
                    {
                        log.Info("Complete with errors when distribute rewards from zone fortress " + fortressId);
                    }
                }
            }
            else if (!reset)
            {
                var updResp = await kingdomManager.UpdateZoneFortress(fortressId, finished: true);
                if (updResp.IsSuccess && updResp.HasData)
                {
                    UpdateFortressData(updResp.Data);
                }
                else
                {
                    log.Info("Error updating zone fortress "+fortressId);
                }
            }

            if (!reset) return;

            var powerData = new PlayerCompleteData()
            {
                Troops = new List<TroopInfos>()
                    {
                        new TroopInfos(0, Common.TroopType.Swordsman, new List<TroopDetails>()
                        {
                            new TroopDetails() { Level = 1, Count = 100000 }
                        })
                    }
            };
            var fortressPower = new BattlePower(powerData, null, CacheTroopDataManager.GetFullTroopData, null);

            var fortressData = new ZoneFortressData()
            {
                StartTime = DateTime.UtcNow,
                Duration = 3600 * 24,
                PlayerTroops = new List<PlayerTroops>()
                {
                    new PlayerTroops(0, powerData.Troops)
                }
            };
            var json = JsonConvert.SerializeObject(fortressData);
            var resp = await GameService.BKingdomManager.UpdateZoneFortress(fortressId,
                    hitPoints: fortressPower.HitPoints, attack: fortressPower.AttackCalc,
                    defense: fortressPower.DefenseCalc, finished: false, data: json);
            if (!resp.IsSuccess) log.Info("Error reinitializing zone fortress "+fortressId);
        }

        private void FinishEventProcess(int count)
        {
            log.Info("FinishEventProcess: " + count);
            reseting = count;
            if (reseting == 0) return;

            new DelayedAction().WaitForCallBack(async () =>
            {
                var success = false;
                if (DateTime.UtcNow > currentEventEndTimeUTC)
                {
                    foreach (var fortressData in allZoneFortress)
                    {
                        if (!fortressData.Finished) await TryToCloseFortress(fortressData.ZoneFortressId, true);
                    }

                    var nextEventDay = DateTime.UtcNow.Date;
                    while (nextEventDay.DayOfWeek != EVENT_DAY_OF_WEEK)
                    {
                        nextEventDay = nextEventDay.AddDays(1);
                    }

                    if (DateTime.UtcNow > nextEventDay.AddHours(EVENT_HOUR_OF_DAY))
                    {
                        nextEventDay = nextEventDay.AddDays(7);
                    }
                    var nextEventStartTime = nextEventDay.AddHours(EVENT_HOUR_OF_DAY);
                    var nextEventEndTime = nextEventStartTime.AddDays(5);

                    var resp = await kingdomManager.CreateGloryKingdomEvent(nextEventStartTime, nextEventEndTime);
                    if (resp.IsSuccess && resp.HasData)
                    {
                        currentEventEndTimeUTC = resp.Data.EndTime.ToUniversalTime();
                        success = true;
                        count = 0;
                    }
                }

                if (!success) count--;

                FinishEventProcess(count);
            }, 1000);
        }
    }
}

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
    public class KingdomChatManager : BaseUserDataManager//, IKingdomPvPManager
    {
        public static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly IUserMailManager mailManager = new UserMailManager();

        private async Task<Response<UnderAttackReport>> SaveAlertMail(int watchLevel, PlayerCompleteData attackerData, int defenderId, MapLocation loc)
        {
            var attackerMailInfo = new UnderAttackReport()
            {
                AttackerId = attackerData.PlayerId,
                DefenderId = defenderId
            };

//            try
            {
                GenerateAlertMailFields(attackerData, loc, watchLevel, attackerMailInfo);
            }
//            catch { }

            try
            {
                await mailManager.SaveMail(defenderId, MailType.UnderAttack, JsonConvert.SerializeObject(attackerMailInfo));
            }
            catch (Exception ex)
            {
                return new Response<UnderAttackReport>(CaseType.Error, ErrorManager.ShowError(ex));
            }

            return new Response<UnderAttackReport>(attackerMailInfo, CaseType.Success, "Success");
        }

        private static void GenerateAlertMailFields(PlayerCompleteData attackerData, MapLocation location, int watchTowerLevel, UnderAttackReport attackerMailInfo)
        {
            //Reveals the incoming troops Player name.
            if (watchTowerLevel > 0)
            {
//                var attackerInfo = await new AccountManager().GetAccountInfo(attackerId);
//                if ((attackerInfo != null) && attackerInfo.IsSuccess && attackerInfo.HasData)
//                {
                    attackerMailInfo.AttackerUsername = attackerData.PlayerName;
//                }
            }

            //Reveals the exact location of the incoming troops origin.
            if (watchTowerLevel >= 3) attackerMailInfo.Location = location;

            //Reveals the incoming troops estimated time of arrival.
            if (watchTowerLevel >= 7)
            {
                attackerMailInfo.StartTime = attackerData.MarchingArmy.StartTime;
                attackerMailInfo.ReachedTime = attackerData.MarchingArmy.ReachedTime;
            }
//            if (watchTowerLevel >= 7) attackerMailInfo.TimeTaken = attacker.Data.MarchingArmy.TimeLeft;

            var troops = new List<TroopData>();
            //Reveals the total size of incoming troops.
            if (watchTowerLevel >= 11)
            {
                var armySize = 0;
                foreach (var troop in attackerData.MarchingArmy.Troops)
                {
                    if (troop == null) continue;

                    foreach (var item in troop.TroopData)
                    {
                        if (item == null) continue;

                        armySize += item.FinalCount;

                        troops.Add(new TroopData()
                        {
                            Type = troop.TroopType.ToString(),
                            Level = item.Level,
                            Name = $"{troop.TroopType} Lvl.{item.Level}",
                            Count = item.FinalCount
                        }); ;
                    }
                }

                attackerMailInfo.TotalTroopSize = armySize;
            }

            //Reveals the exact king level of the incoming troops.
            if (watchTowerLevel >= 17) attackerMailInfo.KingLevel = attackerData.King.Level;

            //Reveals the soldier types of the incoming troops.
            if (watchTowerLevel >= 19) attackerMailInfo.Troops = troops;

            //"Reveals the number of each soldier type from the incoming troops."
            if (watchTowerLevel < 23) foreach (var item in troops) item.Count = 0; //hide amounts

            attackerMailInfo.TotalHeroSize = 0;
            attackerMailInfo.Heroes = null;
            if ((attackerData.MarchingArmy.Heroes != null) && (attackerData.MarchingArmy.Heroes.Count > 0) &&
                (watchTowerLevel >= 25))
            {
                //Displays the amount of heroes in the dispatch.
                attackerMailInfo.TotalHeroSize = attackerData.MarchingArmy.Heroes.Count;

                //Displays the type of Heroes in the dispatch.
                if (watchTowerLevel >= 30)
                {
                    var heroNames = new List<TroopData>();
                    foreach (var item in attackerData.MarchingArmy.Heroes)
                    {
                        var heroData = CacheHeroDataManager.GetFullHeroDataID(item);
                        if (heroData != null)
                        {
                            heroNames.Add(new TroopData() { Name = heroData.Info.Name });
                        }
                    }
                    attackerMailInfo.Heroes = heroNames;
                }
            }
        }



    }
}

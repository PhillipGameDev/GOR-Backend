using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Business.Manager.UserData;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models;
using GameOfRevenge.WebAdmin.Models;

namespace GameOfRevenge.WebAdmin.Pages
{
    public class _UserTroopsViewModel : PageModel
    {
        public List<TroopInfos> Data { get; set; }

        public _UserTroopsViewModel(FullPlayerCompleteData fullPlayerData)
        {
            Data = ViewTroops(fullPlayerData);
        }

        public static List<TroopInfos> ViewTroops(FullPlayerCompleteData fullPlayerData)
        {
            var list = new List<TroopInfos>();
            var types = Enum.GetValues(typeof(TroopType));
            foreach (TroopType troopType in types)
            {
                if (troopType == TroopType.Other) continue;

                TroopInfos troop = null;
                if ((fullPlayerData != null) && (fullPlayerData.Troops != null))
                {
                    troop = fullPlayerData.Troops.Find(x => (x.TroopType == troopType));
                }
                if (troop == null) troop = new TroopInfos(0, troopType, null);

                list.Add(troop);
            }

            return list;
        }

        public static async Task<IActionResult> OnGetTroopsViewAsync(int playerId)
        {
            FullPlayerCompleteData fullPlayerData = null;
            var resp = await BaseUserDataManager.GetFullPlayerData(playerId);
            if (resp.IsSuccess && resp.HasData)
            {
                fullPlayerData = new FullPlayerCompleteData(resp.Data);
            }

            return UsersModel.NewPartial("_UserTroopsView", new _UserTroopsViewModel(fullPlayerData));
        }

        public static async Task<IActionResult> OnGetEditTroopViewAsync(int playerId, string troopType)
        {
            FullPlayerCompleteData fullPlayerData = null;
            var resp = await BaseUserDataManager.GetFullPlayerData(playerId);
            if (resp.IsSuccess && resp.HasData)
            {
                fullPlayerData = new FullPlayerCompleteData(resp.Data);
            }
            var list = new List<TroopDetails>();
            if (Enum.TryParse(troopType, out TroopType troop))
            {
                var troopInfo = CacheTroopDataManager.TroopInfos.FirstOrDefault(x => (x.Info.Code == troop));
                int maxLevel = (troopInfo != null) ? troopInfo.Levels.Max(x => x.Data.Level) : 0;

                for (int lvl = 1; lvl <= maxLevel; lvl++)
                {
                    TroopDetails userTroop = null;
                    var troopGroup = fullPlayerData.Troops?.Find(x => (x.TroopType == troop));
                    if (troopGroup != null) userTroop = troopGroup.TroopData?.Find(x => (x.Level == lvl));
                    if (userTroop == null) userTroop = new TroopDetails() { Level = lvl };
                    list.Add(userTroop);
                }
            }
            var model = new InputTroopModel() {
                PlayerId = playerId,
                TroopType = troopType,
                Options = list
            };

            return UsersModel.NewPartial("Forms/_EditTroopsView", model);
        }

        public static async Task<IActionResult> OnPostSaveTroopChangesAsync(InputTroopModel inputTroop)
        {
            var playerId = inputTroop.PlayerId;
            var troopType = inputTroop.TroopType;
            var values = inputTroop.TroopValues;

            if (!Enum.TryParse(troopType, out TroopType troop)) throw new Exception("Error in form values");

            var troopChanges = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(values);
            var existChange = troopChanges?.Values.FirstOrDefault(x => !string.IsNullOrEmpty(x));
            if (existChange != null)
            {
                var resp = await BaseUserDataManager.GetFullPlayerData(playerId);
                if (!resp.IsSuccess || !resp.HasData) throw new DataNotExistExecption(resp.Message);

                var allTroops = resp.Data.Troops;
                if (allTroops == null) allTroops = new List<TroopInfos>();

                List<TroopDetails> troopDataList;
                var troops = allTroops.Find(x => (x.TroopType == troop));
                if (troops == null)
                {
                    troopDataList = new List<TroopDetails>();
                    troops = new TroopInfos(0, troop, troopDataList);
                    allTroops.Add(troops);
                }
                else
                {
                    troopDataList = troops.TroopData;
                }

                var changed = false;
                foreach (var change in troopChanges)
                {
                    if (!int.TryParse(change.Key, out int lvl) || !int.TryParse(change.Value, out int count))
                    {
                        throw new Exception("Error in form values");
                    }

                    lvl++;
                    var troopData = troopDataList.Find(x => (x.Level == lvl));
                    if (troopData == null)
                    {
                        troopData = new TroopDetails() { Level = lvl };
                        troopDataList.Add(troopData);
                    }
                    troopData.Count = count;
                    changed = true;
/*                    foreach (var troopGroup in marchingTroop.TroopData)
                {
                    var troopData = troopDataList.Find(x => (x.Level == troopGroup.Level));
                    troopData.Count -= troopGroup.Count;
                }*/
                }
                if (changed)
                {
                    var a = new UserTroopManager();
                    var updResp = await a.UpdateTroops(playerId, troop, troopDataList);
                    if (!updResp.IsSuccess) throw new Exception(updResp.Message);
                }
            }

            return new JsonResult(new { Success = true });
        }
    }
}
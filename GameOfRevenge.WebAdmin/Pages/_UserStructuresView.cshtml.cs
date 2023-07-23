using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Business.Manager.UserData;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.WebAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAdmin.Pages
{
    public class _UserStructuresViewModel : PageModel
    {
        public StructuresAndTroops Data { get; set; }

        public _UserStructuresViewModel(FullPlayerCompleteData fullPlayerData)
        {
            if (fullPlayerData != null)
            {
                Data = new StructuresAndTroops()
                {
                    Structures = fullPlayerData.Structures,
                    Troops = fullPlayerData.Troops
                };
            }
        }

        public static List<UnavaliableTroopInfo> TrainingTroop(List<TroopInfos> troops, int location)
        {
            var list = new List<UnavaliableTroopInfo>();
            foreach (var troop in troops)
            {
                foreach (var troopData in troop.TroopData)
                {
                    if (troopData.InTraning == null) continue;

                    foreach (var inTraining in troopData.InTraning)
                    {
                        if (inTraining.BuildingLocId != location) continue;

                        if (inTraining.TimeLeft > 0) list.Add(inTraining);
//                        if ((lastInTraining == null) || (inTraining.TimeLeft > lastInTraining.TimeLeft))
//                        {
//                            lastInTraining = inTraining;
//                        }
                    }
                }
            }
            return list;
        }

        public static List<UnavaliableTroopInfo> RecoveringTroop(List<TroopInfos> troops, int location)
        {
            var list = new List<UnavaliableTroopInfo>();
            foreach (var troop in troops)
            {
                foreach (var troopData in troop.TroopData)
                {
                    if (troopData.InRecovery == null) continue;

                    foreach (var inRecovery in troopData.InRecovery)
                    {
                        if (inRecovery.BuildingLocId != location) continue;

                        if (inRecovery.TimeLeft > 0) list.Add(inRecovery);
//                        if ((lastInTraining == null) || (inTraining.TimeLeft > lastInTraining.TimeLeft))
//                        {
//                            lastInTraining = inTraining;
//                        }
                    }
                }
            }
            return list;
        }

        public static List<int> GetStructureValues(StructureType structureType, int location)
        {
            var buildingInfo = CacheStructureDataManager.StructureInfos.FirstOrDefault(x => (x.Info.Code == structureType));
            int maxLevel = (buildingInfo != null) ? buildingInfo.Levels.Max(x => x.Data.Level) : 0;

            var values = new List<int>()
            {
                maxLevel
            };

            return values;
        }

        public static async Task<IActionResult> OnGetStructuresViewAsync(int playerId)
        {
            Console.WriteLine("get structures ply=" + playerId);
            FullPlayerCompleteData fullPlayerData = null;
            var resp = await BaseUserDataManager.GetFullPlayerData(playerId);
            if (resp.IsSuccess && resp.HasData)
            {
                fullPlayerData = new FullPlayerCompleteData(resp.Data);
            }

            return UserModel.NewPartial("_UserStructuresView", new _UserStructuresViewModel(fullPlayerData));
        }

        public static async Task<IActionResult> OnGetEditStructureViewAsync(int playerId, string structureType, int location)
        {
            Console.WriteLine("get edit structures ply=" + playerId+" type="+structureType+" loc="+location);
            InputStructureModel model = null;
            if (Enum.TryParse(structureType, out StructureType structure) && (structure != StructureType.Unknown))
            {
                var resp = await BaseUserDataManager.GetFullPlayerData(playerId);
                if (resp.IsSuccess && resp.HasData)
                {
                    var building = resp.Data.Structures.FirstOrDefault(x => (x.StructureType == structure))?
                                .Buildings.FirstOrDefault(x => (x.Location == location));
                    model = new InputStructureModel()
                    {
                        PlayerId = playerId,
                        StructureType = structureType,
                        StructureLocation = location,
                        Structure = building
                    };
                }
            }

            return UserModel.NewPartial("Forms/_EditStructureView", model);
        }

        public static async Task<IActionResult> OnPostSaveStructureChangesAsync(InputStructureModel inputStructure)
        {
            var playerId = inputStructure.PlayerId;
            var structureType = inputStructure.StructureType;
            var location = inputStructure.StructureLocation;
            var values = inputStructure.StructureValues;
            Console.WriteLine("post save structures ply=" + playerId + " type=" + structureType + " loc=" + location+" values="+values);

            if (!Enum.TryParse(structureType, out StructureType structure)) throw new Exception("Error in form values");

            var dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(values);
            var existChange = dic?.Values.FirstOrDefault(x => !string.IsNullOrEmpty(x));
            if (existChange != null)
            {
                var structureChanges = new Dictionary<string, int>();
                foreach (var change in dic)
                {
                    if (!int.TryParse(change.Value, out int val)) throw new Exception("Error in form values");

                    structureChanges.Add(change.Key, val);
                }

                var a = new UserStructureManager();
                var updResp = await a.UpdateBuildingData(playerId, structure, location, structureChanges);
                if (!updResp.IsSuccess) throw new Exception(updResp.Message);
            }

            return new JsonResult(new { Success = true });
        }
    }
}
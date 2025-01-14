﻿using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GameOfRevenge.Common.Models;
using GameOfRevenge.WebAdmin.Models;

namespace GameOfRevenge.WebAdmin.Pages
{
    public class _UserBackupsViewModel : PageModel
    {
        public List<PlayerBackupTable> Data { get; set; }

        public string FullDate(DateTime date)
        {
            return date.ToString("yyy/M/d h:mm:ss tt");// ToLongDateString() + ' ' + date.ToLongTimeString();
        }

        public _UserBackupsViewModel(List<PlayerBackupTable> backups)
        {
            Data = backups;
        }

/*        public static List<UserHeroDetails> ViewHeroes(FullPlayerCompleteData fullPlayerData)
        {
            var list = new List<UserHeroDetails>();
            var types = Enum.GetValues(typeof(HeroType));
            foreach (HeroType heroType in types)
            {
                if (heroType == HeroType.Unknown) continue;

                UserHeroDetails hero = null;
                if ((fullPlayerData != null) && (fullPlayerData.Heroes != null))
                {
                    hero = fullPlayerData.Heroes.Find(x => (x.HeroType == heroType));
                }
                if (hero == null) hero = new UserHeroDetails() { HeroType = heroType };

                list.Add(hero);
            }

            return list;
        }*/

        public static async Task<IActionResult> OnGetBackupsViewAsync(UsersModel userModel, int playerId)
        {
//            Console.WriteLine("get backups ply="+playerId);

            List<PlayerBackupTable> list = null;
            var resp = await userModel.AdminManager.GetPlayerBackups(playerId);
            if (resp.IsSuccess && resp.HasData)
            {
                list = resp.Data;
            }

            return UsersModel.NewPartial("_UserBackupsView", new _UserBackupsViewModel(list));
        }

        public static IActionResult OnGetRestoreBackupView(UsersModel userModel, int playerId, long backupId, string date)
        {
//            Console.WriteLine("get restore backup ply="+playerId+" id="+ backupId+" date="+date);
            InputRestoreModel model = null;
//            var resp = await userModel.AdminManager.GetPlayerBackup(backupId);
//            if (resp.IsSuccess && resp.HasData)
            {
                model = new InputRestoreModel()
                {
                    PlayerId = playerId,
                    BackupId = backupId,
                    BackupDate = date
                };
            }

            return UsersModel.NewPartial("Forms/_RestoreBackupView", model);
        }

        public static async Task<IActionResult> OnPostRestoreBackupAsync(UsersModel userModel, InputRestoreModel inputRestore)
        {
            int playerId = inputRestore.PlayerId;
            long backupId = inputRestore.BackupId;
//            Console.WriteLine("post restore ply = " + playerId + " id = " + backupId);

            var resp = await userModel.AdminManager.RestorePlayerBackup(playerId, backupId);
            if (!resp.IsSuccess) throw new Exception(resp.Message);




/*            var resp = await userModel.AdminManager.GetPlayerBackup(backupId);
            if (!resp.IsSuccess || !resp.HasData) throw new Exception(resp.Message);

            var json = resp.Data.Data;
            Console.WriteLine("player data =" + json);
            AllPlayerData allPlayerData = null;
            try
            {
                allPlayerData = Newtonsoft.Json.JsonConvert.DeserializeObject<AllPlayerData>(json);
            }
            catch
            {
                return userModel.BadRequest();
            }*/

//            var pdm = new PlayerDataManager();
//            pdm.UpdatePlayerDataID(0, 0, "");

//            fullPlayerData.PlayerName

            return new JsonResult(new { Success = true });
        }


        public static IActionResult OnGetCreateBackupView(UsersModel userModel, int playerId)
        {
//            Console.WriteLine("get create backup ply=" + playerId);
            InputBackupModel model = null;
            //            var resp = await BaseUserDataManager.GetFullPlayerData(playerId);
            //            if (resp.IsSuccess && resp.HasData)
            {
                model = new InputBackupModel()
                {
                    PlayerId = playerId
                };
            }

            return UsersModel.NewPartial("Forms/_CreateBackupView", model);
        }

        public static async Task<IActionResult> OnPostCreateBackupAsync(UsersModel userModel, InputBackupModel inputBackup)
        {
            int playerId = inputBackup.PlayerId;
//            Console.WriteLine("post create backup ply = " + playerId);

            var allFullPlayerData = await userModel.AdminManager.GetAllFullPlayerData(playerId);
            if (allFullPlayerData == null) throw new Exception("Error retrieving user data");

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(allFullPlayerData);
            var resp = await userModel.AdminManager.SavePlayerBackup(playerId, "Manual backup", json);
            if (!resp.IsSuccess) throw new Exception(resp.Message);

            return new JsonResult(new { Success = true });
        }
    }
}
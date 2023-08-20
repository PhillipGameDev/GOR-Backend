using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Business.Manager.UserData;
using GameOfRevenge.WebAdmin.Models;

namespace GameOfRevenge.WebAdmin.Pages
{
    //    [IgnoreAntiforgeryToken(Order = 1001)]
    public class UsersModel : PageModel
    {
        private readonly IAdminDataManager manager;

        private static List<PlayerID> Users { get; set; } = new List<PlayerID>();
        private static List<int> Pages { get; set; } = new List<int>();

        public UserTable UserTable { get; set; } = new UserTable();

        public IAdminDataManager AdminManager => manager;

        public UsersModel(IAdminDataManager adminDataManager)
        {
            manager = adminDataManager;
            //            string title = ViewData["Title"] as string;
        }

        public static PartialViewResult NewPartial(string name, object model)
        {
            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };
            viewData.Add("Region", "HTMLSection");

            var partialResult = new PartialViewResult
            {
                ViewName = name,
                ViewData = viewData
            };

            return partialResult;
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToPage("/login");
        }

        public async Task<IActionResult> OnGetFullPlayerDataAsync(int playerId)//method used after restore backup
        {
            var fullPlayerData = await manager.GetFullPlayerData(playerId, false);

            string json = null;
            if (fullPlayerData != null) json = Newtonsoft.Json.JsonConvert.SerializeObject(fullPlayerData);

            return new JsonResult(new { Success = true, Data = json });
        }

        public async Task<IActionResult> OnGetUserViewAsync(int playerId)
        {
            var fullPlayerData = await manager.GetFullPlayerData(playerId, true, true);
            if (fullPlayerData == null) return BadRequest();

            return NewPartial("_UserView", fullPlayerData);
        }

        async Task SetPageAsync(int pageIndex)
        {
            var userTable = new UserTable();
            userTable.CurrentPage = pageIndex;

            var resp = await manager.GetPlayersInfo(Pages[pageIndex]);
            if (resp.IsSuccess)
            {
                if (resp.HasData && (resp.Data.Count > 0))
                {
                    userTable.Users = resp.Data.ToArray();
                }
                else
                {
                    userTable.Users = new PlayerInfo[] { new PlayerInfo() { Name = "No Users" } };
                }
            }
            else
            {
                userTable.Users = new PlayerInfo[] { new PlayerInfo() { Name = resp.Message } };
            }
            userTable.LastPage = (int)Math.Ceiling(Users.Count / 10f) - 1;

            var len = 9;
            var pad = (len - 1) / 2;
            var minPage = pageIndex - pad;
            if (minPage < 0)
            {
                minPage = 0;
            }
            else if ((pageIndex + pad) > userTable.LastPage)
            {
                minPage = userTable.LastPage - (len - 1);
            }
            userTable.OffsetPage = minPage;
            var pages = new int[len];
            for (var idx = 0; idx < len; idx++)
            {
                pages[idx] = Users[(minPage + idx) * 10].PlayerId - 1;
            }
            userTable.Pages = pages;

            UserTable = userTable;
        }

        public async Task OnGetAsync()
        {
            var resp = await manager.GetPlayers();
            if (resp.IsSuccess)
            {
                if (resp.HasData && (resp.Data.Count > 0))
                {
                    Users = resp.Data;
                }
            }
            var pages = new List<int>();
            var len = Users.Count;
            for (var idx = 0; idx < len; idx += 10)
            {
                pages.Add(Users[idx].PlayerId - 1);
            }
            Pages = pages;

            await SetPageAsync(0);
        }

        public async Task<IActionResult> OnGetUserPageViewAsync(int pageIndex, string userId)
        {
            if (userId != null)
            {
                if (int.TryParse(userId, out int id))
                {
                    if (id <= Pages[0])
                    {
                        pageIndex = 0;
                    }
                    else
                    {
                        pageIndex = Pages.FindLastIndex(x => (x < id));
                    }
                    await SetPageAsync(pageIndex);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                await SetPageAsync(pageIndex);
            }

            return NewPartial("_UsersTableView", UserTable);
        }



        [BindProperty]
        public InputResourceModel InputResource { get; set; }

        public async Task<IActionResult> OnGetEditResourceViewAsync(int playerId, string resourceType)
        {
//            Console.WriteLine("get edit resource ply="+playerId+" type="+ resourceType);
            InputResourceModel model = null;
            if (Enum.TryParse(resourceType, out ResourceType resource) && (resource != ResourceType.Other))
            {
                var resp = await BaseUserDataManager.GetFullPlayerData(playerId);
                if (resp.IsSuccess && resp.HasData)
                {
                    long value = 0;
                    switch (resource)
                    {
                        case ResourceType.Food: value = resp.Data.Resources.Food; break;
                        case ResourceType.Wood: value = resp.Data.Resources.Wood; break;
                        case ResourceType.Ore: value = resp.Data.Resources.Ore; break;
                        case ResourceType.Gems: value = resp.Data.Resources.Gems; break;
                        case ResourceType.Gold: value = resp.Data.Resources.Gold; break;
                    }
                    model = new InputResourceModel()
                    {
                        PlayerId = playerId,
                        ResourceType = resourceType,
                        Value = value
                    };
                }
            }

            return NewPartial("Forms/_EditResourceView", model);
        }
        public async Task<IActionResult> OnPostSaveResourceChangeAsync()
        {
            var playerId = InputResource.PlayerId;
            var resourceType = InputResource.ResourceType;
            var resourceValue = InputResource.ResourceValue;
//            Console.WriteLine("on save resource ply="+ playerId+" type="+ resourceType+ " value=" + resourceValue);

            try
            {
                if (!Enum.TryParse(resourceType, out ResourceType resource) ||
                    (resource == ResourceType.Other) || !long.TryParse(resourceValue, out long val))
                {
                    throw new Exception("Error in form values");
                }

                var a = new UserResourceManager();
                var updResp = await a.UpdateResource(playerId, resource, val);
                if (!updResp.IsSuccess) throw new Exception(updResp.Message);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

            return new JsonResult(new { Success = true });
        }

        [BindProperty]
        public InputStructureModel InputStructure { get; set; }

        public async Task<IActionResult> OnGetStructuresViewAsync(int playerId) =>
                await _UserStructuresViewModel.OnGetStructuresViewAsync(playerId);
        public async Task<IActionResult> OnGetEditStructureViewAsync(int playerId, string structureType, int location) =>
                await _UserStructuresViewModel.OnGetEditStructureViewAsync(playerId, structureType, location);
        public async Task<IActionResult> OnPostSaveStructureChangesAsync()
        {
            try
            {
                return await _UserStructuresViewModel.OnPostSaveStructureChangesAsync(InputStructure);
            }
            catch { }

            return BadRequest();
        }

        [BindProperty]
        public InputTroopModel InputTroop { get; set; }

        public async Task<IActionResult> OnGetTroopsViewAsync(int playerId) =>
                await _UserTroopsViewModel.OnGetTroopsViewAsync(playerId);
        public async Task<IActionResult> OnGetEditTroopViewAsync(int playerId, string troopType) =>
                await _UserTroopsViewModel.OnGetEditTroopViewAsync(playerId, troopType);
        public async Task<IActionResult> OnPostSaveTroopChangesAsync()
        {
            try
            {
                return await _UserTroopsViewModel.OnPostSaveTroopChangesAsync(InputTroop);
            }
            catch { }

            return BadRequest();
        }

        [BindProperty]
        public InputHeroModel InputHero { get; set; }

        public async Task<IActionResult> OnGetHeroesViewAsync(int playerId) =>
                await _UserHeroesViewModel.OnGetHeroesViewAsync(playerId);
        public async Task<IActionResult> OnGetEditHeroViewAsync(int playerId, string heroType) =>
                await _UserHeroesViewModel.OnGetEditHeroViewAsync(playerId, heroType);
        public async Task<IActionResult> OnPostSaveHeroChangesAsync()
        {
            try
            {
                return await _UserHeroesViewModel.OnPostSaveHeroChangesAsync(InputHero);
            }
            catch { }

            return BadRequest();
        }

        [BindProperty]
        public InputRewardModel InputReward { get; set; }

        public async Task<IActionResult> OnGetRewardsViewAsync(int playerId) =>
                await _UserRewardsViewModel.OnGetRewardsViewAsync(manager, playerId);
        public async Task<IActionResult> OnGetEditRewardViewAsync(int playerId, long playerDataId, string description) =>
                await _UserRewardsViewModel.OnGetEditRewardViewAsync(manager, playerId, playerDataId, description);
        public async Task<IActionResult> OnPostSaveRewardChangeAsync()
        {
            try
            {
                return await _UserRewardsViewModel.OnPostSaveRewardChangeAsync(manager, InputReward);
            }
            catch { }

            return BadRequest();
        }

        [BindProperty]
        public InputRewardsModel InputRewards { get; set; }

        public async Task<IActionResult> OnGetAddRewardsViewAsync(int playerId, bool applyToAll) =>
                await _UserRewardsViewModel.OnGetAddRewardsViewAsync(manager, playerId, applyToAll);
        public async Task<IActionResult> OnPostAddRewardAsync()
        {
            try
            {
                return await _UserRewardsViewModel.OnPostAddRewardAsync(manager, InputRewards);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return BadRequest();
        }


        [BindProperty]
        public InputRestoreModel InputRestore { get; set; }

        public IActionResult OnGetRestoreBackupView(int playerId, long backupId, string date) =>
                _UserBackupsViewModel.OnGetRestoreBackupView(this, playerId, backupId, date);
        public async Task<IActionResult> OnPostRestoreBackupAsync()
        {
            try
            {
                return await _UserBackupsViewModel.OnPostRestoreBackupAsync(this, InputRestore);
            }
            catch { }

            return BadRequest();
        }

        [BindProperty]
        public InputBackupModel InputBackup { get; set; }

        public async Task<IActionResult> OnGetBackupsViewAsync(int playerId) =>
                await _UserBackupsViewModel.OnGetBackupsViewAsync(this, playerId);
        public IActionResult OnGetCreateBackupView(int playerId) =>
                _UserBackupsViewModel.OnGetCreateBackupView(this, playerId);
        public async Task<IActionResult> OnPostCreateBackupAsync()
        {
            try
            {
                return await _UserBackupsViewModel.OnPostCreateBackupAsync(this, InputBackup);
            }
            catch { }

            return BadRequest();
        }
    }
}
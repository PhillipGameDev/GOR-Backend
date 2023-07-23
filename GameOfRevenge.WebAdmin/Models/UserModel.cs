using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Models.Clan;
using GameOfRevenge.Common;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Business.Manager.UserData;
using WebAdmin.Pages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GameOfRevenge.WebAdmin.Models
{
    //    [IgnoreAntiforgeryToken(Order = 1001)]
    public class UserModel : PageModel
    {
        private readonly IAdminDataManager manager;
        private readonly IClanManager userClanManager;

        private static List<PlayerID> Users { get; set; } = new List<PlayerID>();
        private static List<int> Pages { get; set; } = new List<int>();

        public UserTable UserTable { get; set; } = new UserTable();

        public UserModel(IAdminDataManager adminDataManager, IClanManager clanManager)
        {
            manager = adminDataManager;
            userClanManager = clanManager;
            //            string title = ViewData["Title"] as string;
        }

        public static PartialViewResult NewPartial(string name, object model)
        {
            var partialResult = new PartialViewResult
            {
                ViewName = name,
                ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                }
            };

            return partialResult;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToPage("/Login");
        }

        public async Task<IActionResult> OnGetUserViewAsync(int playerId)
        {
            FullPlayerCompleteData fullPlayerData = null;
            var respInfo = await manager.GetPlayersInfo(playerId, 1);
            if (respInfo.IsSuccess && respInfo.HasData && (respInfo.Data.Count > 0))
            {
                var playerInfo = respInfo.Data[0];

                var resp = await BaseUserDataManager.GetFullPlayerData(playerId);
                if (resp.IsSuccess && resp.HasData)
                {
                    fullPlayerData = new FullPlayerCompleteData(resp.Data);
                    var clanData = userClanManager.GetClanData(playerInfo.AllianceId);
                    if (clanData.Result.IsSuccess && clanData.Result.HasData)
                    {
                        fullPlayerData.Clan = clanData.Result.Data;
                    }
                }
            }

            if (fullPlayerData == null)
            {
                fullPlayerData = new FullPlayerCompleteData() { PlayerId = playerId };
            }

            return Partial("_UserView", fullPlayerData);
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

        public async Task<IActionResult> OnGetUserPageViewAsync(int pageIndex)
        {
            await SetPageAsync(pageIndex);

            return Partial("_UsersTableView", UserTable);
        }








        [BindProperty]
        public InputResourceModel InputResource { get; set; }

        public async Task<IActionResult> OnGetEditResourceViewAsync(int playerId, string resourceType)
        {
            Console.WriteLine("get edit resource ply="+playerId+" type="+ resourceType);
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
            Console.WriteLine("on save resource ply="+ playerId+" type="+ resourceType+ " value=" + resourceValue);

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
            Console.WriteLine("on save hero");
            try
            {
                return await _UserHeroesViewModel.OnPostSaveHeroChangesAsync(InputHero);
            }
            catch { }

            return BadRequest();
        }
    }

    public class UserTable
    {
        public PlayerInfo[] Users { get; set; }
        public int[] Pages { get; set; }
        public int CurrentPage { get; set; }
        public int LastPage { get; set; }
        public int OffsetPage { get; set; }

        public int PageFromOffset(int shift)
        {
            var page = OffsetPage + shift;
            if (page < 0) page = 0;
            if (page > LastPage) page = LastPage;
            return page;
        }

        public int PageFromCurrent(int shift)
        {
            if (shift < 0)
            {
                return Math.Max(CurrentPage + shift, 0);
            }
            else
            {
                return Math.Min(CurrentPage + shift, LastPage);
            }
        }
    }

    public class FullPlayerCompleteData : PlayerCompleteData
    {
        public ClanData Clan { get; set; }

        public int KingLevel => (King != null)? King.Level : 0;
        public long Food => (Resources != null) ? Resources.Food : 0;
        public long Wood => (Resources != null) ? Resources.Wood : 0;
        public long Ore => (Resources != null) ? Resources.Ore : 0;
        public long Gems => (Resources != null) ? Resources.Gems : 0;

        public FullPlayerCompleteData()
        {
        }

        public FullPlayerCompleteData(PlayerCompleteData data)
        {
            PlayerId = data.PlayerId;
            PlayerName = data.PlayerName;
            IsDeveloper = data.IsDeveloper;
            IsAdmin = data.IsAdmin;

            HelpedBuild = data.HelpedBuild;
            ClanId = data.ClanId;
            King = data.King;
            VIP = data.VIP;
            VIPPoints = data.VIPPoints;
            Workers = data.Workers;

            Resources = data.Resources;
            MarchingArmy = data.MarchingArmy;

            Structures = data.Structures;
            Troops = data.Troops;
            Technologies = data.Technologies;
            Items = data.Items;
            Boosts = data.Boosts;
            Heroes = data.Heroes;
        }
    }
}

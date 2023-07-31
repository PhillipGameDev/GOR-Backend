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
using System.Linq;
using GameOfRevenge.Common.Models.Structure;
using GameOfRevenge.Common.Models.Quest;
using GameOfRevenge.Business.Manager.GameDef;
using static GameOfRevenge.WebAdmin.Models.UserModel;

namespace GameOfRevenge.WebAdmin.Models
{
    //    [IgnoreAntiforgeryToken(Order = 1001)]
    public class UserModel : PageModel
    {
        private readonly IAdminDataManager manager;
        private readonly IClanManager userClanManager;
        private readonly IUserQuestManager userQuestManager;
        private readonly IPlayerDataManager playerDataManager;

        private static List<PlayerID> Users { get; set; } = new List<PlayerID>();
        private static List<int> Pages { get; set; } = new List<int>();

        public UserTable UserTable { get; set; } = new UserTable();

        public IAdminDataManager AdminManager => manager;

        public UserModel(IAdminDataManager adminDataManager, IPlayerDataManager playerManager,
                        IClanManager clanManager, IUserQuestManager questManager)
        {
            manager = adminDataManager;
            userClanManager = clanManager;
            userQuestManager = questManager;
            playerDataManager = playerManager;
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

        public async Task<AllPlayerData> GetAllPlayerData(int playerId)
        {
            try
            {
                var allPlayerData = new AllPlayerData();

                //PLAYER INFO
                var playerInfo = await manager.GetPlayerInfo(playerId);
                if (!playerInfo.IsSuccess || !playerInfo.HasData) throw new Exception();

                allPlayerData.PlayerInfo = playerInfo.Data;

                //ALL PLAYER DATA
                var allData = await playerDataManager.GetAllPlayerData(playerId);
                if (!allData.IsSuccess || !allData.HasData) throw new Exception();

                allPlayerData.PlayerData = allData.Data;

                //QUESTS
                var questsResp = await userQuestManager.GetAllQuestProgress(playerId);
                if (!questsResp.IsSuccess || !questsResp.HasData) throw new Exception();

                allPlayerData.QuestData = questsResp.Data;

                return allPlayerData;
            }
            catch
            {
            }

            return null;
        }

        public class PlayerDataReward : PlayerRewardDataTable
        {
            public int RewardId { get; set; }
        }

        public async Task<List<PlayerDataReward>> GetAllPlayerRewards(int playerId)
        {
            var rewardsResp = await userQuestManager.GetUserAllRewards(playerId);
            if (rewardsResp.IsSuccess && rewardsResp.HasData)
            {
                var allPlayerRewards = new List<PlayerDataReward>();
                var playerDataResp = await playerDataManager.GetAllPlayerData(playerId, DataType.Reward);
                if (playerDataResp.IsSuccess && playerDataResp.HasData)
                {
                    foreach (var userReward in rewardsResp.Data)
                    {
                        var reward = new PlayerDataReward()
                        {
                            PlayerDataId = userReward.PlayerDataId,
                            DataType = userReward.DataType,
                            ValueId = userReward.ValueId,
                            Value = userReward.Value,
                            Count = userReward.Count,
                            RewardId = playerDataResp.Data.Find(x => (x.Id == userReward.PlayerDataId)).ValueId
                        };
                        allPlayerRewards.Add(reward);
                    }
                    allPlayerRewards = allPlayerRewards.OrderBy(x => x.DataType).ThenBy(x => x.RewardId).ThenBy(x => x.ValueId).ThenBy(x => x.Value).ToList();
                }

                return allPlayerRewards;
            }

            return null;
        }

        public async Task<List<DataReward>> GetAvailableRewards()
        {
            var qm = new QuestManager();
            var resp = await qm.GetAllQuestRewards();
            if (resp.IsSuccess && resp.HasData)
            {
                return resp.Data;
            }

            return null;
        }

        public async Task<FullPlayerCompleteData> GetFullPlayerData(int playerId, bool allData = true, bool getBackups = false)
        {
            FullPlayerCompleteData fullPlayerData = null;

            var resp = await BaseUserDataManager.GetFullPlayerData(playerId);
            if (resp.IsSuccess && resp.HasData)
            {
                fullPlayerData = new FullPlayerCompleteData(resp.Data);

                //CLAN
                var clanResp = userClanManager.GetClanData(fullPlayerData.ClanId);
                if (clanResp.Result.IsSuccess && clanResp.Result.HasData)
                {
                    fullPlayerData.Clan = clanResp.Result.Data;
                }

                if (allData)
                {
                    //REWARDS
                    fullPlayerData.Rewards = await GetAllPlayerRewards(playerId);

                    //QUESTS
                    var questsResp = await userQuestManager.GetAllQuestProgress(playerId);
                    if (questsResp.IsSuccess && questsResp.HasData)
                    {
                        fullPlayerData.Quests = questsResp.Data;
                    }
                }

                //BACKUPS
                if (getBackups)
                {
                    var backupResp = await AdminManager.GetPlayerBackups(playerId);
                    if (backupResp.IsSuccess)
                    {
                        fullPlayerData.Backups = backupResp.Data;
                    }
                }
            }
            else
            {
                Console.WriteLine(resp.Message);
            }

            return fullPlayerData;
        }

        public async Task<IActionResult> OnGetFullPlayerDataAsync(int playerId)//method used after restore backup
        {
            var fullPlayerData = await GetFullPlayerData(playerId, false);

            string json = null;
            if (fullPlayerData != null) json = Newtonsoft.Json.JsonConvert.SerializeObject(fullPlayerData);

            return new JsonResult(new { Success = true, Data = json });
        }

        public async Task<IActionResult> OnGetUserViewAsync(int playerId)
        {
            var fullPlayerData = await GetFullPlayerData(playerId, true, true);

            if (fullPlayerData == null)
            {
                return BadRequest();
//                fullPlayerData = new FullPlayerCompleteData() { PlayerId = playerId };
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

            return Partial("_UsersTableView", UserTable);
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
        public async Task<IActionResult> OnGetSaveResourceChangeAsync(int playerId, string resourceType, string resourceValue)
        {
            InputResource = new InputResourceModel
            {
                PlayerId = playerId,
                ResourceType = resourceType,
                ResourceValue = resourceValue
            };
            return await OnPostSaveResourceChangeAsync();
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
        public async Task<IActionResult> OnGetSaveStructureChangesAsync(int playerId, string structureType, int structureLocation, string structureValues)
        {
            InputStructure = new InputStructureModel
            {
                PlayerId = playerId,
                StructureType = structureType,
                StructureLocation = structureLocation,
                StructureValues = structureValues
            };
            return await OnPostSaveStructureChangesAsync();
        }
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
        public async Task<IActionResult> OnGetSaveTroopChangesAsync(int playerId, string troopType, string troopValues)
        {
            InputTroop = new InputTroopModel
            {
                PlayerId = playerId,
                TroopType = troopType,
                TroopValues = troopValues
            };
            return await OnPostSaveTroopChangesAsync();
        }
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
        public async Task<IActionResult> OnGetSaveHeroChangesAsync(int playerId, string heroType, string heroValues)
        {
            InputHero = new InputHeroModel
            {
                PlayerId = playerId,
                HeroType = heroType,
                HeroValues = heroValues
            };
            return await OnPostSaveHeroChangesAsync();
        }
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
                await _UserRewardsViewModel.OnGetRewardsViewAsync(this, playerId);
        public async Task<IActionResult> OnGetEditRewardViewAsync(int playerId, long playerDataId, string description) =>
                await _UserRewardsViewModel.OnGetEditRewardViewAsync(this, playerId, playerDataId, description);
        public async Task<IActionResult> OnGetSaveRewardChangeAsync(int playerId, long playerDataId, int rewardValue)
        {
            InputReward = new InputRewardModel()
            {
                PlayerId = playerId,
                PlayerDataId = playerDataId,
                RewardValue = rewardValue
            };
            return await OnPostSaveRewardChangeAsync();
        }
        public async Task<IActionResult> OnPostSaveRewardChangeAsync()
        {
            try
            {
                return await _UserRewardsViewModel.OnPostSaveRewardChangeAsync(this, InputReward);
            }
            catch { }

            return BadRequest();
        }

        [BindProperty]
        public InputRewardsModel InputRewards { get; set; }

        public async Task<IActionResult> OnGetAddRewardsViewAsync(int playerId, bool applyToAll) =>
                await _UserRewardsViewModel.OnGetAddRewardsViewAsync(this, playerId, applyToAll);
        public async Task<IActionResult> OnGetAddRewardAsync(int playerId, string rewardValues, bool applyToAll)
        {
            InputRewards = new InputRewardsModel()
            {
                PlayerId = playerId,
                ApplyToAll = applyToAll,
                RewardValues = rewardValues
            };
            return await OnPostAddRewardAsync();
        }
        public async Task<IActionResult> OnPostAddRewardAsync()
        {
            try
            {
                return await _UserRewardsViewModel.OnPostAddRewardAsync(this, InputRewards);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return BadRequest();
        }


        [BindProperty]
        public InputRestoreModel InputRestore { get; set; }

//        public async Task<IActionResult> OnGetBackupsViewAsync(int playerId) =>
//                await _UserBackupsViewModel.OnGetBackupsViewAsync(playerId);
        public IActionResult OnGetRestoreBackupView(int playerId, long backupId, string date) =>
                _UserBackupsViewModel.OnGetRestoreBackupView(this, playerId, backupId, date);
        public async Task<IActionResult> OnGetRestoreBackupAsync(int playerId, long backupId)
        {
            InputRestore = new InputRestoreModel()
            {
                PlayerId = playerId,
                BackupId = backupId
            };
            return await OnPostRestoreBackupAsync();
        }
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
        public async Task<IActionResult> OnGetCreateBackupAsync(int playerId)
        {
            InputBackup = new InputBackupModel()
            {
                PlayerId = playerId
            };

            return await OnPostCreateBackupAsync();
        }
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
        public List<PlayerDataReward> Rewards { get; set; }
        public List<PlayerBackupTable> Backups { get; set; }
        public List<PlayerQuestDataTable> Quests { get; set; }

        public int KingLevel => (King != null)? King.Level : 0;
        public int CastleLevel
        {
            get
            {
                var structure = Structures?.Find(x => (x.StructureType == StructureType.CityCounsel));
                var castle = structure?.Buildings.FirstOrDefault();
                var castleLvl = (castle != null) ? castle.CurrentLevel : 0;

                return castleLvl;
            }
        }
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

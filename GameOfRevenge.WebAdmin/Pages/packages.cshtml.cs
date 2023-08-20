using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Business.Manager.GameDef;
using GameOfRevenge.WebAdmin.Models;
using System.Net.Http;
using System.Net;
using System.Net.NetworkInformation;
using GameOfRevenge.Common;

namespace GameOfRevenge.WebAdmin.Pages
{
    public class GameModel : PageModel
    {
        [BindProperty]
        public IReadOnlyList<StorePackageTable> Data { get; set; }

        private readonly IAdminDataManager manager;

        public GameModel(IAdminDataManager adminDataManager)
        {
            manager = adminDataManager;
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

        public IActionResult OnGetOptionDescription(string dataType, int valueId, int value)
        {
            var resp = "";
            try
            {
                var reward = new DataReward()
                {
                    DataType = (DataType)Enum.Parse(typeof(DataType), dataType),
                    ValueId = valueId,
                    Value = value
                };

                resp = reward.GetProperties().Item1;
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

            return new JsonResult(new { Success = true, Data = resp });
        }

        private IPAddress GetLocalIPAddress()
        {
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var networkInterface in networkInterfaces)
            {
                var ipProps = networkInterface.GetIPProperties();

                foreach (var ip in ipProps.UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ip.Address;
                    }
                }
            }

            throw new Exception("Local IP not found");
        }

        public async Task<IActionResult> OnGetResetCacheAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = $"http://{GetLocalIPAddress()}:9001/api/Server/ResetConnection";
                    HttpResponseMessage resp = await client.GetAsync(url);
                    if (resp.IsSuccessStatusCode)
                    {
//                        string responseBody = await resp.Content.ReadAsStringAsync();
//                        Console.WriteLine("Response:" + responseBody);
                    }
                    else
                    {
                        Console.WriteLine("Error: " + resp.StatusCode);
                        throw new Exception("Error: " + resp.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

            return new JsonResult(new { Success = true });
        }

        public static string GetName(string productId)
        {
            var resp = productId;
            switch (productId)
            {
                case "p_a001": resp = "Favorites Pack"; break;
                case "p_a002": resp = "Rookie Starter Pack"; break;
                case "p_a003": resp = "Hero Boost Pack"; break;
                case "p_a004": resp = "Construction Pack"; break;
                case "p_a005": resp = "Grand Construction Pack"; break;
                case "p_a006": resp = "Super Value Offer Pack"; break;
                case "r_a001": resp = "Peasant's Bounty Pack"; break;
                case "r_a002": resp = "Merchant's Treasure Pack"; break;
                case "r_a003": resp = "King's Fortune Pack"; break;
            }

            return resp;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var resp = await manager.GetPackages();
            if (!resp.IsSuccess || !resp.HasData) return BadRequest();

            Data = resp.Data;

            return Page();
        }

        public async Task<IActionResult> OnGetPackageViewAsync(int packageId)
        {
//            Console.WriteLine("package=" + packageId);

            var packages = await manager.GetAllProductPackages();
            if (packages == null) return BadRequest();

            var package = packages.Find(x => (x.PackageId == packageId));
            if (package == null) return BadRequest();

            return NewPartial("_PackageView", new InputPackageModel(package, GetName(package.ProductId), package.Active));
        }

        [BindProperty]
        public InputPackageModel InputPackage { get; set; }

        public IActionResult OnGetEditPackageView(int packageId, string name, int cost, bool active)
        {
//            Console.WriteLine("edit package=" + packageId+" cost="+cost+" active="+active);

            var package = new InputPackageModel()
            {
                PackageId = packageId,
                PackageName = name,
                PackageCost = cost,
                PackageActive = active
            };

            return NewPartial("Forms/_EditPackageView", package);
        }
        public async Task<IActionResult> OnPostSavePackageChangeAsync()
        {
            var packageId = InputPackage.PackageId;
            var packageCost = InputPackage.PackageCost;
            var packageActive = InputPackage.PackageActive;
//            Console.WriteLine("on save package pkg="+ packageId + " cost="+ packageCost + " active=" + packageActive);

            try
            {
                var resp = await manager.UpdatePackage(packageId, packageCost, packageActive);
                if (!resp.IsSuccess) throw new Exception(resp.Message);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

            return new JsonResult(new { Success = true });
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

        [BindProperty]
        public InputPackageRewardModel InputReward { get; set; }

        public async Task<IActionResult> OnGetPackageRewardsViewAsync(int packageId) =>
                await _PackageRewardsViewModel.OnGetPackageRewardsViewAsync(manager, packageId);
        public IActionResult OnGetEditPackageRewardView(int packageId, int rewardId, int count, string description) =>
                _PackageRewardsViewModel.OnGetEditPackageRewardView(manager, packageId, rewardId, count, description);
        public async Task<IActionResult> OnPostSavePackageRewardChangeAsync()
        {
            try
            {
                return await _PackageRewardsViewModel.OnPostSavePackageRewardChangeAsync(manager, InputReward);
            }
            catch { }

            return BadRequest();
        }

        [BindProperty]
        public InputPackageRewardsModel InputRewards { get; set; }

        public async Task<IActionResult> OnGetAddPackageRewardsViewAsync(int packageId, int questId) =>
                await _PackageRewardsViewModel.OnGetAddPackageRewardsViewAsync(manager, packageId, questId);
        public async Task<IActionResult> OnPostAddPackageRewardAsync()
        {
            try
            {
                return await _PackageRewardsViewModel.OnPostAddPackageRewardAsync(manager, InputRewards);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return BadRequest();
        }
    }
}
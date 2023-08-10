using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.WebAdmin.Models;

namespace GameOfRevenge.WebAdmin.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public ChartData Data { get; set; }

        private readonly IAdminDataManager manager;

        public IndexModel(IAdminDataManager adminDataManager)
        {
            manager = adminDataManager;
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToPage("/login");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Data = new ChartData();
            var resp = await manager.GetDailyVisits();
            if (resp.IsSuccess && resp.HasData)
            {
                Data.NewUsers = '[' + resp.Data.NewUsers + ']';
                Data.Recurring = '[' + resp.Data.Recurring + ']';
            }
            else
            {
                Console.WriteLine("error "+resp.Message);
            }

            var resp2 = await manager.GetActiveUsers();
            if (resp2.IsSuccess && resp2.HasData)
            {
                Data.WithinOneMonth = resp2.Data.WithinOneMonth;
                Data.WithinThreeMonths = resp2.Data.WithinThreeMonths;
                Data.WithinSixMonths = resp2.Data.WithinSixMonths;
            }

            return Page();
        }
    }
}
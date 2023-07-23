using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Business.Manager.UserData;
using GameOfRevenge.Common.Interface.UserData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAdmin.Pages
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

    public class ChartData
    {
        public string NewUsers;
        public string Recurring;
        public int WithinOneMonth;
        public int WithinThreeMonths;
        public int WithinSixMonths;

        public int TotalNewUsers
        {
            get
            {
                var total = 0;
                var strs = NewUsers.Split(',');
                foreach (var str in strs)
                {
                    if (int.TryParse(str, out int val)) total += val;
                }
                return total;
            }
        }

        public ChartData()
        {
        }
    }
}
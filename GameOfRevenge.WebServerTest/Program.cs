using System;
using GameOfRevenge.Business;
using System.Configuration;
using System.Threading.Tasks;

namespace GameOfRevenge.WebServer
{
    public class Program
    {
        static void Main(string[] args)
        {

            //            Config.ConnectionString = ConfigurationManager.AppSettings["ConString"];
            Config.ConnectionString = "Data Source=52.194.60.233,1433;Initial Catalog=GameOfRevenge;Persist Security Info=True;User ID=developer;Password=developer";

            Config.DefaultWorldCode = ConfigurationManager.AppSettings["DefaultWorldCode"];

            Console.WriteLine(">>>>>> Main "+ Config.ConnectionString);
            Test().Wait();
        }

        private static async Task Test()
        {
            await Startup.ReloadDataBaseDataAsync();
        }
    }
}

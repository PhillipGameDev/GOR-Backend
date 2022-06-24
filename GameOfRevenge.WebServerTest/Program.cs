using System;
using GameOfRevenge.Business;
using System.Configuration;
using System.Threading.Tasks;
using GameOfRevenge.Business.Manager.UserData;

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
            var manager = new PlayerDataManager();
            var response = await manager.GetAllPlayerStoredData(1071);
            string res = Newtonsoft.Json.JsonConvert.SerializeObject(response);
            System.Console.WriteLine(res);
//            await Startup.ReloadDataBaseDataAsync();
            
        }
    }
}

namespace GameOfRevenge.WebServer
{
    public interface IAppSettings
    {
        string ConnectionString { get; set; }
        string Secret { get; set; }
        string ServerVersion { get; set; }
    }

    public class AppSettings : IAppSettings
    {
        public string Secret { get; set; }
        public string ServerVersion { get; set; }
        public string ConnectionString { get; set; }
        public static IAppSettings Config { get; set; }
    }
}

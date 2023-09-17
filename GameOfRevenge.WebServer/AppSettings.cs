namespace GameOfRevenge.WebServer
{
    public interface IAppSettings
    {
        string ConnectionString { get; }
        string Secret { get; }
        string ServerVersion { get; }
        string DefaultWorldCode { get; }
    }

    public class AppSettings : IAppSettings
    {
        public string ConnectionString { get; set; }
        public string Secret { get; set; }
        public string ServerVersion { get; set; }
        public string DefaultWorldCode { get; set; }
    }
}

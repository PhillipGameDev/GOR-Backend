using System;

namespace GameOfRevenge.Business
{
    public static class Config
    {
        public static string Secret { get; set; }
        public static string ServerVersion { get; set; }
        public static string ConnectionString { get; set; }
        public static string DefaultWorldCode { get; set; }

        public static DateTime CurrentTime => DateTime.Now;
        public static DateTime UtcTime => DateTime.UtcNow;
    }
}

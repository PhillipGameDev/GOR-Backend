using System;

namespace GameOfRevenge.Business
{
    public static class Config
    {
        public static string ConnectionString { get; set; }
        public static string DefaultWorldCode { get; set; }

        public static DateTime CurrentTime { get => DateTime.Now; }
        public static DateTime UtcTime { get => DateTime.UtcNow; }
    }
}

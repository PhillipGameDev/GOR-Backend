using System;
using System.IO;
using Photon.SocketServer;
using log4net.Config;
using ExitGames.Diagnostics.Monitoring;
using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using GameOfRevenge.Common.Services;
using System.Threading.Tasks;
using System.Threading;

namespace GameOfRevenge.GameApplication
{
    public class PhotonApplication : ApplicationBase
    {
        /// <summary>
        /// Used to publish diagnostic counters.
        /// </summary>
        public readonly CounterSamplePublisher CounterPublisher;
        private readonly ILogger log = LogManager.GetCurrentClassLogger();
        private bool setupCompleted = false;
        private const int sleepTimerForSetup = 5000;

        public PhotonApplication()
        {
            CounterPublisher = new CounterSamplePublisher(1);
        }

        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            while (!setupCompleted) Thread.Sleep(sleepTimerForSetup);
            return new GorMmoPeer(initRequest);
        }

        protected override void Setup()
        {
            LoadLog4NetConfig();
            AppDomain.CurrentDomain.UnhandledException += AppDomain_OnUnhandledException;
            new DelayedAction().WaitForCallBack(Initialize, 15000);
        }

        private void LoadLog4NetConfig()
        {
            log4net.GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(ApplicationRootPath, "log");
            var configFileInfo = new FileInfo(Path.Combine(BinaryPath, "log4net.config"));
            if (configFileInfo.Exists)
            {
                LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
                XmlConfigurator.ConfigureAndWatch(configFileInfo);
            }
        }

        public void Initialize()
        {
            GameService.StartInstance();
            log.Info("GOR SERVER STARTED");
            setupCompleted = true;
        }

        protected override void TearDown()
        {
            setupCompleted = false;
            log.Info("GOR SERVER SHUTDOWN");
        }

        private void AppDomain_OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            log.Error(e.ExceptionObject);
        }
    }
}

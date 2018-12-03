using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using NLog.Targets.Wrappers;

namespace Fleur
{
    internal class Program
    {
        private static void SetupNLog(bool logToFile = false, string logFileName = "log.txt")
        {
            var config = new NLog.Config.LoggingConfiguration();

            var logConsole = new NLog.Targets.ColoredConsoleTarget("logconsole");
            var asyncLogConsole = new AsyncTargetWrapper(logConsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, asyncLogConsole);

            if (logToFile)
            {
                var logFile = new NLog.Targets.FileTarget("logfile") { FileName = logFileName };
                var asyncLogFile = new AsyncTargetWrapper(logFile);
                config.AddRule(LogLevel.Info, LogLevel.Fatal, asyncLogFile);
            }

            LogManager.Configuration = config;
        }

        private static Exception LoadConfig()
        {
            try
            {
                Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
            }
            catch (Exception ex)
            {
                Config = new Config();
                return ex;
            }

            return null;
        }

        private static Exception SaveConfig()
        {
            try
            {
                File.WriteAllText("config.json", JsonConvert.SerializeObject(Config, Formatting.Indented));
            }
            catch (Exception ex)
            {
                return ex;
            }

            return null;
        }

        private static async Task Main(string[] args)
        {
            var loaded = LoadConfig();
            SetupNLog(Config.LogToFile);

            Logger.Info("Welcome to OdrabiamyDumper!");

            if (loaded != null)
            {
                if (loaded is FileNotFoundException)
                {
                    var saved = SaveConfig();

                    Logger.Fatal(loaded, "User config does not exist!");

                    if (saved == null)
                    {
                        Logger.Info("A default config has been saved to current working directory. Please take a look at it.");
                    }
                    else
                    {
                        Logger.Fatal(saved, "Failed to save default user config!");
                    }
                }
                else
                {
                    Logger.Fatal(loaded, "Failed to load user config!");
                }

                return;
            }

            var shouldDump = Config.OperationMode == Config.EOperationMode.Full ||
                             Config.OperationMode == Config.EOperationMode.JustDump;

            var shouldCache = Config.OperationMode == Config.EOperationMode.Full ||
                             Config.OperationMode == Config.EOperationMode.JustCache;

            foreach (var sessionCookie in Program.Config.SessionCookies)
            {
                try
                {
                    var client = new Client(sessionCookie);

                    if (shouldCache)
                        await Cache.UpdateCache(client);

                    if (shouldDump)
                        await StaticGenerator.GenerateFromCache();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString);
                }
            }

            Logger.Info("Done!");
        }

        public static Config Config { get; private set; }

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    }
}

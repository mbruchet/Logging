using System.IO;
using System.Reflection;
using ECommerce.Logging.Data;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ECommerce.Logging.Log4NetAppender
{
    public class Log4NetAppender : ILoggingAppender
    {
        private readonly LoggingSettings _settings;

        public Log4NetAppender(LoggingSettings settings)
        {
            _settings = settings;
        }

        public void InitializeAppender()
        {
            var logRepo = LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
            XmlConfigurator.Configure(logRepo, new FileInfo("log4net.config"));
        }

        public void Log(LoggingItem obj)
        {
            var log = LogManager.GetLogger(Assembly.GetEntryAssembly(), obj.Application);

            switch (obj.EventLevel)
            {
                case LogLevel.Debug:
                case LogLevel.Trace:
                case LogLevel.Information:
                    log.Info(JsonConvert.SerializeObject(obj));
                    break;
                case LogLevel.Warning:
                    log.Warn(JsonConvert.SerializeObject(obj));
                    break;
                case LogLevel.Critical:
                case LogLevel.Error:
                    log.Error(JsonConvert.SerializeObject(obj));
                    break;
                case LogLevel.None:
                    break;
                default:
                    log.Info(JsonConvert.SerializeObject(obj));
                    break;
            }
        }
    }
}

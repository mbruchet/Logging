using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Reflection;
using Ecommerce.Data.RepositoryStore;
using log4net;
using log4net.Config;
using log4net.Core;
using log4net.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ECommerce.Logging.Api.Data
{
    /// <inheritdoc />
    public class LoggingRepository : RepositoryStoreFactory<LoggingItem>, ILoggingRepository
    {
        private readonly ILoggerRepository _logRepo;
        private readonly LoggingSettings _logSettings;

        public LoggingRepository(string assembly, ConnectionOptions connectionOptions, ILoggerFactory loggerFactory, DiagnosticSource diagnosticSource, IOptions<LoggingSettings> options) : 
            base(assembly, connectionOptions, loggerFactory, diagnosticSource)
        {
            AfterInsert = OnInsert;

            _logRepo = LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
            XmlConfigurator.Configure(_logRepo, new FileInfo("log4net.config"));

            _logSettings = options.Value;
        }

        private void OnInsert(LoggingItem obj)
        {
            if (obj.EventLevel <= _logSettings.PublishEventLevel) return;
            var log = LogManager.GetLogger(Assembly.GetEntryAssembly(), obj.Application);

            switch (obj.EventLevel)
            {
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
                default:
                    log.Info(JsonConvert.SerializeObject(obj));
                    break;
            }
        }
    }
}

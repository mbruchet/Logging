using System.Diagnostics;
using System.IO;
using System.Reflection;
using Ecommerce.Data.RepositoryStore;
using ECommerce.Core;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ECommerce.Logging.Data
{
    /// <inheritdoc cref="ILoggingRepository" />
    public class LoggingRepository : RepositoryStoreFactory<LoggingItem>, ILoggingRepository
    {
        private readonly LoggingSettings _logSettings;
        private readonly ILoggingAppender _loggingAppender;

        public LoggingRepository(string assembly, ConnectionOptions connectionOptions, ILoggerFactory loggerFactory, DiagnosticSource diagnosticSource, IOptions<LoggingSettings> options) : 
            base(assembly, connectionOptions, loggerFactory, diagnosticSource)
        {
            AfterInsert = OnInsert;

            _logSettings = options.Value;

            _loggingAppender = PluginContainer.GetInstance<ILoggingAppender>(_logSettings.Appender, _logSettings);
            _loggingAppender.InitializeAppender();
        }

        private void OnInsert(LoggingItem obj)
        {
            if (obj.EventLevel <= _logSettings.PublishEventLevel) return;
            _loggingAppender.Log(obj);
        }
    }
}

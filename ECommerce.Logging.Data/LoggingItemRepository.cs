using System.Diagnostics;
using System.Threading.Tasks;
using Ecommerce.Data.RepositoryStore;
using ECommerce.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ECommerce.Logging.Data
{
    /// <inheritdoc cref="ILoggingRepository" />
    public class LoggingItemRepository : RepositoryStoreFactory<LoggingItem>, ILoggingRepository
    {
        private readonly LoggingSettings _logSettings;
        private readonly ILoggingAppender _loggingAppender;
        private LoggingRuleManager _loggingRuleManager;

        public LoggingItemRepository(string assembly, ConnectionOptions connectionOptions, ILoggerFactory loggerFactory, DiagnosticSource diagnosticSource, IOptions<LoggingSettings> options, IConfiguration configuration) : 
            base(assembly, connectionOptions, loggerFactory, diagnosticSource)
        {
            AfterInsert = OnInsert;

            _logSettings = options.Value;

            _loggingAppender = PluginContainer.GetInstance<ILoggingAppender>(_logSettings.Appender, _logSettings);
            _loggingAppender.InitializeAppender();

            var loggingRuleRepository = new LoggingRuleRepository(_logSettings.NotificationRules.ProviderAssembly, new ConnectionOptions
            {
                Provider = _logSettings.NotificationRules.ProviderType,
                ConnectionString = _logSettings.NotificationRules.ConnectionString
            }, loggerFactory, diagnosticSource);

            _loggingRuleManager = new LoggingRuleManager(loggingRuleRepository, configuration, loggerFactory, diagnosticSource);
        }

        private void OnInsert(LoggingItem obj)
        {
            if (obj.EventLevel <= _logSettings.PublishEventLevel) return;
            _loggingAppender.Log(obj);
            Task.Run(() => _loggingRuleManager.Push(obj));
        }
    }
}

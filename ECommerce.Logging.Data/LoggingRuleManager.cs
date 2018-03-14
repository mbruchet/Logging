using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using Ecommerce.Data.RepositoryStore;
using ECommerce.Events.Clients.Core;
using ECommerce.Events.Clients.PublisherClient;
using ECommerce.Events.Data.Repositories;
using ECommerce.Remote;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ECommerce.Logging.Data
{
    public class LoggingRuleManager
    {
        private readonly LoggingRuleRepository _loggingRuleRepository;
        private readonly IConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;
        private readonly DiagnosticSource _diagnosticSource;
        private ConcurrentBag<LoggingRule> _rules;
        private readonly ConcurrentDictionary<string, IPublisherClientService> _publishers = new ConcurrentDictionary<string, IPublisherClientService>();
        private HttpClient _httpClient;

        public LoggingRuleManager(LoggingRuleRepository loggingRuleRepository, IConfiguration configuration, ILoggerFactory loggerFactory, DiagnosticSource diagnosticSource, HttpClient httpClient = null)
        {
            _loggingRuleRepository = loggingRuleRepository;
            _configuration = configuration;
            _loggerFactory = loggerFactory;
            _diagnosticSource = diagnosticSource;
            _httpClient = httpClient;

            _loggingRuleRepository.AfterInsert = rule => _rules.Add(rule);

            _loggingRuleRepository.AfterUpdate = rule =>
                _rules = new ConcurrentBag<LoggingRule>(_rules.Where(x => x.Id != rule.Id));

            _loggingRuleRepository.AfterDelete = rule => _rules = new ConcurrentBag<LoggingRule>(_rules.Where(x => x.Id != rule.Id));
        }

        public void Push(LoggingItem loggingItem)
        {
            if (_rules == null)
            {
                var searchRules = _loggingRuleRepository.SearchAsync(x => x.IsEnabled).Result;

                if (searchRules.IsSuccessful && searchRules.Result != null)
                    _rules = new ConcurrentBag<LoggingRule>(searchRules.Result);
            }

            if (_rules == null) return;

            foreach (var rule in _rules)
            {
                if (Evaluate(rule, loggingItem))
                {
                    Notify(rule, loggingItem);
                }
            }
        }

        private void Notify(LoggingRule rule, LoggingItem loggingItem)
        {
            if (_publishers.ContainsKey(rule.Notification)) return;

            var settings = new NotificationServiceSettings();
            _configuration.GetSection("Notification").Bind(settings);

            var options = Options.Create(settings);

            settings.ServiceName = rule.Notification;

            var publisherSettings = new RemoteServiceSettings();
            _configuration.GetSection("Notification:Publisher").Bind(publisherSettings);

            _publishers.TryAdd(rule.Notification,
                RegisterPublisher(publisherSettings, options, _loggerFactory, _diagnosticSource, settings));

            _publishers[rule.Notification]?.Publish(JsonConvert.SerializeObject(loggingItem));
        }

        private IPublisherClientService RegisterPublisher(RemoteServiceSettings publisherSettings, IOptions<NotificationServiceSettings> options, ILoggerFactory loggerFactory, DiagnosticSource diagnosticSource, NotificationServiceSettings settings)
        {
            return publisherSettings.IsLocal ? RegisterLocalPublisher(options, loggerFactory, diagnosticSource, settings) : RegisterRemotePublisher(publisherSettings);
        }

        private IPublisherClientService RegisterRemotePublisher(RemoteServiceSettings publisherSettings)
        {
            return new RemotePublisherClientService(publisherSettings, _httpClient);
        }

        private IPublisherClientService RegisterLocalPublisher(IOptions<NotificationServiceSettings> options, ILoggerFactory loggerFactory, DiagnosticSource diagnosticSource, NotificationServiceSettings settings)
        {
            var eventChannelRepository = new EventChannelRepository(settings.Repository.ProviderAssembly,
                new ConnectionOptions
                {
                    Provider = settings.Repository.ProviderType,
                    ConnectionString = settings.Repository.Channel,
                }, loggerFactory, diagnosticSource);

            var eventSubscriptionRepository = new EventSubscriptionRepository(eventChannelRepository,
                settings.Repository.ProviderAssembly,
                new ConnectionOptions
                {
                    Provider = settings.Repository.ProviderType,
                    ConnectionString = settings.Repository.Subscription
                }, loggerFactory, diagnosticSource);

            var eventRepository = new EventRepository(eventChannelRepository, settings.Repository.ProviderAssembly,
                new ConnectionOptions
                {
                    Provider = settings.Repository.ProviderType,
                    ConnectionString = settings.Repository.Events
                }, loggerFactory, diagnosticSource);

            return new LocalPublisherClientService(eventChannelRepository,
                eventSubscriptionRepository, eventRepository,
                options, loggerFactory, diagnosticSource);
        }

        private bool Evaluate(LoggingRule rule, LoggingItem loggingItem)
        {            
            var lambdaParser = new NReco.Linq.LambdaParser();
            var varContext = new Dictionary<string, object> {["LoggingItem"] = loggingItem, ["Warning"]=LogLevel.Warning, ["Error"]=LogLevel.Error, ["Information"]=LogLevel.Information};
            return (bool) lambdaParser.Eval(rule.Rule, varContext);
        }
    }
}
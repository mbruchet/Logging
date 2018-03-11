using System;
using System.Collections.Concurrent;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace ECommerce.Logging.Client
{
    public class RemoteLoggerProvider : ILoggerProvider
    {
        private readonly string _remoteUrl;
        private readonly HttpClient _httpClient;
        private readonly string _application;
        private readonly string _service;
        private readonly string _environment;
        private readonly LogLevel _minLogLevel;
        private readonly RemoteLoggerSetting _config;
        private readonly ConcurrentDictionary<string, RemoteLogger> _loggers = new ConcurrentDictionary<string, RemoteLogger>();
        private bool _disposed;

        public RemoteLoggerProvider(RemoteLoggerSetting config, HttpClient httpClient = null)
        {
            _config = config;
            _httpClient = httpClient;
        }

        public RemoteLoggerProvider(HttpClient httpClient, string application, string service, string environment, LogLevel minLogLevel)
        {
            _httpClient = httpClient;
            _application = application;
            _service = service;
            _environment = environment;
            _minLogLevel = minLogLevel;
        }

        public RemoteLoggerProvider(string remoteUrl, string application, string service, string environment, LogLevel minLogLevel)
        {
            _remoteUrl = remoteUrl;
            _minLogLevel = minLogLevel;
            _application = application;
            _service = service;
            _environment = environment;
        }

        public ILogger CreateLogger(string categoryName)
        {
            if(_config != null) return _loggers.GetOrAdd(categoryName, name => new RemoteLogger(name, _config, _httpClient));

            return _httpClient != null ? 
                _loggers.GetOrAdd(categoryName, name => new RemoteLogger(name, _httpClient, _application, _service, _environment, _minLogLevel)) : 
                _loggers.GetOrAdd(categoryName, name => new RemoteLogger(name, _remoteUrl, _application, _service, _environment, _minLogLevel));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing || _disposed) return;

            _loggers.Clear();
            _disposed = true;
        }
    }
}
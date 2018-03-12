using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using ECommerce.Logging.Data;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ECommerce.Logging.Client
{
    public class RemoteLogger:ILogger
    {
        public string Application { get; }
        public string Service { get; }
        public string Environment { get; }

        private readonly string _name;
        private readonly RemoteLoggerSetting _setting;
        private readonly HttpClient _httpClient;

        public RemoteLogger(string name, string remoteUrl, string application, string service, string environment, LogLevel minEventLevel)
        {
            _name = name;
            _httpClient = new HttpClient(){BaseAddress = new Uri(remoteUrl)};

            Application = application;
            Service = service;
            Environment = environment;

            _setting = new RemoteLoggerSetting
            {
                IsEnabled = true,
                MinEventLevel = minEventLevel,
                RemoteUrl = remoteUrl
            };
        }

        public RemoteLogger(string name, HttpClient httpClient, string application, string service, string environment, LogLevel minEventLevel)
        {
            _name = name;
            _httpClient = httpClient;

            Application = application;
            Service = service;
            Environment = environment;

            _setting = new RemoteLoggerSetting
            {
                IsEnabled = true,
                MinEventLevel = minEventLevel,
                RemoteUrl = httpClient.BaseAddress.ToString()
            };
        }

        public RemoteLogger(string name, RemoteLoggerSetting setting, HttpClient httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient(){BaseAddress =  new Uri(setting.RemoteUrl) };

            _name = name;
            _setting = setting;

            Application = setting.Application;
            Service = setting.Service;
            Environment = setting.Environment;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            if (formatter == null) throw new ArgumentNullException(nameof(formatter));

            var message = formatter(state, exception);

            var json = JsonConvert.SerializeObject(new LoggingItem
            {
                Application = Application,
                Service = Service,
                Environment = Environment,
                Category = _name,
                EventLevel = logLevel,
                EventDate = DateTime.Now,
                Server = System.Environment.MachineName,
                Message = message,
                EventId = eventId.ToString()
            });

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            PublishWithRetrying(json);
        }

        private void PublishWithRetrying(string json)
        {
            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            {
                var result = _httpClient.PostAsync("api/logging", content).Result;
                result.EnsureSuccessStatusCode();
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _setting.IsEnabled && _setting.MinEventLevel < logLevel;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}

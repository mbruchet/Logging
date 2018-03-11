using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace ECommerce.Logging.Client
{
    public static class RemoteLoggerProviderExtension
    {
        public static ILoggerFactory AddRemoteLogger(this ILoggerFactory loggerFactory, RemoteLoggerSetting config, HttpClient httpClient = null)
        {
            loggerFactory.AddProvider(new RemoteLoggerProvider(config, httpClient));
            return loggerFactory;
        }

        public static ILoggerFactory AddRemoteLogger(this ILoggerFactory loggerFactory, string remoteUrl, string applicationName, string service,
            string environment, LogLevel minLogLevel)
        {
            loggerFactory.AddProvider(new RemoteLoggerProvider(remoteUrl, applicationName, service, environment, LogLevel.Trace));
            return loggerFactory;
        }


        public static ILoggerFactory AddRemoteLogger(this ILoggerFactory loggerFactory, HttpClient httpClient, string applicationName, string service,
            string environment, LogLevel minLogLevel)
        {
            loggerFactory.AddProvider(new RemoteLoggerProvider(httpClient, applicationName, service, environment, LogLevel.Trace));
            return loggerFactory;
        }
    } 
}
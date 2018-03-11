using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ECommerce.Logging.Client;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Xunit;

namespace ECommerce.Logging.Api.Tests
{
    public class RemoteLoggerExtensionTest:IDisposable
    {
        private bool _disposed = false;
        private TestServer _host;

        public RemoteLoggerExtensionTest()
        {
            _host = new LoggingControllerHost().Server;
        }

        [Fact]
        public void WithTheLoggerExtension_ShouldCreateLoggingItem()
        {
            var httpClient = _host.CreateClient();

            var loggerFactory = new LoggerFactory();

            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            var settings = new RemoteLoggerSetting();

            ConfigurationBuilder config = new ConfigurationBuilder();
            config.AddJsonFile("appSettings.json");

            config.Build().GetSection("Logging").Bind(settings);

            settings.RemoteUrl = httpClient.BaseAddress.ToString();

            loggerFactory.AddRemoteLogger(settings, httpClient);

            var logger = loggerFactory.CreateLogger("Test");

            logger.Should().NotBeNull();
            logger.LogInformation("This is a test");

            var logFile = "c:\\temp\\app.log";

            File.Exists(logFile).Should().BeTrue();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                //cleanup
            }
        }
    }
}

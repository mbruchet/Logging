using System;
using System.IO;
using ECommerce.Logging.Client;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using Xunit;

namespace ECommerce.Logging.Api.Tests
{
    public class RemoteLoggerProviderTest : IDisposable
    {
        private bool _disposed = false;
        private TestServer _host;

        public RemoteLoggerProviderTest()
        {
            _host = new LoggingControllerHost().Server;
        }

        [Fact]
        public void WithTheLogger_ShouldCreateLoggingItem()
        {
            var httpClient = _host.CreateClient();

            var loggerFactory = new LoggerFactory();

            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            loggerFactory.AddProvider(new RemoteLoggerProvider(httpClient, "MyApplication", "MyService", "Development", LogLevel.Trace));

            var logger = loggerFactory.CreateLogger("Test");

            logger.Should().NotBeNull();
            logger.LogError("This is a test");

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

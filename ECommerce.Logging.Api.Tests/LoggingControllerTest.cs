using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using ECommerce.Logging.Data;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Xunit;

namespace ECommerce.Logging.Api.Tests
{
    public class LoggingControllerTest : IDisposable
    {
        private bool _disposed = false;
        private TestServer _host;

        public LoggingControllerTest()
        {
            _host = new LoggingControllerHost().Server;
        }

        [Fact]
        public void ShouldCreateLoggingItem()
        {
            var httpClient = _host.CreateClient();

            var json = JsonConvert.SerializeObject(new LoggingItem
            {
                Application = "MyApplication",
                Service = "MyService",
                Environment = "Test",
                EventLevel = LogLevel.Information,
                EventDate = DateTime.Now,
                Server = Environment.MachineName,
                Message = "This is a test",
                EventId = Guid.NewGuid().ToString()
            });

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            {
                var result = httpClient.PostAsync("api/logging", content).Result;
                result.Should().NotBeNull();
                result.IsSuccessStatusCode.Should().BeTrue();
            }

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

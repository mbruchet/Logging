using System.Diagnostics.Tracing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ECommerce.Logging.Api.Data
{
    public class LoggingSettings
    {
        public LogLevel MemoryEventLevel { get; set; }
        public LogLevel PublishEventLevel { get; set; }
        public RepositorySetting Repository { get; set; }
    }

    [JsonObject]
    public class RepositorySetting
    {
        [JsonProperty("providerAssembly")]
        public string ProviderAssembly { get; set; }

        [JsonProperty("providerType")]
        public string ProviderType { get; set; }

        [JsonProperty("connectionString")]
        public string ConnectionString { get; set; }
    }
}

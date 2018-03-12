using Microsoft.Extensions.Logging;

namespace ECommerce.Logging.Data
{
    public class LoggingSettings
    {
        public LogLevel MemoryEventLevel { get; set; }
        public LogLevel PublishEventLevel { get; set; }
        public RepositorySetting Repository { get; set; }
        public string Appender { get; set; }
    }
}

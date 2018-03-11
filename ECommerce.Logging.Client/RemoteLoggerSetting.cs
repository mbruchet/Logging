using Microsoft.Extensions.Logging;

namespace ECommerce.Logging.Client
{
    public class RemoteLoggerSetting
    {
        public bool IsEnabled { get; set; }
        public LogLevel MinEventLevel { get; set; }
        public string RemoteUrl { get; set; }
        public string Application { get; set; }
        public string Service { get; set; }
        public string Environment { get; set; }
    }
}

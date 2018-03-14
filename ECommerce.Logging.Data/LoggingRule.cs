using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ECommerce.Logging.Data
{
    public class LoggingRule
    {
        [Key]
        public string Id { get; set; }

        [JsonProperty("rule")]
        public string Rule { get; set; }

        [JsonProperty("notification")]
        public string Notification { get; set; }

        [JsonProperty("isEnabled")]
        public bool IsEnabled { get; set; } = true;
    }
}

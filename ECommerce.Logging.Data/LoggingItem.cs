using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace ECommerce.Logging.Data
{
    public class LoggingItem
    {
        [Key]
        public string Id { get; set; }
        public string Application { get; set; }
        public string Service { get; set; }
        public string Environment { get; set; }
        public string Server { get; set; }
        public DateTime EventDate { get; set; }
        public LogLevel EventLevel { get; set; }
        public string Message { get; set; }
        public string EventId { get; set; }
        public string Logger { get; set; }
    }
}

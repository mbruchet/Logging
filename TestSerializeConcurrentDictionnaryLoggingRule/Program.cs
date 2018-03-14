using System;
using System.Collections.Concurrent;
using System.IO;
using ECommerce.Logging.Data;
using Newtonsoft.Json;

namespace TestSerializeConcurrentDictionnaryLoggingRule
{
    class Program
    {
        static void Main(string[] args)
        {
            var dico = new ConcurrentDictionary<string, LoggingRule>();

            var id = Guid.NewGuid();

            dico.TryAdd(id.ToString(), new LoggingRule
            {
                Id = id.ToString(),
                Rule = "LoggingItem.EventLevel > 4",
                Notification = "ErrorOccured",
                IsEnabled = true
            });

            id = Guid.NewGuid();

            dico.TryAdd(id.ToString(), new LoggingRule
            {
                Id = id.ToString(),
                Rule = "LoggingItem.EventLevel > 3 && LoggingItem.Category.Contains(\"MyApplication\")",
                Notification = "MyApplicationLogging",
                IsEnabled = true
            });

            var json = JsonConvert.SerializeObject(dico);

            File.WriteAllText(new FileInfo("rules.json").FullName, json);

            Console.WriteLine("Hello World!");
        }
    }
}

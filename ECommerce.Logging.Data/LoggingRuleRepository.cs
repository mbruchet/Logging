using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Ecommerce.Data.RepositoryStore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ECommerce.Logging.Data
{
    public class LoggingRuleRepository: RepositoryStoreFactory<LoggingRule>
    {
        public LoggingRuleRepository(string assembly, ConnectionOptions connectionOptions, 
            ILoggerFactory loggerFactory, DiagnosticSource diagnosticSource) : base(assembly, connectionOptions, loggerFactory, diagnosticSource)
        {
        }
    }
}

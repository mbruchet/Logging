using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace ECommerce.Logging.Api.Data
{
    public interface ILoggingRepository
    {
        Task<ExecutionResult<LoggingItem>> AddAsync(LoggingItem value);
        Task<ExecutionResult<LoggingItem>> UpdateAsync(LoggingItem value);
        Task<ExecutionResult<LoggingItem>> RemoveAsync(LoggingItem value);
        Task<ExecutionResult<IEnumerable<LoggingItem>>> SearchAsync(Func<LoggingItem, bool> filter);
        Task<ExecutionResult<LoggingItem>> SearchASingleItemAsync(Func<LoggingItem, bool> filter);
        Action<LoggingItem> AfterDelete { get; set; }
    }
}
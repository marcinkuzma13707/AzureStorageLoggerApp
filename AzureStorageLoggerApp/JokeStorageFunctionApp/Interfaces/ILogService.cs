using JokeStorageFunctionApp.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JokeStorageFunctionApp.Interfaces;

public interface ILogService
{
    Task AddLogAsync(LogEntry logEntry);
    Task<LogServiceResult> GetLogsAsync(DateTime from, DateTime to, string continuationToken = null);
}

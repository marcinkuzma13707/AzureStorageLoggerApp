using JokeStorageFunctionApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JokeStorageFunctionApp.Interfaces;

public interface ILogService
{
    Task AddLogAsync(LogEntry logEntry);
    Task<List<LogEntry>> GetLogsAsync(DateTime from, DateTime to);
}

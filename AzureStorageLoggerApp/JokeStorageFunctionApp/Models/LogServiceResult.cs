using System.Collections.Generic;

namespace JokeStorageFunctionApp.Models;

public class LogServiceResult
{
    public List<LogEntry> Logs { get; set; }
    public string ContinuationToken { get; set; }
}
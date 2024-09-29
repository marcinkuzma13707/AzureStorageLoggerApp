using Azure;
using Azure.Data.Tables;
using System;

namespace JokeStorageFunctionApp.Models;

public class LogEntry : ITableEntity
{
    public string Id { get; set; }
    public string PartitionKey { get; set; } = "JokeLogs";
    public string RowKey { get; set; } // Typically this is the log ID
    public DateTime TimeStamp { get; set; }
    public string Status { get; set; } // Success or Failure
    public string Message { get; set; } // Optional, for error messages

    // Required for ITableEntity
    public ETag ETag { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
}

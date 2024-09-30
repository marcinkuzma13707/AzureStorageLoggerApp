using Azure;
using Azure.Data.Tables;
using System;

namespace JokeStorageFunctionApp.Models;

public class LogEntry : ITableEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTime TimeStamp { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }
    public ETag ETag { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
}
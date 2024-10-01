using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Data.Tables;
using JokeStorageFunctionApp.Interfaces;
using JokeStorageFunctionApp.Models;

namespace JokeStorageFunctionApp.Services;

public class LogService : ILogService
{
    private readonly TableClient _tableClient;

    public LogService(TableServiceClient tableServiceClient, string tableName)
    {
        _tableClient = tableServiceClient.GetTableClient(tableName);
        _tableClient.CreateIfNotExists();
    }

    public async Task AddLogAsync(LogEntry logEntry)
    {
        await _tableClient.AddEntityAsync(logEntry);
    }

    public async Task<LogServiceResult> GetLogsAsync(DateTime from, DateTime to, string continuationToken = null)
    {
        var logs = new List<LogEntry>();

        var query = _tableClient.QueryAsync<LogEntry>(
            filter: $"Timestamp ge datetime'{from:O}' and Timestamp le datetime'{to:O}'",
            maxPerPage: 100,
            cancellationToken: CancellationToken.None);

        string newContinuationToken = null;

        await foreach (var page in query.AsPages(continuationToken))
        {
            logs.AddRange(page.Values);

            newContinuationToken = page.ContinuationToken;
            break;
        }

        return new LogServiceResult
        {
            Logs = logs,
            ContinuationToken = newContinuationToken
        };
    }
}

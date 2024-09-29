﻿using Azure.Data.Tables;
using JokeStorageFunctionApp.Interfaces;
using JokeStorageFunctionApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JokeStorageFunctionApp.Services;

public class LogService : ILogService
{
    private readonly TableClient _tableClient;

    public LogService(TableServiceClient tableServiceClient, string tableName)
    {
        _tableClient = tableServiceClient.GetTableClient(tableName);
        _tableClient.CreateIfNotExists();  // Ensure table exists
    }

    public async Task AddLogAsync(LogEntry logEntry)
    {
        await _tableClient.AddEntityAsync(logEntry);
    }

    public async Task<List<LogEntry>> GetLogsAsync(DateTime from, DateTime to)
    {
        var logs = new List<LogEntry>();
        var query = _tableClient.QueryAsync<LogEntry>(filter: $"Timestamp ge datetime'{from:O}' and Timestamp le datetime'{to:O}'");

        await foreach (var log in query)
        {
            logs.Add(log);
        }

        return logs;
    }
}

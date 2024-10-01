using JokeStorageFunctionApp.Interfaces;
using JokeStorageFunctionApp.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace JokeStorageFunctionApp.Functions;

public class JokeFetcherFunction
{
    private readonly ILogService _logService;
    private readonly IBlobService _blobService;
    private readonly HttpClient _httpClient;
    private readonly ILogger<JokeFetcherFunction> _logger;
    private const string ApiUrl = "https://official-joke-api.appspot.com/random_joke";

    public JokeFetcherFunction(ILogService logService, IBlobService blobService, HttpClient httpClient, ILogger<JokeFetcherFunction> logger)
    {
        _logService = logService;
        _blobService = blobService;
        _httpClient = httpClient;
        _logger = logger;
    }

    [FunctionName("FetchJokeEveryMinute")]
    public async Task Run([TimerTrigger("*/1 * * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation($"Fetching joke at: {DateTime.Now}");

        try
        {
            var response = await _httpClient.GetStringAsync(ApiUrl);

            var logEntry = new LogEntry
            {
                PartitionKey = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                RowKey = Guid.NewGuid().ToString(),
                TimeStamp = DateTime.UtcNow,
                Status = "Success",
                Message = "Joke fetched successfully"
            };

            await _logService.AddLogAsync(logEntry);
            await _blobService.SavePayloadAsync(logEntry.RowKey, response);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching joke.");

            var logEntry = new LogEntry
            {
                PartitionKey = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                RowKey = Guid.NewGuid().ToString(),
                TimeStamp = DateTime.UtcNow,
                Status = "Failure",
                Message = ex.Message
            };

            await _logService.AddLogAsync(logEntry);
        }
    }
}

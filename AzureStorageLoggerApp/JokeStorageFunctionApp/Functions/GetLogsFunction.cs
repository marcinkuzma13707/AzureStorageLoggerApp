using JokeStorageFunctionApp.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System;
using System.Threading.Tasks;


namespace JokeStorageFunctionApp.Functions;

public class GetLogsFunction
{
    private readonly ILogService _logService;

    public GetLogsFunction(ILogService logService)
    {
        _logService = logService;
    }

    // GET API to fetch logs for a specific time period
    [FunctionName("GetLogs")]
    public async Task<IActionResult> GetLogs(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "logs")] HttpRequest req)
    {
        // Retrieve query parameters
        var from = DateTime.Parse(req.Query["from"]);
        var to = DateTime.Parse(req.Query["to"]);

        // Fetch logs within the specified period
        var logs = await _logService.GetLogsAsync(from, to);

        return new OkObjectResult(logs);
    }
}

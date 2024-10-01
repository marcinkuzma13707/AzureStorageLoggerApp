using System;
using System.Linq;
using System.Threading.Tasks;
using JokeStorageFunctionApp.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace JokeStorageFunctionApp.Functions;

public class GetLogsFunction
{
    private readonly ILogService _logService;

    public GetLogsFunction(ILogService logService)
    {
        _logService = logService;
    }

    [FunctionName("GetLogs")]
    public async Task<IActionResult> GetLogs(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "logs")] HttpRequest req)
    {
        try
        {
            if (!DateTime.TryParse(req.Query["from"], out DateTime from) || !DateTime.TryParse(req.Query["to"], out DateTime to))
            {
                return new BadRequestObjectResult("Invalid date range. Please specify valid 'from' and 'to' query parameters.");
            }

            var continuationToken = req.Query["continuationToken"];

            var result = await _logService.GetLogsAsync(from, to, continuationToken);

            if (result.Logs == null || !result.Logs.Any())
            {
                return new NotFoundResult();
            }

            var response = new
            {
                logs = result.Logs,
                continuationToken = result.ContinuationToken
            };

            return new OkObjectResult(response);
        }
        catch (Exception ex)
        {
            return new ObjectResult(new { message = ex.Message }) { StatusCode = 500 };
        }
    }
}
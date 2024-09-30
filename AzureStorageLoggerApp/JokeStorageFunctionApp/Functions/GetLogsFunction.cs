using JokeStorageFunctionApp.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System;
using System.Linq;
using System.Threading.Tasks;


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

            var logs = await _logService.GetLogsAsync(from, to);

            if (logs == null || !logs.Any())
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(logs);
        }
        catch (Exception ex)
        {
            return new ObjectResult(new { message = ex.Message }) { StatusCode = 500 };
        }
    }
}

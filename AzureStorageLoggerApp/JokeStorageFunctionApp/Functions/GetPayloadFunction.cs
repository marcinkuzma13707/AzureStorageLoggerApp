using JokeStorageFunctionApp.Exceptions;
using JokeStorageFunctionApp.Interfaces;
using JokeStorageFunctionApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace JokeStorageFunctionApp.Functions;

public class GetPayloadFunction
{
    private readonly IBlobService _blobService;

    public GetPayloadFunction(IBlobService blobService)
    {
        _blobService = blobService;
    }

    [FunctionName("GetPayload")]
    public async Task<IActionResult> GetPayload(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "payload/{id}")] HttpRequest req, string id)
    {
        try
        {
            var payloadString = await _blobService.GetPayloadAsync(id);

            var payloadObject = JsonConvert.DeserializeObject<JokeResponse>(payloadString);

            return new OkObjectResult(payloadObject);
        }
        catch (BlobNotFoundException)
        {
            return new NotFoundObjectResult($"Blob with id {id} not found.");
        }
        catch (Exception ex)
        {
            return new ObjectResult(new { message = ex.Message }) { StatusCode = 500 };
        }
    }
}

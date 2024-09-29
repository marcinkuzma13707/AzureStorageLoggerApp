using JokeStorageFunctionApp.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Threading.Tasks;

namespace JokeStorageFunctionApp.Functions
{
    public class GetPayloadFunction
    {
        private readonly IBlobService _blobService;

        public GetPayloadFunction(IBlobService blobService)
        {
            _blobService = blobService;
        }

        // GET API to fetch a payload from the blob storage for a specific log entry
        [FunctionName("GetPayload")]
        public async Task<IActionResult> GetPayload(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "payload/{id}")] HttpRequest req, string id)
        {
            // Fetch payload from Blob Storage
            var payload = await _blobService.GetPayloadAsync(id);

            if (payload == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(payload);
        }
    }
}

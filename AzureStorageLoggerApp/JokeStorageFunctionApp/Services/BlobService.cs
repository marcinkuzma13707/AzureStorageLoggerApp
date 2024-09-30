using Azure.Storage.Blobs;
using JokeStorageFunctionApp.Exceptions;
using JokeStorageFunctionApp.Interfaces;
using System;
using System.Threading.Tasks;

namespace JokeStorageFunctionApp.Services;

public class BlobService : IBlobService
{
    private readonly BlobContainerClient _blobContainerClient;

    public BlobService(BlobContainerClient blobContainerClient)
    {
        _blobContainerClient = blobContainerClient;
        _blobContainerClient.CreateIfNotExists();
    }

    public async Task SavePayloadAsync(string id, string content)
    {
        var blobClient = _blobContainerClient.GetBlobClient($"{id}.json");
        await blobClient.UploadAsync(new BinaryData(content));
    }

    public async Task<string> GetPayloadAsync(string id)
    {
        var blobClient = _blobContainerClient.GetBlobClient($"{id}.json");

        if (!await blobClient.ExistsAsync())
        {
            throw new BlobNotFoundException($"Blob with id {id} not found.");
        }

        var download = await blobClient.DownloadContentAsync();
        return download.Value.Content.ToString();
    }
}

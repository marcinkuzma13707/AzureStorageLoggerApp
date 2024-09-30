using Azure.Data.Tables;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using JokeStorageFunctionApp.Services;
using JokeStorageFunctionApp.Interfaces;
using System;
using Azure.Storage.Blobs;

[assembly: FunctionsStartup(typeof(JokeStorageFunctionApp.Startup))]

namespace JokeStorageFunctionApp;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddSingleton((s) =>
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            return new TableServiceClient(connectionString);
        });

        builder.Services.AddSingleton((s) =>
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            var blobServiceClient = new BlobServiceClient(connectionString);

            var blobContainerName = Environment.GetEnvironmentVariable("BlobContainerName");
            return blobServiceClient.GetBlobContainerClient(blobContainerName);
        });

        builder.Services.AddSingleton<ILogService>(s =>
        {
            var tableServiceClient = s.GetRequiredService<TableServiceClient>();

            var tableName = Environment.GetEnvironmentVariable("TableName");
            return new LogService(tableServiceClient, tableName);
        });

        builder.Services.AddSingleton<IBlobService, BlobService>();

        builder.Services.AddHttpClient();
    }
}

